// GraphicEditor/UI/PluginManagerForm.cs
using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using GraphicEditor.Plugins;

namespace GraphicEditor.UI
{
    /// <summary>
    /// Form for managing plugins - shows loaded plugins and allows loading new ones
    /// </summary>
    public partial class PluginManagerForm : Form
    {
        private readonly PluginLoader _pluginLoader;
        private ListBox pluginListBox;
        private TextBox detailsTextBox;
        private Button loadPluginButton;
        private Button refreshButton;
        private Button closeButton;
        private Label statusLabel;

        /// <summary>
        /// Event fired when plugins are loaded or unloaded
        /// </summary>
        public event EventHandler PluginListChanged;

        public PluginManagerForm(PluginLoader pluginLoader)
        {
            _pluginLoader = pluginLoader;
            InitializeComponent();
            RefreshPluginList();

            // Subscribe to plugin events
            _pluginLoader.PluginLoaded += OnPluginLoaded;
            _pluginLoader.PluginLoadError += OnPluginLoadError;
        }

        private void InitializeComponent()
        {
            this.Text = "Plugin Manager";
            this.Size = new Size(650, 550);
            this.StartPosition = FormStartPosition.CenterParent;
            this.MinimumSize = new Size(600, 450);
            this.BackColor = SystemColors.Control;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;

            // Title label
            Label titleLabel = new Label
            {
                Text = "Plugin Manager",
                Location = new Point(12, 9),
                Size = new Size(200, 30),
                Font = new Font("Arial", 14, FontStyle.Bold),
                ForeColor = Color.DarkBlue
            };

            // Info label
            Label infoLabel = new Label
            {
                Text = "Loaded plugins (must be signed with valid signature):",
                Location = new Point(12, 45),
                Size = new Size(400, 20),
                Font = new Font("Arial", 9, FontStyle.Italic)
            };

            // Plugin list box
            pluginListBox = new ListBox
            {
                Location = new Point(12, 70),
                Size = new Size(350, 300),
                Font = new Font("Consolas", 10),
                BorderStyle = BorderStyle.Fixed3D
            };
            pluginListBox.SelectedIndexChanged += PluginListBox_SelectedIndexChanged;

            // Details text box
            detailsTextBox = new TextBox
            {
                Location = new Point(370, 70),
                Size = new Size(250, 300),
                Multiline = true,
                ReadOnly = true,
                ScrollBars = ScrollBars.Vertical,
                Font = new Font("Consolas", 9),
                BackColor = Color.WhiteSmoke,
                BorderStyle = BorderStyle.Fixed3D
            };

            // Load plugin button
            loadPluginButton = new Button
            {
                Text = "Load Plugin...",
                Location = new Point(12, 380),
                Size = new Size(120, 35),
                BackColor = Color.LightGreen,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Arial", 9, FontStyle.Bold)
            };
            loadPluginButton.Click += LoadPluginButton_Click;

            // Refresh button
            refreshButton = new Button
            {
                Text = "Refresh",
                Location = new Point(140, 380),
                Size = new Size(100, 35),
                FlatStyle = FlatStyle.Flat
            };
            refreshButton.Click += RefreshButton_Click;

            // Close button
            closeButton = new Button
            {
                Text = "Close",
                Location = new Point(540, 470),
                Size = new Size(80, 30),
                FlatStyle = FlatStyle.Flat
            };
            closeButton.Click += (s, e) => this.Close();

            // Status label
            statusLabel = new Label
            {
                Text = "Ready",
                Location = new Point(12, 430),
                Size = new Size(600, 30),
                Font = new Font("Arial", 8),
                ForeColor = Color.Gray,
                BorderStyle = BorderStyle.None
            };

            // Add all controls
            this.Controls.AddRange(new Control[] {
                titleLabel, infoLabel, pluginListBox, detailsTextBox,
                loadPluginButton, refreshButton, closeButton, statusLabel
            });
        }

        private void RefreshPluginList()
        {
            pluginListBox.Items.Clear();

            // Add 2D plugins
            foreach (var plugin in _pluginLoader.Loaded2DPlugins)
            {
                pluginListBox.Items.Add(new PluginListItem(plugin, "2D"));
            }

            // Add 3D plugins
            foreach (var plugin in _pluginLoader.Loaded3DPlugins)
            {
                pluginListBox.Items.Add(new PluginListItem(plugin, "3D"));
            }

            if (pluginListBox.Items.Count == 0)
            {
                pluginListBox.Items.Add("No plugins loaded");
                detailsTextBox.Text = "No plugins are currently loaded.\n\n" +
                                      "To load a plugin:\n" +
                                      "1. Create a signed DLL with IShape2DPlugin or IShape3DPlugin\n" +
                                      "2. Place the DLL and its .sig file in the Plugins folder\n" +
                                      "3. Click 'Load Plugin' or restart the application";
            }
        }

