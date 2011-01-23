using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.IO;

namespace MDump
{
    class Globals
    {
        public static Encoding MDDataEncoding
        {
            get { return Encoding.UTF8; }
        }
        public static Color ValidColor { get { return Color.Green; } }
        public static Color WarningColor { get { return Color.Goldenrod; } }
        public static Color InvalidColor { get { return Color.Red; } }
        public static Color ValidBGColor { get { return Color.LightGreen; } }
        public static Color WarningBGColor { get { return Color.Yellow; } }
        public static Color InvalidBGColor { get { return Color.PaleVioletRed; } }
        public static bool IsValidDirName(string name)
        {
            return name.IndexOfAny(invalidDirNameChars) == -1;
        }

        private static char[] invalidDirNameChars;

        static Globals()
        {
            List<char> invDirNameCharList = new List<char>();
            invDirNameCharList.Add(Path.PathSeparator);
            invDirNameCharList.Add(Path.DirectorySeparatorChar);
            invDirNameCharList.Add(Path.AltDirectorySeparatorChar);
            invDirNameCharList.AddRange(Path.GetInvalidPathChars());
            invalidDirNameChars = invDirNameCharList.ToArray();
        }
    }
}
