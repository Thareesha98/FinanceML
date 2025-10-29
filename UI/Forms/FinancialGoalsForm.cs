using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Windows.Forms;
using FinanceML.Core.Models;
using FinanceML.Core.Services;

namespace FinanceML.UI.Forms
{
    public partial class FinancialGoalsForm : Form
    {
        private DataService dataService;
        private List<FinancialGoal> goals;

        public FinancialGoalsForm()
        {
            dataService = DataService.Instance;
            goals = new List<FinancialGoal>();
            InitializeComponent();
            ApplyModernStyling();
            LoadGoals();
        }

        private void InitializeComponent()
        {
            this.titleLabel = new Label();
            this.goalsListView = new ListView();
            this.addGoalBtn = new Button();
            this.editGoalBtn = new Button();
            this.deleteGoalBtn = new Button();
            this.progressPanel = new Panel();
            this.closeBtn = new Button();
            
            this.SuspendLayout();
            
            // Form
            this.AutoScaleDimensions = new SizeF(7F, 15F);
            this.AutoScaleMode = AutoScaleMode.Font;
            this.BackColor = Color.FromArgb(248, 249, 250);
            this.ClientSize = new Size(900, 600);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FinancialGoalsForm";
            this.StartPosition = FormStartPosition.CenterParent;
            this.Text = "Financial Goals Manager";
            
            // Title Label
            this.titleLabel.Font = new Font("Segoe UI", 24F, FontStyle.Bold);
            this.titleLabel.ForeColor = Color.FromArgb(31, 41, 55);
            this.titleLabel.Location = new Point(30, 30);
            this.titleLabel.Size = new Size(400, 40);
            this.titleLabel.Text = "🎯 Financial Goals";
            
            // Goals ListView
            this.goalsListView.BackColor = Color.White;
            this.goalsListView.BorderStyle = BorderStyle.None;
            this.goalsListView.Font = new Font("Segoe UI", 11F);
            this.goalsListView.FullRowSelect = true;
            this.goalsListView.GridLines = true;
            this.goalsListView.Location = new Point(30, 90);
            this.goalsListView.Size = new Size(600, 400);
            this.goalsListView.View = View.Details;
            this.goalsListView.SelectedIndexChanged += OnGoalSelected;
            
            // Add columns
            this.goalsListView.Columns.Add("Goal", 200);
            this.goalsListView.Columns.Add("Target Amount", 120);
            this.goalsListView.Columns.Add("Current Amount", 120);
            this.goalsListView.Columns.Add("Progress", 100);
            this.goalsListView.Columns.Add("Target Date", 100);
            
            // Add Goal Button
            this.addGoalBtn.BackColor = Color.FromArgb(37, 99, 235);
            this.addGoalBtn.FlatStyle = FlatStyle.Flat;
            this.addGoalBtn.FlatAppearance.BorderSize = 0;
            this.addGoalBtn.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            this.addGoalBtn.ForeColor = Color.White;
            this.addGoalBtn.Location = new Point(650, 90);
            this.addGoalBtn.Size = new Size(200, 50);
            this.addGoalBtn.Text = "➕ Add New Goal";
            this.addGoalBtn.UseVisualStyleBackColor = false;
            this.addGoalBtn.Click += OnAddGoalClick;
            
            // Edit Goal Button
            this.editGoalBtn.BackColor = Color.FromArgb(245, 158, 11);
            this.editGoalBtn.FlatStyle = FlatStyle.Flat;
            this.editGoalBtn.FlatAppearance.BorderSize = 0;
            this.editGoalBtn.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            this.editGoalBtn.ForeColor = Color.White;
            this.editGoalBtn.Location = new Point(650, 160);
            this.editGoalBtn.Size = new Size(200, 50);
            this.editGoalBtn.Text = "✏️ Edit Goal";
            this.editGoalBtn.UseVisualStyleBackColor = false;
            this.editGoalBtn.Enabled = false;
            this.editGoalBtn.Click += OnEditGoalClick;
            
            // Delete Goal Button
            this.deleteGoalBtn.BackColor = Color.FromArgb(239, 68, 68);
            this.deleteGoalBtn.FlatStyle = FlatStyle.Flat;
            this.deleteGoalBtn.FlatAppearance.BorderSize = 0;
            this.deleteGoalBtn.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            this.deleteGoalBtn.ForeColor = Color.White;
            this.deleteGoalBtn.Location = new Point(650, 230);
            this.deleteGoalBtn.Size = new Size(200, 50);
            this.deleteGoalBtn.Text = "🗑️ Delete Goal";
            this.deleteGoalBtn.UseVisualStyleBackColor = false;
            this.deleteGoalBtn.Enabled = false;
            this.deleteGoalBtn.Click += OnDeleteGoalClick;
            
            // Progress Panel
            this.progressPanel.BackColor = Color.White;
            this.progressPanel.Location = new Point(650, 300);
            this.progressPanel.Size = new Size(200, 190);
            this.progressPanel.Paint += OnProgressPanelPaint;
            
            // Close Button
            this.closeBtn.BackColor = Color.FromArgb(107, 114, 128);
            this.closeBtn.FlatStyle = FlatStyle.Flat;
            this.closeBtn.FlatAppearance.BorderSize = 0;
            this.closeBtn.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            this.closeBtn.ForeColor = Color.White;
            this.closeBtn.Location = new Point(750, 520);
            this.closeBtn.Size = new Size(100, 40);
            this.closeBtn.Text = "Close";
            this.closeBtn.UseVisualStyleBackColor = false;
            this.closeBtn.Click += (s, e) => this.Close();
            
            // Add controls to form
            this.Controls.Add(this.titleLabel);
            this.Controls.Add(this.goalsListView);
            this.Controls.Add(this.addGoalBtn);
            this.Controls.Add(this.editGoalBtn);
            this.Controls.Add(this.deleteGoalBtn);
            this.Controls.Add(this.progressPanel);
            this.Controls.Add(this.closeBtn);
            
            this.ResumeLayout(false);
        }

