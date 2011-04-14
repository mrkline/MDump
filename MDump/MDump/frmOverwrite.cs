using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace MDump
{
    /// <summary>
    /// Dialog used for resolving filename conflicts.
    /// The dialog returns DialogResult.OK unless they click
    /// abort or close the dialog another way.
    /// </summary>
    public partial class frmOverwrite : Form
    {
        /// <summary>
        /// Possible actions the user can take to resolve conflicts
        /// </summary>
        [Flags]
        public enum Action :  ushort
        {
            /// <summary>
            /// The user did not take an action yet. (default value)
            /// </summary>
            None = 0,
            /// <summary>
            /// Overwrite the old file with the new one.
            /// </summary>
            Overwrite = 1,
            /// <summary>
            /// Rename the new file.
            /// </summary>
            Rename = 2,
            /// Do not save the new file.
            /// </summary>
            Skip = 3,
            /// <summary>
            /// Bit for when actions should be applied to a single item
            /// </summary>
            SingleAction = 1 << 14,
            /// <summary>
            /// Bit for when actions should be applied to all subsequent items
            /// </summary>
            ContinuingAction = 1 << 15,
            /// <summary>
            /// Mask for bits indicating the action taken
            /// </summary>
            ActionMask = 3,
            /// <summary>
            /// Mask for bits indicating the length of the action
            /// </summary>
            DurationMask = 0xC000
        }

        /// <summary>
        /// Gets the action the user last selected to resolve the filename conflict
        /// </summary>
        public Action SelectedAction { get; private set; }
        /// <summary>
        /// Gets or sets the name of the file in conflict
        /// </summary>
        public String Filename { get; set; }

        public frmOverwrite()
        {
            SelectedAction = Action.None;
            InitializeComponent();
        }

        private void btnOverwrite_Click(object sender, EventArgs e)
        {
            DialogResult = System.Windows.Forms.DialogResult.OK;
            SelectedAction = Action.Overwrite | Action.SingleAction;
        }

        private void btnOverwriteAll_Click(object sender, EventArgs e)
        {
            DialogResult = System.Windows.Forms.DialogResult.OK;
            SelectedAction = Action.Overwrite | Action.ContinuingAction;
        }

        private void btnRename_Click(object sender, EventArgs e)
        {
            DialogResult = System.Windows.Forms.DialogResult.OK;
            SelectedAction = Action.Rename | Action.SingleAction;
        }

        private void btnRenameAll_Click(object sender, EventArgs e)
        {
            DialogResult = System.Windows.Forms.DialogResult.OK;
            SelectedAction = Action.Rename | Action.ContinuingAction;
        }

        private void btnSkip_Click(object sender, EventArgs e)
        {
            DialogResult = System.Windows.Forms.DialogResult.OK;
            SelectedAction = Action.Skip | Action.SingleAction;
        }

        private void btnSkipAll_Click(object sender, EventArgs e)
        {
            DialogResult = System.Windows.Forms.DialogResult.OK;
            SelectedAction = Action.Skip | Action.ContinuingAction;
        }

        private void btnAbort_Click(object sender, EventArgs e)
        {
            SelectedAction = Action.None;
            DialogResult = System.Windows.Forms.DialogResult.Abort;
        }

        private void frmOverwrite_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                SelectedAction = Action.None;
                DialogResult = System.Windows.Forms.DialogResult.Abort;
            }
        }

        private void frmOverwrite_Load(object sender, EventArgs e)
        {
            lblFilename.Text = Filename;
        }
    }
}
