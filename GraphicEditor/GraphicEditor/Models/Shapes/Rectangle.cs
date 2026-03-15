using System.Collections.Generic;
using System.Linq;

namespace GraphicEditor.Models.Shapes
{

    /// Rectangle shape implementation

    public class Rectangle : IShape
    {
        public Point2D TopLeft { get; set; }
        public float Width { get; set; }
        public float Height { get; set; }

        public Rectangle()
        {
            TopLeft = new Point2D();
            Width = 0;
            Height = 0;
        }

        public string GetName() => "Rectangle";

        public List<Point2D> GetPoints()
        {
            return new List<Point2D>
            {
                new Point2D(TopLeft.X, TopLeft.Y),
                new Point2D(TopLeft.X + Width, TopLeft.Y),
                new Point2D(TopLeft.X + Width, TopLeft.Y + Height),
                new Point2D(TopLeft.X, TopLeft.Y + Height),
                new Point2D(TopLeft.X, TopLeft.Y)
            };
        }

        public void SetParameters(string input)
        {
            var parts = input.Split(',').Select(float.Parse).ToArray();
            if (parts.Length >= 4)
            {
                TopLeft.X = parts[0];
                TopLeft.Y = parts[1];
                Width = parts[2];
                Height = parts[3];
            }
        }
    }
}