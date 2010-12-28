using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace MDump
{
    /// <summary>
    /// Form to modify program options
    /// </summary>
    public partial class frmOptions : Form
    {
        private const string validMessage = " is an existing directory.";
        private static readonly Color validColor = Color.Green;
        private const string invalidMessage = " is not an existing directory.";
        private static readonly Color invalidColor = Color.Red;

        /// <summary>
        /// Construct the form using default options
        /// </summary>
        public frmOptions() : this(new MDumpOptions())
        { }

        /// <summary>
        /// Construct this form using provided options
        /// </summary>
        /// <param name="opts">Options with which to initialize the form</param>
        public frmOptions(MDumpOptions opts)
        {
            InitializeComponent();
            Setup(opts);
        }

        /// <summary>
        /// Set this form using provided options.  Called by the constructor
        /// and the "reset to defaults" button
        /// </summary>
        /// <param name="opts">Options with which to initialize the form</param>
        private void Setup(MDumpOptions opts)
        {
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
            nudMaxMergeSize.Value = Convert.ToDecimal(opts.MaxMergeSize / 1024);
            chkAddTitleBar.Checked = opts.AddTitleBar;
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

        /// <summary>
        /// Used to retrieve the options the user configured in this form.
        /// </summary>
        /// <returns>The options represented by the form controls</returns>
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
            opts.MaxMergeSize = Convert.ToInt32(nudMaxMergeSize.Value) * 1024;
            opts.AddTitleBar = chkAddTitleBar.Checked;
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

        private void btnDefaults_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Are you sure you want to reset all options to their default values?",
                "Reset to defaults?", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2)
                == System.Windows.Forms.DialogResult.Yes)
            {
                Setup(new MDumpOptions());
            }
        }
    }
}
