namespace GraphicEditor.Models
{

    /// Represents a 2D point with X and Y coordinates

    public class Point2D
    {
        public float X { get; set; }
        public float Y { get; set; }

        public Point2D(float x, float y)
        {
            X = x;
            Y = y;
        }

        public Point2D()
        {
            X = 0;
            Y = 0;
        }
    }
}