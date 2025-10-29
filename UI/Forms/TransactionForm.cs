using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Windows.Forms;
using FinanceML.Core.Models;
using FinanceML.Core.Services;
namespace FinanceML
{
    public partial class TransactionForm : Form
    {
        private Transaction? editingTransaction;
        private DataService dataService;
        
        public TransactionForm() : this(null)
        {
        }

        public TransactionForm(Transaction? transaction)
        {
            dataService = DataService.Instance;
            editingTransaction = transaction;
            InitializeComponent();
            
            // Set form title and button text
            this.Text = editingTransaction == null ? "Add Transaction" : "Edit Transaction";
            titleLabel.Text = editingTransaction == null ? "Add New Transaction" : "Edit Transaction";
            saveButton.Text = editingTransaction == null ? "Save" : "Update";
            
            // Initialize combo boxes with icons
            categoryComboBox.Items.AddRange(new string[] {
                "Food & Dining", "Transportation", "Shopping", "Entertainment",
                "Bills & Utilities", "Healthcare", "Education", "Travel", "Income", "Other"
            });
            categoryComboBox.SelectedIndex = 0;
            
            typeComboBox.Items.AddRange(new string[] { "Expense", "Income" });
            typeComboBox.SelectedIndex = 0;
            
            datePicker.Value = DateTime.Today;
            
            // Apply modern styling
            ApplyModernStyling();
            
            if (editingTransaction != null)
            {
                LoadTransactionData();
            }
            
            // Add smart categorization event handlers
            descriptionTextBox.TextChanged += OnDescriptionChanged;
            amountTextBox.TextChanged += OnAmountChanged;
        }

        private string CleanCategoryName(string category)
        {
            if (category.Contains("🍽️")) return "Food & Dining";
            if (category.Contains("🚗")) return "Transportation";
            if (category.Contains("🛍️")) return "Shopping";
            if (category.Contains("🎬")) return "Entertainment";
            if (category.Contains("💡")) return "Bills & Utilities";
            if (category.Contains("🏥")) return "Healthcare";
            if (category.Contains("📚")) return "Education";
            if (category.Contains("✈️")) return "Travel";
            if (category.Contains("💰")) return "Income";
            if (category.Contains("📦")) return "Other";
            return category; // Return as-is if no emoji found
        }
        
        private void ApplyModernStyling()
        {
            // Set form properties for modern look
            this.SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | ControlStyles.DoubleBuffer, true);
            
            // Style text boxes
            foreach (Control control in this.Controls)
            {
                StyleControl(control);
            }
        }
        
        private void StyleControl(Control control)
        {
            if (control is TextBox textBox)
            {
                textBox.BorderStyle = BorderStyle.None;
                textBox.BackColor = Color.FromArgb(249, 250, 251);
                textBox.Font = new Font("Segoe UI", 11F);
            }
            else if (control is ComboBox comboBox)
            {
                comboBox.FlatStyle = FlatStyle.Flat;
                comboBox.BackColor = Color.FromArgb(249, 250, 251);
                comboBox.Font = new Font("Segoe UI", 11F);
            }
            else if (control is DateTimePicker dateTimePicker)
            {
                dateTimePicker.Font = new Font("Segoe UI", 11F);
            }
            
            // Recursively style child controls
            foreach (Control child in control.Controls)
            {
                StyleControl(child);
            }
        }
        
        private void LoadTransactionData()
        {
            if (editingTransaction != null)
            {
                datePicker.Value = editingTransaction.Date;
                descriptionTextBox.Text = editingTransaction.Description;
                
                categoryComboBox.Text = CleanCategoryName(editingTransaction.Category);
                amountTextBox.Text = editingTransaction.Amount.ToString("F2");
                typeComboBox.SelectedItem = editingTransaction.Type.ToString();
            }
        }

