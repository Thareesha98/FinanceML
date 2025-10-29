using System;
using System.Drawing;
using System.Windows.Forms;

namespace FinanceML
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            
            // Main application loop - handle login/logout cycle
            bool continueRunning = true;
            
            while (continueRunning)
            {
                // Show login dialog
                using (var loginForm = new LoginForm())
                {
                    var loginResult = loginForm.ShowDialog();
                    
                    if (loginResult == DialogResult.OK)
                    {
                        // User logged in successfully, show main form
                        using (var mainForm = new MainForm())
                        {
                            var mainResult = mainForm.ShowDialog();
                            
                            // Check if user logged out (DialogResult.Abort) or just closed the app
                            if (mainResult != DialogResult.Abort)
                            {
                                continueRunning = false; // User closed the app
                            }
                            // If DialogResult.Abort, continue the loop to show login again
                        }
                    }
                    else
                    {
                        // User cancelled login or closed login form
                        continueRunning = false;
                    }
                }
            }
        }
    }
}
