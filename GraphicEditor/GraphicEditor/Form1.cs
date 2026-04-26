using GraphicEditor.Controllers;
using GraphicEditor.Models.Shapes;
using GraphicEditor.Models.Shapes3D;
using Newtonsoft.Json;  
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using GraphicEditor.Models;

namespace GraphicEditor
{
    public partial class MainForm : Form
    {
        private readonly ExtendedShapeController _controller;
        private bool _is3DMode;
        private bool _isDrawing;
        private Point? _startPoint;
        private Point? _currentPoint;
        private string _currentShapeType;

        // Controls for the editor panel
        private GroupBox modeGroup;
        private RadioButton mode2DRadio;
        private RadioButton mode3DRadio;
        private GroupBox shapeGroup;
        private ComboBox shapeTypeCombo;
        private CheckBox drawModeCheckBox;
        private Button clearButton;
        private Label instructionLabel;
        private Label statusLabel;
        private ListBox shapesListBox;
        // Controls for editing
        private GroupBox editGroup;
        private Panel editPropertiesPanel;
        private TextBox txtParam1;
        private TextBox txtParam2;
        private TextBox txtParam3;
        private TextBox txtParam4;
        private Label lblParam1;
        private Label lblParam2;
        private Label lblParam3;
        private Label lblParam4;
        private Button btnUpdate;

        public MainForm()
        {
            _controller = new ExtendedShapeController();
            _is3DMode = false;
            _isDrawing = true; // Drawing mode enabled by default

            InitializeComponent();
            InitializeCustomControls();
            UpdateShapeTypes();
            UpdateShapesList();
        }


        /// Initializes custom controls and adds them to panelEditor