        private void OnSaveClick(object sender, EventArgs e)
        {
            if (ValidateInput())
            {
                var transaction = new Transaction
                {
                    Date = datePicker.Value.Date,
                    Description = descriptionTextBox.Text.Trim(),
                    Category = categoryComboBox.Text,
                    Amount = decimal.Parse(amountTextBox.Text),
                    Type = Enum.TryParse(typeComboBox.SelectedItem?.ToString(), out TransactionType type) ? type : TransactionType.Expense
                };

                if (editingTransaction == null)
                {
                    dataService.AddTransaction(transaction);
                    

                    
                    MessageBox.Show("Transaction added successfully!", "Success", 
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    transaction.Id = editingTransaction.Id;
                    dataService.UpdateTransaction(transaction);
                    MessageBox.Show("Transaction updated successfully!", "Success", 
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }

                this.DialogResult = DialogResult.OK;
                this.Close();
            }
        }
        
        private void OnCancelClick(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
        
        private void OnDescriptionChanged(object sender, EventArgs e)
        {
            if (editingTransaction != null) return; // Don't auto-categorize when editing
            
            SuggestCategory();
        }
        
        private void OnAmountChanged(object sender, EventArgs e)
        {
            if (editingTransaction != null) return; // Don't auto-categorize when editing
            
            SuggestCategory();
        }
        
        private void SuggestCategory()
        {
            // Category suggestion feature is currently disabled
        }

        
        private bool ValidateInput()
        {
            if (string.IsNullOrWhiteSpace(descriptionTextBox.Text))
            {
                ShowValidationError("Please enter a description.", descriptionTextBox);
                return false;
            }
            
            if (string.IsNullOrWhiteSpace(amountTextBox.Text))
            {
                ShowValidationError("Please enter an amount.", amountTextBox);
                return false;
            }
            
            if (!decimal.TryParse(amountTextBox.Text, out decimal amount) || amount <= 0)
            {
                ShowValidationError("Please enter a valid positive amount.", amountTextBox);
                return false;
            }
            
            return true;
        }
        
        private void ShowValidationError(string message, Control control)
        {
            MessageBox.Show(message, "⚠️ Validation Error", 
                MessageBoxButtons.OK, MessageBoxIcon.Warning);
            control.Focus();
            
            // Add visual feedback
            control.BackColor = Color.FromArgb(254, 242, 242);
            var timer = new System.Windows.Forms.Timer();
            timer.Interval = 2000;
            timer.Tick += (s, e) => {
                timer.Stop();
                timer.Dispose();
                control.BackColor = Color.FromArgb(249, 250, 251);
            };
            timer.Start();
        }
        
        // Custom paint events for modern styling
        private void OnCardPaint(object sender, PaintEventArgs e)
        {
            var panel = sender as Panel;
            var rect = new Rectangle(0, 0, panel.Width, panel.Height);
            
            using (var path = GetRoundedRectangle(rect, 16))
            {
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                
                // Draw shadow
                using (var shadowBrush = new SolidBrush(Color.FromArgb(20, 0, 0, 0)))
                {
                    var shadowRect = new Rectangle(rect.X + 3, rect.Y + 3, rect.Width, rect.Height);
                    using (var shadowPath = GetRoundedRectangle(shadowRect, 16))
                    {
                        e.Graphics.FillPath(shadowBrush, shadowPath);
                    }
                }
                
                // Draw card background
                using (var brush = new SolidBrush(Color.White))
                {
                    e.Graphics.FillPath(brush, path);
                }
                
                // Draw border
                using (var pen = new Pen(Color.FromArgb(229, 231, 235), 1))
                {
                    e.Graphics.DrawPath(pen, path);
                }
            }
        }
        
        private void OnHeaderPaint(object sender, PaintEventArgs e)
        {
            var panel = sender as Panel;
            var rect = new Rectangle(0, 0, panel.Width, panel.Height);
            
            using (var path = GetRoundedRectangle(rect, 16, true)) // Only round top corners
            {
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                
                // Create gradient
                using (var brush = new LinearGradientBrush(rect, 
                    Color.FromArgb(59, 130, 246), Color.FromArgb(37, 99, 235), LinearGradientMode.Vertical))
                {
                    e.Graphics.FillPath(brush, path);
                }
            }
        }
        
        private GraphicsPath GetRoundedRectangle(Rectangle rect, int radius, bool topOnly = false)
        {
            var path = new GraphicsPath();
            int diameter = radius * 2;
            
            // Top corners
            path.AddArc(rect.X, rect.Y, diameter, diameter, 180, 90);
            path.AddArc(rect.Right - diameter, rect.Y, diameter, diameter, 270, 90);
            
            if (topOnly)
            {
                // Square bottom corners
                path.AddLine(rect.Right, rect.Y + radius, rect.Right, rect.Bottom);
                path.AddLine(rect.Right, rect.Bottom, rect.X, rect.Bottom);
                path.AddLine(rect.X, rect.Bottom, rect.X, rect.Y + radius);
            }
            else
            {
                // Rounded bottom corners
                path.AddArc(rect.Right - diameter, rect.Bottom - diameter, diameter, diameter, 0, 90);
                path.AddArc(rect.X, rect.Bottom - diameter, diameter, diameter, 90, 90);
            }
            
            path.CloseFigure();
            return path;
        }
    }
}
