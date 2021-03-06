﻿using System.Collections.Generic;
using System.IO;

namespace MDump
{
    /// <summary>
    /// Manages path validation. The idea behind this class is to isolate this functionality
    /// so that future changes are simple (as they weren't the first time it changed).
    /// </summary>
    static class PathUtils
    {
        /// <summary>
        /// Array of invalid directory MDump character names
        /// </summary>
        private static char[] invalidDirNameChars;
        /// <summary>
        /// Array of invalid characters for the name of an image to be merged
        /// </summary>
        private static char[] invalidMergeNameChars;

        private static readonly string[] supportedImageExtensions = { "bmp", "gif", "exif", "jpg",
                                                              "jpeg", "png", "tif", "tiff" };

        /// <summary>
        /// Constructor
        /// </summary>
        static PathUtils()
        {
            List<char> invDirNameCharList = new List<char>();
            invDirNameCharList.Add(Path.PathSeparator);
            invDirNameCharList.Add(Path.DirectorySeparatorChar);
            invDirNameCharList.Add(Path.AltDirectorySeparatorChar);
            invDirNameCharList.AddRange(Path.GetInvalidPathChars());
            invalidDirNameChars = invDirNameCharList.ToArray();

            invalidMergeNameChars = Path.GetInvalidFileNameChars();
        }

        public static string InvalidDirNameMsg { get { return " is not a valid folder name."; } }
        public static string InvalidDirNameTitle { get { return "Invalid folder name"; } }
        public static string InvalidBmpNameMsg
            { get { return "Extensions will automatically be added to the images when they are split again"; } }
        public static string InvalidBmpTagTitle { get { return "Invalid image name"; } }

        /// <summary>
        /// Gets the directory of the executable of this running application
        /// </summary>
        public static string AppDir
        {
            get { return Path.GetDirectoryName(System.Windows.Forms.Application.ExecutablePath) + @"\"; }
        }        
        
        /// <summary>
        /// Gets a placeholder for discarded filenames
        /// </summary>
        public static string DiscardedFilename { get { return "\a"; } }

        /// <summary>
        /// Returns true if an image can be generated from the file
        /// </summary>
        /// <param name="filepath">File to test</param>
        /// <returns>true if the provided file has one of the supported image extensions</returns>
        public static bool IsSupportedImage(string filepath)
        {
            try
            {
                System.Drawing.Bitmap bmp = new System.Drawing.Bitmap(filepath);
                bmp.Dispose();
            }
            catch
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// Gets a new name for a duplicate filename
        /// </summary>
        /// <param name="filename">Original filename</param>
        /// <returns>The new, non-conflicting filename</returns>
        public static string GetRename(string filename)
        {
            string nameSansExt = Path.GetFileNameWithoutExtension(filename);
            string ext = Path.GetExtension(filename);
            string newName;
            int copyNum = 2;

            do
            {
                newName = nameSansExt + " (" + copyNum.ToString() + ")" + ext;
                ++copyNum;
            } while (File.Exists(newName));

            return newName;
        }


        /// <summary>
        /// Returns true if the provided directory name is valid
        /// </summary>
        /// <param name="name">Directory name to test</param>
        /// <returns>true if the provided directory name is valid</returns>
        public static bool IsValidDirName(string name)
        {
            return name.Length > 0 && name.IndexOfAny(invalidDirNameChars) == -1;
        }

        /// <summary>
        /// Returns true if the provided bitmap name is valid
        /// </summary>
        /// <param name="name">bitmap name to test</param>
        /// <returns>true if the provided bitmap name/name is valid</returns>
        public static bool IsValidMergeName(string name)
        {
            if (name.IndexOfAny(invalidMergeNameChars) == -1)
            {
                // No image extensions allowed!
                foreach (string ext in supportedImageExtensions)
                {
                    if (name.EndsWith('.' + ext))
                    {
                        return false;
                    }
                }
                return name.Length > 0;
            }
            return false;
        }
    }
}
