// ShapeFilterPlugin/ShapeFilterPlugin.cs
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Linq;
using GraphicEditor.Plugins;
using GraphicEditor.Plugins.DataProcessors;

namespace ShapeFilterPlugin
{
    /// <summary>
    /// Data processor plugin that filters shapes when saving
    /// Only keeps selected shape types
    /// </summary>
    public class ShapeFilterProcessor : IDataProcessorPlugin
    {
        // Plugin metadata
        public string PluginId => "com.graphiceditor.shapefilter";
        public string PluginName => "Shape Filter";
        public string Version => "1.0.0";
        public string Author => "GraphicEditor Team";
        public string Description => "Filters shapes when saving (keeps only selected shape types)";

        public string ProcessorName => "Shape Filter";
        public bool IsEnabled { get; set; } = false; // Off by default
        public int ExecutionOrder { get; set; } = 20; // After pretty formatter

        // Settings - which shape types to keep
        private bool _keepCircles = true;
        private bool _keepRectangles = true;
        private bool _keepTriangles = true;
        private bool _keepPentagons = true;
        private bool _keepHexagons = true;
        private bool _keepStars = true;

        public ShapeFilterProcessor()
        {
        }

        /// <summary>
        /// Filters shapes before saving
        /// </summary>
        public string ProcessBeforeSave(string xmlData)
        {
            if (string.IsNullOrWhiteSpace(xmlData))
                return xmlData;

            try
            {
                return FilterShapes(xmlData);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"ShapeFilter error: {ex.Message}");
                return xmlData;
            }
        }

        /// <summary>
        /// Restores original data on load (no filtering needed)
        /// </summary>
        public string ProcessAfterLoad(string xmlData)
        {
            // No filtering on load
            return xmlData;
        }

        /// <summary>
        /// Filters shapes based on selected types
        /// </summary>
        private string FilterShapes(string xmlData)
        {
            XDocument doc = XDocument.Parse(xmlData);

            // Build list of shape types to keep
            List<string> typesToKeep = new List<string>();
            if (_keepCircles) typesToKeep.Add("Circle");
            if (_keepRectangles) typesToKeep.Add("Rectangle");
            if (_keepTriangles) typesToKeep.Add("Triangle");
            if (_keepPentagons) typesToKeep.Add("Pentagon");
            if (_keepHexagons) typesToKeep.Add("Hexagon");
            if (_keepStars) typesToKeep.Add("Star");

            // If no types selected, keep everything
            if (typesToKeep.Count == 0)
                return xmlData;

            // Find all Shape elements and filter
            var shapes = doc.Descendants("Shape");
            var shapesToRemove = shapes
                .Where(shape => !typesToKeep.Contains(shape.Attribute("Type")?.Value))
                .ToList();

            // Remove filtered shapes
            foreach (var shape in shapesToRemove)
            {
                shape.Remove();
            }

            return doc.ToString(SaveOptions.None);
        }

        /// <summary>
        /// Creates settings control for this processor
        /// </summary>
        public Control GetSettingsControl()
        {
            var panel = new Panel
            {
                Size = new System.Drawing.Size(300, 250),
                Padding = new Padding(10)
            };

            // Title
            var lblTitle = new Label
            {
                Text = "Select shape types to KEEP when saving:",
                Location = new System.Drawing.Point(5, 5),
                Size = new System.Drawing.Size(280, 25),
                Font = new System.Drawing.Font("Arial", 10, System.Drawing.FontStyle.Bold)
            };

            // Checkboxes for each shape type
            var chkCircles = new CheckBox
            {
                Text = "Circle",
                Location = new System.Drawing.Point(20, 40),
                Size = new System.Drawing.Size(120, 25),
                Checked = _keepCircles
            };

            var chkRectangles = new CheckBox
            {
                Text = "Rectangle",
                Location = new System.Drawing.Point(20, 70),
                Size = new System.Drawing.Size(120, 25),
                Checked = _keepRectangles
            };

            var chkTriangles = new CheckBox
            {
                Text = "Triangle",
                Location = new System.Drawing.Point(20, 100),
                Size = new System.Drawing.Size(120, 25),
                Checked = _keepTriangles
            };

            var chkPentagons = new CheckBox
            {
                Text = "Pentagon",
                Location = new System.Drawing.Point(160, 40),
                Size = new System.Drawing.Size(120, 25),
                Checked = _keepPentagons
            };

            var chkHexagons = new CheckBox
            {
                Text = "Hexagon",
                Location = new System.Drawing.Point(160, 70),
                Size = new System.Drawing.Size(120, 25),
                Checked = _keepHexagons
            };

            var chkStars = new CheckBox
            {
                Text = "Star",
                Location = new System.Drawing.Point(160, 100),
                Size = new System.Drawing.Size(120, 25),
                Checked = _keepStars
            };

            // Info label
            var lblInfo = new Label
            {
                Text = "Note: Only selected shape types will be saved to file.\nUnselected shapes will be filtered out.",
                Location = new System.Drawing.Point(5, 140),
                Size = new System.Drawing.Size(280, 50),
                ForeColor = System.Drawing.Color.Gray,
                Font = new System.Drawing.Font("Arial", 8)
            };

            // Apply button
            var btnApply = new Button
            {
                Text = "Apply",
                Location = new System.Drawing.Point(5, 200),
                Size = new System.Drawing.Size(80, 30),
                BackColor = System.Drawing.Color.LightGreen
            };

            panel.Controls.AddRange(new Control[] {
                lblTitle, chkCircles, chkRectangles, chkTriangles,
                chkPentagons, chkHexagons, chkStars, lblInfo, btnApply
            });

            // Store settings reference
            btnApply.Tag = new { chkCircles, chkRectangles, chkTriangles, chkPentagons, chkHexagons, chkStars };
            btnApply.Click += (s, e) =>
            {
                _keepCircles = chkCircles.Checked;
                _keepRectangles = chkRectangles.Checked;
                _keepTriangles = chkTriangles.Checked;
                _keepPentagons = chkPentagons.Checked;
                _keepHexagons = chkHexagons.Checked;
                _keepStars = chkStars.Checked;
            };

            return panel;
        }

        /// <summary>
        /// Applies settings from the control
        /// </summary>
        public void ApplySettings()
        {
            // Settings are applied via the Apply button
        }

        /// <summary>
        /// Updates settings programmatically
        /// </summary>
        public void UpdateSettings(bool circles, bool rectangles, bool triangles,
                                   bool pentagons, bool hexagons, bool stars)
        {
            _keepCircles = circles;
            _keepRectangles = rectangles;
            _keepTriangles = triangles;
            _keepPentagons = pentagons;
            _keepHexagons = hexagons;
            _keepStars = stars;
        }
    }
}