        private void InitializeCustomControls()
        {
            int currentY = 80;

            // Mode selection group
            modeGroup = new GroupBox
            {
                Text = "Mode",
                Location = new Point(40, currentY),
                Size = new Size(500, 100)
            };

            mode2DRadio = new RadioButton
            {
                Text = "2D",
                Location = new Point(10, 40),
                Size = new Size(100, 25),
                Checked = true
            };
            mode2DRadio.CheckedChanged += ModeRadio_CheckedChanged;

            mode3DRadio = new RadioButton
            {
                Text = "3D",
                Location = new Point(120, 40),
                Size = new Size(100, 25)
            };
            mode3DRadio.CheckedChanged += ModeRadio_CheckedChanged;

            modeGroup.Controls.AddRange(new Control[] { mode2DRadio, mode3DRadio });

            currentY += 70;

            // Shape selection group
            shapeGroup = new GroupBox
            {
                Text = "Shape",
                Location = new Point(60, currentY + 50),
                Size = new Size(500, 100)
            };

            shapeTypeCombo = new ComboBox
            {
                Location = new Point(70, 45),
                Size = new Size(280, 25),
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            shapeTypeCombo.SelectedIndexChanged += ShapeTypeCombo_SelectedIndexChanged;

            shapeGroup.Controls.Add(shapeTypeCombo);

            currentY += 70;

            // Drawing mode checkbox
            drawModeCheckBox = new CheckBox
            {
                Text = "Drawing Mode",
                Location = new Point(20, currentY + 100),
                Size = new Size(10, 30),
                Checked = true,
                Font = new Font("Arial", 10, FontStyle.Bold),
                ForeColor = Color.Blue
            };
            drawModeCheckBox.CheckedChanged += DrawModeCheckBox_CheckedChanged;

            currentY += 40;

            // Add Plugin Manager button (добавь в метод InitializeCustomControls)
            Button pluginManagerButton = new Button
            {
                Text = "Plugin Manager",
                Location = new Point(20, currentY + 100), // подбери подходящую позицию
                Size = new Size(150, 35),
                BackColor = Color.LightSteelBlue,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Arial", 10, FontStyle.Bold)
            };
            pluginManagerButton.Click += PluginManagerButton_Click;
            panelEditor.Controls.Add(pluginManagerButton);

            currentY += 60;

            // Instructions label
            instructionLabel = new Label
            {
                Text = "✓ Drawing Mode ON\nClick and drag on canvas",
                Location = new Point(20, currentY),
                Size = new Size(280, 60),
                Font = new Font("Arial", 10, FontStyle.Bold),
                ForeColor = Color.Green,
                TextAlign = ContentAlignment.MiddleLeft
            };

            currentY += 80;

            // Status label
            statusLabel = new Label
            {
                Text = "Ready - Select a shape and draw",
                Location = new Point(20, currentY),
                Size = new Size(280, 40),
                Font = new Font("Arial", 9, FontStyle.Regular),
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = Color.White,
                TextAlign = ContentAlignment.MiddleLeft
            };


            // Shapes list group
            GroupBox listGroup = new GroupBox
            {
                Text = "Shapes List",
                Location = new Point(20, currentY + 100),
                Size = new Size(500, 200)
            };

            shapesListBox = new ListBox
            {
                Location = new Point(10, 25),
                Size = new Size(480, 140),
                Font = new Font("Arial", 9)
            };
            shapesListBox.SelectedIndexChanged += ShapesListBox_SelectedIndexChanged;

            listGroup.Controls.Add(shapesListBox);

            currentY += 220; 


            // Edit properties group
            editGroup = new GroupBox
            {
                Text = "Edit Properties",
                Location = new Point(20, currentY + 100),
                Size = new Size(500, 200)
            };

            editPropertiesPanel = new Panel
            {
                Location = new Point(10, 25),
                Size = new Size(480, 120)
            };

            // Create dynamic property controls (will be populated when shape is selected)
            txtParam1 = new TextBox { Location = new Point(120, 10), Size = new Size(150, 25), Visible = false };
            txtParam2 = new TextBox { Location = new Point(120, 45), Size = new Size(150, 25), Visible = false };
            txtParam3 = new TextBox { Location = new Point(120, 80), Size = new Size(150, 25), Visible = false };
            txtParam4 = new TextBox { Location = new Point(120, 115), Size = new Size(150, 25), Visible = false };

            lblParam1 = new Label { Location = new Point(20, 13), Size = new Size(90, 25), Text = "", Visible = false };
            lblParam2 = new Label { Location = new Point(20, 48), Size = new Size(90, 25), Text = "", Visible = false };
            lblParam3 = new Label { Location = new Point(20, 83), Size = new Size(90, 25), Text = "", Visible = false };
            lblParam4 = new Label { Location = new Point(20, 118), Size = new Size(90, 25), Text = "", Visible = false };

            btnUpdate = new Button
            {
                Text = "Update Shape",
                Location = new Point(10, 150),
                Size = new Size(120, 35)
            };
            btnUpdate.Click += BtnUpdate_Click;

            editPropertiesPanel.Controls.AddRange(new Control[] {
                                                    txtParam1, txtParam2, txtParam3, txtParam4,
                                                    lblParam1, lblParam2, lblParam3, lblParam4
                                                });

            editGroup.Controls.AddRange(new Control[] { editPropertiesPanel, btnUpdate });

            currentY += 220; // Adjust for next controls

            // Update the panelEditor.Controls.AddRange to include editGroup
            panelEditor.Controls.AddRange(new Control[] {
                                            modeGroup,
                                            shapeGroup,
                                            listGroup,
                                            editGroup  
                                        });

            // Add all controls to panelEditor
            panelEditor.Controls.AddRange(new Control[] {
                modeGroup,
                shapeGroup,
                    listGroup,
            });

            // Setup mouse events for panelDraw
            panelDraw.MouseDown += PanelDraw_MouseDown;
            panelDraw.MouseMove += PanelDraw_MouseMove;
            panelDraw.MouseUp += PanelDraw_MouseUp;
            panelDraw.Paint += PanelDraw_Paint;
            panelDraw.BackColor = Color.White;
        }

        private void PluginManagerButton_Click(object sender, EventArgs e)
        {
            var pluginManager = new GraphicEditor.UI.PluginManagerForm(_controller.PluginLoader);

            // Subscribe to plugin list changes
            pluginManager.PluginListChanged += (s, args) =>
            {
                // Refresh shape types and list when plugins change
                this.BeginInvoke(new Action(() =>
                {
                    UpdateShapeTypes();      // Refresh the shape dropdown
                    UpdateShapesList();      // Refresh the shapes listbox
                    panelDraw.Invalidate();  // Redraw canvas
                    statusLabel.Text = "Plugins updated - shape list refreshed";
                }));
            };

            pluginManager.ShowDialog(this);
        }
        private void UpdateShapeTypes()
        {
            shapeTypeCombo.Items.Clear();
            string[] shapes = _controller.GetAvailableShapes();
            shapeTypeCombo.Items.AddRange(shapes);
            if (shapes.Length > 0)
            {
                shapeTypeCombo.SelectedIndex = 0;
                _currentShapeType = shapes[0];
            }
        }

        private void ShapeTypeCombo_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (shapeTypeCombo.SelectedItem != null)
            {
                _currentShapeType = shapeTypeCombo.SelectedItem.ToString();
                statusLabel.Text = $"Selected: {_currentShapeType} - Draw on canvas";
            }
        }

        /// <summary>
        /// Update selected shape properties
        /// </summary>
        private void updateToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Just trigger the same update as the Update button
            BtnUpdate_Click(sender, e);
        }


