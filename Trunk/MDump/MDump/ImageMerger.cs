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
    /// Merges and saves images.
    /// </summary>
    class ImageMerger
    {
        private const string MagicString = "MDmpMrge";

        private static readonly byte[] magicBytes = new System.Text.UTF8Encoding().GetBytes(MagicString);
        private static readonly int numMagicPixels = magicBytes.Length / 4
            + (magicBytes.Length % 4 > 0 ? 1 : 0);

        public static bool IsMergedImage(Bitmap bmp)
        {
            if (bmp.PixelFormat != System.Drawing.Imaging.PixelFormat.Format32bppArgb)
            {
                throw new ArgumentException("Bitmap must be 32-bit ARGB format");
            }

            Rectangle magicRect = new Rectangle(0, 0, numMagicPixels, 1);
            BitmapData bd = bmp.LockBits(magicRect, ImageLockMode.ReadOnly, bmp.PixelFormat);
            byte[] bmpBytes = new byte[magicBytes.Length];
            Marshal.Copy(bd.Scan0, bmpBytes, 0, bmpBytes.Length);
            bmp.UnlockBits(bd);
            for (int c = 0; c < magicBytes.Length; ++c)
            {
                if (bmpBytes[c] != magicBytes[c])
                    return false;
            }

            return true;
        }


        public static void MergeImages(IEnumerable<Bitmap> images, MDumpOptions opts)
        {
            //TEMP: Save each image, then all the images.  This is a test to see if 
            //the total size is similar to the sums of the individuals
            const string tmpDir = @"\tmp\";
            string cd = System.IO.Directory.GetCurrentDirectory();

            System.IO.Directory.CreateDirectory(cd + tmpDir);

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

            string filename = cd + tmpDir + "Merged.png";
            
            //Test the libpng code out
            BitmapData bmpData = merged.LockBits(new Rectangle(0, 0, merged.Width, merged.Height),
                ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
            PNGOps.SavePNG(bmpData.Scan0, bmpData.Width, bmpData.Height, filename, true);
            merged.UnlockBits(bmpData);
        }
    }
}
