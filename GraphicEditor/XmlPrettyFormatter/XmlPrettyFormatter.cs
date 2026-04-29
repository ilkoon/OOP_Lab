// XmlPrettyFormatter/XmlPrettyFormatter.cs
using System;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using GraphicEditor.Plugins;
using GraphicEditor.Plugins.DataProcessors;

namespace XmlPrettyFormatter
{
    /// <summary>
    /// Data processor plugin that formats XML with proper indentation
    /// </summary>
    public class XmlPrettyFormatterPlugin : IDataProcessorPlugin
    {
        // Plugin metadata
        public string PluginId => "com.graphiceditor.xmlprettyformatter";
        public string PluginName => "XML Pretty Formatter";
        public string Version => "1.0.0";
        public string Author => "GraphicEditor Team";
        public string Description => "Formats XML with proper indentation and line breaks";

        public string ProcessorName => "XML Pretty Formatter";
        public bool IsEnabled { get; set; } = true;
        public int ExecutionOrder { get; set; } = 10;

        // Settings
        private int _indentSize = 2;
        private bool _addXmlDeclaration = true;

        // Path for saving settings
        private string _settingsPath;

        public XmlPrettyFormatterPlugin()
        {
            _settingsPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Plugins", "XmlPrettyFormatter_settings.xml");
            LoadSettings();
        }

        private void LoadSettings()
        {
            try
            {
                if (File.Exists(_settingsPath))
                {
                    XmlDocument doc = new XmlDocument();
                    doc.Load(_settingsPath);
                    _indentSize = int.Parse(doc.SelectSingleNode("/Settings/IndentSize")?.InnerText ?? "2");
                    _addXmlDeclaration = bool.Parse(doc.SelectSingleNode("/Settings/AddXmlDeclaration")?.InnerText ?? "true");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"XmlPrettyFormatter load settings error: {ex.Message}");
            }
        }

        private void SaveSettings()
        {
            try
            {
                XmlDocument doc = new XmlDocument();
                XmlDeclaration decl = doc.CreateXmlDeclaration("1.0", "utf-8", null);
                doc.AppendChild(decl);

                XmlElement root = doc.CreateElement("Settings");
                doc.AppendChild(root);

                XmlElement indentElem = doc.CreateElement("IndentSize");
                indentElem.InnerText = _indentSize.ToString();
                root.AppendChild(indentElem);

                XmlElement declElem = doc.CreateElement("AddXmlDeclaration");
                declElem.InnerText = _addXmlDeclaration.ToString();
                root.AppendChild(declElem);

                doc.Save(_settingsPath);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"XmlPrettyFormatter save settings error: {ex.Message}");
            }
        }

