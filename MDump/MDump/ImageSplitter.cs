﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Threading;
using System.Runtime.InteropServices;
using System.IO;
using System.Windows.Forms;

namespace MDump
{
    /// <summary>
    /// Splits and saves images in a separate thread, providing the wait dialog with callbacks
    /// to indicate its progress.
    /// </summary>
    class ImageSplitter
    {
        #region Constants
        private const string successMsg = "Images were all successfully split in to ";
        private const string dataExtractionErrorMsg = "An error occurred while retrieving the MDump data."
            + "from the merged image";
        private const string unexpecError = "An unexpected error occurred while splitting.\n";
        private const string successTitle = "Success";
        private const string splitFailedTitle = "Error while splitting";

        /// <summary>
        /// Gets the split keyword for split images which have the file name format of
        /// (uniqueName).(keyword)(number).png
        /// </summary>
        public static string SplitKeyword
        {
            get { return "split"; }
        }
        #endregion

        /// <summary>
        /// Callback stages for splitting
        /// </summary>
        public enum SplitStage
        {
            /// <summary>
            /// Starting split process. Integer value is the number of merges to split.
            /// </summary>
            Starting,
            /// <summary>
            /// Starting to split a new merged image.
            /// String value is the name of the merged image.
            /// Integer value is the number of images in the merge.
            /// </summary>
            SplittingNewMerge,
            /// <summary>
            /// Splitting an image out of a merged image.
            /// String value is the name of the image being split from the merge.
            /// Integer value is the number of images split out of the current merge so far.
            /// </summary>
            SplittingImage,
            /// <summary>
            /// Finished splitting a merged image.
            /// Integer value is the number of images merged so far.
            /// </summary>
            FinishedMerge,
            /// <summary>
            /// Split operation done. Data is not used and can be null
            /// </summary>
            Done
        }

        /// <summary>
        /// Used to pass additional information to the callback.
        /// Means different things depending on the current SplitStage
        /// </summary>
        public class SplitCallbackData
        {
            public string StringValue { get; private set; }
            public int IntegerValue { get; private set; }

            public SplitCallbackData(string str, int i)
            {
                StringValue = str;
                IntegerValue = i;
            }

            public SplitCallbackData(string str)
            {
                StringValue = str;
                IntegerValue = 0;
            }

            public SplitCallbackData(int i)
            {
                StringValue = string.Empty;
                IntegerValue = i;
            }
        }

        /// <summary>
        /// A callback to inform the GUI thread of what the split thread is doing
        /// </summary>
        /// <param name="stage">Current stage of split procedure</param>
        /// <param name="data">Has different menaing for each stage</param>
        public delegate void SplitCallback(SplitStage stage, SplitCallbackData data);

        /// <summary>
        /// Used to pass the arguments of SplitImages to the thread it creates
        /// </summary>
        private class SplitThreadArgs
        {
            public List<Bitmap> Bitmaps { get; private set; }
            public MDumpOptions Options { get; private set; }
            public string SplitPath { get; private set; }
            public SplitCallback Callback { get; private set; }

            public SplitThreadArgs(List<Bitmap> bitmaps, MDumpOptions opts, string splitPath,
                SplitCallback callback)
            {
                Bitmaps = bitmaps;
                Options = opts;
                SplitPath = splitPath;
                Callback = callback;
            }
        }

        /// <summary>
        /// Split and save images in another thread,
        /// passing a wait dialog updates on its current state via callbacks
        /// </summary>
        /// <param name="bitmaps">Bitmaps to split and save. Their tag contains afile name or path.</param>
        /// <param name="opts">Options to use for splitting the images</param>
        /// <param name="splitPath">Directory to split merge images to</param>
        /// <param name="callback">Callback for wait form to show user what is going on</param>
        public static void SplitImages(List<Bitmap> bitmaps, MDumpOptions opts, string splitPath,
            SplitCallback callback)
        {
            SplitThreadArgs ta = new SplitThreadArgs(bitmaps, opts, splitPath, callback);
            Thread thread = new Thread(SplitThreadProc);
            thread.Start(ta);
        }

