using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace FinanceML.Core.Services
{
    /// <summary>
    /// High-modularity theme engine for WinForms.
    /// Supports DI, dynamic theme switching, custom theme registration,
    /// and granular helper methods for high maintainability.
    /// </summary>
    public class ThemeManager : IThemeManager
    {
        private readonly Dictionary<string, ThemeDefinition> _themes = new();

        // Current theme name
        public string CurrentTheme { get; private set; } = "Light";

        public ThemeManager()
        {
            RegisterBuiltInThemes();
        }

        // ==========================================================
        // PUBLIC API
        // ==========================================================

        public void RegisterTheme(string name, ThemeDefinition theme)
        {
            ValidateThemeName(name);
            ValidateThemeDefinition(theme);

            if (!_themes.ContainsKey(name))
                _themes.Add(name, theme);
        }

        public void ApplyTheme(Form form, string? theme = null)
        {
            if (form == null)
                return;

            ResolveTheme(theme);

            if (!TryGetTheme(out var definition))
                return;

            ApplyFormProperties(form, definition);
            ApplyControlsRecursively(form.Controls, definition);
        }

        // ==========================================================
        // THEME RESOLUTION HELPERS
        // ==========================================================

        private void ResolveTheme(string? theme)
        {
            if (!string.IsNullOrWhiteSpace(theme))
                CurrentTheme = theme;
        }

        private bool TryGetTheme(out ThemeDefinition theme)
        {
            return _themes.TryGetValue(CurrentTheme, out theme!);
        }

        // ==========================================================
        // FORM + CONTROL APPLICATION
        // ==========================================================

        private void ApplyFormProperties(Form form, ThemeDefinition theme)
        {
            form.BackColor = theme.Background;
            form.ForeColor = theme.Foreground;
        }

        private void ApplyControlsRecursively(Control.ControlCollection controls, ThemeDefinition theme)
        {
            foreach (Control control in controls)
            {
                ApplyControlTheme(control, theme);

                if (control.HasChildren)
                    ApplyControlsRecursively(control.Controls, theme);
            }
        }

        private void ApplyControlTheme(Control control, ThemeDefinition theme)
        {
            switch (control)
            {
                case Panel p:
                    ApplyPanelTheme(p, theme);
                    break;

                case GroupBox g:
                    ApplyGroupBoxTheme(g, theme);
                    break;

                case Label l:
                    ApplyLabelTheme(l, theme);
                    break;

                case TextBox tb:
                    ApplyTextBoxTheme(tb, theme);
                    break;

                case ComboBox cb:
                    ApplyComboBoxTheme(cb, theme);
                    break;

                case Button btn:
                    ApplyButtonTheme(btn, theme);
                    break;

                case DataGridView dgv:
                    ApplyGridTheme(dgv, theme);
                    break;
            }
        }

        // ==========================================================
        // CONTROL-SPECIFIC HELPERS
        // ==========================================================

        private void ApplyPanelTheme(Panel panel, ThemeDefinition theme)
        {
            panel.BackColor = theme.PanelBackground;
            panel.ForeColor = theme.PanelForeground;
        }

        private void ApplyGroupBoxTheme(GroupBox g, ThemeDefinition theme)
        {
            g.ForeColor = theme.Foreground;
        }

        private void ApplyLabelTheme(Label l, ThemeDefinition theme)
        {
            l.ForeColor = theme.Foreground;
        }

        private void ApplyTextBoxTheme(TextBox tb, ThemeDefinition theme)
        {
            tb.BackColor = theme.InputBackground;
            tb.ForeColor = theme.InputForeground;
        }

        private void ApplyComboBoxTheme(ComboBox cb, ThemeDefinition theme)
        {
            cb.BackColor = theme.InputBackground;
            cb.ForeColor = theme.InputForeground;
        }

        private void ApplyButtonTheme(Button btn, ThemeDefinition theme)
        {
            btn.BackColor = theme.PanelBackground;
            btn.ForeColor = theme.PanelForeground;
            btn.FlatStyle = FlatStyle.Flat;
            btn.FlatAppearance.BorderColor = theme.BorderColor;
        }

        private void ApplyGridTheme(DataGridView grid, ThemeDefinition theme)
        {
            grid.BackgroundColor = theme.Background;
            grid.ForeColor = theme.Foreground;

            grid.EnableHeadersVisualStyles = false;
            grid.ColumnHeadersDefaultCellStyle.BackColor = theme.PanelBackground;
            grid.ColumnHeadersDefaultCellStyle.ForeColor = theme.PanelForeground;
        }

        // ==========================================================
        // VALIDATION HELPERS
        // ==========================================================

        private void ValidateThemeName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Theme name cannot be empty.", nameof(name));
        }

        private void ValidateThemeDefinition(ThemeDefinition? theme)
        {
            if (theme == null)
                throw new ArgumentNullException(nameof(theme), "Theme definition cannot be null.");
        }

        // ==========================================================
        // BUILT-IN THEMES
        // ==========================================================

        private void RegisterBuiltInThemes()
        {
            AddLightTheme();
            AddDarkTheme();
            AddMidnightTheme();
        }

        private void AddLightTheme()
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
        }

        private void AddDarkTheme()
        {
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
        }

        private void AddMidnightTheme()
        {
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

