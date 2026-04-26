using System;
using System.Collections.Generic;
using System.Drawing;
using GraphicEditor.Models;

namespace GraphicEditor.Plugins
{
    /// <summary>
    /// Interface for plugin metadata (identification info)
    /// </summary>
    public interface IPluginMetadata
    {
        string PluginId { get; }      // Unique identifier like "com.mycompany.myshape"
        string PluginName { get; }    // Display name like "Star Shape"
        string Version { get; }       // Version like "1.0.0"
        string Author { get; }        // Author name
        string Description { get; }   // Short description
    }

    /// <summary>
    /// Interface for 2D shape plugin
    /// </summary>
    public interface IShape2DPlugin : IPluginMetadata
    {
        /// <summary>
        /// Creates a new instance of the shape
        /// </summary>
        IPluginShape2D CreateShape();

        /// <summary>
        /// Gets the display name for UI (same as PluginName usually)
        /// </summary>
        string DisplayName { get; }

        /// <summary>
        /// Preview drawing while dragging on canvas
        /// </summary>
        void DrawPreview(Graphics g, Point start, Point current);

        /// <summary>
        /// Generates parameters string from mouse coordinates
        /// </summary>
        string GenerateParameters(Point start, Point end);
    }

    /// <summary>
    /// Interface for 3D shape plugin
    /// </summary>
    public interface IShape3DPlugin : IPluginMetadata
    {
        /// <summary>
        /// Creates a new instance of the 3D shape
        /// </summary>
        IPluginShape3D CreateShape();

        /// <summary>
        /// Gets the display name for UI
        /// </summary>
        string DisplayName { get; }

        /// <summary>
        /// Preview drawing while dragging on canvas
        /// </summary>
        void DrawPreview(Graphics g, Point start, Point current);

        /// <summary>
        /// Generates parameters string from mouse coordinates
        /// </summary>
        string GenerateParameters(Point start, Point end);
    }

    /// <summary>
    /// Interface for plugin shape instance (2D)
    /// </summary>
    public interface IPluginShape2D
    {
        string GetName();
        List<Point2D> GetPoints();
        void SetParameters(string input);
    }

    /// <summary>
    /// Interface for plugin shape instance (3D)
    /// </summary>
    public interface IPluginShape3D
    {
        string GetName();
        List<Point3D> GetPoints3D();
        void SetParameters(string input);
    }
}