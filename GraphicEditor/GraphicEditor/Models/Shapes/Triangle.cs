using System.Collections.Generic;
using System.Linq;

namespace GraphicEditor.Models.Shapes
{

    /// Triangle shape implementation

    public class Triangle : IShape
    {
        public Point2D Point1 { get; set; }
        public Point2D Point2 { get; set; }
        public Point2D Point3 { get; set; }

        public Triangle()
        {
            Point1 = new Point2D();
            Point2 = new Point2D();
            Point3 = new Point2D();
        }

        public string GetName() => "Triangle";

        public List<Point2D> GetPoints()
        {
            return new List<Point2D>
            {
                new Point2D(Point1.X, Point1.Y),
                new Point2D(Point2.X, Point2.Y),
                new Point2D(Point3.X, Point3.Y),
                new Point2D(Point1.X, Point1.Y)
            };
        }

        public void SetParameters(string input)
        {
            var parts = input.Split(',').Select(float.Parse).ToArray();
            if (parts.Length >= 6)
            {
                Point1.X = parts[0];
                Point1.Y = parts[1];
                Point2.X = parts[2];
                Point2.Y = parts[3];
                Point3.X = parts[4];
                Point3.Y = parts[5];
            }
        }
    }
}