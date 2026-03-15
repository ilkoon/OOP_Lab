using System;
using System.Collections.Generic;
using GraphicEditor.Models.Shapes;
using GraphicEditor.Models.Shapes3D;

namespace GraphicEditor.Factories
{

    /// Factory for creating 3D shapes

    public class Shape3DFactory : IShapeFactory
    {
        private readonly Dictionary<string, Func<IShape3D>> _shapeCreators3D;

        public Shape3DFactory()
        {
            _shapeCreators3D = new Dictionary<string, Func<IShape3D>>
            {
                { "Cube", () => new Cube() },
                { "Sphere", () => new Sphere() },
                { "Tetrahedron", () => new Tetrahedron() }
            };
        }

        public IShape CreateShape(string shapeType)
        {
            throw new NotSupportedException("3D factory cannot create 2D shapes");
        }

        public IShape3D CreateShape3D(string shapeType)
        {
            if (_shapeCreators3D.ContainsKey(shapeType))
            {
                return _shapeCreators3D[shapeType]();
            }
            throw new ArgumentException($"Unknown 3D shape type: {shapeType}");
        }

        public string[] GetAvailableShapes()
        {
            return new string[0];
        }

        public string[] GetAvailableShapes3D()
        {
            return new List<string>(_shapeCreators3D.Keys).ToArray();
        }
    }
}