using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace MDump
{
    /// <summary>
    /// Class that holds Global Color information
    /// </summary>
    static class Colors
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
    }
}
