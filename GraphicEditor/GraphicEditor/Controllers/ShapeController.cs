using System.Collections.Generic;
using GraphicEditor.Factories;
using GraphicEditor.Models;
using GraphicEditor.Models.Shapes;
using GraphicEditor.Models.Shapes3D;

namespace GraphicEditor.Controllers
{

    /// Controller that handles shape creation and management

    public class ShapeController
    {
        private readonly List<IShape> _shapes2D;
        private readonly List<IShape3D> _shapes3D;
        private readonly ShapeFactoryProvider _factoryProvider;
        private bool _is3DMode;

        public ShapeController()
        {
            _shapes2D = new List<IShape>();
            _shapes3D = new List<IShape3D>();
            _factoryProvider = new ShapeFactoryProvider();
            _is3DMode = false;
        }

        public void SetMode(bool is3DMode)
        {
            _is3DMode = is3DMode;
            _factoryProvider.SetMode(is3DMode);
        }

        public void CreateShape(string shapeType, string parameters)
        {
            if (_is3DMode)
            {
                var shape3D = _factoryProvider.GetCurrentFactory().CreateShape3D(shapeType);
                shape3D.SetParameters(parameters);
                _shapes3D.Add(shape3D);
            }
            else
            {
                var shape = _factoryProvider.GetCurrentFactory().CreateShape(shapeType);
                shape.SetParameters(parameters);
                _shapes2D.Add(shape);
            }
        }

        public List<IShape> GetShapesForRendering()
        {
            var renderShapes = new List<IShape>();
            renderShapes.AddRange(_shapes2D);

            foreach (var shape3D in _shapes3D)
            {
                renderShapes.Add(new ProjectedShapeAdapter(shape3D));
            }

            return renderShapes;
        }

        public string[] GetAvailableShapes()
        {
            return _factoryProvider.GetAvailableShapes();
        }

        public void ClearAll()
        {
            _shapes2D.Clear();
            _shapes3D.Clear();
        }

        private class ProjectedShapeAdapter : IShape
        {
            private readonly IShape3D _shape3D;

            public ProjectedShapeAdapter(IShape3D shape3D)
            {
                _shape3D = shape3D;
            }

            public string GetName() => _shape3D.GetName() + " (3D)";

            public List<Point2D> GetPoints()
            {
                var points3D = _shape3D.GetPoints3D();
                var points2D = new List<Point2D>();

                foreach (var point3D in points3D)
                {
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
}