using System;
using System.Drawing;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Threading;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace MDump
{
    /// <summary>
    /// Merges and saves images in a separate thread, providing the wait dialog with callbacks
    /// to indicate its progress.
    /// </summary>
    class ImageMerger
    {
        /// <summary>
        /// The magic string that determines if the file we're looking at is an MDump merged image
        /// </summary>
        private const string MagicString = "MDmpMrge";

        /// <summary>
        /// The byte representation of the magic string
        /// </summary>
        private static readonly byte[] magicBytes = new System.Text.UTF8Encoding().GetBytes(MagicString);

        /// <summary>
        /// Tests if the provided file is an MDump merged image
        /// </summary>
        /// <param name="file">Filename of the file to check</param>
        /// <returns>true if the file is an MDump merged image, otherwise false</returns>
        public static bool IsMergedImage(string file)
        {
            //TODO: Implement
            return false;
        }

        /// <summary>
        /// Merges images in a separate thread,
        /// passing a wait dialog updates on its current state via callbacks
        /// </summary>
        /// <param name="images">Bitmaps to merge and save</param>
        /// <param name="opts">Options to use for merging the images</param>
        public static void MergeImages(IEnumerable<Bitmap> images, MDumpOptions opts)
        {
            //TODO: Multithreading and callbacks

            //Merge images
            int maxWidth = 0;
            int maxHeight = 0;
            foreach(Bitmap bmp in images)
            {
                if (bmp.Width > maxWidth)
                {
                    maxWidth = bmp.Width;
                }
                maxHeight += bmp.Height;
            }

            Bitmap merged = BTBitmapMapper.MergeImages(images, new Size(maxWidth, maxHeight),
                PixelFormat.Format32bppArgb);

            string filename = "Merged.png";
            
            //Test the libpng code out
            BitmapData bmpData = merged.LockBits(new Rectangle(0, 0, merged.Width, merged.Height),
                ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
            PNGOps.SavePNG(bmpData.Scan0, bmpData.Width, bmpData.Height, filename, true);
            merged.UnlockBits(bmpData);
        }
    }
}
