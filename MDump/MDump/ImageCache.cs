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
        /// <summary>
        /// The ImageCacheTicket is attached to an image's tag.
        /// When the image, it's tag, and the ticket are garbage collected,
        /// the cache is cleared of that image
        /// </summary>
        public class ImageCacheTicket
        {
            private const int deletionWaitMS = 500;

            private string path;

            internal ImageCacheTicket(string filepath)
            {
                path = filepath;
            }

            ~ImageCacheTicket()
            {
                new System.Threading.Thread(DeleteThreadProc).Start(path);
            }

            private static void DeleteThreadProc(object arg)
            {
                string path = (string)arg;
                System.Threading.Thread.Sleep(deletionWaitMS);
                File.Delete(path);
            }
        }

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

        private ImageCache()
        {
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
        /// <param name="force32BppARGB">true if returned image must be in 32-bpp ARGB format</param>
        /// <param name="ticket">
        /// The ticket to be attached to the image. When the ticket goes out of scope,
        /// the cached image is deleted
        /// </param>
        /// <returns>The cached copy of the image</returns>
        public Bitmap CreateCachedImage(string filepath, bool force32BppARGB, out ImageCacheTicket ticket)
        {
            string copyPath = cacheDir + Path.GetFileName(filepath);
            File.Copy(filepath, copyPath);
            Bitmap ret = new Bitmap(copyPath);

            if (force32BppARGB && ret.PixelFormat != System.Drawing.Imaging.PixelFormat.Format32bppArgb)
            {
                ret = ret.Clone(new Rectangle(0, 0, ret.Width, ret.Height),
                        System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            }
            ticket = new ImageCacheTicket(copyPath);
            return ret;
        }
    }
}
