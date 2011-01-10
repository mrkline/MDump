using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.IO;

namespace MDump
{
    class ErrorHandling
    {
        public static string SupportEmail
        {
            get { return "Support.MDump@gmail.com"; }
        }

        public static string ErrorFilename
        {
            get { return "ErrorDump.report"; }
        }

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

        public static void LogException(Exception ex)
        {
            using(StreamWriter sw = new StreamWriter(ErrorFilename, true, Encoding.UTF8))
            {
                Version ver = Assembly.GetExecutingAssembly().GetName().Version;
                sw.WriteLine("Error on " + DateTime.Now.ToShortDateString());
                sw.WriteLine("MDump version " + ver.Major.ToString() + '.'
                    + ver.Minor.ToString() + " Build " + ver.Build.ToString());
                sw.WriteLine();
                sw.WriteLine("Exception message is as follows:");
                sw.WriteLine(ex.Message);
                sw.WriteLine();
                sw.WriteLine("Stack trace:");
                sw.WriteLine(ex.StackTrace);
                sw.WriteLine();
            }
        }
    }
}
