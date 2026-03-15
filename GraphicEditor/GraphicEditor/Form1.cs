using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using GraphicEditor.Controllers;
using GraphicEditor.Models.Shapes;

namespace GraphicEditor
{
    public partial class MainForm : Form
    {
        private readonly ShapeController _controller;
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

        public MainForm()
        {
            _controller = new ShapeController();
            _is3DMode = false;
            _isDrawing = true; // Drawing mode enabled by default

            InitializeComponent();
            InitializeCustomControls();
            UpdateShapeTypes();
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

            // Add all controls to panelEditor
            panelEditor.Controls.AddRange(new Control[] {
                modeGroup,
                shapeGroup,
            });

            // Setup mouse events for panelDraw
            panelDraw.MouseDown += PanelDraw_MouseDown;
            panelDraw.MouseMove += PanelDraw_MouseMove;
            panelDraw.MouseUp += PanelDraw_MouseUp;
            panelDraw.Paint += PanelDraw_Paint;
            panelDraw.BackColor = Color.White;
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

        private string GenerateShapeParameters(string shapeType, Point start, Point end)
        {
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

        private void DrawCurrentPreview(Graphics g)
        {
            if (_startPoint == null || _currentPoint == null || !_isDrawing)
                return;

            using (Pen previewPen = new Pen(Color.Gray, 1))
            {
                previewPen.DashStyle = System.Drawing.Drawing2D.DashStyle.Dash;

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

                    default:
                        g.DrawLine(previewPen, _startPoint.Value, _currentPoint.Value);
                        break;
                }
            }

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
            panelDraw.Invalidate();
            statusLabel.Text = "Canvas cleared";
        }
    }
}