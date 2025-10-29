using System;
using System.Drawing;
using System.Windows.Forms;

namespace FinanceML.Core.Services
{
    public static class ThemeManager
    {
        public static void ApplyTheme(Form form, string theme = "Light")
        {
            switch (theme)
            {
                case "Dark":
                    ApplyDarkTheme(form);
                    break;
                case "Light":
                default:
                    ApplyLightTheme(form);
                    break;
            }
        }

        private static void ApplyLightTheme(Form form)
        {
            form.BackColor = Color.FromArgb(248, 249, 250);
            form.ForeColor = Color.FromArgb(52, 58, 64);
            
            ApplyThemeToControls(form.Controls, isLight: true);
        }

        private static void ApplyDarkTheme(Form form)
        {
            form.BackColor = Color.FromArgb(33, 37, 41);
            form.ForeColor = Color.FromArgb(248, 249, 250);
            
            ApplyThemeToControls(form.Controls, isLight: false);
        }

        private static void ApplyThemeToControls(Control.ControlCollection controls, bool isLight)
        {
            foreach (Control control in controls)
            {
                if (control is Panel)
                {
                    var panelControl = (Panel)control;
                    if (isLight)
                    {
                        panelControl.BackColor = Color.FromArgb(248, 249, 250);
                        panelControl.ForeColor = Color.FromArgb(52, 58, 64);
                    }
                    else
                    {
                        panelControl.BackColor = Color.FromArgb(52, 58, 64);
                        panelControl.ForeColor = Color.FromArgb(248, 249, 250);
                    }
                }
                else if (control is GroupBox)
                {
                    var groupBoxControl = (GroupBox)control;
                    if (isLight)
                    {
                        groupBoxControl.ForeColor = Color.FromArgb(52, 58, 64);
                    }
                    else
                    {
                        groupBoxControl.ForeColor = Color.FromArgb(248, 249, 250);
                    }
                }
                else if (control is Label)
                {
                    var labelControl = (Label)control;
                    if (isLight)
                    {
                        labelControl.ForeColor = Color.FromArgb(73, 80, 87);
                    }
                    else
                    {
                        labelControl.ForeColor = Color.FromArgb(248, 249, 250);
                    }
                }
                else if (control is TextBox)
                {
                    var textBoxControl = (TextBox)control;
                    if (isLight)
                    {
                        textBoxControl.BackColor = Color.White;
                        textBoxControl.ForeColor = Color.FromArgb(52, 58, 64);
                    }
                    else
                    {
                        textBoxControl.BackColor = Color.FromArgb(73, 80, 87);
                        textBoxControl.ForeColor = Color.FromArgb(248, 249, 250);
                    }
                }
                else if (control is ComboBox)
                {
                    var comboBoxControl = (ComboBox)control;
                    if (isLight)
                    {
                        comboBoxControl.BackColor = Color.White;
                        comboBoxControl.ForeColor = Color.FromArgb(52, 58, 64);
                    }
                    else
                    {
                        comboBoxControl.BackColor = Color.FromArgb(73, 80, 87);
                        comboBoxControl.ForeColor = Color.FromArgb(248, 249, 250);
                    }
                }

                // Recursively apply to child controls
                if (control.HasChildren)
                {
                    ApplyThemeToControls(control.Controls, isLight);
                }
            }
        }

        public static string GetCurrentCurrency()
        {
            return "LKR (Rs)"; // Default currency, will be updated by settings
        }

        public static string GetCurrencySymbol()
        {
            string currency = GetCurrentCurrency();
            return currency switch
            {
                "LKR (Rs)" => "Rs",
                "USD ($)" => "$",
                "EUR (€)" => "€",
                "GBP (£)" => "£",
                "JPY (¥)" => "¥",
                "CAD ($)" => "C$",
                "AUD ($)" => "A$",
                _ => "Rs"
            };
        }
    }
}
