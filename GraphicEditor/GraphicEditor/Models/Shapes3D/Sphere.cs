using System;
using System.Collections.Generic;
using System.Linq;
using GraphicEditor.Models;

namespace GraphicEditor.Models.Shapes3D
{

    /// Sphere 3D shape implementation

    public class Sphere : IShape3D
    {
        public Point3D Center { get; set; }
        public float Radius { get; set; }

        public Sphere()
        {
            Center = new Point3D(0, 0, 0);
            Radius = 1;
        }

        public string GetName() => "Sphere";

        public List<Point3D> GetPoints3D()
        {
            var points = new List<Point3D>();
            int segments = 20;

            for (int i = 0; i <= segments; i++)
            {
                float theta = (float)(2 * Math.PI * i / segments);

                points.Add(new Point3D(
                    Center.X + Radius * (float)Math.Cos(theta),
                    Center.Y + Radius * (float)Math.Sin(theta),
                    Center.Z
                ));

                points.Add(new Point3D(
                    Center.X + Radius * (float)Math.Cos(theta),
                    Center.Y,
                    Center.Z + Radius * (float)Math.Sin(theta)
                ));

                points.Add(new Point3D(
                    Center.X,
                    Center.Y + Radius * (float)Math.Cos(theta),
                    Center.Z + Radius * (float)Math.Sin(theta)
                ));
            }

            return points;
        }

        public void SetParameters(string input)
        {
            var parts = input.Split(',').Select(float.Parse).ToArray();
            if (parts.Length >= 4)
            {
                Center.X = parts[0];
                Center.Y = parts[1];
                Center.Z = parts[2];
                Radius = parts[3];
            }
        }
    }
}