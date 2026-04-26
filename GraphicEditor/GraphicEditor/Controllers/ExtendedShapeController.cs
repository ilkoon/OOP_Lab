// GraphicEditor/Controllers/ExtendedShapeController.cs
using System;
using System.Collections.Generic;
using System.Linq;
using GraphicEditor.Adapters;
using GraphicEditor.Models.Shapes;
using GraphicEditor.Models.Shapes3D;
using GraphicEditor.Plugins;

namespace GraphicEditor.Controllers
{
    /// <summary>
    /// Extended controller that supports plugin shapes
    /// Inherits from ShapeController and adds plugin functionality
    /// </summary>
    public class ExtendedShapeController : ShapeController
    {
        private readonly PluginLoader _pluginLoader;
        private readonly List<IPluginShape2D> _pluginShapes2D;
        private readonly List<IPluginShape3D> _pluginShapes3D;

        // Maps display name -> plugin
        private readonly Dictionary<string, IShape2DPlugin> _plugin2DMap;
        private readonly Dictionary<string, IShape3DPlugin> _plugin3DMap;

        // Built-in shape names for reference
        private static readonly string[] BuiltIn2DShapes = { "Circle", "Rectangle", "Triangle" };
        private static readonly string[] BuiltIn3DShapes = { "Cube", "Sphere", "Tetrahedron" };

        public IReadOnlyList<IPluginShape2D> PluginShapes2D => _pluginShapes2D.AsReadOnly();
        public IReadOnlyList<IPluginShape3D> PluginShapes3D => _pluginShapes3D.AsReadOnly();
        public PluginLoader PluginLoader => _pluginLoader;

        public ExtendedShapeController() : base()
        {
            _pluginLoader = new PluginLoader();
            _pluginShapes2D = new List<IPluginShape2D>();
            _pluginShapes3D = new List<IPluginShape3D>();
            _plugin2DMap = new Dictionary<string, IShape2DPlugin>();
            _plugin3DMap = new Dictionary<string, IShape3DPlugin>();

            // Load plugins from the Plugins folder
            _pluginLoader.LoadAllPlugins();
            RegisterPlugins();
        }

        /// <summary>
        /// Registers all loaded plugins for use
        /// </summary>
        private void RegisterPlugins()
        {
            foreach (var plugin in _pluginLoader.Loaded2DPlugins)
            {
                _plugin2DMap[plugin.DisplayName] = plugin;
            }

            foreach (var plugin in _pluginLoader.Loaded3DPlugins)
            {
                _plugin3DMap[plugin.DisplayName] = plugin;
            }
        }

        /// <summary>
        /// Creates a shape (either built-in or from plugin)
        /// </summary>
        public new void CreateShape(string shapeType, string parameters)
        {
            // Try built-in shapes first (handled by base class)
            if (BuiltIn2DShapes.Contains(shapeType) || BuiltIn3DShapes.Contains(shapeType))
            {
                base.CreateShape(shapeType, parameters);
                return;
            }

            // Handle plugin 2D shapes
            if (_plugin2DMap.ContainsKey(shapeType))
            {
                var plugin = _plugin2DMap[shapeType];
                var shape = plugin.CreateShape();
                shape.SetParameters(parameters);
                _pluginShapes2D.Add(shape);
                return;
            }

            // Handle plugin 3D shapes
            if (_plugin3DMap.ContainsKey(shapeType))
            {
                var plugin = _plugin3DMap[shapeType];
                var shape = plugin.CreateShape();
                shape.SetParameters(parameters);
                _pluginShapes3D.Add(shape);
                return;
            }

            throw new ArgumentException($"Unknown shape type: {shapeType}");
        }

        /// <summary>
        /// Gets all available shapes (built-in + plugins)
        /// </summary>
        public new string[] GetAvailableShapes()
        {
            var builtIn = base.GetAvailableShapes();
            var pluginShapes = _plugin2DMap.Keys.Concat(_plugin3DMap.Keys).ToArray();
            return builtIn.Concat(pluginShapes).ToArray();
        }

        /// <summary>
        /// Gets all shapes for rendering (built-in + plugins, with projection for 3D)
        /// </summary>
        public new List<IShape> GetShapesForRendering()
        {
            var shapes = base.GetShapesForRendering();

            // Add plugin 2D shapes
            foreach (var shape in _pluginShapes2D)
            {
                shapes.Add(new PluginShape2DAdapter(shape));
            }

            // Add plugin 3D shapes (projected to 2D for rendering)
            foreach (var shape3D in _pluginShapes3D)
            {
                shapes.Add(new ProjectedPluginShapeAdapter(shape3D));
            }

            return shapes;
        }

        /// <summary>
        /// Gets a 2D plugin by shape type name
        /// </summary>
        public IShape2DPlugin Get2DPlugin(string shapeType)
        {
            return _plugin2DMap.ContainsKey(shapeType) ? _plugin2DMap[shapeType] : null;
        }

        /// <summary>
        /// Gets a 3D plugin by shape type name
        /// </summary>
        public IShape3DPlugin Get3DPlugin(string shapeType)
        {
            return _plugin3DMap.ContainsKey(shapeType) ? _plugin3DMap[shapeType] : null;
        }

        /// <summary>
        /// Clears all shapes (built-in and plugins)
        /// </summary>
        public new void ClearAll()
        {
            base.ClearAll();
            _pluginShapes2D.Clear();
            _pluginShapes3D.Clear();
        }

        /// <summary>
        /// Reloads and re-registers all plugins
        /// </summary>
        public void ReloadPlugins()
        {
            // Clear existing plugin mappings
            _plugin2DMap.Clear();
            _plugin3DMap.Clear();

            // Re-register plugins
            RegisterPlugins();
        }

        /// <summary>
        /// Deletes a shape at the specified index
        /// </summary>
        public new void DeleteShape(int index)
        {
            int builtIn2DCount = base.GetAllShapes2D().Count;
            int builtIn3DCount = base.GetAllShapes3D().Count;
            int totalBuiltIn = builtIn2DCount + builtIn3DCount;

            if (index < totalBuiltIn)
            {
                // Delete built-in shape
                base.DeleteShape(index);
            }
            else
            {
                int pluginIndex = index - totalBuiltIn;
                int plugin2DCount = _pluginShapes2D.Count;

                if (pluginIndex < plugin2DCount)
                {
                    // Delete plugin 2D shape
                    _pluginShapes2D.RemoveAt(pluginIndex);
                }
                else
                {
                    // Delete plugin 3D shape
                    int plugin3DIndex = pluginIndex - plugin2DCount;
                    if (plugin3DIndex < _pluginShapes3D.Count)
                    {
                        _pluginShapes3D.RemoveAt(plugin3DIndex);
                    }
                }
            }
        }

        /// <summary>
        /// Gets total count of all shapes (built-in + plugins)
        /// </summary>
        public new int GetShapeCount()
        {
            return base.GetShapeCount() + _pluginShapes2D.Count + _pluginShapes3D.Count;
        }
    }
}