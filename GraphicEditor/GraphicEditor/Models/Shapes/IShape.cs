using System.Collections.Generic;

namespace GraphicEditor.Models.Shapes
{
 
    /// Base interface for all 2D shapes

    public interface IShape
    {

        /// Gets the list of points that define the shape

        List<Point2D> GetPoints();


        /// Gets the name of the shape

        string GetName();


        /// Sets shape parameters from user input

        void SetParameters(string input);
    }
}