        /// <summary>
        /// Safely parses float from text, returns default if invalid
        /// </summary>
        private float SafeParseFloat(string text, float defaultValue = 0)
        {
            if (float.TryParse(text, out float result))
                return result;
            return defaultValue;
        }


        /// <summary>
        /// Displays properties of the selected shape in the edit panel
        /// </summary>
        private void DisplayShapeProperties()
        {
            // Hide all property controls initially
            txtParam1.Visible = false;
            txtParam2.Visible = false;
            txtParam3.Visible = false;
            txtParam4.Visible = false;
            lblParam1.Visible = false;
            lblParam2.Visible = false;
            lblParam3.Visible = false;
            lblParam4.Visible = false;

            if (shapesListBox.SelectedIndex == -1)
            {
                btnUpdate.Enabled = false;
                return;
            }

            btnUpdate.Enabled = true;
            int selectedIndex = shapesListBox.SelectedIndex;
            int shapes2DCount = _controller.GetAllShapes2D().Count;

            if (selectedIndex < shapes2DCount)
            {
                // 2D shape selected
                var shape = _controller.GetAllShapes2D()[selectedIndex];
                Display2DShapeProperties(shape);
            }
            else
            {
                // 3D shape selected
                int shape3DIndex = selectedIndex - shapes2DCount;
                var shape = _controller.GetAllShapes3D()[shape3DIndex];
                Display3DShapeProperties(shape);
            }
        }

        /// <summary>
        /// Displays properties for 2D shapes
        /// </summary>
        private void Display2DShapeProperties(IShape shape)
        {
            switch (shape.GetName())
            {
                case "Circle":
                    var circle = shape as Circle;
                    lblParam1.Text = "Center X:";
                    lblParam2.Text = "Center Y:";
                    lblParam3.Text = "Radius:";

                    txtParam1.Text = circle.Center.X.ToString();
                    txtParam2.Text = circle.Center.Y.ToString();
                    txtParam3.Text = circle.Radius.ToString();

                    txtParam1.Visible = true;
                    txtParam2.Visible = true;
                    txtParam3.Visible = true;
                    lblParam1.Visible = true;
                    lblParam2.Visible = true;
                    lblParam3.Visible = true;
                    break;

                case "Rectangle":
                    var rect = shape as GraphicEditor.Models.Shapes.Rectangle;
                    lblParam1.Text = "X:";
                    lblParam2.Text = "Y:";
                    lblParam3.Text = "Width:";
                    lblParam4.Text = "Height:";

                    txtParam1.Text = rect.TopLeft.X.ToString();
                    txtParam2.Text = rect.TopLeft.Y.ToString();
                    txtParam3.Text = rect.Width.ToString();
                    txtParam4.Text = rect.Height.ToString();

                    txtParam1.Visible = true;
                    txtParam2.Visible = true;
                    txtParam3.Visible = true;
                    txtParam4.Visible = true;
                    lblParam1.Visible = true;
                    lblParam2.Visible = true;
                    lblParam3.Visible = true;
                    lblParam4.Visible = true;
                    break;

                case "Triangle":
                    var tri = shape as Triangle;
                    lblParam1.Text = "X1, Y1:";
                    lblParam2.Text = "X2, Y2:";
                    lblParam3.Text = "X3, Y3:";

                    txtParam1.Text = $"{tri.Point1.X},{tri.Point1.Y}";
                    txtParam2.Text = $"{tri.Point2.X},{tri.Point2.Y}";
                    txtParam3.Text = $"{tri.Point3.X},{tri.Point3.Y}";

                    txtParam1.Visible = true;
                    txtParam2.Visible = true;
                    txtParam3.Visible = true;
                    lblParam1.Visible = true;
                    lblParam2.Visible = true;
                    lblParam3.Visible = true;
                    break;
            }
        }

