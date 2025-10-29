using System;
using System.Drawing;
using System.Windows.Forms;
using FinanceML.Core.Models;
using FinanceML.Core.Services;

namespace FinanceML
{
    public partial class BudgetForm : Form
    {
        private Budget? editingBudget;
        private DataService dataService;
        
        public BudgetForm() : this(null)
        {
        }

        public BudgetForm(Budget? budget)
        {
            dataService = DataService.Instance;
            editingBudget = budget;
            InitializeComponent();
            
            // Set form title and button text
            this.Text = editingBudget == null ? "Create Budget" : "Edit Budget";
            titleLabel.Text = editingBudget == null ? "Create New Budget" : "Edit Budget";
            saveButton.Text = editingBudget == null ? "Create Budget" : "Update Budget";
            
            // Initialize combo boxes
            categoryComboBox.Items.AddRange(new string[] {
                "Food & Dining", "Transportation", "Shopping", "Entertainment",
                "Bills & Utilities", "Healthcare", "Education", "Travel", "Other"
            });
            categoryComboBox.SelectedIndex = 0;
            
            periodComboBox.Items.AddRange(new string[] { "Monthly", "Weekly", "Yearly" });
            periodComboBox.SelectedIndex = 0;
            
            startDatePicker.Value = DateTime.Today;
            endDatePicker.Value = DateTime.Today.AddMonths(1);
            
            if (editingBudget != null)
            {
                LoadBudgetData();
            }
        }
        
        private void LoadBudgetData()
        {
            if (editingBudget != null)
            {
                nameTextBox.Text = editingBudget.Name;
                categoryComboBox.Text = editingBudget.Category;
                amountTextBox.Text = editingBudget.Amount.ToString("F2");
                periodComboBox.SelectedItem = editingBudget.Period.ToString();
                startDatePicker.Value = editingBudget.StartDate;
                endDatePicker.Value = editingBudget.EndDate;
            }
        }

        private void OnSaveClick(object sender, EventArgs e)
        {
            if (ValidateInput())
            {
                var budget = new Budget
                {
                    Name = nameTextBox.Text.Trim(),
                    Category = categoryComboBox.Text,
                    Amount = decimal.Parse(amountTextBox.Text),
                    Period = Enum.TryParse(periodComboBox.SelectedItem?.ToString(), out BudgetPeriod period) ? period : BudgetPeriod.Monthly,
                    StartDate = startDatePicker.Value.Date,
                    EndDate = endDatePicker.Value.Date
                };

                if (editingBudget == null)
                {
                    dataService.AddBudget(budget);
                    MessageBox.Show("Budget created successfully!", "Success", 
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    budget.Id = editingBudget.Id;
                    dataService.UpdateBudget(budget);
                    MessageBox.Show("Budget updated successfully!", "Success", 
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
        
        private bool ValidateInput()
        {
            if (string.IsNullOrWhiteSpace(nameTextBox.Text))
            {
                MessageBox.Show("Please enter a budget name.", "Validation Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                nameTextBox.Focus();
                return false;
            }
            
            if (string.IsNullOrWhiteSpace(amountTextBox.Text))
            {
                MessageBox.Show("Please enter a budget amount.", "Validation Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                amountTextBox.Focus();
                return false;
            }
            
            if (!decimal.TryParse(amountTextBox.Text, out decimal amount) || amount <= 0)
            {
                MessageBox.Show("Please enter a valid positive amount.", "Validation Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                amountTextBox.Focus();
                return false;
            }
            
            if (endDatePicker.Value <= startDatePicker.Value)
            {
                MessageBox.Show("End date must be after start date.", "Validation Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                endDatePicker.Focus();
                return false;
            }
            
            return true;
        }
    }
}
