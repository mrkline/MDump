using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace MDump
{
    public partial class frmOptions : Form
    {
        public frmOptions() : this(new MDumpOptions())
        { }

        public frmOptions(MDumpOptions opts)
        {
            InitializeComponent();
            switch (opts.MergePathOpts)
            {
                case MDumpOptions.PathOptions.PreservePath:
                    radSaveFilePath.Checked = true;
                    break;

                case MDumpOptions.PathOptions.PreserveName:
                    radSaveFilenames.Checked = true;
                    break;

                case MDumpOptions.PathOptions.Discard:
                    radDiscardFilenames.Checked = true;
                    break;
            }
            nudMaxMergeSize.Value = Convert.ToDecimal(opts.MaxMergeSize);
            if (opts.PromptForSplitDestination)
            {
                radSplitAsk.Checked = true;
            }
            else
            {
                radSplitToFolder.Enabled = true;
            }
            txtSplitToFolder.Text = opts.SplitDestination;
            txtSplitToFolder.Enabled = btnSplitBrowse.Enabled = !opts.PromptForSplitDestination;
            switch (opts.SplitPathOpts)
            {
                case MDumpOptions.PathOptions.PreservePath:
                    radUsePathInfo.Checked = true;
                    break;

                case MDumpOptions.PathOptions.PreserveName:
                    radUseFilename.Checked = true;
                    break;

                case MDumpOptions.PathOptions.Discard:
                    radIgnore.Checked = true;
                    break;
            }
        }

        public MDumpOptions GetOptions()
        {
            MDumpOptions opts = new MDumpOptions();
            if(radSaveFilePath.Checked)
            {
                opts.MergePathOpts = MDumpOptions.PathOptions.PreservePath;
            }
            else if(radSaveFilenames.Checked)
            {
                opts.MergePathOpts = MDumpOptions.PathOptions.PreserveName;
            }
            else
            {
                opts.MergePathOpts = MDumpOptions.PathOptions.Discard;
            }
            if (radUsePathInfo.Checked)
            {
                opts.SplitPathOpts = MDumpOptions.PathOptions.PreservePath;
            }
            else if (radUseFilename.Checked)
            {
                opts.SplitPathOpts = MDumpOptions.PathOptions.PreserveName;
            }
            else
            {
                opts.SplitPathOpts = MDumpOptions.PathOptions.Discard;
            }
            opts.MaxMergeSize = Convert.ToUInt32(nudMaxMergeSize.Value);
            opts.PromptForSplitDestination = radSplitAsk.Enabled;
            opts.SplitDestination = txtSplitToFolder.Text;
            return opts;
        }

        private void btnCancelOptions_Click(object sender, EventArgs e)
        {
            DialogResult = System.Windows.Forms.DialogResult.Cancel;
            Close();
        }

        private void btnSaveOptions_Click(object sender, EventArgs e)
        {
            DialogResult = System.Windows.Forms.DialogResult.OK;
            Close();
        }

        private void btnSplitBrowse_Click(object sender, EventArgs e)
        {
            if (dlgFolderBrowse.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                txtSplitToFolder.Text = dlgFolderBrowse.SelectedPath;
            }
        }

        private void txtSplitToFolder_Leave(object sender, EventArgs e)
        {
            if (!txtSplitToFolder.Text.Equals(string.Empty) 
                && !System.IO.Directory.Exists(txtSplitToFolder.Text))
            {
                MessageBox.Show(txtSplitToFolder.Text + " is not a valid folder", "Invalid Path",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                txtSplitToFolder.Focus();
                txtSplitToFolder.SelectAll();
            }
        }

        private void radSplitToFolder_CheckedChanged(object sender, EventArgs e)
        {
            txtSplitToFolder.Enabled = btnSplitBrowse.Enabled = radSplitToFolder.Checked;
        }
    }
}