        /// <summary>
        /// Displays properties for 3D shapes
        /// </summary>
        private void Display3DShapeProperties(IShape3D shape)
        {
            switch (shape.GetName())
            {
                case "Cube":
                    var cube = shape as Cube;
                    lblParam1.Text = "Center X:";
                    lblParam2.Text = "Center Y:";
                    lblParam3.Text = "Center Z:";
                    lblParam4.Text = "Size:";

                    txtParam1.Text = cube.Center.X.ToString();
                    txtParam2.Text = cube.Center.Y.ToString();
                    txtParam3.Text = cube.Center.Z.ToString();
                    txtParam4.Text = cube.Size.ToString();

                    txtParam1.Visible = true;
                    txtParam2.Visible = true;
                    txtParam3.Visible = true;
                    txtParam4.Visible = true;
                    lblParam1.Visible = true;
                    lblParam2.Visible = true;
                    lblParam3.Visible = true;
                    lblParam4.Visible = true;
                    break;

                case "Sphere":
                    var sphere = shape as Sphere;
                    lblParam1.Text = "Center X:";
                    lblParam2.Text = "Center Y:";
                    lblParam3.Text = "Center Z:";
                    lblParam4.Text = "Radius:";

                    txtParam1.Text = sphere.Center.X.ToString();
                    txtParam2.Text = sphere.Center.Y.ToString();
                    txtParam3.Text = sphere.Center.Z.ToString();
                    txtParam4.Text = sphere.Radius.ToString();

                    txtParam1.Visible = true;
                    txtParam2.Visible = true;
                    txtParam3.Visible = true;
                    txtParam4.Visible = true;
                    lblParam1.Visible = true;
                    lblParam2.Visible = true;
                    lblParam3.Visible = true;
                    lblParam4.Visible = true;
                    break;

                case "Tetrahedron":
                    var tetra = shape as Tetrahedron;
                    lblParam1.Text = "Center X:";
                    lblParam2.Text = "Center Y:";
                    lblParam3.Text = "Center Z:";
                    lblParam4.Text = "Size:";

                    txtParam1.Text = tetra.Center.X.ToString();
                    txtParam2.Text = tetra.Center.Y.ToString();
                    txtParam3.Text = tetra.Center.Z.ToString();
                    txtParam4.Text = tetra.Size.ToString();

                    txtParam1.Visible = true;
                    txtParam2.Visible = true;
                    txtParam3.Visible = true;
                    txtParam4.Visible = true;
                    lblParam1.Visible = true;
                    lblParam2.Visible = true;
                    lblParam3.Visible = true;
                    lblParam4.Visible = true;
                    break;
            }
        }

        /// <summary>
        /// Updates the selected shape with new property values
        /// </summary>
        private void BtnUpdate_Click(object sender, EventArgs e)
        {
            if (shapesListBox.SelectedIndex == -1)
            {
                MessageBox.Show("Please select a shape to update.", "No Selection",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            try
            {
                int selectedIndex = shapesListBox.SelectedIndex;
                int shapes2DCount = _controller.GetAllShapes2D().Count;

                if (selectedIndex < shapes2DCount)
                {
                    // Update 2D shape
                    var shape = _controller.GetAllShapes2D()[selectedIndex];
                    Update2DShape(shape);
                }
                else
                {
                    // Update 3D shape
                    int shape3DIndex = selectedIndex - shapes2DCount;
                    var shape = _controller.GetAllShapes3D()[shape3DIndex];
                    Update3DShape(shape);
                }

                // Refresh display
                UpdateShapesList();  // Refresh list to show updated info
                panelDraw.Invalidate();  // Redraw canvas
                DisplayShapeProperties();  // Refresh property display

                statusLabel.Text = "Shape updated successfully";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error updating shape: {ex.Message}", "Update Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                statusLabel.Text = "Error updating shape";
            }
        }

        /// <summary>
        /// Updates a 2D shape with new property values
        /// </summary>
        private void Update2DShape(IShape shape)
        {
            switch (shape.GetName())
            {
                case "Circle":
                    var circle = shape as Circle;
                    circle.Center.X = float.Parse(txtParam1.Text);
                    circle.Center.Y = float.Parse(txtParam2.Text);
                    circle.Radius = float.Parse(txtParam3.Text);
                    break;

                case "Rectangle":
                    var rect = shape as GraphicEditor.Models.Shapes.Rectangle;
                    rect.TopLeft.X = float.Parse(txtParam1.Text);
                    rect.TopLeft.Y = float.Parse(txtParam2.Text);
                    rect.Width = float.Parse(txtParam3.Text);
                    rect.Height = float.Parse(txtParam4.Text);
                    break;

                case "Triangle":
                    var tri = shape as Triangle;
                    string[] parts1 = txtParam1.Text.Split(',');
                    string[] parts2 = txtParam2.Text.Split(',');
                    string[] parts3 = txtParam3.Text.Split(',');

                    tri.Point1.X = float.Parse(parts1[0]);
                    tri.Point1.Y = float.Parse(parts1[1]);
                    tri.Point2.X = float.Parse(parts2[0]);
                    tri.Point2.Y = float.Parse(parts2[1]);
                    tri.Point3.X = float.Parse(parts3[0]);
                    tri.Point3.Y = float.Parse(parts3[1]);
                    break;
            }
        }

        /// <summary>
        /// Updates a 3D shape with new property values
        /// </summary>
        private void Update3DShape(IShape3D shape)
        {
            switch (shape.GetName())
            {
                case "Cube":
                    var cube = shape as Cube;
                    cube.Center.X = float.Parse(txtParam1.Text);
                    cube.Center.Y = float.Parse(txtParam2.Text);
                    cube.Center.Z = float.Parse(txtParam3.Text);
                    cube.Size = float.Parse(txtParam4.Text);
                    break;

                case "Sphere":
                    var sphere = shape as Sphere;
                    sphere.Center.X = float.Parse(txtParam1.Text);
                    sphere.Center.Y = float.Parse(txtParam2.Text);
                    sphere.Center.Z = float.Parse(txtParam3.Text);
                    sphere.Radius = float.Parse(txtParam4.Text);
                    break;

                case "Tetrahedron":
                    var tetra = shape as Tetrahedron;
                    tetra.Center.X = float.Parse(txtParam1.Text);
                    tetra.Center.Y = float.Parse(txtParam2.Text);
                    tetra.Center.Z = float.Parse(txtParam3.Text);
                    tetra.Size = float.Parse(txtParam4.Text);
                    break;
            }
        }

        /// <summary>
        /// Handles selection change in shapes listbox
        /// </summary>
        private void ShapesListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (shapesListBox.SelectedIndex == -1) return;

            int selectedIndex = shapesListBox.SelectedIndex;
            int shapes2DCount = _controller.GetAllShapes2D().Count;

            if (selectedIndex < shapes2DCount)
            {
                // Selected 2D shape
                var shape = _controller.GetAllShapes2D()[selectedIndex];
                statusLabel.Text = $"Selected: 2D {shape.GetName()} - Modify properties and click Update";
            }
            else
            {
                // Selected 3D shape
                int shape3DIndex = selectedIndex - shapes2DCount;
                var shape = _controller.GetAllShapes3D()[shape3DIndex];
                statusLabel.Text = $"Selected: 3D {shape.GetName()} - Modify properties and click Update";
            }

            // Display properties for editing
            DisplayShapeProperties();
        }

        private void ModeRadio_CheckedChanged(object sender, EventArgs e)
        {
            if (sender == mode2DRadio && mode2DRadio.Checked)
            {
                _is3DMode = false;
            }
            else if (sender == mode3DRadio && mode3DRadio.Checked)
            {
                _is3DMode = true;
            }

            _controller.SetMode(_is3DMode);
            UpdateShapeTypes();
            panelDraw.Invalidate();

            statusLabel.Text = $"Switched to {(_is3DMode ? "3D" : "2D")} mode";
        }

        private void DrawModeCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            _isDrawing = drawModeCheckBox.Checked;

            if (_isDrawing)
            {
                instructionLabel.Text = "✓ Drawing Mode ON\nClick and drag on canvas";
                instructionLabel.ForeColor = Color.Green;
                panelDraw.Cursor = Cursors.Cross;
            }
            else
            {
                instructionLabel.Text = "✗ Drawing Mode OFF\nEnable to draw";
                instructionLabel.ForeColor = Color.Red;
                panelDraw.Cursor = Cursors.Default;
                _startPoint = null;
                _currentPoint = null;
            }

            panelDraw.Invalidate();
        }

