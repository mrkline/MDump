﻿using System;
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
    /// Form to view and modify program options
    /// </summary>
    public partial class frmOptions : Form
    {
        private const string validMessage = " is an existing directory.";
        private static readonly Color validColor = Color.Green;
        private const string invalidMessage = " is not an existing directory.";
        private static readonly Color invalidColor = Color.Red;

        private const int kBytesPerKB = 1024;

        /// <summary>
        /// Construct the form using default options
        /// </summary>
        public frmOptions()
            : this(new MDumpOptions())
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
            //Build the combo box, picking out the selected item
            int idx = 0;
            bool idxFound = false;
            foreach (string str in MasterFormatHandler.Instance.SupportedFormatNames)
            {
                cmbFormat.Items.Add(str);

                if (str == opts.MergeFormat)
                {
                    idxFound = true;
                }

                if (!idxFound)
                {
                    ++idx;
                }
            }
            cmbFormat.SelectedIndex = idx;
            trkCompression.Maximum = Enum.GetValues(typeof(MDumpOptions.CompressionLevel)).Length - 1;
            trkCompression.Value = (int)opts.CompLevel;
            nudMaxMergeSize.Value = Convert.ToDecimal(opts.MaxMergeSize / kBytesPerKB);
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
        /// 
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
            opts.MergeFormat = cmbFormat.Text;
            opts.CompLevel = (MDumpOptions.CompressionLevel)trkCompression.Value;
            opts.MaxMergeSize = Convert.ToInt32(nudMaxMergeSize.Value) * kBytesPerKB;
            opts.AddTitleBar = chkAddTitleBar.Checked;
            return opts;
        }

        private void btnCancelOptions_Click(object sender, EventArgs e)
        {
            DialogResult = System.Windows.Forms.DialogResult.Cancel;
        }

        private void btnSaveOptions_Click(object sender, EventArgs e)
        {
            DialogResult = System.Windows.Forms.DialogResult.OK;
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

        private void ttpExplanations_Popup(object sender, PopupEventArgs e)
        {
        }
    }
}
