// MetadataAdderPlugin/MetadataAdderPlugin.cs
using GraphicEditor.Plugins;
using GraphicEditor.Plugins.DataProcessors;
using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Linq;

namespace MetadataAdderPlugin
{
    /// <summary>
    /// Data processor plugin that adds metadata to XML when saving
    /// </summary>
    public class MetadataAdderProcessor : IDataProcessorPlugin
    {
        // Plugin metadata
        public string PluginId => "com.graphiceditor.metadataadder";
        public string PluginName => "Metadata Adder";
        public string Version => "1.0.0";
        public string Author => "GraphicEditor Team";
        public string Description => "Adds metadata (timestamp, application info) to saved XML";

        public string ProcessorName => "Metadata Adder";
        public bool IsEnabled { get; set; } = true;
        public int ExecutionOrder { get; set; } = 5;

        // Settings
        private string _appName = "GraphicEditor";
        private string _dateFormat = "yyyy-MM-dd HH:mm:ss";
        private bool _includeAppVersion = true;
        private bool _includeUsername = true;

        // Path for saving settings
        private string _settingsPath;

        public MetadataAdderProcessor()
        {
            _settingsPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Plugins", "MetadataAdder_settings.xml");
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
                    _appName = doc.SelectSingleNode("/Settings/AppName")?.InnerText ?? "GraphicEditor";
                    _dateFormat = doc.SelectSingleNode("/Settings/DateFormat")?.InnerText ?? "yyyy-MM-dd HH:mm:ss";
                    _includeAppVersion = bool.Parse(doc.SelectSingleNode("/Settings/IncludeAppVersion")?.InnerText ?? "true");
                    _includeUsername = bool.Parse(doc.SelectSingleNode("/Settings/IncludeUsername")?.InnerText ?? "true");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"MetadataAdder load settings error: {ex.Message}");
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

                XmlElement appNameElem = doc.CreateElement("AppName");
                appNameElem.InnerText = _appName;
                root.AppendChild(appNameElem);

                XmlElement dateFormatElem = doc.CreateElement("DateFormat");
                dateFormatElem.InnerText = _dateFormat;
                root.AppendChild(dateFormatElem);

                XmlElement includeVersionElem = doc.CreateElement("IncludeAppVersion");
                includeVersionElem.InnerText = _includeAppVersion.ToString();
                root.AppendChild(includeVersionElem);

                XmlElement includeUserElem = doc.CreateElement("IncludeUsername");
                includeUserElem.InnerText = _includeUsername.ToString();
                root.AppendChild(includeUserElem);

