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
        private const string mergeTitle = "Merging Images...";
        private const string splitTitle = "Splitting Images...";

        private frmMain.Mode mode;
        private List<Bitmap> bmpList;
        private MDumpOptions opts;
        private string path;
       
        public frmWait(frmMain.Mode mode, List<Bitmap> bmpList, MDumpOptions opts,
            string path)
        {
            InitializeComponent();
            this.mode = mode;
            this.bmpList = bmpList;
            this.opts = opts;
            this.path = path;
        }

        private void frmWait_Shown(object sender, EventArgs e)
        {
            if (mode == frmMain.Mode.Merge)
            {
                prgIndividual.Style = ProgressBarStyle.Marquee;
                Text = mergeTitle;
                opts.SetBaseDirectory(bmpList);
                ImageMerger.MergeImages(bmpList, opts, path, MergeCallback);
            }
            else
            {
                prgIndividual.Style = ProgressBarStyle.Blocks;
                Text = splitTitle;
                opts.ClearBaseDirectory();
                ImageSplitter.SplitImages(bmpList, opts, path, SplitCallback);
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
                        lblWaitStatus.Text = "Starting merge process...";
                        prgOverall.Maximum = (int)info;
                        prgOverall.Value = current;
                        break;

                    case ImageMerger.MergeStage.DeterminingNumPerMerge:
                        ImageMerger.LastAttemptInfo lastInfo = (ImageMerger.LastAttemptInfo)info;
                        string msg = "Determining number of images we can fit in one merged image...\n";
                        switch (lastInfo)
                        {
                            case ImageMerger.LastAttemptInfo.TooLarge:
                                msg += "The last attempt created a PNG too large for the maximum size set in the options.\n"
                                    + "Now trying with fewer images";
                                break;

                            case ImageMerger.LastAttemptInfo.TooSmall:
                                msg += "The last attempt created a PNG too large for the maximum size set in the options.\n"
                                    + "Now trying with more images";
                                break;
                        }
                        lblWaitStatus.Text = msg;
                        prgOverall.Value = current;
                        break;

                    case ImageMerger.MergeStage.Saving:
                        lblWaitStatus.Text = "Determined the most images that can be fit in this merged image.\n"
                                + "Now saving " + (string)info;
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
        private void SplitCallback(ImageSplitter.SplitStage stage, ImageSplitter.SplitCallbackData data)
        {
            if (InvokeRequired)
            {
                Invoke(new ImageSplitter.SplitCallback(SplitCallback), new object[] { stage, data });
            }
            else
            {
                switch (stage)
                {
                    case ImageSplitter.SplitStage.Done:
                        Close();
                        break;
                }
            }
        }
    }
}
