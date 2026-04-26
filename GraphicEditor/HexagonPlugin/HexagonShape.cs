// HexagonPlugin/HexagonShape.cs
using System;
using System.Collections.Generic;
using System.Drawing;
using GraphicEditor.Models;
using GraphicEditor.Plugins;

namespace HexagonPlugin
{
    /// <summary>
    /// Hexagon 2D shape plugin
    /// This plugin adds a regular hexagon shape to the graphic editor
    /// </summary>
    public class Hexagon2DPlugin : IShape2DPlugin
    {
        // Plugin metadata - уникальные идентификаторы для каждого плагина
        public string PluginId => "com.graphiceditor.hexagon";
        public string PluginName => "Hexagon";
        public string Version => "1.0.0";
        public string Author => "GraphicEditor Team";
        public string Description => "Adds a regular hexagon shape to the canvas";
        public string DisplayName => "Hexagon";

        /// <summary>
        /// Creates a new instance of the hexagon shape
        /// </summary>
        public IPluginShape2D CreateShape()
        {
            return new HexagonShape();
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
                DrawHexagon(g, pen, start.X, start.Y, radius);
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

        private void DrawHexagon(Graphics g, Pen pen, float centerX, float centerY, float radius)
        {
            PointF[] points = new PointF[7]; // 6 points + closing point
            for (int i = 0; i <= 6; i++)
            {
                // Hexagon has 6 sides = 60 degrees each
                // Start from top (-90 degrees)
                double angle = i * 2 * Math.PI / 6 - Math.PI / 2;
                float x = centerX + radius * (float)Math.Cos(angle);
                float y = centerY + radius * (float)Math.Sin(angle);
                points[i] = new PointF(x, y);
            }
            g.DrawPolygon(pen, points);
        }
    }

    /// <summary>
    /// Hexagon shape implementation
    /// </summary>
    public class HexagonShape : IPluginShape2D
    {
        public Point2D Center { get; set; }
        public float Radius { get; set; }

        public HexagonShape()
        {
            Center = new Point2D();
            Radius = 0;
        }

        public string GetName()
        {
            return "Hexagon";
        }

        public List<Point2D> GetPoints()
        {
            var points = new List<Point2D>();

            // Generate 6 vertices of the hexagon
            for (int i = 0; i <= 6; i++)
            {
                // Hexagon has 6 sides = 60 degrees each
                // Start from top (-90 degrees)
                double angle = i * 2 * Math.PI / 6 - Math.PI / 2;
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