using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace GitContributionApp
{
    /// <summary>
    /// Main form for the multi-tab utility application.
    /// Includes a text editor and a data entry form with validation.
    /// </summary>
    public partial class NotepadForm : Form
    {
        // Used to track the file path of the currently open text file.
        private string currentFilePath = null;

        public NotepadForm()
        {
            InitializeComponent();
            // Set the initial state of the form
            this.Text = "Untitled - Multi-Tab Utility";
        }

        #region Text Editor Tab Logic

        // --- File Menu ---

        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Clear the text editor
            rtbEditor.Clear();
            currentFilePath = null;
            this.Text = "Untitled - Multi-Tab Utility";
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                ofd.Filter = "Text Files (*.txt)|*.txt|Rich Text Files (*.rtf)|*.rtf|All Files (*.*)|*.*";
                ofd.DefaultExt = ".txt";

                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        // Save the file path
                        currentFilePath = ofd.FileName;
                        
                        // Load the file content
                        // Check file extension to use the correct load method
                        if (Path.GetExtension(currentFilePath).ToLower() == ".rtf")
                        {
                            rtbEditor.LoadFile(currentFilePath, RichTextBoxStreamType.RichText);
                        }
                        else
                        {
                            rtbEditor.LoadFile(currentFilePath, RichTextBoxStreamType.PlainText);
                        }
                        
                        // Update the form title with the file name
                        this.Text = Path.GetFileName(currentFilePath) + " - Multi-Tab Utility";
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error opening file: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // If the file hasn't been saved before, use "Save As" logic
            if (string.IsNullOrEmpty(currentFilePath))
            {
                saveAsToolStripMenuItem_Click(sender, e);
            }
            else
            {
                // Save the file to its current path
                SaveFile(currentFilePath);
            }
        }

        private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (SaveFileDialog sfd = new SaveFileDialog())
            {
                sfd.Filter = "Text Files (*.txt)|*.txt|Rich Text Files (*.rtf)|*.rtf|All Files (*.*)|*.*";
                sfd.DefaultExt = ".txt";

                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    currentFilePath = sfd.FileName;
                    SaveFile(currentFilePath);
                    this.Text = Path.GetFileName(currentFilePath) + " - Multi-Tab Utility";
                }
            }
        }

        /// <summary>
        /// Helper method to save the file based on its extension.
        /// </summary>
        private void SaveFile(string filePath)
        {
            try
            {
                if (Path.GetExtension(filePath).ToLower() == ".rtf")
                {
                    rtbEditor.SaveFile(filePath, RichTextBoxStreamType.RichText);
                }
                else
                {
                    rtbEditor.SaveFile(filePath, RichTextBoxStreamType.PlainText);
                }
                // Update status bar
                toolStripStatusLabel.Text = "File saved successfully.";
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error saving file: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Close the application
            Application.Exit();
        }

        // --- Edit Menu ---

        private void undoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (rtbEditor.CanUndo)
            {
                rtbEditor.Undo();
            }
        }

        private void redoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (rtbEditor.CanRedo)
            {
                rtbEditor.Redo();
            }
        }

        private void cutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (rtbEditor.SelectionLength > 0)
            {
                rtbEditor.Cut();
            }
        }

        private void copyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (rtbEditor.SelectionLength > 0)
            {
                rtbEditor.Copy();
            }
        }

        private void pasteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (Clipboard.ContainsText(TextDataFormat.Rtf) || Clipboard.ContainsText(TextDataFormat.Text))
            {
                rtbEditor.Paste();
            }
        }

        private void selectAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            rtbEditor.SelectAll();
        }

        #endregion

        #region Data Entry Tab Logic

        private void btnSubmit_Click(object sender, EventArgs e)
        {
            // Clear previous errors before validating again
            errorProvider.Clear();

            // Run all validation checks
            if (ValidateName() && ValidateEmail() && ValidatePhone())
            {
                // If all validation passes
                string successMessage = $"Submission Successful!\n\n" +
                                        $"Name: {txtName.Text}\n" +
                                        $"Email: {txtEmail.Text}\n" +
                                        $"Phone: {txtPhone.Text}";
                
                MessageBox.Show(successMessage, "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                
                // Clear the form
                txtName.Clear();
                txtEmail.Clear();
                txtPhone.Clear();
            }
            else
            {
                // If validation fails
                MessageBox.Show("Please correct the errors on the form.", "Validation Failed", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        // --- Validation Methods ---

        private bool ValidateName()
        {
            if (string.IsNullOrWhiteSpace(txtName.Text))
            {
                errorProvider.SetError(txtName, "Name is required.");
                return false;
            }
            return true;
        }

        private bool ValidateEmail()
        {
            if (string.IsNullOrWhiteSpace(txtEmail.Text))
            {
                errorProvider.SetError(txtEmail, "Email is required.");
                return false;
            }
            
            // Basic email regex validation
            string emailPattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
            if (!Regex.IsMatch(txtEmail.Text, emailPattern))
            {
                errorProvider.SetError(txtEmail, "Please enter a valid email address.");
                return false;
            }
            return true;
        }

        private bool ValidatePhone()
        {
            // Optional field, but if filled, it must be valid
            if (!string.IsNullOrWhiteSpace(txtPhone.Text))
            {
                // Simple validation: allows numbers, spaces, hyphens, and parentheses
                string phonePattern = @"^[\d\s\-\(\)]+$";
                if (!Regex.IsMatch(txtPhone.Text, phonePattern))
                {
                    errorProvider.SetError(txtPhone, "Invalid phone number format.");
                    return false;
                }
            }
            return true;
        }

        // --- Real-time Validation (Validating event) ---

        private void txtName_Validating(object sender, System.ComponentModel.CancelEventArgs e)
        {
            // ValidateName(); // You can enable this for real-time validation
        }

        private void txtEmail_Validating(object sender, System.ComponentModel.CancelEventArgs e)
        {
            // ValidateEmail(); // You can enable this for real-time validation
        }

        private void txtPhone_Validating(object sender, System.ComponentModel.CancelEventArgs e)
        {
            // ValidatePhone(); // You can enable this for real-time validation
        }

        #endregion
    }
}
