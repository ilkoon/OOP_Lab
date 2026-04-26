
using System;
using System.Collections.Generic;
using System.Drawing;
using GraphicEditor.Models;
using GraphicEditor.Plugins;

namespace PentagonPlugin
{
    /// <summary>
    /// Pentagon 2D shape plugin
    /// This plugin adds a pentagon shape to the graphic editor
    /// </summary>
    public class Pentagon2DPlugin : IShape2DPlugin
    {
        // Plugin metadata
        public string PluginId => "com.graphiceditor.pentagon";
        public string PluginName => "Pentagon";
        public string Version => "1.0.0";
        public string Author => "GraphicEditor Team";
        public string Description => "Adds a regular pentagon shape to the canvas";
        public string DisplayName => "Pentagon";

        /// <summary>
        /// Creates a new instance of the pentagon shape
        /// </summary>
        public IPluginShape2D CreateShape()
        {
            return new PentagonShape();
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
                DrawPentagon(g, pen, start.X, start.Y, radius);
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

        private void DrawPentagon(Graphics g, Pen pen, float centerX, float centerY, float radius)
        {
            PointF[] points = new PointF[6];
            for (int i = 0; i <= 5; i++)
            {
                // Angle: start from top (-90 degrees), 5 sides = 72 degrees each
                double angle = i * 2 * Math.PI / 5 - Math.PI / 2;
                float x = centerX + radius * (float)Math.Cos(angle);
                float y = centerY + radius * (float)Math.Sin(angle);
                points[i] = new PointF(x, y);
            }
            g.DrawPolygon(pen, points);
        }
    }

    /// <summary>
    /// Pentagon shape implementation
    /// </summary>
    public class PentagonShape : IPluginShape2D
    {
        public Point2D Center { get; set; }
        public float Radius { get; set; }

        public PentagonShape()
        {
            Center = new Point2D();
            Radius = 0;
        }

        public string GetName()
        {
            return "Pentagon";
        }

        public List<Point2D> GetPoints()
        {
            var points = new List<Point2D>();

            // Generate 5 vertices of the pentagon
            for (int i = 0; i <= 5; i++)
            {
                // Angle: start from top (-90 degrees), 5 sides = 72 degrees each
                double angle = i * 2 * Math.PI / 5 - Math.PI / 2;
                float x = Center.X + Radius * (float)Math.Cos(angle);
                float y = Center.Y + Radius * (float)Math.Sin(angle);
                points.Add(new Point2D(x, y));
            }

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