using System;
using System.IO;
using System.Reflection;
using System.Text;

namespace MDump
{
    /// <summary>
    /// Used for application-wide error handling
    /// </summary>
    class ErrorHandling
    {
        /// <summary>
        /// Gets the email to send support questions and error reports to.
        /// </summary>
        public static string SupportEmail
        {
            get { return "MDumpHelp@gmail.com"; }
        }

        /// <summary>
        /// Gets the file name of the error log.
        /// </summary>
        public static string ErrorFilename
        {
            get { return "ErrorDump.txt"; }
        }

        /// <summary>
        /// Gets an error message indicating that the error log can be sent to
        /// the support email address
        /// </summary>
        public static string ErrorMessage
        {
            get
            {
                return "An error report was generated and saved to " + ErrorHandling.ErrorFilename
                    + " (in the same folder as the program)."
                    + "You can help fix the problem by attaching it to an email sent to "
                    + ErrorHandling.SupportEmail;
            }
        }

        /// <summary>
        /// Log information about an exception in the error log
        /// </summary>
        /// <param name="ex">exception to log</param>
        public static void LogException(Exception ex)
        {
            try
            {
                using (StreamWriter sw = new StreamWriter(PathUtils.AppDir + ErrorFilename, true, Encoding.UTF8))
                {
                    Version ver = Assembly.GetExecutingAssembly().GetName().Version;
                    sw.WriteLine("*****Begin Error Report*****");
                    sw.WriteLine("Error on " + DateTime.Now.ToShortDateString());
                    sw.WriteLine("MDump version " + ver.Major.ToString() + "."
                        + ver.Minor.ToString() + " Build " + ver.Build.ToString());
                    sw.WriteLine();
                    sw.WriteLine("Exception is of type: " + ex.GetType() + ".");
                    sw.WriteLine("Exception message is:");
                    sw.WriteLine(ex.Message);
                    sw.WriteLine();
                    sw.WriteLine("Stack trace:");
                    sw.WriteLine(ex.StackTrace);
                    sw.WriteLine("*****End Error Report*****");
                    sw.WriteLine();
                }
            }
            catch
            {
                //This would be a sad day indeed.
                System.Windows.Forms.MessageBox.Show("An error occurred, and another error occurred while"
                    + " trying to save info about the error.  This program will now go cry in a corner.",
                    "Error recording error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
            }
        }
    }
}