        private void ClearButton_Click(object sender, EventArgs e)
        {
            _controller.ClearAll();
            _startPoint = null;
            _currentPoint = null;
            panelDraw.Invalidate();
            statusLabel.Text = "Canvas cleared";
        }

        #region Mouse Drawing Events

        private void PanelDraw_MouseDown(object sender, MouseEventArgs e)
        {
            if (!_isDrawing || e.Button != MouseButtons.Left)
                return;

            _startPoint = e.Location;
            _currentPoint = e.Location;

            statusLabel.Text = $"Drawing started at ({e.X}, {e.Y})";
        }

        private void PanelDraw_MouseMove(object sender, MouseEventArgs e)
        {
            if (!_isDrawing || _startPoint == null)
                return;

            _currentPoint = e.Location;
            panelDraw.Invalidate();
        }

        private void PanelDraw_MouseUp(object sender, MouseEventArgs e)
        {
            if (!_isDrawing || _startPoint == null || e.Button != MouseButtons.Left)
                return;

            try
            {
                string parameters = GenerateShapeParameters(_currentShapeType,
                    _startPoint.Value, e.Location);

                _controller.CreateShape(_currentShapeType, parameters);

                UpdateShapesList();

                statusLabel.Text = $"Created {_currentShapeType}";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error creating shape: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                statusLabel.Text = "Error creating shape";
            }
            finally
            {
                _startPoint = null;
                _currentPoint = null;
                panelDraw.Invalidate();
            }
        }

