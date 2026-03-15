using GraphicEditor.Models;

namespace GraphicEditor.Models
{

    /// Handles projection of 3D points to 2D for rendering

    public static class Projection
    {

        /// Projects a 3D point to 2D using orthographic projection

        public static Point2D ProjectTo2D(Point3D point3D)
        {
            return new Point2D(point3D.X, point3D.Y);
        }
    }
}