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
    /// Form to prompt the user on the location to which files should be split.
    /// </summary>
    public partial class frmSplitDest : Form
    {
        #region String Constants
        private const string ignoreInfoLabel = "Select a name to save all split images as:";
        private const string useInfoLabel = "Select a name to use for any merges that didn't save file info:";
        private const string hasExtensionFilenameStatus = "Do not add an extension to the file name.\nIt will be done automatically";
        private const string invalidFilenameStatus = "This is not a valid file name.";
        #endregion

        private readonly Color defaultTextBackColor;

        public frmSplitDest()
        {
            InitializeComponent();
            lblInvalidDir.ForeColor = Colors.InvalidColor;
            defaultTextBackColor = txtFilename.BackColor;
            //Set current directory to the application directory so relative paths
            //work as expected
            Directory.SetCurrentDirectory(PathUtils.AppDir);
        }

        public string SplitDir
        {
            get
            {
                if (txtDir.Text.Length == 0)
                {
                    return Directory.GetCurrentDirectory();
                }
                else
                {
                    return Path.GetFullPath(txtDir.Text);
                }
            }
        }

        public string SplitPath
        {
            get
            {
                if (txtDir.Text.EndsWith(Path.DirectorySeparatorChar.ToString())
                    || txtDir.Text.EndsWith(Path.AltDirectorySeparatorChar.ToString()))
                {
                    return SplitDir + txtFilename.Text;
                }
                else
                {
                    return SplitDir + Path.DirectorySeparatorChar + txtFilename.Text;
                }
            }
        }

        private void frmSplitDest_Load(object sender, EventArgs e)
        {
            txtFilename.Text = string.Empty;
            if (MDumpOptions.Instance.SplitPathOpts == MDumpOptions.PathOptions.Discard)
            {
                lblSelectFilename.Text = ignoreInfoLabel;
            }
            else
            {
                lblSelectFilename.Text = useInfoLabel;
            }
            // Necessary in case the previously selected directory was deleted
            UpdateFilenameAndOKStatus();
        }

        private void btnBrowse_Click(object sender, EventArgs e)
        {
            if (dlgSplitDir.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                txtDir.Text = dlgSplitDir.SelectedPath;
            }
        }

        private void txtDir_TextChanged(object sender, EventArgs e)
        {
            UpdateFilenameAndOKStatus();
        }

        private void txtFilename_TextChanged(object sender, EventArgs e)
        {
            UpdateFilenameAndOKStatus();     
        }

        private void UpdateFilenameAndOKStatus()
        {
            bool filenameOkay, dirOkay;

            //Text length is zero, so don't judge it yet.
            if(txtFilename.Text.Length == 0)
            {
                lblFilenameStatus.Visible = false;
                txtFilename.BackColor = defaultTextBackColor;
                filenameOkay = false;
            }
            //File name has invalid characters
            else if (txtFilename.Text.IndexOfAny(Path.GetInvalidFileNameChars()) != -1)
            {
                lblFilenameStatus.ForeColor = Colors.InvalidColor;
                lblFilenameStatus.Text = invalidFilenameStatus;
                lblFilenameStatus.Visible = true;
                txtFilename.BackColor = Colors.InvalidBGColor;
                filenameOkay = false;
            }
            //Filename has extension (which we don't want them to do)
            else if (txtFilename.Text.IndexOf('.') != -1)
            {
                lblFilenameStatus.ForeColor = Colors.InvalidColor;
                lblFilenameStatus.Text = hasExtensionFilenameStatus;
                lblFilenameStatus.Visible = true;
                txtFilename.BackColor = Colors.InvalidBGColor;
                filenameOkay = false;
            }
            else
            {
                lblFilenameStatus.Visible = false;
                txtFilename.BackColor = Colors.ValidBGColor;
                filenameOkay = true;
            }

            //Check if directory is valid
            if (txtDir.Text.Length == 0 || Directory.Exists(txtDir.Text))
            {
                dirOkay = true;
                lblInvalidDir.Visible = false;
                txtDir.BackColor = txtDir.Text.Length > 0 ? Colors.ValidBGColor : defaultTextBackColor;
            }
            // Directory isn't good to go
            else
            {
                txtDir.BackColor = Colors.InvalidBGColor;
                lblInvalidDir.Visible = true;
                dirOkay = false;
            }

            btnOK.Enabled = filenameOkay && dirOkay;
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            DialogResult = System.Windows.Forms.DialogResult.OK;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = System.Windows.Forms.DialogResult.Cancel;
        }

        private static bool IsInt(string str)
        {
            try
            {
                Convert.ToInt32(str);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