        /// <summary>
        /// Formats XML before saving (makes it pretty)
        /// </summary>
        public string ProcessBeforeSave(string xmlData)
        {
            if (string.IsNullOrWhiteSpace(xmlData))
                return xmlData;

            try
            {
                return FormatXml(xmlData);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"XmlPrettyFormatter error: {ex.Message}");
                return xmlData;
            }
        }

        /// <summary>
        /// Loads XML after loading (no change needed)
        /// </summary>
        public string ProcessAfterLoad(string xmlData)
        {
            return xmlData;
        }

        /// <summary>
        /// Formats XML string with proper indentation
        /// </summary>
        private string FormatXml(string xmlData)
        {
            try
            {
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(xmlData);

                using (var stringWriter = new StringWriter())
                {
                    var settings = new XmlWriterSettings
                    {
                        Indent = true,
                        IndentChars = new string(' ', _indentSize),
                        NewLineChars = Environment.NewLine,
                        NewLineHandling = NewLineHandling.Replace,
                        OmitXmlDeclaration = !_addXmlDeclaration,
                        Encoding = Encoding.UTF8
                    };

                    using (var xmlWriter = XmlWriter.Create(stringWriter, settings))
                    {
                        doc.Save(xmlWriter);
                    }

                    return stringWriter.ToString();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"FormatXml error: {ex.Message}");
                return xmlData;
            }
        }

        /// <summary>
        /// Creates settings control for this processor
        /// </summary>
        public Control GetSettingsControl()
        {
            var panel = new Panel
            {
                Size = new System.Drawing.Size(350, 250),
                Padding = new Padding(10)
            };

            int y = 5;

            // Title
            var lblTitle = new Label
            {
                Text = "XML PRETTY FORMATTER SETTINGS",
                Location = new System.Drawing.Point(5, y),
                Size = new System.Drawing.Size(320, 30),
                Font = new System.Drawing.Font("Arial", 10, System.Drawing.FontStyle.Bold),
                ForeColor = System.Drawing.Color.Blue,
                TextAlign = ContentAlignment.MiddleCenter
            };
            y += 40;

            // Indent size label
            var lblIndent = new Label
            {
                Text = "Indent size (spaces):",
                Location = new System.Drawing.Point(5, y),
                Size = new System.Drawing.Size(140, 25)
            };

            // Indent size numeric up down
            var numIndent = new NumericUpDown
            {
                Location = new System.Drawing.Point(150, y),
                Size = new System.Drawing.Size(60, 25),
                Minimum = 1,
                Maximum = 8,
                Value = _indentSize
            };
            y += 40;

            // Add XML declaration checkbox
            var chkDeclaration = new CheckBox
            {
                Text = "Add XML declaration (<?xml version=\"1.0\"?>)",
                Location = new System.Drawing.Point(5, y),
                Size = new System.Drawing.Size(320, 25),
                Checked = _addXmlDeclaration
            };
            y += 50;

            // Preview label
            var lblPreview = new Label
            {
                Text = "Preview of indentation:",
                Location = new System.Drawing.Point(5, y),
                Size = new System.Drawing.Size(200, 20),
                Font = new System.Drawing.Font("Arial", 8, System.Drawing.FontStyle.Bold)
            };
            y += 22;

            var txtPreview = new TextBox
            {
                Location = new System.Drawing.Point(5, y),
                Size = new System.Drawing.Size(320, 60),
                Multiline = true,
                ReadOnly = true,
                Font = new System.Drawing.Font("Consolas", 8),
                BackColor = System.Drawing.Color.WhiteSmoke,
                Text = $"<root>{Environment.NewLine}{new string(' ', _indentSize)}<child/>{Environment.NewLine}</root>"
            };
            y += 70;

            // Update preview when settings change
            numIndent.ValueChanged += (s, e) =>
            {
                int spaces = (int)numIndent.Value;
                txtPreview.Text = $"<root>{Environment.NewLine}{new string(' ', spaces)}<child/>{Environment.NewLine}</root>";
            };

            // Apply button
            var btnApply = new Button
            {
                Text = "Apply Pretty Formatter Settings",
                Location = new System.Drawing.Point(5, y),
                Size = new System.Drawing.Size(200, 35),
                BackColor = System.Drawing.Color.LightGreen,
                FlatStyle = FlatStyle.Flat,
                Font = new System.Drawing.Font("Arial", 9, System.Drawing.FontStyle.Bold)
            };

            btnApply.Click += (s, e) =>
            {
                _indentSize = (int)numIndent.Value;
                _addXmlDeclaration = chkDeclaration.Checked;
                SaveSettings();

                btnApply.Text = "✓ Applied!";
                btnApply.BackColor = System.Drawing.Color.Green;

                MessageBox.Show($"XML Pretty Formatter settings applied!\n\n" +
                    $"Indent size: {_indentSize} spaces\n" +
                    $"XML Declaration: {(_addXmlDeclaration ? "Yes" : "No")}",
                    "Settings Applied", MessageBoxButtons.OK, MessageBoxIcon.Information);

                var timer = new Timer { Interval = 1500 };
                timer.Tick += (t, ev) =>
                {
                    btnApply.Text = "Apply Pretty Formatter Settings";
                    btnApply.BackColor = System.Drawing.Color.LightGreen;
                    timer.Stop();
                };
                timer.Start();
            };

            panel.Controls.AddRange(new Control[] {
                lblTitle, lblIndent, numIndent, chkDeclaration,
                lblPreview, txtPreview, btnApply
            });

            return panel;
        }

        /// <summary>
        /// Applies settings from the control
        /// </summary>
        public void ApplySettings()
        {
            // Settings are applied via the Apply button in the control
        }
    }
}