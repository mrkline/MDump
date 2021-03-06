﻿using System;
using System.Threading;
using System.Windows.Forms;

//! Base namespace for the application
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
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.ThreadException += new ThreadExceptionEventHandler(GlobalExceptionHandler);
            Application.Run(new frmMain());
        }

        static void GlobalExceptionHandler(object sender, ThreadExceptionEventArgs e)
        {
            ErrorHandling.LogException(e.Exception);
            MessageBox.Show("A fatal unexpected error occurred.\n"
                + ErrorHandling.ErrorMessage, "Unexpected fatal error", MessageBoxButtons.OK,
                MessageBoxIcon.Error);
            Application.Exit();
        }
    }
}