        /// <summary>
        /// Generates parameters string for shape creation based on mouse coordinates
        /// </summary>
        private string GenerateShapeParameters(string shapeType, Point start, Point end)
        {
            // Check for plugin shapes first
            var plugin2D = _controller.Get2DPlugin(shapeType);
            var plugin3D = _controller.Get3DPlugin(shapeType);

            if (plugin2D != null)
            {
                return plugin2D.GenerateParameters(start, end);
            }

            if (plugin3D != null)
            {
                return plugin3D.GenerateParameters(start, end);
            }

            // Built-in shapes
            int x1 = Math.Min(start.X, end.X);
            int y1 = Math.Min(start.Y, end.Y);
            int x2 = Math.Max(start.X, end.X);
            int y2 = Math.Max(start.Y, end.Y);

            switch (shapeType)
            {
                case "Circle":
                    int radius = (int)Math.Sqrt(Math.Pow(end.X - start.X, 2) + Math.Pow(end.Y - start.Y, 2));
                    return $"{start.X},{start.Y},{radius}";

                case "Rectangle":
                    int width = Math.Abs(end.X - start.X);
                    int height = Math.Abs(end.Y - start.Y);
                    return $"{x1},{y1},{width},{height}";

                case "Triangle":
                    int midX = (start.X + end.X) / 2;
                    int height3 = (int)(Math.Abs(end.Y - start.Y) * 0.8);
                    int topY = Math.Min(start.Y, end.Y) - height3;
                    return $"{start.X},{start.Y},{end.X},{end.Y},{midX},{topY}";

                case "Cube":
                case "Sphere":
                case "Tetrahedron":
                    int size = Math.Max(Math.Abs(end.X - start.X), Math.Abs(end.Y - start.Y));
                    return $"{start.X},{start.Y},0,{size}";

                default:
                    throw new NotSupportedException($"Shape {shapeType} not supported");
            }
        }


        /// <summary>
        /// Extracts parameters string from a 2D shape for serialization
        /// </summary>
        /// <summary>
        /// Extracts parameters string from a 2D shape for serialization
        /// </summary>
        private string GetShapeParameters(IShape shape)
        {
            switch (shape.GetName())
            {
                case "Circle":
                    var circle = shape as Circle;
                    return $"{circle.Center.X},{circle.Center.Y},{circle.Radius}";

                case "Rectangle":
                    var rect = shape as GraphicEditor.Models.Shapes.Rectangle;
                    return $"{rect.TopLeft.X},{rect.TopLeft.Y},{rect.Width},{rect.Height}";

                case "Triangle":
                    var tri = shape as Triangle;
                    return $"{tri.Point1.X},{tri.Point1.Y},{tri.Point2.X},{tri.Point2.Y},{tri.Point3.X},{tri.Point3.Y}";

                default:
                    return "";
            }
        }
        /// <summary>
        /// Extracts parameters string from a 3D shape for serialization
        /// </summary>
        private string GetShapeParameters3D(IShape3D shape)
        {
            switch (shape.GetName())
            {
                case "Cube":
                    var cube = shape as Cube;
                    return $"{cube.Center.X},{cube.Center.Y},{cube.Center.Z},{cube.Size}";

                case "Sphere":
                    var sphere = shape as Sphere;
                    return $"{sphere.Center.X},{sphere.Center.Y},{sphere.Center.Z},{sphere.Radius}";

                case "Tetrahedron":
                    var tetra = shape as Tetrahedron;
                    return $"{tetra.Center.X},{tetra.Center.Y},{tetra.Center.Z},{tetra.Size}";

                default:
                    return "";
            }
        }

        #endregion

        #region Painting

        private void PanelDraw_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