        private void ApplyModernStyling()
        {
            // Apply rounded corners to buttons and panels
            var buttons = new[] { addGoalBtn, editGoalBtn, deleteGoalBtn, closeBtn };
            foreach (var button in buttons)
            {
                button.Region = CreateRoundedRegion(button.Size, 8);
            }
            
            progressPanel.Region = CreateRoundedRegion(progressPanel.Size, 12);
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

        private void LoadGoals()
        {
            // Load sample goals for demonstration
            goals = new List<FinancialGoal>
            {
                new FinancialGoal
                {
                    Id = 1,
                    Name = "Emergency Fund",
                    TargetAmount = 50000,
                    CurrentAmount = 25000,
                    TargetDate = DateTime.Now.AddMonths(12),
                    Description = "Build emergency fund for 6 months expenses"
                },
                new FinancialGoal
                {
                    Id = 2,
                    Name = "Vacation Fund",
                    TargetAmount = 30000,
                    CurrentAmount = 12000,
                    TargetDate = DateTime.Now.AddMonths(8),
                    Description = "Save for family vacation"
                },
                new FinancialGoal
                {
                    Id = 3,
                    Name = "New Car",
                    TargetAmount = 200000,
                    CurrentAmount = 80000,
                    TargetDate = DateTime.Now.AddMonths(18),
                    Description = "Save for new car purchase"
                }
            };
            
            RefreshGoalsList();
        }

        private void RefreshGoalsList()
        {
            goalsListView.Items.Clear();
            
            foreach (var goal in goals)
            {
                var progress = goal.TargetAmount > 0 ? (goal.CurrentAmount / goal.TargetAmount) * 100 : 0;
                var item = new ListViewItem(goal.Name);
                item.SubItems.Add($"Rs {goal.TargetAmount:N0}");
                item.SubItems.Add($"Rs {goal.CurrentAmount:N0}");
                item.SubItems.Add($"{progress:F1}%");
                item.SubItems.Add(goal.TargetDate.ToShortDateString());
                item.Tag = goal;
                
                // Color code based on progress
                if (progress >= 100)
                    item.ForeColor = Color.FromArgb(16, 185, 129); // Green
                else if (progress >= 50)
                    item.ForeColor = Color.FromArgb(245, 158, 11); // Orange
                else
                    item.ForeColor = Color.FromArgb(239, 68, 68); // Red
                
                goalsListView.Items.Add(item);
            }
        }

        private void OnGoalSelected(object sender, EventArgs e)
        {
            var hasSelection = goalsListView.SelectedItems.Count > 0;
            editGoalBtn.Enabled = hasSelection;
            deleteGoalBtn.Enabled = hasSelection;
            progressPanel.Invalidate(); // Refresh progress display
        }

        private void OnAddGoalClick(object sender, EventArgs e)
        {
            using (var dialog = new GoalEditDialog())
            {
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    var newGoal = new FinancialGoal
                    {
                        Id = goals.Count + 1,
                        Name = dialog.GoalName,
                        TargetAmount = dialog.TargetAmount,
                        CurrentAmount = dialog.CurrentAmount,
                        TargetDate = dialog.TargetDate,
                        Description = dialog.Description
                    };
                    
                    goals.Add(newGoal);
                    RefreshGoalsList();
                }
            }
        }

