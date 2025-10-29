using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using FinanceML.Core.Services;

namespace FinanceML
{
    public partial class RegistrationForm : Form
    {
        private UserService userService;

        public RegistrationForm()
        {
            userService = UserService.Instance;
            InitializeComponent();
            ApplyModernStyling();
        }
        
        private void ApplyModernStyling()
        {
            // Apply modern gradient background
            backgroundPanel.Paint += OnBackgroundPaint;
            
            // Apply modern card styling
            registrationCard.Paint += OnCardPaint;
            
            // Apply modern button styling
            registerButton.Paint += OnButtonPaint;
            cancelButton.Paint += OnButtonPaint;
            backToLoginButton.Paint += OnButtonPaint;
            
            // Apply modern textbox styling
            firstNameTextBox.Paint += OnTextBoxPaint;
            lastNameTextBox.Paint += OnTextBoxPaint;
            usernameTextBox.Paint += OnTextBoxPaint;
            emailTextBox.Paint += OnTextBoxPaint;
            passwordTextBox.Paint += OnTextBoxPaint;
            confirmPasswordTextBox.Paint += OnTextBoxPaint;
        }

        private void OnRegisterClick(object sender, EventArgs e)
        {
            if (ValidateInput())
            {
                bool success = userService.RegisterUser(
                    usernameTextBox.Text.Trim(),
                    emailTextBox.Text.Trim(),
                    passwordTextBox.Text,
                    firstNameTextBox.Text.Trim(),
                    lastNameTextBox.Text.Trim()
                );

                if (success)
                {
                    MessageBox.Show("Registration successful! You can now login with your credentials.",
                        "Registration Complete", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }
                else
                {
                    MessageBox.Show("Registration failed. Username or email may already exist.",
                        "Registration Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void OnCancelClick(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void OnBackToLoginClick(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Retry; // Use Retry to indicate back to login
            this.Close();
        }

        private bool ValidateInput()
        {
            // Validate first name
            if (string.IsNullOrWhiteSpace(firstNameTextBox.Text))
            {
                MessageBox.Show("Please enter your first name.", "Validation Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                firstNameTextBox.Focus();
                return false;
            }

            // Validate last name
            if (string.IsNullOrWhiteSpace(lastNameTextBox.Text))
            {
                MessageBox.Show("Please enter your last name.", "Validation Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                lastNameTextBox.Focus();
                return false;
            }

            // Validate username
            if (string.IsNullOrWhiteSpace(usernameTextBox.Text))
            {
                MessageBox.Show("Please enter a username.", "Validation Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                usernameTextBox.Focus();
                return false;
            }

            if (usernameTextBox.Text.Length < 3)
            {
                MessageBox.Show("Username must be at least 3 characters long.", "Validation Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                usernameTextBox.Focus();
                return false;
            }

            // Validate email
            if (string.IsNullOrWhiteSpace(emailTextBox.Text))
            {
                MessageBox.Show("Please enter your email address.", "Validation Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                emailTextBox.Focus();
                return false;
            }

            if (!IsValidEmail(emailTextBox.Text))
            {
                MessageBox.Show("Please enter a valid email address.", "Validation Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                emailTextBox.Focus();
                return false;
            }

            // Validate password
            if (string.IsNullOrWhiteSpace(passwordTextBox.Text))
            {
                MessageBox.Show("Please enter a password.", "Validation Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                passwordTextBox.Focus();
                return false;
            }

            if (passwordTextBox.Text.Length < 6)
            {
                MessageBox.Show("Password must be at least 6 characters long.", "Validation Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                passwordTextBox.Focus();
                return false;
            }

            // Validate confirm password
            if (passwordTextBox.Text != confirmPasswordTextBox.Text)
            {
                MessageBox.Show("Passwords do not match.", "Validation Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                confirmPasswordTextBox.Focus();
                return false;
            }

            return true;
        }

        private bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }
        
        private void OnBackgroundPaint(object sender, PaintEventArgs e)
        {
            var panel = sender as Panel;
            var rect = new Rectangle(0, 0, panel.Width, panel.Height);
            
            // Create gradient background
            using (var brush = new System.Drawing.Drawing2D.LinearGradientBrush(rect, 
                Color.FromArgb(59, 130, 246), 
                Color.FromArgb(147, 51, 234), 
                System.Drawing.Drawing2D.LinearGradientMode.ForwardDiagonal))
            {
                e.Graphics.FillRectangle(brush, rect);
            }
        }
        
        private void OnCardPaint(object sender, PaintEventArgs e)
        {
            var panel = sender as Panel;
            var rect = new Rectangle(0, 0, panel.Width, panel.Height);
            
            using (var path = GetRoundedRectangle(rect, 12))
            {
                e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                
                // Draw shadow
                for (int i = 0; i < 3; i++)
                {
                    var shadowAlpha = 8 - (i * 2);
                    var shadowOffset = i + 2;
                    using (var shadowBrush = new SolidBrush(Color.FromArgb(shadowAlpha, 0, 0, 0)))
                    {
                        var shadowRect = new Rectangle(rect.X + shadowOffset, rect.Y + shadowOffset, rect.Width, rect.Height);
                        using (var shadowPath = GetRoundedRectangle(shadowRect, 12))
                        {
                            e.Graphics.FillPath(shadowBrush, shadowPath);
                        }
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
        
        private void OnButtonPaint(object sender, PaintEventArgs e)
        {
            var button = sender as Button;
            var rect = new Rectangle(0, 0, button.Width, button.Height);
            
            using (var path = GetRoundedRectangle(rect, 8))
            {
                e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                
                // Create gradient for button
                Color startColor, endColor;
                if (button == registerButton)
                {
                    startColor = Color.FromArgb(34, 197, 94);
                    endColor = Color.FromArgb(16, 185, 129);
                }
                else if (button == backToLoginButton)
                {
                    startColor = Color.FromArgb(59, 130, 246);
                    endColor = Color.FromArgb(37, 99, 235);
                }
                else
                {
                    startColor = Color.FromArgb(156, 163, 175);
                    endColor = Color.FromArgb(107, 114, 128);
                }
                
                using (var brush = new System.Drawing.Drawing2D.LinearGradientBrush(rect, startColor, endColor, System.Drawing.Drawing2D.LinearGradientMode.Vertical))
                {
                    e.Graphics.FillPath(brush, path);
                }
                
                // Draw border
                using (var pen = new Pen(Color.FromArgb(30, 0, 0, 0), 1))
                {
                    e.Graphics.DrawPath(pen, path);
                }
                
                // Draw text with proper visibility
                var textRect = new Rectangle(rect.X + 5, rect.Y, rect.Width - 10, rect.Height);
                var textColor = Color.White;
                var textFlags = TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.WordBreak;
                
                e.Graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;
                TextRenderer.DrawText(e.Graphics, button.Text, button.Font, textRect, textColor, textFlags);
            }
        }
        
        private void OnTextBoxPaint(object sender, PaintEventArgs e)
        {
            var textBox = sender as TextBox;
            var rect = new Rectangle(0, 0, textBox.Width, textBox.Height);
            
            using (var path = GetRoundedRectangle(rect, 6))
            {
                e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                
                // Draw background
                using (var brush = new SolidBrush(Color.White))
                {
                    e.Graphics.FillPath(brush, path);
                }
                
                // Draw border
                using (var pen = new Pen(Color.FromArgb(209, 213, 219), 1))
                {
                    e.Graphics.DrawPath(pen, path);
                }
            }
        }
        
        private System.Drawing.Drawing2D.GraphicsPath GetRoundedRectangle(Rectangle rect, int radius)
        {
            var path = new System.Drawing.Drawing2D.GraphicsPath();
            int diameter = radius * 2;
            
            path.AddArc(rect.X, rect.Y, diameter, diameter, 180, 90);
            path.AddArc(rect.Right - diameter, rect.Y, diameter, diameter, 270, 90);
            path.AddArc(rect.Right - diameter, rect.Bottom - diameter, diameter, diameter, 0, 90);
            path.AddArc(rect.X, rect.Bottom - diameter, diameter, diameter, 90, 90);
            path.CloseFigure();
            
            return path;
        }
    }
}
