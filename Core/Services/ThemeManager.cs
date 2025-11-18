using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace FinanceML.Core.Services
{
    /// <summary>
    /// A fully extensible and DI-friendly WinForms theme engine. 
    /// Supports dynamic theme switching and custom theme definitions.
    /// </summary>
    public class ThemeManager : IThemeManager
    {
        private readonly Dictionary<string, ThemeDefinition> _themes = new();
        public string CurrentTheme { get; private set; } = "Light";

        public ThemeManager()
        {
            RegisterDefaultThemes();
        }

        public void RegisterTheme(string name, ThemeDefinition theme)
        {
            if (!_themes.ContainsKey(name))
                _themes.Add(name, theme);
        }

        public void ApplyTheme(Form form, string? theme = null)
        {
            if (theme != null)
                CurrentTheme = theme;

            if (!_themes.TryGetValue(CurrentTheme, out var definition))
                return;

            ApplyFormTheme(form, definition);
            ApplyControlTheme(form.Controls, definition);
        }

        private void ApplyFormTheme(Form form, ThemeDefinition theme)
        {
            form.BackColor = theme.Background;
            form.ForeColor = theme.Foreground;
        }

        private void ApplyControlTheme(Control.ControlCollection controls, ThemeDefinition theme)
        {
            foreach (Control control in controls)
            {
                switch (control)
                {
                    case Panel p:
                        p.BackColor = theme.PanelBackground;
                        p.ForeColor = theme.PanelForeground;
                        break;

                    case GroupBox g:
                        g.ForeColor = theme.Foreground;
                        break;

                    case Label l:
                        l.ForeColor = theme.Foreground;
                        break;

                    case TextBox tb:
                        tb.BackColor = theme.InputBackground;
                        tb.ForeColor = theme.InputForeground;
                        break;

                    case ComboBox cb:
                        cb.BackColor = theme.InputBackground;
                        cb.ForeColor = theme.InputForeground;
                        break;

                    case Button btn:
                        btn.BackColor = theme.PanelBackground;
                        btn.ForeColor = theme.PanelForeground;
                        btn.FlatStyle = FlatStyle.Flat;
                        btn.FlatAppearance.BorderColor = theme.BorderColor;
                        break;

                    case DataGridView dgv:
                        dgv.BackgroundColor = theme.Background;
                        dgv.ForeColor = theme.Foreground;
                        dgv.EnableHeadersVisualStyles = false;
                        dgv.ColumnHeadersDefaultCellStyle.BackColor = theme.PanelBackground;
                        dgv.ColumnHeadersDefaultCellStyle.ForeColor = theme.PanelForeground;
                        break;
                }

                if (control.HasChildren)
                    ApplyControlTheme(control.Controls, theme);
            }
        }

        // ============================================
        // DEFAULT THEMES
        // ============================================
        private void RegisterDefaultThemes()
        {
            RegisterTheme("Light", new ThemeDefinition
            {
                Background = Color.FromArgb(248, 249, 250),
                Foreground = Color.FromArgb(52, 58, 64),
                PanelBackground = Color.FromArgb(248, 249, 250),
                PanelForeground = Color.FromArgb(52, 58, 64),
                InputBackground = Color.White,
                InputForeground = Color.FromArgb(52, 58, 64),
                BorderColor = Color.FromArgb(200, 200, 200)
            });

            RegisterTheme("Dark", new ThemeDefinition
            {
                Background = Color.FromArgb(33, 37, 41),
                Foreground = Color.FromArgb(248, 249, 250),
                PanelBackground = Color.FromArgb(52, 58, 64),
                PanelForeground = Color.FromArgb(248, 249, 250),
                InputBackground = Color.FromArgb(73, 80, 87),
                InputForeground = Color.FromArgb(248, 249, 250),
                BorderColor = Color.FromArgb(90, 90, 90)
            });

            RegisterTheme("Midnight", new ThemeDefinition
            {
                Background = Color.FromArgb(15, 15, 30),
                Foreground = Color.FromArgb(220, 220, 255),
                PanelBackground = Color.FromArgb(35, 35, 60),
                PanelForeground = Color.White,
                InputBackground = Color.FromArgb(40, 40, 70),
                InputForeground = Color.White,
                BorderColor = Color.FromArgb(70, 70, 100)
            });
        }
    }
}