        /// <summary>
        /// Split thread procedure
        /// </summary>
        /// <param name="args">A SplitThreadArgs object containing the args from SplitImages call</param>
        private static void SplitThreadProc(object args)
        {
            SplitThreadArgs sa = args as SplitThreadArgs;

            //Cache our args since we'll be using them constantly
            SplitCallback callback = sa.Callback;
            MDumpOptions opts = sa.Options;
            string splitPath = sa.SplitPath;
            string splitDir = Path.GetDirectoryName(splitPath) + Path.DirectorySeparatorChar;
            List<string> splitsSaved = new List<string>();

            callback(SplitStage.Starting, new SplitCallbackData(sa.Bitmaps.Count));

            try
            {
                //Split each image
                //int imagesMerged = 0;
                foreach (Bitmap image in sa.Bitmaps)
                {
                    //string filename = image.Tag as string;

                    ////Read the MDump data from the image.
                    //byte[] data = PNGOps.LoadMergedImageData(filename);

                    ////Decode MDump data into a string using the text encoding it was saved with
                    //string decodedData = Colors.MDDataEncoding.GetString(data);
                    //string[] lines = decodedData.Split('\n');
                    ////The first line is the number of images in this merge
                    //callback(SplitStage.SplittingNewMerge,
                    //    new SplitCallbackData(Path.GetFileName(filename), Convert.ToInt32(lines[0])));

                    ////Parse the rest of the lines, each of which represents an image in the merged image
                    //for (int c = 1; c < lines.Length; ++c)
                    //{
                    //    string[] tokens = lines[c].Split(';');
                    //    string savePath = opts.FormatPathForSplit(tokens[0]);
                    //    //Filename was either not saved or is ignored as per options.
                    //    if (opts.DiscardFilename(savePath))
                    //    {
                    //        //The name format of merges is <name>.split<num>.png
                    //        savePath = splitPath + '.' + SplitKeyword + splitsSaved.Count + ".png";
                    //    }
                    //    else
                    //    {
                    //        savePath = splitDir + savePath + ".png";
                    //    }
                    //    callback(SplitStage.SplittingImage,
                    //        new SplitCallbackData(Path.GetFileName(savePath), c - 1));

                    //    int x = Convert.ToInt32(tokens[1]);
                    //    int y = Convert.ToInt32(tokens[2]);
                    //    int width = Convert.ToInt32(tokens[3]);
                    //    int height = Convert.ToInt32(tokens[4]);
                    //    Rectangle rect = new Rectangle(x, y, width, height);
                    //    //Save the image
                    //    Bitmap splitImage = new Bitmap(width, height);
                    //    using (Graphics g = Graphics.FromImage(splitImage))
                    //    {
                    //        g.DrawImage(image, 0, 0, rect, GraphicsUnit.Pixel);
                    //    }
                    //    splitImage.Save(savePath, System.Drawing.Imaging.ImageFormat.Png);
                    //    splitsSaved.Add(savePath);
                    //}
                    //callback(SplitStage.FinishedMerge, new SplitCallbackData(++imagesMerged));
                }
                MessageBox.Show(successMsg + splitDir, successTitle);
            }
            catch (PNGOpsException ex)
            {
                MessageBox.Show(ex.Message, splitFailedTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
                CleanupOnSplitFail(splitsSaved);
            }
            catch(Exception ex)
            {
                ErrorHandling.LogException(ex);
                MessageBox.Show(unexpecError + ErrorHandling.ErrorMessage,
                    splitFailedTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
                CleanupOnSplitFail(splitsSaved);
            }
            finally
            {
                callback(SplitStage.Done, null);
            }
        }

        /// <summary>
        /// Used to delete split files created already if we fail.
        /// This way we don't have a half-baked split.
        /// </summary>
        /// <param name="splitsSaved">Split images already saved</param>
        private static void CleanupOnSplitFail(List<string> splitsSaved)
        {
            foreach (string split in splitsSaved)
            {
                File.Delete(split);
            }
        }
    }
}