        private void OnEditGoalClick(object sender, EventArgs e)
        {
            if (goalsListView.SelectedItems.Count == 0) return;
            
            var selectedGoal = (FinancialGoal)goalsListView.SelectedItems[0].Tag;
            using (var dialog = new GoalEditDialog(selectedGoal))
            {
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    selectedGoal.Name = dialog.GoalName;
                    selectedGoal.TargetAmount = dialog.TargetAmount;
                    selectedGoal.CurrentAmount = dialog.CurrentAmount;
                    selectedGoal.TargetDate = dialog.TargetDate;
                    selectedGoal.Description = dialog.Description;
                    
                    RefreshGoalsList();
                }
            }
        }

        private void OnDeleteGoalClick(object sender, EventArgs e)
        {
            if (goalsListView.SelectedItems.Count == 0) return;
            
            var selectedGoal = (FinancialGoal)goalsListView.SelectedItems[0].Tag;
            var result = MessageBox.Show(
                $"Are you sure you want to delete the goal '{selectedGoal.Name}'?",
                "Confirm Delete",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);
            
            if (result == DialogResult.Yes)
            {
                goals.Remove(selectedGoal);
                RefreshGoalsList();
            }
        }

        private void OnProgressPanelPaint(object sender, PaintEventArgs e)
        {
            if (goalsListView.SelectedItems.Count == 0) return;
            
            var selectedGoal = (FinancialGoal)goalsListView.SelectedItems[0].Tag;
            var progress = selectedGoal.TargetAmount > 0 ? (float)(selectedGoal.CurrentAmount / selectedGoal.TargetAmount) : 0;
            
            var g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            
            var rect = progressPanel.ClientRectangle;
            var centerX = rect.Width / 2;
            var centerY = rect.Height / 2;
            var radius = Math.Min(centerX, centerY) - 20;
            
            // Draw progress circle
            var progressRect = new Rectangle(centerX - radius, centerY - radius, radius * 2, radius * 2);
            
            // Background circle
            using (var pen = new Pen(Color.FromArgb(229, 231, 235), 8))
            {
                g.DrawEllipse(pen, progressRect);
            }
            
            // Progress arc
            var progressColor = progress >= 1.0f ? Color.FromArgb(16, 185, 129) :
                               progress >= 0.5f ? Color.FromArgb(245, 158, 11) :
                               Color.FromArgb(239, 68, 68);
            
            using (var pen = new Pen(progressColor, 8))
            {
                pen.StartCap = LineCap.Round;
                pen.EndCap = LineCap.Round;
                var sweepAngle = progress * 360;
                g.DrawArc(pen, progressRect, -90, sweepAngle);
            }
            
            // Progress text
            var progressText = $"{progress * 100:F1}%";
            using (var font = new Font("Segoe UI", 16F, FontStyle.Bold))
            using (var brush = new SolidBrush(Color.FromArgb(31, 41, 55)))
            {
                var textSize = g.MeasureString(progressText, font);
                g.DrawString(progressText, font, brush, 
                    centerX - textSize.Width / 2, centerY - textSize.Height / 2);
            }
            
            // Goal name
            using (var font = new Font("Segoe UI", 10F))
            using (var brush = new SolidBrush(Color.FromArgb(107, 114, 128)))
            {
                var textSize = g.MeasureString(selectedGoal.Name, font);
                g.DrawString(selectedGoal.Name, font, brush,
                    centerX - textSize.Width / 2, centerY + 30);
            }
        }

        #region Designer Fields
        private Label titleLabel;
        private ListView goalsListView;
        private Button addGoalBtn;
        private Button editGoalBtn;
        private Button deleteGoalBtn;
        private Panel progressPanel;
        private Button closeBtn;
        #endregion
    }

    // Financial Goal Model
    public class FinancialGoal
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public decimal TargetAmount { get; set; }
        public decimal CurrentAmount { get; set; }
        public DateTime TargetDate { get; set; }
        public string Description { get; set; } = string.Empty;
    }
}
