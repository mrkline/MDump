using System.Drawing;

namespace MDump
{
    /// <summary>
    /// Class that holds global color information
    /// </summary>
    static class Colors
    {
        /// <summary>
        /// Color indicative of a valid entry
        /// </summary>
        public static Color ValidColor { get { return Color.Green; } }
        /// <summary>
        /// Color indicative of some sort of warning
        /// </summary>
        public static Color WarningColor { get { return Color.Goldenrod; } }
        /// <summary>
        /// Color indicative of an invalid entry
        /// </summary>
        public static Color InvalidColor { get { return Color.Red; } }
        /// <summary>
        /// Background color for a valid entry
        /// </summary>
        public static Color ValidBGColor { get { return Color.LightGreen; } }
        /// <summary>
        /// Background color indicative of some sort of warning
        /// </summary>
        public static Color WarningBGColor { get { return Color.Yellow; } }
        /// <summary>
        /// Background color for an invalid entry
        /// </summary>
        public static Color InvalidBGColor { get { return Color.PaleVioletRed; } }
    }
}
