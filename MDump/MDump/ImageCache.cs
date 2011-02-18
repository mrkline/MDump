using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Drawing;

namespace MDump
{
    /// <summary>
    /// We want to avoid locking the images we load in. However, it's a lot cheaper in memory
    /// to read images from a file instead of a memory representation. Because of this,
    /// we're going to make copies of the images.
    /// </summary>
    class ImageCache
    {
        #region Singleton

        private static ImageCache _instance = null;
        /// <summary>
        /// Gets the singleton instance of the ImageCache
        /// Not thread-safe, but we don't plan on using this across multiple threads
        /// </summary>
        public static ImageCache Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new ImageCache();
                }
                return _instance;
            }
        }
        #endregion

        /// <summary>
        /// The directory to store cached images in
        /// </summary>
        private readonly string cacheDir = PathUtils.AppDir + @"ImageCache\";
        /// <summary>
        /// Maps created bitmaps to the file used to create them.
        /// </summary>
        private Dictionary<Bitmap, string> fileDictionary;

        private ImageCache()
        {
            fileDictionary = new Dictionary<Bitmap, string>();
            Directory.CreateDirectory(cacheDir);
        }
        ~ImageCache()
        {
            Directory.Delete(cacheDir, true);
        }

        /// <summary>
        /// Create a cached copy of an image. The function assumes that
        /// the image has already been tested for validity and has no problems.
        /// </summary>
        /// <param name="filepath">The image to copy</param>
        /// <returns>The cached copy of the image</returns>
        public Bitmap CreateCachedImage(string filepath, bool force32BitARGB)
        {
            string copyPath = cacheDir + Path.GetFileName(filepath);
            File.Copy(filepath, copyPath);
            Bitmap ret = new Bitmap(copyPath);

            if (force32BitARGB && ret.PixelFormat != System.Drawing.Imaging.PixelFormat.Format32bppArgb)
            {
                ret = ret.Clone(new Rectangle(0, 0, ret.Width, ret.Height),
                        System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            }
            fileDictionary.Add(ret, copyPath);
            return ret;
        }

        /// <summary>
        /// Deletes the cached copy of an image
        /// </summary>
        /// <param name="bmp">The Bitmap object returned by CreateCachedImage</param>
        public void DeleteCachedImage(Bitmap bmp)
        {
            bmp.Dispose();
            File.Delete(fileDictionary[bmp]);
        }
    }
}
