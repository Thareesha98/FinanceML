using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging; // For ImageFormat
using System.Windows.Forms;


{
    public partial class SimplePaintForm : Form
    {
        // Drawing-related fields
        private Bitmap drawingSurface;      // The bitmap we draw on (our canvas)
        private Graphics surfaceGraphics;   // The graphics object for the bitmap
        private Point lastMousePosition;    // The last known mouse position
        private bool isDrawing = false;     // True if the mouse button is down

        // Tool-related fields
        private enum DrawingTool
        {
            Pen,
            Eraser
        }
        private DrawingTool currentTool = DrawingTool.Pen;
        private Color currentColor = Color.Black;
        private int currentPenWidth = 5;

        public SimplePaintForm()
        {
            InitializeComponent();
        }

        private void SimplePaintForm_Load(object sender, EventArgs e)
        {
            // Initialize the drawing surface
            InitializeSurface();
            UpdatePenInfo();
        }

        /// <summary>
        /// Creates or re-creates the drawing bitmap and graphics object
        /// </summary>
        private void InitializeSurface()
        {
            // Dispose old objects if they exist
            surfaceGraphics?.Dispose();
            drawingSurface?.Dispose();

            // Create a new bitmap matching the panel's size
            drawingSurface = new Bitmap(panelDraw.Width, panelDraw.Height);
            
            // Create a graphics object to draw on this bitmap
            surfaceGraphics = Graphics.FromImage(drawingSurface);
            
            // Set high-quality rendering
            surfaceGraphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

            // Clear to white
            surfaceGraphics.Clear(Color.White);

            // Force the panel to redraw
            panelDraw.Invalidate();
        }

        #region Mouse Drawing Events

        private void panelDraw_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                // Start drawing
                isDrawing = true;
                lastMousePosition = e.Location;
            }
        }

        private void panelDraw_MouseMove(object sender, MouseEventArgs e)
        {
            if (isDrawing)
            {
                // Get the appropriate color and pen
                Color drawColor = (currentTool == DrawingTool.Pen) ? currentColor : panelDraw.BackColor;
                using (Pen pen = new Pen(drawColor, currentPenWidth))
                {
                    pen.StartCap = System.Drawing.Drawing2D.LineCap.Round;
                    pen.EndCap = System.Drawing.Drawing2D.LineCap.Round;
                    
                    // Draw a line from the last position to the current one
                    surfaceGraphics.DrawLine(pen, lastMousePosition, e.Location);
                }
                
                // Update the last position
                lastMousePosition = e.Location;
                
                // Tell the panel to repaint
                panelDraw.Invalidate();
            }
        }

        private void panelDraw_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                // Stop drawing
                isDrawing = false;
            }
        }

        /// <summary>
        /// This is the most important event. It draws the in-memory bitmap
        /// onto the panel's visible surface.
        /// </summary>
        private void panelDraw_Paint(object sender, PaintEventArgs e)
        {
            // Draw our bitmap onto the panel
            e.Graphics.DrawImage(drawingSurface, Point.Empty);
        }

        /// <summary>
        /// If the form is resized, we must recreate the canvas
        /// </summary>
        private void panelDraw_Resize(object sender, EventArgs e)
        {
            // Note: A more robust solution would copy the old bitmap
            // onto the new, larger one. For simplicity, we just clear it.
            InitializeSurface();
        }

        #endregion

        #region Tool & Menu Handlers

        private void btnPen_Click(object sender, EventArgs e)
        {
            currentTool = DrawingTool.Pen;
            UpdatePenInfo();
        }

        private void btnEraser_Click(object sender, EventArgs e)
        {
            currentTool = DrawingTool.Eraser;
            UpdatePenInfo();
        }

        private void btnColor_Click(object sender, EventArgs e)
        {
            // Show the color dialog
            if (colorDialog.ShowDialog() == DialogResult.OK)
            {
                currentColor = colorDialog.Color;
                panelColorPreview.BackColor = currentColor;
                UpdatePenInfo();
            }
        }

        private void trackBarPenSize_Scroll(object sender, EventArgs e)
        {
            currentPenWidth = trackBarPenSize.Value;
            UpdatePenInfo();
        }

        /// <summary>
        /// Updates the status strip with the current tool info
        /// </summary>
        private void UpdatePenInfo()
        {
            lblPenSize.Text = $"{currentPenWidth}px";
            statusLabelTool.Text = $"Tool: {currentTool}";
            statusLabelColor.Text = $"Color: {currentColor.Name}";
        }

        // --- Menu Items ---

        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Clear the canvas
            surfaceGraphics.Clear(Color.White);
            panelDraw.Invalidate();
        }

        private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            saveFileDialog.Filter = "PNG Image|*.png|JPEG Image|*.jpg|Bitmap Image|*.bmp";
            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    string filePath = saveFileDialog.FileName;
                    ImageFormat format = ImageFormat.Png; // Default

                    if (filePath.EndsWith(".jpg")) format = ImageFormat.Jpeg;
                    else if (filePath.EndsWith(".bmp")) format = ImageFormat.Bmp;

                    // Save the bitmap (our canvas) to the file
                    drawingSurface.Save(filePath, format);
                    statusLabelMain.Text = "File saved successfully.";
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Failed to save image: " + ex.Message, "Save Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    statusLabelMain.Text = "Save failed.";
                }
            }
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        #endregion
    }
}
