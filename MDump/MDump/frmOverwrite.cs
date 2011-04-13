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
        public enum Action
        {
            /// <summary>
            /// The user did not take an action yet. (default value)
            /// </summary>
            None,
            /// <summary>
            /// Overwrite the old file with the new one.
            /// </summary>
            Overwrite,
            /// <summary>
            /// Overwrite the old file and do so for all conflicts.
            /// </summary>
            OverwriteAll,
            /// <summary>
            /// Rename the new file.
            /// </summary>
            Rename,
            /// <summary>
            /// Rename the new file and do so for all conflicts.
            /// </summary>
            RenameAll,
            /// <summary>
            /// Do not save the new file.
            /// </summary>
            Skip,
            /// <summary>
            /// Do not save the new file and do so for all conflicts.
            /// </summary>
            SkipAll
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

        private void frmOverwrite_Load(object sender, EventArgs e)
        {
            DialogResult = System.Windows.Forms.DialogResult.Abort;
            SelectedAction = Action.None;
        }

        private void btnOverwrite_Click(object sender, EventArgs e)
        {
            DialogResult = System.Windows.Forms.DialogResult.OK;
            SelectedAction = Action.Overwrite;
            Close();
        }

        private void btnOverwriteAll_Click(object sender, EventArgs e)
        {
            DialogResult = System.Windows.Forms.DialogResult.OK;
            SelectedAction = Action.OverwriteAll;
            Close();
        }

        private void btnRename_Click(object sender, EventArgs e)
        {
            DialogResult = System.Windows.Forms.DialogResult.OK;
            SelectedAction = Action.Rename;
            Close();
        }

        private void btnRenameAll_Click(object sender, EventArgs e)
        {
            DialogResult = System.Windows.Forms.DialogResult.OK;
            SelectedAction = Action.RenameAll;
            Close();
        }

        private void btnSkip_Click(object sender, EventArgs e)
        {
            DialogResult = System.Windows.Forms.DialogResult.OK;
            SelectedAction = Action.Skip;
            Close();
        }

        private void btnSkipAll_Click(object sender, EventArgs e)
        {
            DialogResult = System.Windows.Forms.DialogResult.OK;
            SelectedAction = Action.SkipAll;
            Close();
        }

        private void btnAbort_Click(object sender, EventArgs e)
        {
            SelectedAction = Action.None;
            DialogResult = System.Windows.Forms.DialogResult.Abort;
            Close();
        }
    }
}
