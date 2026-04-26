// StarPlugin/StarShape.cs
using System;
using System.Collections.Generic;
using System.Drawing;
using GraphicEditor.Models;
using GraphicEditor.Plugins;

namespace StarPlugin
{
    /// <summary>
    /// Star 2D shape plugin (5-point star)
    /// This plugin adds a star shape to the graphic editor
    /// </summary>
    public class Star2DPlugin : IShape2DPlugin
    {
        // Plugin metadata
        public string PluginId => "com.graphiceditor.star";
        public string PluginName => "Star";
        public string Version => "1.0.0";
        public string Author => "GraphicEditor Team";
        public string Description => "Adds a 5-point star shape to the canvas";
        public string DisplayName => "Star";

        /// <summary>
        /// Creates a new instance of the star shape
        /// </summary>
        public IPluginShape2D CreateShape()
        {
            return new StarShape();
        }

        /// <summary>
        /// Draws preview while dragging mouse
        /// </summary>
        public void DrawPreview(Graphics g, Point start, Point current)
        {
            using (Pen pen = new Pen(Color.Gray, 1))
            {
                pen.DashStyle = System.Drawing.Drawing2D.DashStyle.Dash;
                int radius = CalculateRadius(start, current);
                DrawStar(g, pen, start.X, start.Y, radius);
            }
        }

        /// <summary>
        /// Generates parameters string from mouse coordinates
        /// Format: "centerX,centerY,radius"
        /// </summary>
        public string GenerateParameters(Point start, Point end)
        {
            int radius = CalculateRadius(start, end);
            return $"{start.X},{start.Y},{radius}";
        }

        private int CalculateRadius(Point center, Point edge)
        {
            return (int)Math.Sqrt(
                Math.Pow(edge.X - center.X, 2) +
                Math.Pow(edge.Y - center.Y, 2));
        }

        private void DrawStar(Graphics g, Pen pen, float centerX, float centerY, float outerRadius)
        {
            int points = 5; // 5-point star
            float innerRadius = outerRadius * 0.4f; // Inner radius for the star's indentations
            PointF[] starPoints = new PointF[points * 2 + 1];

            for (int i = 0; i < points * 2; i++)
            {
                double angle = i * Math.PI / points - Math.PI / 2;
                float radius = (i % 2 == 0) ? outerRadius : innerRadius;
                float x = centerX + radius * (float)Math.Cos(angle);
                float y = centerY + radius * (float)Math.Sin(angle);
                starPoints[i] = new PointF(x, y);
            }
            // Close the star
            starPoints[points * 2] = starPoints[0];

            g.DrawPolygon(pen, starPoints);
        }
    }

    /// <summary>
    /// Star shape implementation (5-point star)
    /// </summary>
    public class StarShape : IPluginShape2D
    {
        public Point2D Center { get; set; }
        public float Radius { get; set; }

        public StarShape()
        {
            Center = new Point2D();
            Radius = 0;
        }

        public string GetName()
        {
            return "Star";
        }

        public List<Point2D> GetPoints()
        {
            var points = new List<Point2D>();
            int starPoints = 5;
            float innerRadius = Radius * 0.4f;

            // Generate star vertices (alternating outer and inner points)
            for (int i = 0; i < starPoints * 2; i++)
            {
                double angle = i * Math.PI / starPoints - Math.PI / 2;
                float radius = (i % 2 == 0) ? Radius : innerRadius;
                float x = Center.X + radius * (float)Math.Cos(angle);
                float y = Center.Y + radius * (float)Math.Sin(angle);
                points.Add(new Point2D(x, y));
            }
            // Close the shape by adding the first point again
            points.Add(points[0]);

            return points;
        }

        public void SetParameters(string input)
        {
            var parts = input.Split(',');
            if (parts.Length >= 3)
            {
                Center.X = float.Parse(parts[0]);
                Center.Y = float.Parse(parts[1]);
                Radius = float.Parse(parts[2]);
            }
        }
    }
}