using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace MDump
{
    /// <summary>
    /// Converts image paths to their corresponding bitmap tag strings to be saved and vice-versa.
    /// Also manages path validation. The idea behind this class is to isolate this functionality
    /// so that future changes are simple (as they weren't the first time it changed).
    /// </summary>
    static class PathManager
    {
        private static char[] invalidDirNameChars;
        private static char[] invalidMergeNameChars;

        /// <summary>
        /// Constructor
        /// </summary>
        static PathManager()
        {
            List<char> invDirNameCharList = new List<char>();
            invDirNameCharList.Add(Path.PathSeparator);
            invDirNameCharList.Add(Path.DirectorySeparatorChar);
            invDirNameCharList.Add(Path.AltDirectorySeparatorChar);
            invDirNameCharList.AddRange(Path.GetInvalidPathChars());
            invalidDirNameChars = invDirNameCharList.ToArray();

            List<char> invMergeNameList = new List<char>();
            invMergeNameList.Add('.');
            invMergeNameList.AddRange(Path.GetInvalidFileNameChars());
            invalidMergeNameChars = invMergeNameList.ToArray();
        }

        public static string InvalidDirNameMsg { get { return " is not a valid folder name."; } }
        public static string InvalidDirNameTitle { get { return "Invalid folder name"; } }
        public static string InvalidBmpTagMsg { get { return " is not a valid image name."; } }
        public static string InvalidBmpTagTitle { get { return "Invalid image name"; } }
        
        
        /// <summary>
        /// Gets a placeholder for discarded filenames
        /// </summary>
        public static string DiscardedFilename { get { return "\a"; } }


        /// <summary>
        /// Returns true if the provided directory name is valid
        /// </summary>
        /// <param name="name">Directory name to test</param>
        /// <returns>true if the provided directory name is valid</returns>
        public static bool IsValidDirName(string name)
        {
            return name.IndexOfAny(invalidDirNameChars) == -1;
        }

        /// <summary>
        /// Returns true if the provided bitmap tag/name is valid
        /// </summary>
        /// <param name="tag">bitmap tag/name to test</param>
        /// <returns>true if the provided bitmap tag/name is valid</returns>
        public static bool IsValidMergeName(string tag)
        {
            return tag.IndexOfAny(invalidMergeNameChars) == -1;
        }
    }
}
