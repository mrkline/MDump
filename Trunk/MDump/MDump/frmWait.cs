using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace MDump
{
    public partial class frmWait : Form
    {
        private frmMain.Mode mode;
        private IEnumerable<Bitmap> bmpList;
        private MDumpOptions opts;
       
        public frmWait(frmMain.Mode mode, IEnumerable<Bitmap> bmpList, MDumpOptions opts)
        {
            InitializeComponent();
            this.mode = mode;
            this.bmpList = bmpList;
            this.opts = opts;
        }

        private void frmWait_Shown(object sender, EventArgs e)
        {
            if (mode == frmMain.Mode.Merge)
            {
                Text = "Merging Images...";
                ImageMerger.MergeImages(bmpList, opts);
            }
            else
            {
                Text = "Splitting Images...";
            }
            Close();
        }
    }
}
