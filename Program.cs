using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace FluxNoteV2
{
    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            FirebaseHelper.InitializeFirebase();
            using (LoginForm loginForm = new LoginForm())
            {
                if (loginForm.ShowDialog() == DialogResult.OK) // Show the login form as a dialog
                {
                    // If the login is successful, run the main form
                    Application.Run(new Form1());
                }
                else
                {
                    // If the login failed or the user closed the login form, exit the application
                    Application.Exit();
                }
            }

        }
    }
}
