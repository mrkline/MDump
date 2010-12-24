using System;
using System.Drawing;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Windows.Forms;
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
        private const string mergeFailedMsg = "An error occurred while merging imgaes";
        private const string mergedFailedTitle = "Error while merging";
        private const string sizeTooSmallMsg = "The maximum merge size is too small to fit one of the images."
                                                + " Make it larger and try again";
        private const string sizeTooSmallTitle = "Max size too small";

        /// <summary>
        /// Used to pass argumnents to the merge thread
        /// </summary>
        private class MergeThreadArgs
        {
            public List<Bitmap> Bitmaps { get; set; }
            public MDumpOptions Opts { get; set; }
            public string MergeDir { get; set; }
            public MergeCallback Callback { get; set; }

            public MergeThreadArgs(List<Bitmap> bitmaps, MDumpOptions options,
                string mDir, MergeCallback callback)
            {
                Bitmaps = bitmaps;
                Opts = options;
                MergeDir = mDir;
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
        /// <param name="mergeDir">Directory to merge images to</param>
        /// <param name="callback">Callback for display form to show user what is going on</param>
        public static void MergeImages(List<Bitmap> images, MDumpOptions opts, string mergeDir,
            MergeCallback callback)
        {
            MergeThreadArgs ta = new MergeThreadArgs(images, opts, mergeDir, callback);
            Thread thread = new Thread(MergeThreadProc);
            thread.Start(ta);
        }

        /// <summary>
        /// Merge thread procedure
        /// </summary>
        /// <param name="args">A MergeThreadArgs object containing the thread arguments</param>
        private static void MergeThreadProc(object args)
        {
            //Cast arguments to their actual type
            MergeThreadArgs ta = args as MergeThreadArgs;

            //Cache commonly used values
            List<Bitmap> bitmaps = ta.Bitmaps;
            MergeCallback callback = ta.Callback;
            MDumpOptions opts = ta.Opts;
            string mergeDir = ta.MergeDir;
            int maxMergeSize = opts.MaxMergeSize;

            //Indicates whether or not directory previously existed.
            //Useful for cleanup.
            bool dirPreExisted = Directory.Exists(mergeDir);
            if (!dirPreExisted)
            {
                Directory.CreateDirectory(mergeDir);
            }

            //Keep track of how many images have been merged
            int imagesMerged = 0;

            while (imagesMerged < bitmaps.Count)
            {
                callback(MergeCallbackStage.DeterminingNumPerMerge);

                List<Bitmap> currMergeSet = new List<Bitmap>();
                IntPtr currentMergeMem = IntPtr.Zero, lastMergeMem = IntPtr.Zero;
                int currentMergeSize, lastMergeSize = 0;

                //Start by determining the number of images that can be into one merge.
                //A good starting guess is based on the uncompressed size of the images
                currMergeSet = EstimateMerge(bitmaps, imagesMerged, maxMergeSize);

                //Test the resulting PNG size of the current merge set, then add or remove images
                //in order to get as close as we can to the max without going over
                while (true)
                {
                    if (CreateMergedImage(currMergeSet, opts, out currentMergeMem, out currentMergeSize)
                        != ECode.EC_SUCCESS)
                    {
                        MessageBox.Show(mergeFailedMsg, mergedFailedTitle, MessageBoxButtons.OK,
                            MessageBoxIcon.Error);
                        //Break from both loops
                        imagesMerged = int.MaxValue;
                        break;
                    }
                    if (currentMergeSize > maxMergeSize)
                    {
                        //If not even a single image could fit in the merge, we have an issue
                        if(currMergeSet.Count == 1)
                        {
                            MessageBox.Show(sizeTooSmallMsg, sizeTooSmallTitle, MessageBoxButtons.OK,
                                MessageBoxIcon.Error);
                            //Break from both loops
                            imagesMerged = int.MaxValue;
                            break;
                        }
                        
                        //If the current merge size is over the limit but the last wasn't, use the last one
                        if(lastMergeSize <= maxMergeSize)
                        {
                            PNGOps.FreeUnmanagedData(currentMergeMem);
                            callback(MergeCallbackStage.Saving);

                        }
                    }
                    else
                    {
                        
                    }

                }
                
                //Free any memory we might have
                PNGOps.FreeUnmanagedData(currentMergeMem);
                PNGOps.FreeUnmanagedData(lastMergeMem);
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


            BitmapData bmpData = merged.LockBits(new Rectangle(0, 0, merged.Width, merged.Height),
                ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);

            
            ECode ret = PNGOps.SavePNGToMemory(bmpData.Scan0, bmpData.Width, bmpData.Height, true, mdData, mdData.Length,
                out memImgOut, out imgSizeOut);
            merged.UnlockBits(bmpData);

            return ret;
        }

        /// <summary>
        /// Cleans up a half-done merge if an error occurs.
        /// </summary>
        /// <param name="imagesSaved"></param>
        /// <param name="mergeDirectory"></param>
        /// <param name="dirPreviouslyExisted"></param>
        private static void CleanupOnMergeError(List<String> imagesSaved,
            string mergeDirectory, bool dirPreviouslyExisted)
        {
            //TODO: Pick up here.
        }
    }
}
