using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using FinanceML.Core.Services;

namespace FinanceML
{
    public partial class LoginForm : Form
    {
        private UserService userService;
        private bool isPasswordVisible = false;

        public LoginForm()
        {
            userService = UserService.Instance;
            InitializeComponent();
            
            // Clear text fields - no placeholders
            usernameTextBox.Text = "";
            passwordTextBox.Text = "";
            
            // Apply modern styling
            ApplyModernStyling();
        }
        
        private void ApplyModernStyling()
        {
            // Set form properties for modern look
            this.SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | ControlStyles.DoubleBuffer, true);
            
            // Add rounded corners to login card
            loginCard.Region = CreateRoundedRegion(loginCard.Size, 15);
            loginCardShadow.Region = CreateRoundedRegion(loginCardShadow.Size, 15);
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
        

        
        private void OnLoginClick(object sender, EventArgs e)
        {
            // Clear any previous error
            errorLabel.Visible = false;
            
            var username = usernameTextBox.Text.Trim();
            var password = passwordTextBox.Text;
            
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                ShowError("Please enter both username and password.");
                return;
            }

            // Show loading state
            ShowLoading(true);
            
            // Simulate async login with timer for better UX
            var timer = new System.Windows.Forms.Timer();
            timer.Interval = 1500; // 1.5 seconds
            timer.Tick += (s, args) => {
                timer.Stop();
                timer.Dispose();
                
                ShowLoading(false);
                
                if (userService.LoginUser(username, password))
                {
                    var currentUser = userService.CurrentUser;
                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }
                else
                {
                    ShowError("Invalid username or password. Try 'demo' for both fields.");
                    passwordTextBox.SelectAll();
                    passwordTextBox.Focus();
                }
            };
            timer.Start();
        }
        
        private void ShowError(string message)
        {
            errorLabel.Text = message;
            errorLabel.Visible = true;
            
            // Auto-hide error after 5 seconds
            var timer = new System.Windows.Forms.Timer();
            timer.Interval = 5000;
            timer.Tick += (s, args) => {
                timer.Stop();
                timer.Dispose();
                errorLabel.Visible = false;
            };
            timer.Start();
        }
        
        private void ShowLoading(bool show)
        {
            loadingPanel.Visible = show;
            loginCard.Enabled = !show;
            
            if (show)
            {
                loadingPanel.BringToFront();
            }
        }

        private void OnRegisterClick(object sender, EventArgs e)
        {
            using (var registrationForm = new RegistrationForm())
            {
                var result = registrationForm.ShowDialog();
                if (result == DialogResult.OK)
                {
                    // Registration successful, clear the demo values
                    usernameTextBox.Text = "";
                    passwordTextBox.Text = "";
                    usernameTextBox.Focus();
                }
                else if (result == DialogResult.Retry)
                {
                    // User clicked "Back to Login" - just return to login form
                    usernameTextBox.Focus();
                }
            }
        }
        
        private void OnExitClick(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
        
        private void OnPasswordKeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                OnLoginClick(sender, e);
            }
        }
        
        private void OnShowPasswordClick(object sender, EventArgs e)
        {
            isPasswordVisible = !isPasswordVisible;
            passwordTextBox.UseSystemPasswordChar = !isPasswordVisible;
            showPasswordButton.Text = isPasswordVisible ? "🙈" : "👁";
        }
        
        // Custom paint events for modern styling
        private void OnLoginCardPaint(object sender, PaintEventArgs e)
        {
            var panel = sender as Panel;
            var rect = new Rectangle(0, 0, panel.Width, panel.Height);
            
            using (var path = GetRoundedRectangle(rect, 12))
            {
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                
                // Draw shadow
                using (var shadowBrush = new SolidBrush(Color.FromArgb(30, 0, 0, 0)))
                {
                    var shadowRect = new Rectangle(rect.X + 2, rect.Y + 2, rect.Width, rect.Height);
                    using (var shadowPath = GetRoundedRectangle(shadowRect, 12))
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
        

        
        private void OnButtonPaint(object sender, PaintEventArgs e)
        {
            var button = sender as Button;
            var rect = new Rectangle(0, 0, button.Width, button.Height);
            
            using (var path = GetRoundedRectangle(rect, 8))
            {
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                
                // Create gradient for button
                Color startColor, endColor;
                if (button == loginButton)
                {
                    startColor = Color.FromArgb(59, 130, 246);
                    endColor = Color.FromArgb(37, 99, 235);
                }
                else if (button == registerButton)
                {
                    startColor = Color.FromArgb(34, 197, 94);
                    endColor = Color.FromArgb(16, 185, 129);
                }
                else
                {
                    startColor = Color.FromArgb(156, 163, 175);
                    endColor = Color.FromArgb(107, 114, 128);
                }
                
                using (var brush = new LinearGradientBrush(rect, startColor, endColor, LinearGradientMode.Vertical))
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
                
                // Ensure text is always visible with high contrast
                e.Graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;
                TextRenderer.DrawText(e.Graphics, button.Text, button.Font, textRect, textColor, textFlags);
            }
        }
        
        private GraphicsPath GetRoundedRectangle(Rectangle rect, int radius)
        {
            var path = new GraphicsPath();
            int diameter = radius * 2;
            
            path.AddArc(rect.X, rect.Y, diameter, diameter, 180, 90);
            path.AddArc(rect.Right - diameter, rect.Y, diameter, diameter, 270, 90);
            path.AddArc(rect.Right - diameter, rect.Bottom - diameter, diameter, diameter, 0, 90);
            path.AddArc(rect.X, rect.Bottom - diameter, diameter, diameter, 90, 90);
            path.CloseFigure();
            
            return path;
        }
        
        private void OnBackgroundPaint(object sender, PaintEventArgs e)
        {
            var panel = sender as Panel;
            var rect = new Rectangle(0, 0, panel.Width, panel.Height);
            
            // Create gradient background
            using (var brush = new LinearGradientBrush(rect, 
                Color.FromArgb(59, 130, 246), 
                Color.FromArgb(147, 51, 234), 
                LinearGradientMode.ForwardDiagonal))
            {
                e.Graphics.FillRectangle(brush, rect);
            }
        }
        
        private void OnButtonMouseEnter(object sender, EventArgs e)
        {
            var button = sender as Button;
            if (button == loginButton)
            {
                button.BackColor = Color.FromArgb(29, 78, 216);
            }
            else if (button == registerButton)
            {
                button.BackColor = Color.FromArgb(5, 150, 105);
            }
            else if (button == exitButton)
            {
                button.BackColor = Color.FromArgb(107, 114, 128);
            }
            else if (button == showPasswordButton)
            {
                button.BackColor = Color.FromArgb(243, 244, 246);
            }
            
            button.Invalidate();
        }
        
        private void OnButtonMouseLeave(object sender, EventArgs e)
        {
            var button = sender as Button;
            if (button == loginButton)
            {
                button.BackColor = Color.FromArgb(37, 99, 235);
            }
            else if (button == registerButton)
            {
                button.BackColor = Color.FromArgb(16, 185, 129);
            }
            else if (button == exitButton)
            {
                button.BackColor = Color.FromArgb(156, 163, 175);
            }
            else if (button == showPasswordButton)
            {
                button.BackColor = Color.Transparent;
            }
            
            button.Invalidate();
        }
    }
}
