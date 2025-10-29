using System;
using System.Drawing;
using System.Windows.Forms;
using FinanceML.Core.Services;
using FinanceML.Core.Models;

namespace FinanceML
{
    public partial class SettingsForm : Form
    {
        private ExportImportService exportImportService;
        private DataService dataService;
        private SettingsService settingsService;

        public SettingsForm()
        {
            InitializeComponent();
            exportImportService = ExportImportService.Instance;
            dataService = DataService.Instance;
            settingsService = SettingsService.Instance;
            
            InitializeSettings();
        }

        private void InitializeSettings()
        {
            // Initialize combo boxes
            themeComboBox.Items.AddRange(new string[] { "Light", "Dark", "Auto" });
            currencyComboBox.Items.AddRange(new string[] { "LKR (Rs)", "USD ($)", "EUR (€)", "GBP (£)", "JPY (¥)", "CAD ($)", "AUD ($)" });
            
            // Load current settings
            LoadCurrentSettings();
        }

        private void LoadCurrentSettings()
        {
            var currentSettings = settingsService.CurrentSettings;
            if (currentSettings != null)
            {
                // Set theme
                var themeIndex = themeComboBox.Items.IndexOf(currentSettings.Theme);
                themeComboBox.SelectedIndex = themeIndex >= 0 ? themeIndex : 0;

                // Set currency
                var currencyIndex = currencyComboBox.Items.IndexOf(currentSettings.Currency);
                currencyComboBox.SelectedIndex = currencyIndex >= 0 ? currencyIndex : 0;
            }
            else
            {
                // Default values
                themeComboBox.SelectedIndex = 0; // Light
                currencyComboBox.SelectedIndex = 0; // LKR
            }
        }
        
        private void OnExportClick(object sender, EventArgs e)
        {
            using (var saveDialog = new SaveFileDialog())
            {
                saveDialog.Filter = "PDF Professional Report (*.pdf)|*.pdf|PDF Simple Report (*.pdf)|*.pdf|CSV files (*.csv)|*.csv|JSON files (*.json)|*.json|Text reports (*.txt)|*.txt";
                saveDialog.Title = "Export Financial Data";
                saveDialog.FileName = $"FinanceData_{DateTime.Now:yyyyMMdd}";

                if (saveDialog.ShowDialog() == DialogResult.OK)
                {
                    bool success = false;
                    var extension = System.IO.Path.GetExtension(saveDialog.FileName).ToLower();
                    var filterIndex = saveDialog.FilterIndex;

                    switch (extension)
                    {
                        case ".pdf":
                            if (filterIndex == 1) // Professional PDF
                            {
                                success = exportImportService.ExportToPDF(saveDialog.FileName, 
                                    DateTime.Now.AddYears(-1), DateTime.Now);
                            }
                            else if (filterIndex == 2) // Simple PDF
                            {
                                success = exportImportService.ExportToSimplePDF(saveDialog.FileName, 
                                    DateTime.Now.AddYears(-1), DateTime.Now);
                            }
                            break;
                        case ".csv":
                            success = exportImportService.ExportToCSV(saveDialog.FileName);
                            break;
                        case ".json":
                            success = exportImportService.ExportToJSON(saveDialog.FileName);
                            break;
                        case ".txt":
                            success = exportImportService.ExportReport(saveDialog.FileName, 
                                DateTime.Now.AddYears(-1), DateTime.Now);
                            break;
                    }

                    if (success)
                    {
                        MessageBox.Show("Data exported successfully!", "Export Complete", 
                                       MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }
        }
        
        private void OnImportClick(object sender, EventArgs e)
        {
            using (var openDialog = new OpenFileDialog())
            {
                openDialog.Filter = "CSV files (*.csv)|*.csv|JSON files (*.json)|*.json";
                openDialog.Title = "Import Financial Data";

                if (openDialog.ShowDialog() == DialogResult.OK)
                {
                    var result = MessageBox.Show(
                        "Importing data will add new transactions to your existing data. Continue?",
                        "Confirm Import", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                    if (result == DialogResult.Yes)
                    {
                        bool success = false;
                        var extension = System.IO.Path.GetExtension(openDialog.FileName).ToLower();

                        switch (extension)
                        {
                            case ".csv":
                                success = exportImportService.ImportFromCSV(openDialog.FileName);
                                break;
                            case ".json":
                                success = exportImportService.ImportFromJSON(openDialog.FileName);
                                break;
                        }

                        if (success)
                        {
                            MessageBox.Show("Data imported successfully! Please refresh the main dashboard.", 
                                           "Import Complete", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                    }
                }
            }
        }
        
        private void OnClearDataClick(object sender, EventArgs e)
        {
            var result = MessageBox.Show(
                "⚠️ WARNING: This will permanently delete ALL your financial data!\n\n" +
                "This includes:\n" +
                "• All transactions\n" +
                "• All budgets\n" +
                "• All settings\n\n" +
                "This action CANNOT be undone!\n\n" +
                "Are you absolutely sure you want to continue?", 
                "⚠️ DANGER: Clear All Data", 
                MessageBoxButtons.YesNo, 
                MessageBoxIcon.Warning);
            
            if (result == DialogResult.Yes)
            {
                // Double confirmation
                var confirmResult = MessageBox.Show(
                    "Last chance! Are you really sure you want to delete everything?",
                    "Final Confirmation",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Stop);

                if (confirmResult == DialogResult.Yes)
                {
                    try
                    {
                        dataService.ClearAllData();
                        MessageBox.Show("All data has been permanently deleted.", "Data Cleared", 
                                       MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error clearing data: {ex.Message}", "Error", 
                                       MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }
        
        private void OnSaveClick(object sender, EventArgs e)
        {
            try
            {
                var currentSettings = settingsService.CurrentSettings;
                if (currentSettings != null)
                {
                    // Create updated settings
                    var updatedSettings = new AppSettings
                    {
                        UserId = currentSettings.UserId,
                        Theme = themeComboBox.SelectedItem?.ToString() ?? "Light",
                        Currency = currencyComboBox.SelectedItem?.ToString() ?? "LKR (Rs)",

                        AutoBackup = currentSettings.AutoBackup
                    };

                    // Save settings
                    bool success = settingsService.SaveSettings(updatedSettings);

                    if (success)
                    {
                        MessageBox.Show("Settings saved successfully! Changes will take effect immediately.", "Settings Saved", 
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                        this.DialogResult = DialogResult.OK;
                        this.Close();
                    }
                    else
                    {
                        MessageBox.Show("Failed to save settings. Please try again.", "Error", 
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                else
                {
                    MessageBox.Show("No user settings found. Please log in again.", "Error", 
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving settings: {ex.Message}", "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        
        private void OnCancelClick(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }
}