            DrawAllShapes(e.Graphics);
            DrawCurrentPreview(e.Graphics);
        }

        private void DrawAllShapes(Graphics g)
        {
            var shapes = _controller.GetShapesForRendering();

            using (Pen pen = new Pen(Color.Blue, 2))
            {
                foreach (var shape in shapes)
                {
                    var points = shape.GetPoints();
                    if (points.Count > 1)
                    {
                        for (int i = 0; i < points.Count - 1; i++)
                        {
                            g.DrawLine(pen, points[i].X, points[i].Y, points[i + 1].X, points[i + 1].Y);
                        }
                    }
                }
            }
        }



        /// <summary>
        /// Draws preview of shape while dragging mouse
        /// </summary>
        private void DrawCurrentPreview(Graphics g)
        {
            if (_startPoint == null || _currentPoint == null || !_isDrawing)
                return;

            using (Pen previewPen = new Pen(Color.Gray, 1))
            {
                previewPen.DashStyle = System.Drawing.Drawing2D.DashStyle.Dash;

                // Check if this is a plugin shape
                var plugin2D = _controller.Get2DPlugin(_currentShapeType);
                var plugin3D = _controller.Get3DPlugin(_currentShapeType);

                if (plugin2D != null)
                {
                    // Let the plugin draw its own preview
                    plugin2D.DrawPreview(g, _startPoint.Value, _currentPoint.Value);
                }
                else if (plugin3D != null)
                {
                    // Let the 3D plugin draw its own preview
                    plugin3D.DrawPreview(g, _startPoint.Value, _currentPoint.Value);
                }
                else
                {
                    // Built-in shapes preview
                    switch (_currentShapeType)
                    {
                        case "Circle":
                            int radius = (int)Math.Sqrt(
                                Math.Pow(_currentPoint.Value.X - _startPoint.Value.X, 2) +
                                Math.Pow(_currentPoint.Value.Y - _startPoint.Value.Y, 2));
                            g.DrawEllipse(previewPen,
                                _startPoint.Value.X - radius,
                                _startPoint.Value.Y - radius,
                                radius * 2, radius * 2);
                            break;

                        case "Rectangle":
                            int x = Math.Min(_startPoint.Value.X, _currentPoint.Value.X);
                            int y = Math.Min(_startPoint.Value.Y, _currentPoint.Value.Y);
                            int width = Math.Abs(_currentPoint.Value.X - _startPoint.Value.X);
                            int height = Math.Abs(_currentPoint.Value.Y - _startPoint.Value.Y);
                            g.DrawRectangle(previewPen, x, y, width, height);
                            break;

                        case "Triangle":
                            int midX = (_startPoint.Value.X + _currentPoint.Value.X) / 2;
                            int topY = Math.Min(_startPoint.Value.Y, _currentPoint.Value.Y) -
                                (int)(Math.Abs(_currentPoint.Value.Y - _startPoint.Value.Y) * 0.8);

                            Point[] trianglePoints = new Point[]
                            {
                        _startPoint.Value,
                        _currentPoint.Value,
                        new Point(midX, topY)
                            };
                            g.DrawPolygon(previewPen, trianglePoints);
                            break;

                        case "Cube":
                        case "Sphere":
                        case "Tetrahedron":
                            // Simple rectangle preview for 3D shapes
                            int rectX = Math.Min(_startPoint.Value.X, _currentPoint.Value.X);
                            int rectY = Math.Min(_startPoint.Value.Y, _currentPoint.Value.Y);
                            int rectWidth = Math.Abs(_currentPoint.Value.X - _startPoint.Value.X);
                            int rectHeight = Math.Abs(_currentPoint.Value.Y - _startPoint.Value.Y);
                            g.DrawRectangle(previewPen, rectX, rectY, rectWidth, rectHeight);

                            // Draw diagonal to indicate 3D
                            g.DrawLine(previewPen, rectX, rectY, rectX + rectWidth, rectY + rectHeight);
                            g.DrawLine(previewPen, rectX + rectWidth, rectY, rectX, rectY + rectHeight);
                            break;
                    }
                }
            }

            // Draw start point marker
            using (Brush brush = new SolidBrush(Color.Red))
            {
                g.FillEllipse(brush, _startPoint.Value.X - 3, _startPoint.Value.Y - 3, 6, 6);
            }
        }

        #endregion

        private void clearCanvasToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _controller.ClearAll();
            _startPoint = null;
            _currentPoint = null;
            UpdateShapesList();
            panelDraw.Invalidate();
            statusLabel.Text = "Canvas cleared";
        }


        /// <summary>
        /// Save all shapes to JSON file
        /// </summary>
        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {

            using (SaveFileDialog sfd = new SaveFileDialog())
            {
                sfd.Filter = "JSON files (*.json)|*.json";
                sfd.DefaultExt = "json";
                sfd.FileName = "shapes.json";

                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        var container = new ShapesContainer();

                        // Get all 2D shapes
                        var shapes2D = _controller.GetAllShapes2D();
                        foreach (var shape in shapes2D)
                        {
                            container.Shapes.Add(new ShapeData
                            {
                                Mode = "2D",
                                ShapeType = shape.GetName(),
                                Parameters = GetShapeParameters(shape)
                            });
                        }

                        // Get all 3D shapes
                        var shapes3D = _controller.GetAllShapes3D();
                        foreach (var shape in shapes3D)
                        {
                            container.Shapes.Add(new ShapeData
                            {
                                Mode = "3D",
                                ShapeType = shape.GetName(),
                                Parameters = GetShapeParameters3D(shape)
                            });
                        }

                        container.SavedAt = DateTime.Now;

                        string json = JsonConvert.SerializeObject(container, Formatting.Indented);
                        File.WriteAllText(sfd.FileName, json);

                        statusLabel.Text = $"Saved {container.Shapes.Count} shapes to {sfd.FileName}";
                        MessageBox.Show($"Successfully saved {container.Shapes.Count} shapes!", "Save Successful",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error saving file: {ex.Message}", "Save Error",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                        statusLabel.Text = "Error saving file";
                    }
                }
            }
        }

        private void loadToolStripMenuItem_Click(object sender, EventArgs e)
        {
           /// <summary>
            /// Load shapes from JSON file
            /// </summary>

            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                ofd.Filter = "JSON files (*.json)|*.json";
                ofd.DefaultExt = "json";

                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        string json = File.ReadAllText(ofd.FileName);
                        var container = JsonConvert.DeserializeObject<ShapesContainer>(json);

                        if (container == null || container.Shapes == null)
                        {
                            MessageBox.Show("Invalid file format or empty file", "Load Error",
                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return;
                        }

                        // Clear existing shapes
                        _controller.ClearAll();

                        foreach (var shapeData in container.Shapes)
                        {
                            try
                            {
                                // Temporarily switch mode to match the shape being loaded
                                bool originalMode = _is3DMode;

                                if (shapeData.Mode == "2D")
                                {
                                    // Make sure we're in 2D mode for this shape
                                    if (_is3DMode)
                                    {
                                        _controller.SetMode(false);
                                        _is3DMode = false;
                                        // Update UI to reflect mode change
                                        mode2DRadio.Checked = true;
                                    }
                                }
                                else // "3D"
                                {
                                    // Make sure we're in 3D mode for this shape
                                    if (!_is3DMode)
                                    {
                                        _controller.SetMode(true);
                                        _is3DMode = true;
                                        // Update UI to reflect mode change
                                        mode3DRadio.Checked = true;
                                    }
                                }

                                // Create the shape
                                _controller.CreateShape(shapeData.ShapeType, shapeData.Parameters);
                            }
                            catch (Exception ex)
                            {
                                // Log error but continue loading other shapes
                                System.Diagnostics.Debug.WriteLine($"Error loading shape {shapeData.ShapeType}: {ex.Message}");
                            }
                        }

                        UpdateShapesList();

                        // Refresh the canvas
                        panelDraw.Invalidate();

                        statusLabel.Text = $"Loaded {container.Shapes.Count} shapes from {ofd.FileName}";
                        MessageBox.Show($"Successfully loaded {container.Shapes.Count} shapes!", "Load Successful",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error loading file: {ex.Message}", "Load Error",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                        statusLabel.Text = "Error loading file";
                    }
                }
            }
        }

        private void dleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            /// <summary>
            /// Delete selected shape
            /// </summary>

            if (shapesListBox.SelectedIndex == -1)
            {
                MessageBox.Show("Please select a shape to delete.", "No Selection",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            int selectedIndex = shapesListBox.SelectedIndex;
            int shapes2DCount = _controller.GetAllShapes2D().Count;

            // Confirm deletion
            DialogResult result = MessageBox.Show($"Are you sure you want to delete the selected shape?",
                "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

            if (result == DialogResult.Yes)
            {
                // Temporarily store current mode to know which list to delete from
                bool originalMode = _is3DMode;

                if (selectedIndex < shapes2DCount)
                {
                    // Deleting 2D shape
                    if (originalMode)
                    {
                        _controller.SetMode(false);
                    }
                    _controller.DeleteShape(selectedIndex);
                    if (originalMode)
                    {
                        _controller.SetMode(true);
                    }
                }
                else
                {
                    // Deleting 3D shape
                    int shape3DIndex = selectedIndex - shapes2DCount;
                    if (!originalMode)
                    {
                        _controller.SetMode(true);
                    }
                    _controller.DeleteShape(shape3DIndex);
                    if (!originalMode)
                    {
                        _controller.SetMode(false);
                    }
                }

                // Update UI
                UpdateShapesList();
                panelDraw.Invalidate();

                statusLabel.Text = "Shape deleted successfully";
            }
        }
        

        /// <summary>
        /// Updates the shapes listbox with current shapes
        /// </summary>
        private void UpdateShapesList()
        {
            if (shapesListBox == null) return;

            shapesListBox.Items.Clear();

            var shapes2D = _controller.GetAllShapes2D();
            var shapes3D = _controller.GetAllShapes3D();

            int index = 0;

            // Add 2D shapes
            foreach (var shape in shapes2D)
            {
                string displayText = $"[2D] {shape.GetName()} #{index + 1}";
                shapesListBox.Items.Add(displayText);
                index++;
            }

            // Add 3D shapes
            foreach (var shape in shapes3D)
            {
                string displayText = $"[3D] {shape.GetName()} #{index + 1}";
                shapesListBox.Items.Add(displayText);
                index++;
            }

            if (shapesListBox.Items.Count > 0)
            {
                shapesListBox.SelectedIndex = 0;
            }
        }


        /// <summary>
        /// Updates the delete menu item enabled state
        /// </summary>
        private void UpdateDeleteMenuItem()
        {
            int shapeCount = _controller.GetShapeCount();
            dleteToolStripMenuItem.Enabled = (shapeCount > 0 && shapesListBox.SelectedIndex != -1);
        }
    }
}