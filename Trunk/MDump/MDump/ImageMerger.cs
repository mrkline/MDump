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
        /// Tests if the provided file is an MDump merged image
        /// </summary>
        /// <param name="file">Filename of the file to check</param>
        /// <returns>true if the file is an MDump merged image, otherwise false</returns>
        public static MergedCode IsMergedImage(string file)
        {
            return PNGOps.IsMergedImage(file);
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

            Byte[] mdData;
            Bitmap merged = BTBitmapMapper.MergeImages(images, new Size(maxWidth, maxHeight),
                PixelFormat.Format32bppArgb, out mdData);

            string filename = "Merged.png";

            //Test the libpng code out
            BitmapData bmpData = merged.LockBits(new Rectangle(0, 0, merged.Width, merged.Height),
                ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);

            IntPtr pngUnmanagedData;
            int pngLen;
            PNGOps.SavePNGToMemory(bmpData.Scan0, bmpData.Width, bmpData.Height, filename, true, mdData, mdData.Length,
                out pngUnmanagedData, out pngLen);
            merged.UnlockBits(bmpData);

            Byte[] pngData = new Byte[pngLen];
            Marshal.Copy(pngUnmanagedData, pngData, 0, pngLen);
            PNGOps.FreeUnmanagedData(pngUnmanagedData);
            //Stream it to a file to test
            using(FileStream fs = new FileStream(filename, FileMode.OpenOrCreate))
            {
                using (BinaryWriter ms = new BinaryWriter(fs))
                {
                    fs.Write(pngData, 0, pngLen);
                }
            }
        }
    }
}
