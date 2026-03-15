namespace GraphicEditor
{
    partial class MainForm
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            panelEditor = new Panel();
            menuStrip1 = new MenuStrip();
            fileToolStripMenuItem = new ToolStripMenuItem();
            clearCanvasToolStripMenuItem = new ToolStripMenuItem();
            panelDraw = new Panel();
            panelEditor.SuspendLayout();
            menuStrip1.SuspendLayout();
            SuspendLayout();
            // 
            // panelEditor
            // 
            panelEditor.BackColor = SystemColors.AppWorkspace;
            panelEditor.BorderStyle = BorderStyle.Fixed3D;
            panelEditor.Controls.Add(menuStrip1);
            panelEditor.Dock = DockStyle.Right;
            panelEditor.Location = new Point(1214, 0);
            panelEditor.Name = "panelEditor";
            panelEditor.Size = new Size(723, 1229);
            panelEditor.TabIndex = 0;
            // 
            // menuStrip1
            // 
            menuStrip1.ImageScalingSize = new Size(32, 32);
            menuStrip1.Items.AddRange(new ToolStripItem[] { fileToolStripMenuItem, clearCanvasToolStripMenuItem });
            menuStrip1.Location = new Point(0, 0);
            menuStrip1.Name = "menuStrip1";
            menuStrip1.Size = new Size(719, 42);
            menuStrip1.TabIndex = 0;
            menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            fileToolStripMenuItem.Size = new Size(71, 38);
            fileToolStripMenuItem.Text = "File";
            // 
            // clearCanvasToolStripMenuItem
            // 
            clearCanvasToolStripMenuItem.Name = "clearCanvasToolStripMenuItem";
            clearCanvasToolStripMenuItem.Size = new Size(166, 38);
            clearCanvasToolStripMenuItem.Text = "Clear canvas";
            clearCanvasToolStripMenuItem.Click += clearCanvasToolStripMenuItem_Click;
            // 
            // panelDraw
            // 
            panelDraw.BackColor = SystemColors.ActiveCaptionText;
            panelDraw.Dock = DockStyle.Fill;
            panelDraw.Location = new Point(0, 0);
            panelDraw.Name = "panelDraw";
            panelDraw.Size = new Size(1214, 1229);
            panelDraw.TabIndex = 1;
            // 
            // MainForm
            // 
            AutoScaleDimensions = new SizeF(16F, 32F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1937, 1229);
            Controls.Add(panelDraw);
            Controls.Add(panelEditor);
            Font = new Font("Arial", 10F);
            Icon = (Icon)resources.GetObject("$this.Icon");
            Margin = new Padding(3, 4, 3, 4);
            MinimumSize = new Size(1963, 1250);
            Name = "MainForm";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Graphic Editor";
            panelEditor.ResumeLayout(false);
            panelEditor.PerformLayout();
            menuStrip1.ResumeLayout(false);
            menuStrip1.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private Panel panelEditor;
        private Panel panelDraw;
        private MenuStrip menuStrip1;
        private ToolStripMenuItem fileToolStripMenuItem;
        private ToolStripMenuItem clearCanvasToolStripMenuItem;
    }
}
