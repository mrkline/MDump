using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace MDump
{
    /// <summary>
    /// Manages path validation. The idea behind this class is to isolate this functionality
    /// so that future changes are simple (as they weren't the first time it changed).
    /// </summary>
    static class PathManager
    {
        private static char[] invalidDirNameChars;
        private static char[] invalidMergeNameChars;

        private static readonly string[] supportedImageExtensions = { "bmp", "gif", "exif", "jpg",
                                                              "jpeg", "png", "tif", "tiff" };

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

            invalidMergeNameChars = Path.GetInvalidFileNameChars();
        }

        public static string InvalidDirNameMsg { get { return " is not a valid folder name."; } }
        public static string InvalidDirNameTitle { get { return "Invalid folder name"; } }
        public static string InvalidBmpNameMsg
            { get { return "Extensions will automatically be added to the images when they are split again"; } }
        public static string InvalidBmpTagTitle { get { return "Invalid image name"; } }
        
        
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
            //HACK: Are freshly allocated objects always generation 0?
            int gen;
            try
            {
                System.Drawing.Bitmap bmp = new System.Drawing.Bitmap(filepath);
                gen = GC.GetGeneration(bmp);
            }
            catch
            {
                return false;
            }
            //Clean up from the created bitmap (we don't need it hanging around in memory)
            GC.Collect(gen);
            return true;
        }


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
        /// Returns true if the provided bitmap name/name is valid
        /// </summary>
        /// <param name="name">bitmap name to test</param>
        /// <returns>true if the provided bitmap name/name is valid</returns>
        public static bool IsValidMergeName(string name)
        {
            if (name.IndexOfAny(invalidMergeNameChars) == -1)
            {
                //No image extensions allowed!
                foreach (string ext in supportedImageExtensions)
                {
                    if (name.EndsWith('.' + ext))
                    {
                        return false;
                    }
                }
                return true;
            }
            return false;
        }
    }
}
