// GraphicEditor/Adapters/PluginAdapters.cs
using System.Collections.Generic;
using GraphicEditor.Models;
using GraphicEditor.Models.Shapes;
using GraphicEditor.Models.Shapes3D;
using GraphicEditor.Plugins;

namespace GraphicEditor.Adapters
{
    /// <summary>
    /// Adapts a plugin 2D shape to the IShape interface
    /// This allows plugin shapes to be used alongside built-in shapes
    /// </summary>
    public class PluginShape2DAdapter : IShape
    {
        private readonly IPluginShape2D _pluginShape;

        public PluginShape2DAdapter(IPluginShape2D pluginShape)
        {
            _pluginShape = pluginShape;
        }

        public string GetName()
        {
            return _pluginShape.GetName();
        }

        public List<Point2D> GetPoints()
        {
            return _pluginShape.GetPoints();
        }

        public void SetParameters(string input)
        {
            _pluginShape.SetParameters(input);
        }
    }

    /// <summary>
    /// Adapts a plugin 3D shape to the IShape3D interface
    /// This allows plugin 3D shapes to be used alongside built-in 3D shapes
    /// </summary>
    public class PluginShape3DAdapter : IShape3D
    {
        private readonly IPluginShape3D _pluginShape;

        public PluginShape3DAdapter(IPluginShape3D pluginShape)
        {
            _pluginShape = pluginShape;
        }

        public string GetName()
        {
            return _pluginShape.GetName();
        }

        public List<Point3D> GetPoints3D()
        {
            return _pluginShape.GetPoints3D();
        }

        public void SetParameters(string input)
        {
            _pluginShape.SetParameters(input);
        }
    }

    /// <summary>
    /// Adapts a plugin 3D shape to IShape for 2D rendering (projection)
    /// This takes a 3D shape, projects it to 2D, and presents it as an IShape
    /// </summary>
    public class ProjectedPluginShapeAdapter : IShape
    {
        private readonly IPluginShape3D _shape3D;

        public ProjectedPluginShapeAdapter(IPluginShape3D shape3D)
        {
            _shape3D = shape3D;
        }

        public string GetName()
        {
            return _shape3D.GetName() + " (3D Plugin)";
        }

        public List<Point2D> GetPoints()
        {
            var points3D = _shape3D.GetPoints3D();
            var points2D = new List<Point2D>();

            foreach (var point3D in points3D)
            {
                // Use orthographic projection (drop Z coordinate)
                points2D.Add(Projection.ProjectTo2D(point3D));
            }

            return points2D;
        }

        public void SetParameters(string input)
        {
            _shape3D.SetParameters(input);
        }
    }
}