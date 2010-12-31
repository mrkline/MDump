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
                Text = mergeTitle;
                opts.SetBaseDirectory(bmpList);
                ImageMerger.MergeImages(bmpList, opts, path, MergeCallback);
            }
            else
            {
                opts.BaseDirectory = string.Empty;
                Text = splitTitle;
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

        /// <summary>
        /// Used to invoke a text change from the worker thread
        /// </summary>
        /// <param name="text">lblWaitStatus.Text will be set to this</param>
        private void SetLabelText(string text)
        {
            lblWaitStatus.Text = text;
        }

        /// <summary>
        /// Keeps the user updated to the actions of the worker thread
        /// </summary>
        /// <param name="currStage">Current stage in the merge process</param>
        /// <param name="info">Additional stage-specific info</param>
        private void MergeCallback(ImageMerger.MergeCallbackStage currStage, object info)
        {
            switch (currStage)
            {
                case ImageMerger.MergeCallbackStage.DeterminingNumPerMerge:
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
                    Invoke(new SetTextCallback(SetLabelText),
                        new object[] { msg });
                    break;

                case ImageMerger.MergeCallbackStage.Saving:
                    Invoke(new SetTextCallback(SetLabelText),
                        new object[] { "Determined the most images that can be fit in this merged image.\n"
                            + "Now saving " + (string)info });
                    break;

                case ImageMerger.MergeCallbackStage.Done:
                    Invoke(new CloseCallback(Close));
                    break;
            }
        }
    }
}