                doc.Save(_settingsPath);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"MetadataAdder save settings error: {ex.Message}");
            }
        }

        /// <summary>
        /// Adds metadata to XML before saving
        /// </summary>
        public string ProcessBeforeSave(string xmlData)
        {
            if (string.IsNullOrWhiteSpace(xmlData))
                return xmlData;

            try
            {
                return AddMetadata(xmlData);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"MetadataAdder error: {ex.Message}");
                return xmlData;
            }
        }

        /// <summary>
        /// Removes metadata when loading (optional)
        /// </summary>
        public string ProcessAfterLoad(string xmlData)
        {
            return xmlData;
        }

        /// <summary>
        /// Adds metadata to the XML document
        /// </summary>
        private string AddMetadata(string xmlData)
        {
            XDocument doc = XDocument.Parse(xmlData);

            XElement root = doc.Root;
            if (root == null)
                return xmlData;

            // Find or create Metadata element (as first child)
            XElement metadata = root.Element("Metadata");
            if (metadata == null)
            {
                metadata = new XElement("Metadata");
                root.AddFirst(metadata);
            }
            else
            {
                metadata.RemoveNodes(); // Clear old metadata
            }

            // Add timestamp with selected format
            string timestamp = DateTime.Now.ToString(_dateFormat);
            metadata.SetElementValue("SavedAt", timestamp);

            // Add application name
            metadata.SetElementValue("Application", _appName);

            if (_includeAppVersion)
            {
                metadata.SetElementValue("AppVersion", Application.ProductVersion);
            }

            // Add username
            if (_includeUsername)
            {
                metadata.SetElementValue("SavedBy", Environment.UserName);
            }

            // Add machine name
            metadata.SetElementValue("MachineName", Environment.MachineName);

            // Count shapes
            int shapeCount = doc.Descendants("Shape").Count();
            metadata.SetElementValue("ShapeCount", shapeCount);

            return doc.ToString(SaveOptions.None);
        }

        /// <summary>
        /// Creates settings control for this processor
        /// </summary>
        public Control GetSettingsControl()
        {
            var panel = new Panel
            {
                Size = new System.Drawing.Size(380, 320),
                Padding = new Padding(10)
            };

            int y = 5;

            // Title
            var lblTitle = new Label
            {
                Text = "METADATA ADDER SETTINGS",
                Location = new System.Drawing.Point(5, y),
                Size = new System.Drawing.Size(350, 30),
                Font = new System.Drawing.Font("Arial", 10, System.Drawing.FontStyle.Bold),
                ForeColor = System.Drawing.Color.Purple,
                TextAlign = ContentAlignment.MiddleCenter
            };
            y += 40;

            // App name
            var lblAppName = new Label
            {
                Text = "Application Name:",
                Location = new System.Drawing.Point(5, y),
                Size = new System.Drawing.Size(120, 25)
            };

            var txtAppName = new TextBox
            {
                Text = _appName,
                Location = new System.Drawing.Point(130, y),
                Size = new System.Drawing.Size(200, 25)
            };
            y += 35;

            // Date format
            var lblDateFormat = new Label
            {
                Text = "Date Format:",
                Location = new System.Drawing.Point(5, y),
                Size = new System.Drawing.Size(120, 25)
            };

            var cmbDateFormat = new ComboBox
            {
                Location = new System.Drawing.Point(130, y),
                Size = new System.Drawing.Size(200, 25),
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            cmbDateFormat.Items.AddRange(new[] {
                "yyyy-MM-dd HH:mm:ss",
                "dd/MM/yyyy HH:mm:ss",
                "MM/dd/yyyy HH:mm:ss",
                "yyyy-MM-dd"
            });
            cmbDateFormat.SelectedItem = _dateFormat;
            y += 35;

            // Example of current format
            var lblExample = new Label
            {
                Text = $"Example: {DateTime.Now.ToString(_dateFormat)}",
                Location = new System.Drawing.Point(130, y),
                Size = new System.Drawing.Size(200, 25),
                ForeColor = System.Drawing.Color.DarkGreen,
                Font = new System.Drawing.Font("Arial", 8, System.Drawing.FontStyle.Italic)
            };
            y += 35;

            // Update example when format changes
            cmbDateFormat.SelectedIndexChanged += (s, e) =>
            {
                string format = cmbDateFormat.SelectedItem?.ToString() ?? _dateFormat;
                lblExample.Text = $"Example: {DateTime.Now.ToString(format)}";
            };

            // Checkboxes
            var chkAppVersion = new CheckBox
            {
                Text = "Include application version",
                Location = new System.Drawing.Point(5, y),
                Size = new System.Drawing.Size(220, 25),
                Checked = _includeAppVersion
            };
            y += 30;

            var chkUsername = new CheckBox
            {
                Text = "Include username",
                Location = new System.Drawing.Point(5, y),
                Size = new System.Drawing.Size(220, 25),
                Checked = _includeUsername
            };
            y += 45;

            // Info label
            var lblInfo = new Label
            {
                Text = "Metadata is added inside <Metadata> element at the beginning of XML.",
                Location = new System.Drawing.Point(5, y),
                Size = new System.Drawing.Size(350, 40),
                ForeColor = System.Drawing.Color.Gray,
                Font = new System.Drawing.Font("Arial", 8)
            };
            y += 50;

            // Apply button
            var btnApply = new Button
            {
                Text = "Apply Metadata Settings",
                Location = new System.Drawing.Point(5, y),
                Size = new System.Drawing.Size(200, 35),
                BackColor = System.Drawing.Color.LightGreen,
                FlatStyle = FlatStyle.Flat,
                Font = new System.Drawing.Font("Arial", 9, System.Drawing.FontStyle.Bold)
            };

            btnApply.Click += (s, e) =>
            {
                _appName = txtAppName.Text;
                _dateFormat = cmbDateFormat.SelectedItem?.ToString() ?? _dateFormat;
                _includeAppVersion = chkAppVersion.Checked;
                _includeUsername = chkUsername.Checked;
                SaveSettings();

                btnApply.Text = "✓ Applied!";
                btnApply.BackColor = System.Drawing.Color.Green;

                MessageBox.Show($"Metadata settings applied!\n\n" +
                    $"Application: {_appName}\n" +
                    $"Date format: {_dateFormat}\n" +
                    $"Example: {DateTime.Now.ToString(_dateFormat)}",
                    "Settings Applied", MessageBoxButtons.OK, MessageBoxIcon.Information);

                var timer = new Timer { Interval = 1500 };
                timer.Tick += (t, ev) =>
                {
                    btnApply.Text = "Apply Metadata Settings";
                    btnApply.BackColor = System.Drawing.Color.LightGreen;
                    timer.Stop();
                };
                timer.Start();
            };

            panel.Controls.AddRange(new Control[] {
                lblTitle, lblAppName, txtAppName, lblDateFormat, cmbDateFormat,
                lblExample, chkAppVersion, chkUsername, lblInfo, btnApply
            });

            return panel;
        }

        /// <summary>
        /// Applies settings from the control
        /// </summary>
        public void ApplySettings()
        {
            // Settings applied via Apply button
        }
    }
}