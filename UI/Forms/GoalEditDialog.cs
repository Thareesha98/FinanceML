using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace FinanceML.UI.Forms
{
    public partial class GoalEditDialog : Form
    {
        public string GoalName { get; private set; } = string.Empty;
        public decimal TargetAmount { get; private set; }
        public decimal CurrentAmount { get; private set; }
        public DateTime TargetDate { get; private set; }
        public string Description { get; private set; } = string.Empty;

        private readonly bool isEditMode;

        public GoalEditDialog(FinancialGoal? existingGoal = null)
        {
            isEditMode = existingGoal != null;
            InitializeComponent();
            ApplyModernStyling();
            
            if (existingGoal != null)
            {
                nameTextBox.Text = existingGoal.Name;
                targetAmountTextBox.Text = existingGoal.TargetAmount.ToString();
                currentAmountTextBox.Text = existingGoal.CurrentAmount.ToString();
                targetDatePicker.Value = existingGoal.TargetDate;
                descriptionTextBox.Text = existingGoal.Description;
                
                this.Text = "Edit Financial Goal";
                titleLabel.Text = "✏️ Edit Goal";
            }
        }

        private void InitializeComponent()
        {
            this.titleLabel = new Label();
            this.nameLabel = new Label();
            this.nameTextBox = new TextBox();
            this.targetAmountLabel = new Label();
            this.targetAmountTextBox = new TextBox();
            this.currentAmountLabel = new Label();
            this.currentAmountTextBox = new TextBox();
            this.targetDateLabel = new Label();
            this.targetDatePicker = new DateTimePicker();
            this.descriptionLabel = new Label();
            this.descriptionTextBox = new TextBox();
            this.saveBtn = new Button();
            this.cancelBtn = new Button();
            
            this.SuspendLayout();
            
            // Form
            this.AutoScaleDimensions = new SizeF(7F, 15F);
            this.AutoScaleMode = AutoScaleMode.Font;
            this.BackColor = Color.FromArgb(248, 249, 250);
            this.ClientSize = new Size(500, 600);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "GoalEditDialog";
            this.StartPosition = FormStartPosition.CenterParent;
            this.Text = "Add Financial Goal";
            
            // Title Label
            this.titleLabel.Font = new Font("Segoe UI", 18F, FontStyle.Bold);
            this.titleLabel.ForeColor = Color.FromArgb(31, 41, 55);
            this.titleLabel.Location = new Point(30, 30);
            this.titleLabel.Size = new Size(300, 30);
            this.titleLabel.Text = "➕ Add New Goal";
            
            // Name
            this.nameLabel.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            this.nameLabel.ForeColor = Color.FromArgb(75, 85, 99);
            this.nameLabel.Location = new Point(30, 80);
            this.nameLabel.Size = new Size(100, 25);
            this.nameLabel.Text = "Goal Name:";
            
            this.nameTextBox.Font = new Font("Segoe UI", 11F);
            this.nameTextBox.Location = new Point(30, 110);
            this.nameTextBox.Size = new Size(440, 27);
            this.nameTextBox.PlaceholderText = "e.g., Emergency Fund, Vacation, New Car";
            
            // Target Amount
            this.targetAmountLabel.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            this.targetAmountLabel.ForeColor = Color.FromArgb(75, 85, 99);
            this.targetAmountLabel.Location = new Point(30, 160);
            this.targetAmountLabel.Size = new Size(150, 25);
            this.targetAmountLabel.Text = "Target Amount (Rs):";
            
            this.targetAmountTextBox.Font = new Font("Segoe UI", 11F);
            this.targetAmountTextBox.Location = new Point(30, 190);
            this.targetAmountTextBox.Size = new Size(200, 27);
            this.targetAmountTextBox.PlaceholderText = "50000";
            
            // Current Amount
            this.currentAmountLabel.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            this.currentAmountLabel.ForeColor = Color.FromArgb(75, 85, 99);
            this.currentAmountLabel.Location = new Point(270, 160);
            this.currentAmountLabel.Size = new Size(150, 25);
            this.currentAmountLabel.Text = "Current Amount (Rs):";
            
            this.currentAmountTextBox.Font = new Font("Segoe UI", 11F);
            this.currentAmountTextBox.Location = new Point(270, 190);
            this.currentAmountTextBox.Size = new Size(200, 27);
            this.currentAmountTextBox.PlaceholderText = "0";
            this.currentAmountTextBox.Text = "0";
            
            // Target Date
            this.targetDateLabel.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            this.targetDateLabel.ForeColor = Color.FromArgb(75, 85, 99);
            this.targetDateLabel.Location = new Point(30, 240);
            this.targetDateLabel.Size = new Size(120, 25);
            this.targetDateLabel.Text = "Target Date:";
            
            this.targetDatePicker.Font = new Font("Segoe UI", 11F);
            this.targetDatePicker.Location = new Point(30, 270);
            this.targetDatePicker.Size = new Size(200, 27);
            this.targetDatePicker.Value = DateTime.Now.AddYears(1);
            
            // Description
            this.descriptionLabel.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            this.descriptionLabel.ForeColor = Color.FromArgb(75, 85, 99);
            this.descriptionLabel.Location = new Point(30, 320);
            this.descriptionLabel.Size = new Size(120, 25);
            this.descriptionLabel.Text = "Description:";
            
            this.descriptionTextBox.Font = new Font("Segoe UI", 11F);
            this.descriptionTextBox.Location = new Point(30, 350);
            this.descriptionTextBox.Multiline = true;
            this.descriptionTextBox.Size = new Size(440, 100);
            this.descriptionTextBox.PlaceholderText = "Optional: Add details about this goal...";
            
            // Save Button
            this.saveBtn.BackColor = Color.FromArgb(37, 99, 235);
            this.saveBtn.FlatStyle = FlatStyle.Flat;
            this.saveBtn.FlatAppearance.BorderSize = 0;
            this.saveBtn.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            this.saveBtn.ForeColor = Color.White;
            this.saveBtn.Location = new Point(270, 480);
            this.saveBtn.Size = new Size(100, 40);
            this.saveBtn.Text = "Save";
            this.saveBtn.UseVisualStyleBackColor = false;
            this.saveBtn.Click += OnSaveClick;
            
            // Cancel Button
            this.cancelBtn.BackColor = Color.FromArgb(107, 114, 128);
            this.cancelBtn.FlatStyle = FlatStyle.Flat;
            this.cancelBtn.FlatAppearance.BorderSize = 0;
            this.cancelBtn.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            this.cancelBtn.ForeColor = Color.White;
            this.cancelBtn.Location = new Point(380, 480);
            this.cancelBtn.Size = new Size(100, 40);
            this.cancelBtn.Text = "Cancel";
            this.cancelBtn.UseVisualStyleBackColor = false;
            this.cancelBtn.Click += (s, e) => { this.DialogResult = DialogResult.Cancel; this.Close(); };
            
            // Add controls to form
            this.Controls.Add(this.titleLabel);
            this.Controls.Add(this.nameLabel);
            this.Controls.Add(this.nameTextBox);
            this.Controls.Add(this.targetAmountLabel);
            this.Controls.Add(this.targetAmountTextBox);
            this.Controls.Add(this.currentAmountLabel);
            this.Controls.Add(this.currentAmountTextBox);
            this.Controls.Add(this.targetDateLabel);
            this.Controls.Add(this.targetDatePicker);
            this.Controls.Add(this.descriptionLabel);
            this.Controls.Add(this.descriptionTextBox);
            this.Controls.Add(this.saveBtn);
            this.Controls.Add(this.cancelBtn);
            
            this.ResumeLayout(false);
        }

        private void ApplyModernStyling()
        {
            // Apply rounded corners to buttons and text boxes
            var buttons = new[] { saveBtn, cancelBtn };
            foreach (var button in buttons)
            {
                button.Region = CreateRoundedRegion(button.Size, 8);
            }
            
            var textBoxes = new[] { nameTextBox, targetAmountTextBox, currentAmountTextBox, descriptionTextBox };
            foreach (var textBox in textBoxes)
            {
                textBox.BorderStyle = BorderStyle.FixedSingle;
            }
        }

        private Region CreateRoundedRegion(Size size, int radius)
        {
            GraphicsPath path = new GraphicsPath();
            path.AddArc(0, 0, radius, radius, 180, 90);
            path.AddArc(size.Width - radius, 0, radius, radius, 270, 90);
            path.AddArc(size.Width - radius, size.Height - radius, radius, radius, 0, 90);
            path.AddArc(0, size.Height - radius, radius, radius, 90, 90);
            path.CloseAllFigures();
            return new Region(path);
        }

        private void OnSaveClick(object sender, EventArgs e)
        {
            // Validate inputs
            if (string.IsNullOrWhiteSpace(nameTextBox.Text))
            {
                MessageBox.Show("Please enter a goal name.", "Validation Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                nameTextBox.Focus();
                return;
            }

            if (!decimal.TryParse(targetAmountTextBox.Text, out decimal targetAmount) || targetAmount <= 0)
            {
                MessageBox.Show("Please enter a valid target amount.", "Validation Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                targetAmountTextBox.Focus();
                return;
            }

            if (!decimal.TryParse(currentAmountTextBox.Text, out decimal currentAmount) || currentAmount < 0)
            {
                MessageBox.Show("Please enter a valid current amount.", "Validation Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                currentAmountTextBox.Focus();
                return;
            }

            if (targetDatePicker.Value <= DateTime.Now.Date)
            {
                MessageBox.Show("Target date must be in the future.", "Validation Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                targetDatePicker.Focus();
                return;
            }

            // Set properties
            GoalName = nameTextBox.Text.Trim();
            TargetAmount = targetAmount;
            CurrentAmount = currentAmount;
            TargetDate = targetDatePicker.Value;
            Description = descriptionTextBox.Text.Trim();

            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        #region Designer Fields
        private Label titleLabel;
        private Label nameLabel;
        private TextBox nameTextBox;
        private Label targetAmountLabel;
        private TextBox targetAmountTextBox;
        private Label currentAmountLabel;
        private TextBox currentAmountTextBox;
        private Label targetDateLabel;
        private DateTimePicker targetDatePicker;
        private Label descriptionLabel;
        private TextBox descriptionTextBox;
        private Button saveBtn;
        private Button cancelBtn;
        #endregion
    }
}
