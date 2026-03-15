using System;
using System.Collections.Generic;
using System.Linq;
using GraphicEditor.Models;

namespace GraphicEditor.Models.Shapes3D
{

    /// Tetrahedron 3D shape implementation

    public class Tetrahedron : IShape3D
    {
        public Point3D Center { get; set; }
        public float Size { get; set; }

        public Tetrahedron()
        {
            Center = new Point3D(0, 0, 0);
            Size = 1;
        }

        public string GetName() => "Tetrahedron";

        public List<Point3D> GetPoints3D()
        {
            float half = Size / 2;
            float height = (float)(Size * Math.Sqrt(2.0 / 3.0));

            var vertices = new List<Point3D>
            {
                new Point3D(Center.X, Center.Y + half, Center.Z),
                new Point3D(Center.X - half, Center.Y - half/2, Center.Z + half),
                new Point3D(Center.X + half, Center.Y - half/2, Center.Z + half),
                new Point3D(Center.X, Center.Y - half/2, Center.Z - half)
            };

            var points = new List<Point3D>();
            int[][] edges = new int[][]
            {
                new int[] { 0, 1 }, new int[] { 0, 2 }, new int[] { 0, 3 },
                new int[] { 1, 2 }, new int[] { 1, 3 }, new int[] { 2, 3 }
            };

            foreach (var edge in edges)
            {
                points.Add(vertices[edge[0]]);
                points.Add(vertices[edge[1]]);
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
                Size = parts[3];
            }
        }
    }
}