using System.Collections.Generic;
using GraphicEditor.Models;

namespace GraphicEditor.Models.Shapes3D
{

    /// Base interface for all 3D shapes

    public interface IShape3D
    {
        string GetName();
        List<Point3D> GetPoints3D();
        void SetParameters(string input);
    }
}