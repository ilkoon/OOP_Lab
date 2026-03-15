using GraphicEditor.Models.Shapes;
using GraphicEditor.Models.Shapes3D;

namespace GraphicEditor.Factories
{
    /// Base factory interface for creating shapes

    public interface IShapeFactory
    {
        IShape CreateShape(string shapeType);
        IShape3D CreateShape3D(string shapeType);
        string[] GetAvailableShapes();
        string[] GetAvailableShapes3D();
    }
}