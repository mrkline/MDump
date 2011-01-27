using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace MDump
{
    /// <summary>
    /// Converts image paths to their corresponding bitmap tag strings to be saved and vice-versa.
    /// Also manages path validation
    /// </summary>
    static class PathManager
    {
        private static char[] invalidDirNameChars;
        private static char[] invalidTagChars;

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

            List<char> invTagCharList = new List<char>();
            invTagCharList.Add('.');
            invTagCharList.AddRange(Path.GetInvalidFileNameChars());
            invalidTagChars = invTagCharList.ToArray();
        }

        public static string InvalidDirNameMsg { get { return " is not a valid folder name."; } }
        public static string InvalidDirNameTitle { get { return "Invalid folder name"; } }
        public static string InvalidBmpTagMsg { get { return " is not a valid image name."; } }
        public static string InvalidBmpTagTitle { get { return "Invalid image name"; } }


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
        public static bool IsValidBitmapTag(string tag)
        {
            return tag.IndexOfAny(invalidTagChars) == -1;
        }

        /// <summary>
        /// Converts a image path to a tag to attach to the generated bitmap
        /// </summary>
        /// <param name="path">file path of image</param>
        /// <returns>The string tag to attach to the generated bitmap</returns>
        public static string PathToBitmapTag(string path)
        {
            return Path.GetFileNameWithoutExtension(path);
        }

        /// <summary>
        /// Add ImageDirectory paths to image tags prior to merge.
        /// </summary>
        /// <param name="oldTag">Current tag value</param>
        /// <param name="idPath">ImageDirectory path to add</param>
        /// <returns>New tag with ImageDirectory Path information</returns>
        public static string PathifyBitmapTag(string oldTag, string idPath)
        {
            return idPath + Path.DirectorySeparatorChar + oldTag;
        }

        /// <summary>
        /// Removes ImageDirectory path data from tag after merge is complete
        /// </summary>
        /// <param name="oldTag">Current tag value</param>
        /// <returns>New tag without ImageDirectory path information</returns>
        public static string DepathifyBitmapTag(string oldTag)
        {
            return oldTag.Substring(oldTag.LastIndexOf(Path.DirectorySeparatorChar) + 1);
        }
    }
}
