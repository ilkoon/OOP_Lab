using System;
using System.Collections.Generic;
using System.Linq;

namespace GraphicEditor.Models.Shapes
{

    /// Circle shape implementation

    public class Circle : IShape
    {
        public Point2D Center { get; set; }
        public float Radius { get; set; }

        public Circle()
        {
            Center = new Point2D();
            Radius = 0;
        }

        public string GetName() => "Circle";

        public List<Point2D> GetPoints()
        {
            var points = new List<Point2D>();
            int segments = 30;
            for (int i = 0; i <= segments; i++)
            {
                float angle = (float)(2 * Math.PI * i / segments);
                points.Add(new Point2D(
                    Center.X + Radius * (float)Math.Cos(angle),
                    Center.Y + Radius * (float)Math.Sin(angle)
                ));
            }
            return points;
        }

        public void SetParameters(string input)
        {
            var parts = input.Split(',').Select(float.Parse).ToArray();
            if (parts.Length >= 3)
            {
                Center.X = parts[0];
                Center.Y = parts[1];
                Radius = parts[2];
            }
        }
    }
}