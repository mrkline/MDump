﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Threading;
using System.Windows.Forms;

namespace MDump
{
    /// <summary>
    /// Splits and saves images in a separate thread, providing the wait dialog with callbacks
    /// to indicate its progress.
    /// </summary>
    class ImageSplitter
    {
        #region String Constants
        private const string successMsg = "Images were all successfully split in to ";
        private const string dataExtractionErrorMsg = "An error occurred while retrieving the MDump data."
            + "from the merged image";
        private const string unexpecError = "An unexpected error occurred while splitting.\n";
        private const string successTitle = "Success";
        private const string splitFailedTitle = "Error while splitting";
        private const string unexpectedToken = "An unexpected token was found in the MDump data";

        /// <summary>
        /// Gets the split keyword for split images which have the file name format of
        /// (uniqueName).(keyword)(number).ext
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
            /// Starting split process. String is not used and can be null
            /// </summary>
            Starting,
            /// <summary>
            /// Starting to split a new merged image.
            /// String value is the name of the merged image.
            /// </summary>
            SplittingNewMerge,
            /// <summary>
            /// Splitting an image out of a merged image.
            /// String value is the name of the image being split from the merge.
            /// </summary>
            SplittingImage,
            /// <summary>
            /// Split operation done. String is not used and can be null
            /// </summary>
            Done
        }

        /// <summary>
        /// Used to pass the arguments of SplitImages to the thread it creates
        /// </summary>
        private class SplitThreadArgs
        {
            public List<Bitmap> Bitmaps { get; private set; }
            public string SplitPath { get; private set; }
            public SplitCallback Callback { get; private set; }

            public SplitThreadArgs(List<Bitmap> bitmaps, string splitPath,
                SplitCallback callback)
            {
                Bitmaps = bitmaps;
                SplitPath = splitPath;
                Callback = callback;
            }
        }

        /// <summary>
        /// A callback to inform the GUI thread of what the split thread is doing
        /// </summary>
        /// <param name="stage">Current stage of split procedure</param>
        /// <param name="data">Has different menaing for each stage</param>
        public delegate void SplitCallback(SplitStage stage, string data);

