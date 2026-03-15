using GraphicEditor.Models.Shapes;
using GraphicEditor.Models.Shapes3D;
using System;
using System.Collections.Generic;

namespace GraphicEditor.Factories
{

    /// Factory for creating 2D shapes

    public class Shape2DFactory : IShapeFactory
    {
        private readonly Dictionary<string, Func<IShape>> _shapeCreators;

        public Shape2DFactory()
        {
            _shapeCreators = new Dictionary<string, Func<IShape>>
            {
                { "Circle", () => new Circle() },
                { "Rectangle", () => new Models.Shapes.Rectangle() },
                { "Triangle", () => new Triangle() }
            };
        }

        public IShape CreateShape(string shapeType)
        {
            if (_shapeCreators.ContainsKey(shapeType))
            {
                return _shapeCreators[shapeType]();
            }
            throw new ArgumentException($"Unknown shape type: {shapeType}");
        }

        public IShape3D CreateShape3D(string shapeType)
        {
            throw new NotSupportedException("2D factory cannot create 3D shapes");
        }

        public string[] GetAvailableShapes()
        {
            return new List<string>(_shapeCreators.Keys).ToArray();
        }

        public string[] GetAvailableShapes3D()
        {
            return new string[0];
        }
    }
}