        private void PluginListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (pluginListBox.SelectedItem is PluginListItem item)
            {
                detailsTextBox.Text = item.GetDetails();
            }
        }

        private void LoadPluginButton_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                ofd.Filter = "Plugin DLLs (*.dll)|*.dll";
                ofd.Title = "Select Plugin to Load";
                ofd.InitialDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Plugins");

                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    string pluginsDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Plugins");

                    // Ensure plugins directory exists
                    if (!Directory.Exists(pluginsDir))
                    {
                        Directory.CreateDirectory(pluginsDir);
                    }

                    string targetPath = Path.Combine(pluginsDir, Path.GetFileName(ofd.FileName));

                    // Copy DLL to plugins directory if not already there
                    if (!File.Exists(targetPath))
                    {
                        File.Copy(ofd.FileName, targetPath);

                        // Also copy signature file if it exists next to the DLL
                        string sigSource = Path.ChangeExtension(ofd.FileName, ".sig");
                        if (File.Exists(sigSource))
                        {
                            string sigTarget = Path.ChangeExtension(targetPath, ".sig");
                            File.Copy(sigSource, sigTarget);
                        }
                        else
                        {
                            statusLabel.Text = "Warning: No signature file found. Plugin requires valid signature.";
                            statusLabel.ForeColor = Color.Orange;
                        }
                    }

                    statusLabel.Text = $"Loading plugin: {Path.GetFileName(targetPath)}";
                    statusLabel.ForeColor = Color.Blue;

                    bool success = _pluginLoader.LoadPlugin(targetPath);

                    if (success)
                    {
                        RefreshPluginList();
                        statusLabel.Text = "Plugin loaded successfully!";
                        statusLabel.ForeColor = Color.Green;

                        // Notify that plugin list has changed
                        OnPluginListChanged(EventArgs.Empty);
                    }
                    else
                    {
                        statusLabel.Text = "Failed to load plugin. Check signature and format.";
                        statusLabel.ForeColor = Color.Red;
                    }
                }
            }
        }

        private void RefreshButton_Click(object sender, EventArgs e)
        {
            RefreshPluginList();
            statusLabel.Text = "Plugin list refreshed";
            statusLabel.ForeColor = Color.Gray;
        }

        private void OnPluginLoaded(object sender, PluginLoadEventArgs e)
        {
            this.BeginInvoke(new Action(() =>
            {
                RefreshPluginList();
                statusLabel.Text = $"Loaded: {e.PluginName} v{e.Version} ({e.Type})";
                statusLabel.ForeColor = Color.Green;

                // Notify that plugin list has changed
                OnPluginListChanged(EventArgs.Empty);
            }));
        }

        private void OnPluginLoadError(object sender, PluginLoadErrorEventArgs e)
        {
            this.BeginInvoke(new Action(() =>
            {
                statusLabel.Text = $"Error loading {Path.GetFileName(e.FilePath)}: {e.ErrorMessage}";
                statusLabel.ForeColor = Color.Red;
            }));
        }

        /// <summary>
        /// Triggers the PluginListChanged event
        /// </summary>
        protected virtual void OnPluginListChanged(EventArgs e)
        {
            PluginListChanged?.Invoke(this, e);
        }

        /// <summary>
        /// Helper class for displaying plugins in the listbox
        /// </summary>
        private class PluginListItem
        {
            private readonly IPluginMetadata _plugin;
            private readonly string _type;

            public PluginListItem(IPluginMetadata plugin, string type)
            {
                _plugin = plugin;
                _type = type;
            }

            public override string ToString()
            {
                return $"[{_type}] {_plugin.PluginName} v{_plugin.Version}";
            }

            public string GetDetails()
            {
                return $"Plugin Details:\n" +
                       $"━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━\n\n" +
                       $"Name:        {_plugin.PluginName}\n" +
                       $"ID:          {_plugin.PluginId}\n" +
                       $"Version:     {_plugin.Version}\n" +
                       $"Type:        {_type}\n" +
                       $"Author:      {_plugin.Author}\n" +
                       $"Description: {_plugin.Description}\n";
            }
        }
    }
}