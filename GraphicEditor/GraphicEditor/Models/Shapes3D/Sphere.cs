using System;
using System.Collections.Generic;
using System.Linq;
using GraphicEditor.Models;

namespace GraphicEditor.Models.Shapes3D
{
    /// <summary>
    /// Sphere 3D shape implementation
    /// </summary>
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
            int segments = 25; 
            int circles = 5;   

            for (int j = 1; j <= circles; j++)
            {
                float phi = (float)(Math.PI * j / (circles + 1) - Math.PI / 2);
                float r = Radius * (float)Math.Cos(phi);
                float y = Center.Y + Radius * (float)Math.Sin(phi);

            }

            for (int j = 0; j < circles + 1; j++)
            {
                float theta = (float)(2 * Math.PI * j / (circles + 1));

                for (int i = 0; i <= segments; i++)
                {
                    float phi = (float)(Math.PI * i / segments - Math.PI / 2);
                    float x = Center.X + Radius * (float)Math.Cos(phi) * (float)Math.Cos(theta);
                    float y = Center.Y + Radius * (float)Math.Sin(phi);
                    float z = Center.Z + Radius * (float)Math.Cos(phi) * (float)Math.Sin(theta);
                    points.Add(new Point3D(x, y, z));
                }
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