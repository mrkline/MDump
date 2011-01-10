using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace MDump
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            try
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new frmMain());
            }
            catch (Exception ex)
            {
                ErrorHandling.LogException(ex);
                MessageBox.Show("A fatal unexpected error occurred.\n"
                    + ErrorHandling.ErrorMessage, "Unexpected fatal error", MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }
    }
}
