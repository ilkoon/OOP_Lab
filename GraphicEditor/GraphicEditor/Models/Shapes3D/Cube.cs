using System.Collections.Generic;
using System.Linq;
using GraphicEditor.Models;

namespace GraphicEditor.Models.Shapes3D
{

    /// Cube 3D shape implementation

    public class Cube : IShape3D
    {
        public Point3D Center { get; set; }
        public float Size { get; set; }

        public Cube()
        {
            Center = new Point3D(0, 0, 0);
            Size = 1;
        }

        public string GetName() => "Cube";

        public List<Point3D> GetPoints3D()
        {
            float half = Size / 2;
            return new List<Point3D>
            {
                // Front face
                new Point3D(Center.X - half, Center.Y - half, Center.Z - half),
                new Point3D(Center.X + half, Center.Y - half, Center.Z - half),
                new Point3D(Center.X + half, Center.Y + half, Center.Z - half),
                new Point3D(Center.X - half, Center.Y + half, Center.Z - half),
                new Point3D(Center.X - half, Center.Y - half, Center.Z - half),
                
                // Back face
                new Point3D(Center.X - half, Center.Y - half, Center.Z + half),
                new Point3D(Center.X + half, Center.Y - half, Center.Z + half),
                new Point3D(Center.X + half, Center.Y + half, Center.Z + half),
                new Point3D(Center.X - half, Center.Y + half, Center.Z + half),
                new Point3D(Center.X - half, Center.Y - half, Center.Z + half),
                
                // Connecting edges
                new Point3D(Center.X - half, Center.Y - half, Center.Z - half),
                new Point3D(Center.X - half, Center.Y - half, Center.Z + half),
                new Point3D(Center.X + half, Center.Y - half, Center.Z - half),
                new Point3D(Center.X + half, Center.Y - half, Center.Z + half),
                new Point3D(Center.X + half, Center.Y + half, Center.Z - half),
                new Point3D(Center.X + half, Center.Y + half, Center.Z + half),
                new Point3D(Center.X - half, Center.Y + half, Center.Z - half),
                new Point3D(Center.X - half, Center.Y + half, Center.Z + half)
            };
        }

        public void SetParameters(string input)
        {
            var parts = input.Split(',').Select(float.Parse).ToArray();
            if (parts.Length >= 4)
            {
                Center.X = parts[0];
                Center.Y = parts[1];
                Center.Z = parts[2];
                Size = parts[3];
            }
        }
    }
}