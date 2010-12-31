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
    class MergeException : ApplicationException
    {
        public MergeException(string msg)
            : base(msg) {}
    }

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
        private const string unexpecError = "An unexpected error occurred while merging.";

        /// <summary>
        /// Amount of horizontal padding to use for title bar
        /// </summary>
        private const int kTitleBarPaddingX = 1;
        /// <summary>
        /// Amount of vertical padding to use for title bar
        /// </summary>
        private const int kTitleBarPaddingY = 2;

        /// <summary>
        /// Bytes per pixel, assuming 32-bit RGBA format
        /// </summary>
        private const int kBytesPerPix = 4;
        /// <summary>
        /// Approximation of the number of bytes per pixel, once compressed.
        /// Used for guessing output size before an image is actually run through libpng
        /// TODO: Make dyanamic based on compression level?
        /// </summary>
        private const float kCompressedBytesPerPix = 1.0f;

        /// <summary>
        /// Callback stages for merging
        /// </summary>
        public enum MergeCallbackStage
        {
            /// <summary>
            /// Determining how many images can be fit in to one merge.
            /// Additional info is a DeterminingNumPerMergeInfo class
            /// </summary>
            DeterminingNumPerMerge,
            /// <summary>
            /// Current merge is being written to file.
            /// Additional info is the filename being saved
            /// </summary>
            Saving,
            /// <summary>
            /// Merge operation is done.
            /// Additional info null and not used
            /// </summary>
            Done
        }

        /// <summary>
        /// A callback to inform the GUI thread what the merge thread is doing
        /// </summary>
        /// <param name="currStage">current stage in the merge process</param>
        /// <param name="info">Stage-specific additional info</param>
        public delegate void MergeCallback(MergeCallbackStage currStage, object info);

        /// <summary>
        /// Information about the last merge attempt. Used for a callback
        /// </summary>
        public enum LastAttemptInfo
        {
            /// <summary>
            /// This is the first merge attempt (no previous info)
            /// </summary>
            NoLastMege,
            /// <summary>
            /// Last attempt produced a PNG that was too large
            /// </summary>
            TooSmall,
            /// <summary>
            /// Last attempt produced a PNG that was too small
            /// </summary>
            TooLarge
        }

        /// <summary>
        /// Used to pass argumnents to the merge thread
        /// </summary>
        private class MergeThreadArgs
        {
            public List<Bitmap> Bitmaps { get; set; }
            public MDumpOptions Opts { get; set; }
            public string MergePath { get; set; }
            public MergeCallback Callback { get; set; }

            public MergeThreadArgs(List<Bitmap> bitmaps, MDumpOptions options,
                string mPath, MergeCallback callback)
            {
                Bitmaps = bitmaps;
                Opts = options;
                MergePath = mPath;
                Callback = callback;
            }
        }

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
            string mergePath = ta.MergePath;
            int maxMergeSize = opts.MaxMergeSize;

            //Keep track of how many images have been merged
            int imagesMerged = 0;

            //Keep track of the merges saved (we'll have to clean them up on error)
            List<string> mergesSaved = new List<string>();
            //Pointer variables are placed here so they can be accessed form the finally block
            IntPtr currentMergeMem = IntPtr.Zero, lastMergeMem = IntPtr.Zero;
            try
            {
                while (imagesMerged < bitmaps.Count)
                {
                    //TODO: Pass info back

                    List<Bitmap> currMergeSet = new List<Bitmap>();
                    int currentMergeSize, lastMergeSize = 0;

                    //Filename of the current merge image
                    string filename = mergePath + "." + mergesSaved.Count + ".png";

                    //Start by determining the number of images that can be into one merge.
                    //A good starting guess is based on the uncompressed size of the images
                    currMergeSet = EstimateMerge(bitmaps, imagesMerged, maxMergeSize);

                    callback(MergeCallbackStage.DeterminingNumPerMerge,
                       LastAttemptInfo.NoLastMege);

                    //Test the resulting PNG size of the current merge set, then add or remove images
                    //in order to get as close as we can to the max without going over
                    while (true)
                    {
                        //If the PNG merge code weirds out, break out of here.
                        if (CreateMergedImage(currMergeSet, opts, out currentMergeMem, out currentMergeSize)
                            != ECode.EC_SUCCESS)
                        {
                            throw new MergeException(mergeFailedMsg);
                        }
                        if (currentMergeSize > maxMergeSize)
                        {
                            //If not even a single image could fit in the merge, we have an issue
                            if (currMergeSet.Count == 1)
                            {
                                throw new MergeException(sizeTooSmallMsg);
                            }

                            //If the current merge size is over the limit but the last wasn't, use the last one
                            else if (lastMergeMem != IntPtr.Zero && lastMergeSize <= maxMergeSize)
                            {
                                callback(MergeCallbackStage.Saving, filename);
                                SavePNG(lastMergeMem, lastMergeSize, filename);
                                mergesSaved.Add(filename);
                                //We've saved the current set count - 1 since we're using the last
                                //merge
                                imagesMerged += currMergeSet.Count - 1;
                                break;
                            }
                            //Otherwise decrease the current merge set
                            else
                            {
                                callback(MergeCallbackStage.DeterminingNumPerMerge,
                                  LastAttemptInfo.TooLarge);
                                currMergeSet.RemoveAt(currMergeSet.Count - 1);
                            }
                        }
                        else if (currentMergeSize < maxMergeSize)
                        {
                            //If the current merge size is under the limit but the last wasn't
                            //or we've reached the end of our merge set, use the current merge
                            if (imagesMerged + currMergeSet.Count == bitmaps.Count
                                || lastMergeMem != IntPtr.Zero && lastMergeSize > maxMergeSize)
                            {
                                callback(MergeCallbackStage.Saving, filename);                                
                                SavePNG(currentMergeMem, currentMergeSize, filename);
                                mergesSaved.Add(filename);
                                imagesMerged += currMergeSet.Count;
                                break;
                            }
                            //Otherwise increase the current merge set
                            else
                            {
                               callback(MergeCallbackStage.DeterminingNumPerMerge,
                                    LastAttemptInfo.TooSmall);
                               currMergeSet.Add(bitmaps[imagesMerged + currMergeSet.Count]);
                            }
                        }
                        //The current merge is spot on
                        else
                        {
                            callback(MergeCallbackStage.Saving, filename);
                            SavePNG(currentMergeMem, currentMergeSize, filename);
                            mergesSaved.Add(filename);
                            imagesMerged += currMergeSet.Count;
                            break;
                        }
                        PNGOps.FreeUnmanagedData(lastMergeMem);
                        lastMergeMem = currentMergeMem;
                        lastMergeSize = currentMergeSize;
                    }

                    //Free any memory we might have
                    PNGOps.FreeUnmanagedData(currentMergeMem);
                    PNGOps.FreeUnmanagedData(lastMergeMem);
                    //Reset the pointers
                    currentMergeMem = lastMergeMem = IntPtr.Zero;
                }
            }
            catch (MergeException ex)
            {
                CleanupOnMergeError(mergesSaved);
                MessageBox.Show(ex.Message, mergedFailedTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch
            {
                CleanupOnMergeError(mergesSaved);
                MessageBox.Show(unexpecError, mergedFailedTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                PNGOps.FreeUnmanagedData(currentMergeMem);
                PNGOps.FreeUnmanagedData(lastMergeMem);
                callback(MergeCallbackStage.Done, null);
            }
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
                Bitmap toAdd = all[mergedSoFar + ret.Count];
                ret.Add(toAdd);
                estMergeSize += GestEstimatedCompressedSize(toAdd);
            }
            return ret;
        }

        /// <summary>
        /// Gets the size (in bytes) of a bitmap, assuming 32-bits per pixel
        /// </summary>
        /// <param name="bmp">Bitmap to get the size of</param>
        /// <returns>size of the bitmap in bytes, assuming 32-bits per pixel</returns>
        private  static int GestEstimatedCompressedSize(Bitmap bmp)
        {
            return (int)((float)bmp.Width * (float)bmp.Height * kCompressedBytesPerPix);
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

            if (opts.AddTitleBar)
            {
                Bitmap titleBar;
                int mWidth = merged.Width;
                if (mWidth > MDump.Properties.Resources.TitleBar.Width + kTitleBarPaddingX + 1)
                {
                    titleBar = MDump.Properties.Resources.TitleBar;
                }
                else if (mWidth > MDump.Properties.Resources.TitleBarSm.Width + kTitleBarPaddingX + 1)
                {
                    titleBar = MDump.Properties.Resources.TitleBarSm;
                }
                else
                {
                    titleBar = MDump.Properties.Resources.TitleBarMin;
                }
                
                int titleY = merged.Height + 1;
                int titleHeight = titleBar.Height + 2 * kTitleBarPaddingY;

                Bitmap mergedWithTitle = new Bitmap(merged.Width,
                    merged.Height + titleHeight);

                using (Graphics g = Graphics.FromImage(mergedWithTitle))
                {
                    g.DrawImageUnscaled(merged, 0, 0);
                    g.DrawImageUnscaled(titleBar, 1, titleY);

                    //Uncomment to add a background to the bar
                    //Brush rectBrush = new SolidBrush(Color.Red);
                    //g.FillRectangle(rectBrush, 0, titleY, merged.Width, titleHeight);
                    g.DrawImageUnscaled(titleBar, kTitleBarPaddingX, titleY + kTitleBarPaddingY);
                }
                
                merged = mergedWithTitle;
            }


            BitmapData bmpData = merged.LockBits(new Rectangle(0, 0, merged.Width, merged.Height),
                ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);

            
            ECode ret = PNGOps.SavePNGToMemory(bmpData.Scan0, bmpData.Width, bmpData.Height, true,
                mdData, mdData.Length, opts.CompressionLevel, out memImgOut, out imgSizeOut);
            merged.UnlockBits(bmpData);
            return ret;
        }

        /// <summary>
        /// Saves a PNG in unmanaged memory to a file
        /// </summary>
        /// <param name="pngUnmangedMem">pointer to the PNG in unmanaged memory</param>
        /// <param name="pngLen">length of the PNG, in bytes</param>
        /// <param name="filename">filename to store the PNG to</param>
        private static void SavePNG(IntPtr pngUnmangedMem, int pngLen, string filename)
        {
            Byte[] pngMem = new Byte[pngLen];
            Marshal.Copy(pngUnmangedMem, pngMem, 0, pngLen);
            using (FileStream fs = new FileStream(filename, FileMode.OpenOrCreate))
            {
                using (BinaryWriter ms = new BinaryWriter(fs))
                {
                    fs.Write(pngMem, 0, pngLen);
                }
            }
        }


        /// <summary>
        /// Cleans up a half-done merge if an error occurs.
        /// </summary>
        /// <param name="mergesSaved">A list of the images saved so far (to now delete)</param>
        private static void CleanupOnMergeError(List<string> mergesSaved)
        {
            foreach (string file in mergesSaved)
            {
                File.Delete(file);
            }
        }
    }
}
