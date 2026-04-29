// GraphicEditor/UI/DataProcessorSettingsForm.cs
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using GraphicEditor.Plugins.DataProcessors;

namespace GraphicEditor.UI
{
    public partial class DataProcessorSettingsForm : Form
    {
        private readonly DataProcessorLoader _processorLoader;
        private ListBox processorListBox;
        private Button btnUp;
        private Button btnDown;
        private Button btnSave;
        private Button btnCancel;
        private Panel settingsPanel;
        private Label lblStatus;
        private Label lblInstruction;

        // Keep a separate list for display order
        private List<IDataProcessorPlugin> _displayOrder;

        public DataProcessorSettingsForm(DataProcessorLoader processorLoader)
        {
            _processorLoader = processorLoader;
            _displayOrder = new List<IDataProcessorPlugin>();
            InitializeComponent();
            LoadProcessorList();
        }

        private void InitializeComponent()
        {
            this.Text = "Data Processor Settings";
            this.Size = new Size(850, 650);
            this.StartPosition = FormStartPosition.CenterParent;
            this.MinimumSize = new Size(800, 550);
            this.BackColor = SystemColors.Control;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;

            // Instruction label
            lblInstruction = new Label
            {
                Text = "Configure data processors that transform XML before save and after load:",
                Location = new Point(12, 9),
                Size = new Size(700, 25),
                Font = new Font("Arial", 9, FontStyle.Bold)
            };

            // Processors group box
            GroupBox grpProcessors = new GroupBox
            {
                Text = "Active Processors (order matters - top executes first on save)",
                Location = new Point(12, 45),
                Size = new Size(380, 450)
            };

            processorListBox = new ListBox
            {
                Location = new Point(10, 25),
                Size = new Size(260, 370),
                Font = new Font("Consolas", 11),
                DrawMode = DrawMode.OwnerDrawFixed
            };
            processorListBox.DrawItem += ProcessorListBox_DrawItem;
            processorListBox.MouseClick += ProcessorListBox_MouseClick;
            processorListBox.SelectedIndexChanged += ProcessorListBox_SelectedIndexChanged;

            btnUp = new Button
            {
                Text = "▲ Up",
                Location = new Point(280, 25),
                Size = new Size(85, 35),
                Enabled = false
            };
            btnUp.Click += BtnUp_Click;

            btnDown = new Button
            {
                Text = "▼ Down",
                Location = new Point(280, 70),
                Size = new Size(85, 35),
                Enabled = false
            };
            btnDown.Click += BtnDown_Click;

            Label lblOrderHint = new Label
            {
                Text = "Use Up/Down to change\norder of execution",
                Location = new Point(280, 120),
                Size = new Size(85, 45),
                ForeColor = Color.Gray,
                Font = new Font("Arial", 8, FontStyle.Italic),
                TextAlign = ContentAlignment.MiddleCenter
            };

            grpProcessors.Controls.AddRange(new Control[] {
                processorListBox, btnUp, btnDown, lblOrderHint
            });

            // Settings group box
            GroupBox grpSettings = new GroupBox
            {
                Text = "Processor Settings",
                Location = new Point(410, 45),
                Size = new Size(410, 450),
                Font = new Font("Arial", 9, FontStyle.Bold)
            };

            settingsPanel = new Panel
            {
                Location = new Point(10, 25),
                Size = new Size(390, 410),
                AutoScroll = true,
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = Color.WhiteSmoke
            };

            grpSettings.Controls.Add(settingsPanel);

            // Status label
            lblStatus = new Label
            {
                Text = "Select a processor from the list to see its settings",
                Location = new Point(12, 510),
                Size = new Size(800, 30),
                ForeColor = Color.Gray,
                Font = new Font("Arial", 9, FontStyle.Italic),
                BorderStyle = BorderStyle.None
            };

            // Buttons at bottom
            btnSave = new Button
            {
                Text = "Save and Close",
                Location = new Point(580, 560),
                Size = new Size(120, 40),
                BackColor = Color.LightGreen,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Arial", 10, FontStyle.Bold)
            };
            btnSave.Click += BtnSave_Click;

            btnCancel = new Button
            {
                Text = "Cancel",
                Location = new Point(720, 560),
                Size = new Size(100, 40),
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Arial", 10)
            };
            btnCancel.Click += (s, e) => this.Close();

            this.Controls.AddRange(new Control[] {
                lblInstruction, grpProcessors, grpSettings,
                lblStatus, btnSave, btnCancel
            });
        }

