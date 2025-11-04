using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows.Forms;

namespace GitContributionApp.ImageViewer
{
    /// <summary>
    /// A simple image viewer form with capabilities to open, save, 
    /// rotate, zoom, and apply a grayscale filter.
    /// </summary>
    public partial class ImageViewerForm : Form
    {
        // The currently loaded image
        private Bitmap currentBitmap = null;
        
        // The path of the currently loaded file
        private string currentFilePath = null;
        
        // Current zoom factor
        private double zoomFactor = 1.0;

        public ImageViewerForm()
        {
            InitializeComponent();
            UpdateZoomStatus();
            UpdateImageInfoStatus(null);
        }

        #region File Menu Handlers

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                ofd.Title = "Open Image";
                ofd.Filter = "Image Files|*.bmp;*.jpg;*.jpeg;*.png;*.gif;*.tif;*.tiff|All Files|*.*";
                
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    LoadImage(ofd.FileName);
                }
            }
        }

        private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (currentBitmap == null)
            {
                MessageBox.Show("There is no image to save.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            using (SaveFileDialog sfd = new SaveFileDialog())
            {
                sfd.Title = "Save Image As";
                sfd.Filter = "PNG Image|*.png|JPEG Image|*.jpg|Bitmap Image|*.bmp|GIF Image|*.gif";
                sfd.DefaultExt = "png";
                sfd.FileName = Path.GetFileNameWithoutExtension(currentFilePath) ?? "Untitled";

                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        ImageFormat format = ImageFormat.Png; // Default
                        string ext = Path.GetExtension(sfd.FileName).ToLower();

                        if (ext == ".jpg") format = ImageFormat.Jpeg;
                        else if (ext == ".bmp") format = ImageFormat.Bmp;
                        else if (ext == ".gif") format = ImageFormat.Gif;

                        currentBitmap.Save(sfd.FileName, format);
                        currentFilePath = sfd.FileName;
                        this.Text = "Image Viewer - " + Path.GetFileName(currentFilePath);
                        statusLabelFileName.Text = currentFilePath;
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Failed to save image: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        #endregion

        #region Edit & View Menu Handlers

        private void rotateLeftToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RotateImage(RotateFlipType.Rotate270FlipNone);
        }

        private void rotateRightToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RotateImage(RotateFlipType.Rotate90FlipNone);
        }

        private void zoomInToolStripMenuItem_Click(object sender, EventArgs e)
        {
            zoomFactor *= 1.25; // Increase zoom by 25%
            ApplyZoom();
        }

        private void zoomOutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            zoomFactor /= 1.25; // Decrease zoom by 25%
            ApplyZoom();
        }

        private void resetZoomToolStripMenuItem_Click(object sender, EventArgs e)
        {
            zoomFactor = 1.0;
            ApplyZoom();
        }

        private void grayscaleToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ApplyGrayscaleFilter();
        }

        #endregion

        #region Core Image Logic

        /// <summary>
        /// Loads an image from a file path into the PictureBox.
        /// </summary>
        private void LoadImage(string filePath)
        {
            try
            {
                // Dispose of the old bitmap
                currentBitmap?.Dispose();

                // Load the new bitmap from file
                currentBitmap = new Bitmap(filePath);
                currentFilePath = filePath;
                
                // Reset zoom and apply to PictureBox
                zoomFactor = 1.0;
                ApplyZoom();

                // Update UI
                this.Text = "Image Viewer - " + Path.GetFileName(currentFilePath);
                UpdateImageInfoStatus(currentBitmap);
                statusLabelFileName.Text = currentFilePath;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Could not load image: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                UpdateImageInfoStatus(null);
                statusLabelFileName.Text = "No file loaded.";
            }
        }

        /// <summary>
        /// Applies a rotation to the current image and refreshes the PictureBox.
        /// </summary>
        private void RotateImage(RotateFlipType rotateFlipType)
        {
            if (currentBitmap == null) return;

            currentBitmap.RotateFlip(rotateFlipType);
            ApplyZoom(); // Re-apply zoom to refresh the image display
            UpdateImageInfoStatus(currentBitmap);
        }

        /// <summary>
        _summary>
        /// Applies the current zoomFactor to the PictureBox size.
        /// </summary>
        private void ApplyZoom()
        {
            if (currentBitmap == null)
            {
                pictureBox.Image = null;
                pictureBox.Size = panelMain.Size; // Fit to panel
            }
            else
            {
                // Set the PictureBox size based on the bitmap and zoom
                int newWidth = (int)(currentBitmap.Width * zoomFactor);
                int newHeight = (int)(currentBitmap.Height * zoomFactor);
                pictureBox.Size = new Size(newWidth, newHeight);
                
                // Set the image for display
                // For performance on large zooms, we might create a resized bitmap.
                // For simplicity here, we just assign the original and let PictureBox.Zoom handle it
                // (if SizeMode was Zoom).
                // Since we are manually resizing, we set the original image.
                pictureBox.Image = currentBitmap; 
            }
            
            UpdateZoomStatus();
        }

        /// <summary>
        /// Applies a simple grayscale filter to the current bitmap.
        /// </summary>
        private void ApplyGrayscaleFilter()
        {
            if (currentBitmap == null) return;

            try
            {
                // We create a new bitmap to apply the filter to
                Bitmap newGrayBitmap = new Bitmap(currentBitmap.Width, currentBitmap.Height);

                using (Graphics g = Graphics.FromImage(newGrayBitmap))
                {
                    // Create the grayscale color matrix
                    ColorMatrix colorMatrix = new ColorMatrix(
                        new float[][]
                        {
                            new float[] {.3f, .3f, .3f, 0, 0},
                            new float[] {.59f, .59f, .59f, 0, 0},
                            new float[] {.11f, .11f, .11f, 0, 0},
                            new float[] {0, 0, 0, 1, 0},
                            new float[] {0, 0, 0, 0, 1}
                        });

                    using (ImageAttributes attributes = new ImageAttributes())
                    {
                        attributes.SetColorMatrix(colorMatrix);
                        g.DrawImage(currentBitmap, 
                                    new Rectangle(0, 0, currentBitmap.Width, currentBitmap.Height),
                                    0, 0, currentBitmap.Width, currentBitmap.Height,
                                    GraphicsUnit.Pixel, attributes);
                    }
                }

                // Replace the old bitmap
                currentBitmap.Dispose();
                currentBitmap = newGrayBitmap;
                pictureBox.Image = currentBitmap; // Update the display
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to apply filter: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        #endregion

        #region Status Bar Helpers

        private void UpdateZoomStatus()
        {
            statusLabelZoom.Text = $"Zoom: {zoomFactor:P0}";
        }

        private void UpdateImageInfoStatus(Bitmap bmp)
        {
            if (bmp == null)
            {
                statusLabelImageSize.Text = "N/A";
            }
            else
            {
                statusLabelImageSize.Text = $"Size: {bmp.Width} x {bmp.Height}";
            }
        }

        #endregion
    }
}
