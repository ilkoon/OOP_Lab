using System;
using System.Collections.Generic;

namespace GraphicEditor.Models
{
    /// <summary>
    /// Data Transfer Object for shape serialization
    /// Stores shape information in a format suitable for JSON serialization
    /// </summary>
    public class ShapeData
    {
        /// <summary>
        /// Shape mode: "2D" or "3D"
        /// </summary>
        public string Mode { get; set; }

        /// <summary>
        /// Shape type: Circle, Rectangle, Triangle, Cube, Sphere, Tetrahedron
        /// </summary>
        public string ShapeType { get; set; }

        /// <summary>
        /// Parameters string that defines the shape (e.g., "100,100,50" for circle)
        /// </summary>
        public string Parameters { get; set; }
    }

    /// <summary>
    /// Container class for saving/loading all shapes
    /// </summary>
    public class ShapesContainer
    {
        /// <summary>
        /// List of all shapes
        /// </summary>
        public List<ShapeData> Shapes { get; set; } = new List<ShapeData>();

        /// <summary>
        /// File format version for future compatibility
        /// </summary>
        public int Version { get; set; } = 1;

        /// <summary>
        /// Timestamp when the file was saved
        /// </summary>
        public DateTime SavedAt { get; set; }
    }
}