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
        /// <summary>
        /// Thrown in this class.
        /// Used to differentiate between expected exceptions and unexpected dones.
        /// </summary>
        private class MergeException : ApplicationException
        {
            public MergeException(string msg)
                : base(msg) { }

            public MergeException(string msg, Exception inner)
                : base(msg, inner) { }
        }

        #region String Constants
        private const string successMsg = "Images were all successfully merged in to ";
        private const string mergeFailedMsg = "An error occurred while merging imgaes";
        private const string mergedFailedTitle = "Error while merging";
        private const string successTitle = "Success";
        private const string unexpecError = "An unexpected error occurred while merging.\n";
        #endregion

        #region Numeric Constants
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
        #endregion

        #region Enums
        /// <summary>
        /// Callback stages for merging
        /// </summary>
        public enum MergeStage
        {
            /// <summary>
            /// Starting merge process.
            /// Additional info is the total number of images to be merged
            /// </summary>
            Starting,
            /// <summary>
            /// Determining how many images can be fit in to one merge.
            /// Additional info is an LastAttemptInfo enum.
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
        #endregion

        /// <summary>
        /// A callback to inform the GUI thread what the merge thread is doing
        /// </summary>
        /// <param name="currStage">current stage in the merge process</param>
        /// <param name="current">current number of images merged</param>
        /// <param name="info">Stage-specific additional info</param>
        public delegate void MergeCallback(MergeStage currStage, int current, object info);

        /// <summary>
        /// Used to pass argumnents to the merge thread
        /// </summary>
        private class MergeThreadArgs
        {
            public List<Bitmap> Bitmaps { get; private set; }
            public string MergePath { get; private set; }
            public MergeCallback Callback { get; private set; }

            public MergeThreadArgs(List<Bitmap> bitmaps,
                string mPath, MergeCallback callback)
            {
                Bitmaps = bitmaps;
                MergePath = mPath;
                Callback = callback;
            }
        }

        /// <summary>
        /// Merges images in a separate thread,
        /// passing a wait dialog updates on its current state via callbacks
        /// </summary>
        /// <param name="images">Bitmaps to merge and save. Their tag contains afile name or path.</param>
        /// <param name="mergeDir">Directory to merge images to</param>
        /// <param name="callback">Callback for wait form to show user what is going on</param>
        public static void MergeImages(List<Bitmap> images, string mergeDir,
            MergeCallback callback)
        {
            MergeThreadArgs ta = new MergeThreadArgs(images, mergeDir, callback);
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
            MDumpOptions opts = MDumpOptions.Instance;
            string mergePath = ta.MergePath;
            int maxMergeSize = opts.MaxMergeSize;

            //Keep track of how many images have been merged
            int imagesMerged = 0;

            callback(MergeStage.Starting, 0, bitmaps.Count);

            //Keep track of the merges saved (we'll have to clean them up on error)
            List<string> mergesSaved = new List<string>();

            try
            {
                while (imagesMerged < bitmaps.Count)
                {
                    List<Bitmap> currMergeSet = new List<Bitmap>();

                    //Filename of the current merge image
                    string filename = mergePath + "." + mergesSaved.Count + ".png";

                    //Start by determining the number of images that can be into one merge.
                    //A good starting guess is based on the uncompressed size of the images
                    currMergeSet = EstimateMerge(bitmaps, imagesMerged, maxMergeSize);

                    callback(MergeStage.DeterminingNumPerMerge, imagesMerged,
                       LastAttemptInfo.NoLastMege);

                    byte[] currentMergeMem = null, lastMergeMem = null;

                    //Test the resulting PNG size of the current merge set, then add or remove images
                    //in order to get as close as we can to the max without going over
                    while (true)
                    {
                        //If the PNG merge code weirds out, break out of here.
                        try
                        {
                            currentMergeMem = CreateMergedImage(currMergeSet, opts);
                        }
                        catch (FormatHandlerException ex)
                        {
                            throw new MergeException(mergeFailedMsg, ex);
                        }
                        if (currentMergeMem.Length > maxMergeSize)
                        {
                            //If not even a single image could fit in the merge, we have an issue
                            if (currMergeSet.Count == 1)
                            {
                                throw new MergeException(GenerateTooSmallMessage((currMergeSet[0].Tag as ImageTagBase).Name,
                                    currMergeSet[0].RawFormat.Guid == ImageFormat.Jpeg.Guid));
                            }

                            //If the current merge size is over the limit but the last wasn't, use the last one
                            else if (lastMergeMem != null && currentMergeMem.Length <= maxMergeSize)
                            {
                                callback(MergeStage.Saving, imagesMerged, filename);
                                File.WriteAllBytes(filename, lastMergeMem);
                                mergesSaved.Add(filename);
                                //We've saved the current set count - 1 since we're using the last
                                //merge
                                imagesMerged += currMergeSet.Count - 1;
                                break;
                            }
                            //Otherwise decrease the current merge set
                            else
                            {
                                callback(MergeStage.DeterminingNumPerMerge,
                                    imagesMerged, LastAttemptInfo.TooLarge);
                                currMergeSet.RemoveAt(currMergeSet.Count - 1);
                            }
                        }
                        else if (currentMergeMem.Length < maxMergeSize)
                        {
                            //If the current merge size is under the limit but the last wasn't
                            //or we've reached the end of our merge set, use the current merge
                            if (imagesMerged + currMergeSet.Count == bitmaps.Count
                                || lastMergeMem != null && lastMergeMem.Length > maxMergeSize)
                            {
                                callback(MergeStage.Saving, imagesMerged, filename);
                                File.WriteAllBytes(filename, currentMergeMem);
                                mergesSaved.Add(filename);
                                imagesMerged += currMergeSet.Count;
                                break;
                            }
                            //Otherwise increase the current merge set
                            else
                            {
                                callback(MergeStage.DeterminingNumPerMerge,
                                    imagesMerged, LastAttemptInfo.TooSmall);
                                currMergeSet.Add(bitmaps[imagesMerged + currMergeSet.Count]);
                            }
                        }
                        //The current merge is spot on
                        else
                        {
                            callback(MergeStage.Saving, imagesMerged, filename);
                            File.WriteAllBytes(filename, currentMergeMem);
                            mergesSaved.Add(filename);
                            imagesMerged += currMergeSet.Count;
                            break;
                        }
                        lastMergeMem = currentMergeMem;
                    }
                    
                }
                MessageBox.Show(successMsg + Path.GetDirectoryName(mergePath), successTitle);
            }
            catch (MergeException ex)
            {
                CleanupOnMergeError(mergesSaved);
                MessageBox.Show(ex.Message, mergedFailedTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch(Exception ex)
            {
                CleanupOnMergeError(mergesSaved);
                ErrorHandling.LogException(ex);
                MessageBox.Show(unexpecError + ErrorHandling.ErrorMessage,
                    mergedFailedTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                callback(MergeStage.Done, 0, null);
            }
        }

        /// <summary>
        /// Generates the message for a MessageBox informing the user that one of their images
        /// is too large for the max merge size.
        /// </summary>
        /// <param name="name">Name of the file that is too large</param>
        /// <param name="isJPEG">Should be true if the file started out as a JPEG</param>
        /// <returns>message to display</returns>
        private static string GenerateTooSmallMessage(string name, bool isJPEG)
        {
           string ret = "The maximum merge size is too small to fit the image " + Path.GetFileName(name)
                + ". Increasing the maximum merge size may help.";
                if(isJPEG)
                {
                    ret += " Please note that PNG cannot compress large images as well as JPEG"
                        + " since JPEG sacrifices information to make the image smaller."
                        + " The next major release of MDump will support saving merged images as JPEG ones.";
                }
            return ret;
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
        /// Creates the merged PNG image in memory, passing back the buffer that contains the image.
        /// </summary>
        /// <param name="bitmaps">Bitmaps to merge into a PNG</param>
        /// <param name="opts">Options to use while merging</param>
        /// <param name="handler">Image format handler to save the image with</param>
        /// <returns>The merged image in managed memory</returns>
        private static byte[] CreateMergedImage(IEnumerable<Bitmap> bitmaps, MDumpOptions opts)
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

            byte[] mdData;
            //Merge our images and generate our data
            Bitmap merged = BTBitmapMapper.MergeImages(bitmaps, new Size(maxWidth, maxHeight),
                PixelFormat.Format32bppArgb, opts, out mdData);

            //Tack on the title bar if requested to
            if (opts.AddTitleBar)
            {
                Bitmap titleBar;
                int mWidth = merged.Width;
                //There are three versions of the title bar. Pick the largest one that will fit on the image.
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

                //Draw the title bar
                using (Graphics g = Graphics.FromImage(mergedWithTitle))
                {
                    g.DrawImage(merged, 0, 0, merged.Width, merged.Height);

                    //Uncomment to add a background to the bar
                    //Brush rectBrush = new SolidBrush(Color.Red);
                    //g.FillRectangle(rectBrush, 0, titleY, merged.Width, titleHeight);
                    g.DrawImage(titleBar, kTitleBarPaddingX, titleY + kTitleBarPaddingY,
                        titleBar.Width, titleBar.Height);
                }
                
                merged = mergedWithTitle;
            }

            return MasterFormatHandler.Instance.SaveToMemory(merged, mdData, opts.CompLevel);
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