        /// <summary>
        /// Split and save images in another thread,
        /// passing a wait dialog updates on its current state via callbacks
        /// </summary>
        /// <param name="bitmaps">Bitmaps to split and save. Their tag contains afile name or path.</param>
        /// <param name="splitDir">Directory to split merge images to</param>
        /// <param name="callback">Callback for wait form to show user what is going on</param>
        public static void SplitImages(List<Bitmap> bitmaps, string splitDir,
            SplitCallback callback)
        {
            SplitThreadArgs ta = new SplitThreadArgs(bitmaps, splitDir, callback);
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

            // Cache our args since we'll be using them constantly
            SplitCallback callback = sa.Callback;
            MDumpOptions opts = MDumpOptions.Instance;
            string splitPath = sa.SplitPath;
            string splitDir = splitPath.Substring(0, splitPath.LastIndexOf(Path.DirectorySeparatorChar));
            string splitName = splitPath.Substring(splitPath.LastIndexOf(Path.DirectorySeparatorChar) + 1);
            List<string> splitsSaved = new List<string>();
            List<string> dirsCreated = new List<string>();
            frmOverwrite dlgOverwrite = new frmOverwrite();

            try
            {
                callback(SplitStage.Starting, null);

                // Set the starting directory to the specified one
                Directory.SetCurrentDirectory(splitDir);

                // Split each image
                foreach (Bitmap image in sa.Bitmaps)
                {
                    // Decode MDump data into a string using the text encoding it was saved with
                    string[] dataTokens = MDDataReader.SplitData((image.Tag as MergedImageTag).MDData);
                    
                    // The first token contains the number of images in this merge
                    callback(SplitStage.SplittingNewMerge, ((MergedImageTag)image.Tag).Name);

                    // Parse the rest of the tokens, each of which represents an image in the merged image
                    for (int c = 1; c < dataTokens.Length; ++c)
                    {
                       switch(MDDataReader.GetTokenType(dataTokens[c]))
                       {
                           case MDDataReader.TokenType.Image:
                               Bitmap split = MDDataReader.GetSplitImage(dataTokens[c], image);
                               string saveName = (string)split.Tag;

                               // If we're going to discard the file name or it wasn't given, switch to
                               // the name <name>.split<num>.png
                               // Right now we're only splitting to PNG, regardless of merge format
                               if (opts.SplitPathOpts == MDumpOptions.PathOptions.Discard
                                   || saveName == PathUtils.DiscardedFilename)
                               {
                                   saveName = splitName + '.' + SplitKeyword + splitsSaved.Count + ".png";
                               }
                               else
                               {
                                   saveName += ".png";
                               }
                               callback(SplitStage.SplittingImage, saveName);
                               // Check if we need an overwrite
                               if (File.Exists(saveName))
                               {
                                   // If the selected action is not a continuous one, show the dialog again
                                   if ((dlgOverwrite.SelectedAction & frmOverwrite.Action.ContinuingAction) == 0)
                                   {
                                       // If the user clicked abort, stop the splitting, but don't do cleanup
                                       // as when an error takes place.
                                       dlgOverwrite.Filename = saveName;
                                       if (dlgOverwrite.ShowDialog() != DialogResult.OK)
                                       {
                                           return;
                                       }
                                   }
                                   switch (dlgOverwrite.SelectedAction & frmOverwrite.Action.ActionMask)
                                   {
                                       case frmOverwrite.Action.Overwrite:
                                           File.Delete(saveName);
                                           split.Save(saveName, System.Drawing.Imaging.ImageFormat.Png);
                                           break;

                                       case frmOverwrite.Action.Rename:
                                           split.Save(PathUtils.GetRename(saveName), System.Drawing.Imaging.ImageFormat.Png);
                                           break;

                                       case frmOverwrite.Action.Skip:
                                           continue;
                                   }
                               }
                               else
                               {
                                   split.Save(saveName, System.Drawing.Imaging.ImageFormat.Png);
                               }
                               splitsSaved.Add(Directory.GetCurrentDirectory() + Path.DirectorySeparatorChar + saveName);
                               break;

                           case MDDataReader.TokenType.Directory:
                               if (opts.SplitPathOpts == MDumpOptions.PathOptions.PreservePath)
                               {
                                   string dir = splitDir + MDDataReader.GetDirectory(dataTokens[c]);
                                   if (!Directory.Exists(dir))
                                   {
                                       dirsCreated.Add(Path.GetFullPath(dir));
                                   }
                                   Directory.CreateDirectory(dir);
                                   Directory.SetCurrentDirectory(dir);
                               }
                               break;

                           default:
                               throw new ApplicationException(unexpectedToken);
                       }
                        
                    }
                }
                MessageBox.Show(successMsg + splitDir, successTitle);
            }
            catch (FormatHandlerException ex)
            {
                MessageBox.Show(ex.Message, splitFailedTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
                CleanupOnSplitFail(splitsSaved, dirsCreated);
            }
            catch(Exception ex)
            {
                ErrorHandling.LogException(ex);
                MessageBox.Show(unexpecError + ErrorHandling.ErrorMessage,
                    splitFailedTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
                CleanupOnSplitFail(splitsSaved, dirsCreated);
            }
            finally
            {
                // Set current directory back to us so that the app doesn't have a lock
                // on the output directory
                Directory.SetCurrentDirectory(PathUtils.AppDir);
                callback(SplitStage.Done, null);
            }
        }

        /// <summary>
        /// Used to delete split files created already if we fail.
        /// This way we don't have a half-baked split.
        /// </summary>
        /// <param name="splitsSaved">Split images already saved</param>
        /// <param name="dirsCreated">Image directories already created</param>
        private static void CleanupOnSplitFail(List<string> splitsSaved, List<string> dirsCreated)
        {
            foreach (string split in splitsSaved)
            {
                File.Delete(split);
            }
            foreach (string dir in dirsCreated)
            {
                Directory.Delete(dir);
            }
        }
    }
}
