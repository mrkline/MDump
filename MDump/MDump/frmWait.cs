using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Threading;

namespace MDump
{
    public partial class frmWait : Form
    {
        #region String Constants
        private const string mergeTitle = "Merging Images...";
        private const string splitTitle = "Splitting Images...";
        private const string startingMerge = "Starting merge process...";
        private const string determiningImgNum = "Determining number of images we can fit in one merged image...\n\n";
        private const string wasTooLarge = "The last attempt created an image too large for the maximum size set in the options.\n"
                                    + "Now trying with fewer images";
        private const string wasTooSmall = "The last attempt created an image too large for the maximum size set in the options.\n"
                                    + "Now trying with more images";
        private const string nowSaving = "Determined the most images that can be fit in this merged image.\n"
                                + "Now saving ";
        #endregion

        private frmMain.Mode mode;
        private List<Bitmap> bmpList;
        private string path;

        private string currentMerge;
       
        public frmWait(frmMain.Mode mode, List<Bitmap> bmpList, string path)
        {
            InitializeComponent();
            this.mode = mode;
            this.bmpList = bmpList;
            this.path = path;
        }

        private void frmWait_Shown(object sender, EventArgs e)
        {
            if (mode == frmMain.Mode.Merge)
            {
                Text = mergeTitle;
                ImageMerger.MergeImages(bmpList, path, MergeCallback);
            }
            else
            {
                Text = splitTitle;
                ImageSplitter.SplitImages(bmpList, path, SplitCallback);
            }
        }

        /// <summary>
        /// Used to invoke a close() call to the form when done from worker thread
        /// </summary>
        private delegate void CloseCallback();
        /// <summary>
        /// Used to invoke a text change from the worker thread
        /// </summary>
        /// <param name="text">lblWaitStatus.Text will be set to this</param>
        private delegate void SetTextCallback(string text);

        private delegate void SetProgressCallback(ProgressBar bar, int value);

        /// <summary>
        /// Used to invoke a text change from the worker thread
        /// </summary>
        /// <param name="text">lblWaitStatus.Text will be set to this</param>
        private void SetLabelText(string text)
        {
            lblWaitStatus.Text = text;
        }

        private void SetMax(ProgressBar bar, int max)
        {
            bar.Maximum = max;
        }

        private void SetProgress(ProgressBar bar, int prog)
        {
            bar.Value = prog;
        }

        /// <summary>
        /// Keeps the user updated to the actions of the merge thread
        /// </summary>
        /// <param name="currStage">current stage in the merge process</param>
        /// <param name="current">current number of images merged</param>
        /// <param name="info">Stage-specific additional info</param>
        private void MergeCallback(ImageMerger.MergeStage currStage, int current, object info)
        {
            if(InvokeRequired)
            {
                Invoke(new ImageMerger.MergeCallback(MergeCallback),
                    new object[] { currStage, current, info });
            }
            else
            {
                switch (currStage)
                {
                    case ImageMerger.MergeStage.Starting:
                        lblWaitStatus.Text = startingMerge;
                        prgOverall.Maximum = (int)info;
                        prgOverall.Value = current;
                        break;

                    case ImageMerger.MergeStage.DeterminingNumPerMerge:
                        ImageMerger.LastAttemptInfo lastInfo = (ImageMerger.LastAttemptInfo)info;
                        string msg = determiningImgNum;
                        switch (lastInfo)
                        {
                            case ImageMerger.LastAttemptInfo.TooLarge:
                                msg += wasTooLarge;
                                break;

                            case ImageMerger.LastAttemptInfo.TooSmall:
                                msg += wasTooSmall;
                                break;
                        }
                        lblWaitStatus.Text = msg;
                        prgOverall.Value = current;
                        break;

                    case ImageMerger.MergeStage.Saving:
                        lblWaitStatus.Text = nowSaving + (string)info;
                        prgOverall.Value = current;
                        break;

                    case ImageMerger.MergeStage.Done:
                        Close();
                        break;
                }
            }
        }

        /// <summary>
        /// Keeps the user updated to the actions of the split thread
        /// </summary>
        /// <param name="stage">Current stage of split procedure</param>
        /// <param name="data">Has different menaing for each stage</param>
        private void SplitCallback(ImageSplitter.SplitStage stage, string data)
        {
            if (InvokeRequired)
            {
                Invoke(new ImageSplitter.SplitCallback(SplitCallback), new object[] { stage, data });
            }
            else
            {
                switch (stage)
                {
                    case ImageSplitter.SplitStage.Starting:
                        lblWaitStatus.Text = "Starting the split operation.";
                        break;

                    case ImageSplitter.SplitStage.SplittingNewMerge:
                        currentMerge = data;
                        lblWaitStatus.Text = "Splitting " + data + "...\n";
                        break;

                    case ImageSplitter.SplitStage.SplittingImage:
                        lblWaitStatus.Text = "Splitting " + data
                            + " from " + currentMerge;
                        break;

                    case ImageSplitter.SplitStage.Done:
                        Close();
                        break;
                }
            }
        }
    }
}
