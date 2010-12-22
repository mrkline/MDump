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
        /// Used to pass argumnents to the merge thread
        /// </summary>
        private class MergeThreadArgs
        {
            public List<Bitmap> Bitmaps { get; set; }
            public MDumpOptions Opts { get; set; }
            public MergeCallback Callback { get; set; }

            public MergeThreadArgs(List<Bitmap> bitmaps, MDumpOptions options, MergeCallback callback)
            {
                Bitmaps = bitmaps;
                Opts = options;
                Callback = callback;
            }
        }

        /// <summary>
        /// Callback stages for merging
        /// </summary>
        public enum MergeCallbackStage
        {
            /// <summary>
            /// Determining how many images can be fit in to one merge
            /// </summary>
            DeterminingNumPerMerge,
            /// <summary>
            /// Current merge is being written to file
            /// </summary>
            Saving,
            /// <summary>
            /// Merge operation is done
            /// </summary>
            Done
        }

        /// <summary>
        /// A callback to inform the GUI thread what the merge thread is doing
        /// </summary>
        /// <param name="currStage">current stage in the merge process</param>
        public delegate void MergeCallback(MergeCallbackStage currStage);

        /// <summary>
        /// Bytes per pixel, assuming 32-bit RGBA format
        /// </summary>
        private const int kBytesPerPix = 4;

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
        public static void MergeImages(List<Bitmap> images, MDumpOptions opts, MergeCallback callback)
        {
            MergeThreadArgs ta = new MergeThreadArgs(images, opts, callback);
            Thread thread = new Thread(MergeThreadProc);
            thread.Start(ta);
        }

        /// <summary>
        /// Merge thread procedure
        /// </summary>
        /// <param name="args">A MergeThreadArgs object containing the thread arguments</param>
        private static void MergeThreadProc(object args)
        {
            //TODO: Handle edge case where not even one image can fit in the
            //given merge size.

            MergeThreadArgs ta = args as MergeThreadArgs;
            
            //Cache commonly used values
            List<Bitmap> bitmaps = ta.Bitmaps;
            MergeCallback callback = ta.Callback;
            int maxMergeSize = ta.Opts.MaxMergeSize;

            int imagesMerged = 0;

            while (imagesMerged < bitmaps.Count)
            {
                callback(MergeCallbackStage.DeterminingNumPerMerge);

                List<Bitmap> currMergeSet = new List<Bitmap>();
                IntPtr currentMerge, lastMerge;
                int currentMergeSize = 0, lastMergeSize = 0;

                //Start by determining the number of images that can be into one merge.
                //A good starting guess is based on the uncompressed size of the images
                currMergeSet = EstimateMerge(bitmaps, imagesMerged, maxMergeSize);

                //Test the resulting PNG size of the current merge set, then add or remove images
                //in order to get as close as we can to the max without going over
                //TODO: Pick up here
            }
          
            //Stream it to a file to test
            //using (FileStream fs = new FileStream(filename, FileMode.OpenOrCreate))
            //{
            //    using (BinaryWriter ms = new BinaryWriter(fs))
            //    {
            //        fs.Write(pngData, 0, pngLen);
            //    }
            //}
            callback(MergeCallbackStage.Done);
        }

        /// <summary>
        /// Estimates how many bitmaps should be merged based on their uncompressed size
        /// </summary>
        /// <param name="all">all bitmaps to be merged.  This returns a subset</param>
        /// <param name="mergedSoFar">the number of bitmaps that have been merged so far</param>
        /// <param name="maxMergeSize">the max merge size (in bytes)</param>
        /// <returns>the subset of all that should be merged (by estimation)</returns>
        private static List<Bitmap> EstimateMerge(List<Bitmap> all, int mergedSoFar, int maxMergeSize)
        {
            List<Bitmap> ret = new List<Bitmap>();
            int estMergeSize = 0;
            while (ret.Count + mergedSoFar < all.Count
                && estMergeSize < maxMergeSize)
            {
                Bitmap toAdd = ret[mergedSoFar + ret.Count];
                ret.Add(toAdd);
                estMergeSize += BmpSize(toAdd);
            }
            return ret;
        }

        /// <summary>
        /// Gets the size (in bytes) of a bitmap, assuming 32-bits per pixel
        /// </summary>
        /// <param name="bmp">Bitmap to get the size of</param>
        /// <returns>size of the bitmap in bytes, assuming 32-bits per pixel</returns>
        private  static int BmpSize(Bitmap bmp)
        {
            return bmp.Width * bmp.Height * kBytesPerPix;
        }

        /// <summary>
        /// Creates the merged image in memory, passing back a pointer to the image along with its size.
        /// </summary>
        /// <param name="bitmaps">Bitmaps to merge into a PNG</param>
        /// <param name="opts">Options to use while merging</param>
        /// <param name="memImgOut">Pointer to the PNG file now residing in unmanaged memory</param>
        /// <param name="imgSizeOut">Size of PNG residing in memory (in bytes)</param>
        /// <returns>the error code of PNGOps.SavePNGToMemory</returns>
        private static ECode CreateMergedImage(IEnumerable<Bitmap> bitmaps, MDumpOptions opts, out IntPtr memImgOut, out int imgSizeOut)
        {
            int maxWidth = 0;
            int maxHeight = 0;
            foreach (Bitmap bmp in bitmaps)
            {
                if (bmp.Width > maxWidth)
                {
                    maxWidth = bmp.Width;
                }
                maxHeight += bmp.Height;
            }

            Byte[] mdData;
            Bitmap merged = BTBitmapMapper.MergeImages(bitmaps, new Size(maxWidth, maxHeight),
                PixelFormat.Format32bppArgb, opts, out mdData);

            string filename = "Merged.png";

            //Test the libpng code out
            BitmapData bmpData = merged.LockBits(new Rectangle(0, 0, merged.Width, merged.Height),
                ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);

            
            ECode ret = PNGOps.SavePNGToMemory(bmpData.Scan0, bmpData.Width, bmpData.Height, filename, true, mdData, mdData.Length,
                out memImgOut, out imgSizeOut);
            merged.UnlockBits(bmpData);

            return ret;
        }
    }
}