        private void LoadProcessorList()
        {
            // Copy to display order and sort by ExecutionOrder
            _displayOrder.Clear();
            _displayOrder.AddRange(_processorLoader.LoadedProcessors.OrderBy(p => p.ExecutionOrder));

            processorListBox.Items.Clear();
            foreach (var processor in _displayOrder)
            {
                processorListBox.Items.Add(processor);
            }

            if (processorListBox.Items.Count > 0)
            {
                processorListBox.SelectedIndex = 0;
            }
        }

        private void ProcessorListBox_DrawItem(object sender, DrawItemEventArgs e)
        {
            if (e.Index < 0 || e.Index >= _displayOrder.Count) return;

            var processor = _displayOrder[e.Index];
            e.DrawBackground();

            // Draw checkbox
            Rectangle checkBoxRect = new Rectangle(e.Bounds.X + 3, e.Bounds.Y + (e.Bounds.Height - 13) / 2, 14, 14);
            ControlPaint.DrawCheckBox(e.Graphics, checkBoxRect,
                processor.IsEnabled ? ButtonState.Checked : ButtonState.Normal);

            // Draw processor name with color coding
            string text = processor.ProcessorName;
            Color textColor = processor.IsEnabled ? Color.Black : Color.Gray;

            // Color code by type
            if (text.Contains("Pretty")) textColor = Color.Blue;
            else if (text.Contains("Filter")) textColor = Color.Green;
            else if (text.Contains("Metadata")) textColor = Color.Purple;

            Rectangle textRect = new Rectangle(e.Bounds.X + 22, e.Bounds.Y, e.Bounds.Width - 30, e.Bounds.Height);

            using (Brush brush = new SolidBrush(textColor))
            {
                using (Font boldFont = new Font(e.Font, FontStyle.Bold))
                {
                    e.Graphics.DrawString(text, boldFont, brush, textRect);
                }
            }

            e.DrawFocusRectangle();
        }

        private void ProcessorListBox_MouseClick(object sender, MouseEventArgs e)
        {
            int index = processorListBox.IndexFromPoint(e.Location);
            if (index >= 0 && index < _displayOrder.Count)
            {
                Rectangle itemRect = processorListBox.GetItemRectangle(index);
                Rectangle checkBoxRect = new Rectangle(itemRect.X + 3, itemRect.Y + (itemRect.Height - 13) / 2, 14, 14);

                if (checkBoxRect.Contains(e.Location))
                {
                    var processor = _displayOrder[index];
                    processor.IsEnabled = !processor.IsEnabled;
                    processorListBox.Invalidate();

                    // Refresh settings panel if this processor is selected
                    if (processorListBox.SelectedIndex == index)
                    {
                        LoadSettingsForProcessor(processor);
                    }

                    UpdateStatusMessage(processor);
                }
            }
        }

        private void ProcessorListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (processorListBox.SelectedIndex >= 0 && processorListBox.SelectedIndex < _displayOrder.Count)
            {
                var processor = _displayOrder[processorListBox.SelectedIndex];
                btnUp.Enabled = processorListBox.SelectedIndex > 0;
                btnDown.Enabled = processorListBox.SelectedIndex < _displayOrder.Count - 1;

                LoadSettingsForProcessor(processor);
                UpdateStatusMessage(processor);
            }
            else
            {
                btnUp.Enabled = false;
                btnDown.Enabled = false;
                settingsPanel.Controls.Clear();
                lblStatus.Text = "Select a processor from the list to see its settings";
                lblStatus.ForeColor = Color.Gray;
            }
        }

        private void LoadSettingsForProcessor(IDataProcessorPlugin processor)
        {
            settingsPanel.Controls.Clear();

            // Add a header label to confirm which processor we're configuring
            var headerLabel = new Label
            {
                Text = $"═══ {processor.ProcessorName.ToUpper()} CONFIGURATION ═══",
                Location = new Point(5, 5),
                Size = new Size(370, 30),
                Font = new Font("Arial", 9, FontStyle.Bold),
                TextAlign = ContentAlignment.MiddleCenter,
                BackColor = Color.LightGray
            };

            var settingsControl = processor.GetSettingsControl();
            if (settingsControl != null)
            {
                settingsControl.Location = new Point(5, 40);
                settingsControl.Size = new Size(370, 360);
                settingsControl.BackColor = Color.White;

                settingsPanel.Controls.Add(headerLabel);
                settingsPanel.Controls.Add(settingsControl);

                lblStatus.Text = $"Configuring: {processor.ProcessorName} - Change settings and click 'Apply' button below";
                lblStatus.ForeColor = Color.Blue;
            }
            else
            {
                var lblNoSettings = new Label
                {
                    Text = $"No configurable settings for\n{processor.ProcessorName}",
                    Location = new Point(10, 50),
                    Size = new Size(350, 60),
                    TextAlign = ContentAlignment.MiddleCenter,
                    ForeColor = Color.Gray,
                    Font = new Font("Arial", 10)
                };
                settingsPanel.Controls.Add(headerLabel);
                settingsPanel.Controls.Add(lblNoSettings);
                lblStatus.Text = $"{processor.ProcessorName} has no configurable settings";
                lblStatus.ForeColor = Color.Gray;
            }
        }

        private void UpdateStatusMessage(IDataProcessorPlugin processor)
        {
            string status = processor.IsEnabled
                ? $"{processor.ProcessorName} is ENABLED - will be applied during save/load"
                : $"{processor.ProcessorName} is DISABLED - will be skipped";
            lblStatus.Text = status;
            lblStatus.ForeColor = processor.IsEnabled ? Color.Green : Color.Gray;
        }

        private void UpdateExecutionOrders()
        {
            int order = 0;
            for (int i = 0; i < _displayOrder.Count; i++)
            {
                _displayOrder[i].ExecutionOrder = order;
                order += 10;
            }
        }

        private void BtnUp_Click(object sender, EventArgs e)
        {
            int index = processorListBox.SelectedIndex;
            if (index > 0)
            {
                // Swap in display order
                var temp = _displayOrder[index];
                _displayOrder[index] = _displayOrder[index - 1];
                _displayOrder[index - 1] = temp;

                // Update listbox
                processorListBox.Items.Clear();
                foreach (var processor in _displayOrder)
                {
                    processorListBox.Items.Add(processor);
                }
                processorListBox.SelectedIndex = index - 1;

                UpdateExecutionOrders();
                processorListBox.Invalidate();
            }
        }

        private void BtnDown_Click(object sender, EventArgs e)
        {
            int index = processorListBox.SelectedIndex;
            if (index < _displayOrder.Count - 1)
            {
                // Swap in display order
                var temp = _displayOrder[index];
                _displayOrder[index] = _displayOrder[index + 1];
                _displayOrder[index + 1] = temp;

                // Update listbox
                processorListBox.Items.Clear();
                foreach (var processor in _displayOrder)
                {
                    processorListBox.Items.Add(processor);
                }
                processorListBox.SelectedIndex = index + 1;

                UpdateExecutionOrders();
                processorListBox.Invalidate();
            }
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}