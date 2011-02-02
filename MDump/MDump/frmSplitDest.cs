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
    public partial class frmSplitDest : Form
    {
        #region String Constants
        private const string ignoreInfoLabel = "Select a name to save all split images as:";
        private const string useInfoLabel = "Select a name to use for any merges that didn't save file info:";
        private const string invalidDirStatus = "This is not an existing folder";
        private const string noRelativeDirsStatus = "Relative directories are not allowed";
        private const string overwriteFilenameStatus = "Split files with this name already exist in this folder.\nThey will be overwritten.";
        private const string hasExtensionFilenameStatus = "Do not add an extension to the file name.\nIt will be done automatically";
        private const string invalidFilenameStatus = "This is not a valid file name.";
        #endregion

        private readonly Color defaultTextBackColor;
        private readonly MDumpOptions opts;

        public frmSplitDest(MDumpOptions options)
        {
            opts = options;
            InitializeComponent();
            lblDirStatus.ForeColor = Colors.InvalidColor;
            defaultTextBackColor = txtFilename.BackColor;
        }

        public string SplitDir
        {
            get
            {
                if (txtDir.Text.Length == 0)
                {
                    return Path.GetDirectoryName(Application.ExecutablePath);
                }
                else
                {
                    return txtDir.Text;
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
            txtFilename.BackColor = defaultTextBackColor;
            btnOK.Enabled = false;
            if (opts.SplitPathOpts == MDumpOptions.PathOptions.Discard)
            {
                lblSelectFilename.Text = ignoreInfoLabel;
            }
            else
            {
                lblSelectFilename.Text = useInfoLabel;
            }
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
            if (txtDir.Text.Length == 0)
            {
                lblDirStatus.Visible = false;
                txtDir.BackColor = defaultTextBackColor;
            }
            else if (!Directory.Exists(txtDir.Text))
            {
                lblDirStatus.Text = invalidDirStatus;
                lblDirStatus.Visible = true;
                txtDir.BackColor = Colors.InvalidColor;
            }
            //Relative paths are not allowed
            else if (txtDir.Text.Contains(".."))
            {
                lblDirStatus.Text = noRelativeDirsStatus;
                lblDirStatus.Visible = true;
                txtDir.BackColor = Colors.InvalidBGColor;
            }
            else
            {
                lblDirStatus.Visible = false;
                txtDir.BackColor = Colors.ValidBGColor;
            }
            UpdateFilenameAndOKStatus();
        }

        private void txtFilename_TextChanged(object sender, EventArgs e)
        {
            UpdateFilenameAndOKStatus();     
        }

        private void UpdateFilenameAndOKStatus()
        {
            //Text length is zero, so don't judge it yet.
            if(txtFilename.Text.Length == 0)
            {
                lblFilenameStatus.Visible = false;
                txtFilename.BackColor = defaultTextBackColor;
                btnOK.Enabled = false;
            }
            //File name has invalid characters
            else if (txtFilename.Text.IndexOfAny(Path.GetInvalidFileNameChars()) != -1)
            {
                lblFilenameStatus.ForeColor = Colors.InvalidColor;
                lblFilenameStatus.Text = invalidFilenameStatus;
                lblFilenameStatus.Visible = true;
                txtFilename.BackColor = Colors.InvalidBGColor;
                btnOK.Enabled = false;
            }
            //Filename has extension (which we don't want them to do)
            else if (txtFilename.Text.IndexOf('.') != -1)
            {
                lblFilenameStatus.ForeColor = Colors.InvalidColor;
                lblFilenameStatus.Text = hasExtensionFilenameStatus;
                lblFilenameStatus.Visible = true;
                txtFilename.BackColor = Colors.InvalidBGColor;
                btnOK.Enabled = false;
            }
            //File is valid and directory is nonexistant.  This is a valid case.
            else if (txtDir.Text.Length == 0)
            {
                lblFilenameStatus.Visible = false;
                txtFilename.BackColor = Colors.ValidBGColor;
                btnOK.Enabled = true;
            }
            //Check if directory is valid and check if an overwrite would be needed
            else if (Directory.Exists(txtDir.Text) && !txtDir.Text.Contains(".."))
            {
                //Get all images in the directory that start with the name provided
                string[] dirFiles = Directory.GetFiles(SplitDir);

                //Get requested filename
                string fn = SplitPath;
                
                //Check if we need an overwrite
                bool overwriteRequired = false;
                foreach (string file in dirFiles)
                {
                    //The name format of splits is <name>.split<num>.png
                    string test = Path.GetFileName(file);
                    string[] tokens = Path.GetFileName(file).Split('.');
                    if (tokens.Length == 3
                        && tokens[0].Equals(txtFilename.Text, StringComparison.InvariantCultureIgnoreCase)
                        && tokens[1].StartsWith(ImageSplitter.SplitKeyword)
                        && IsInt(tokens[1].Substring(ImageSplitter.SplitKeyword.Length))
                        && tokens[2].Equals("png", StringComparison.InvariantCultureIgnoreCase))
                    {
                        overwriteRequired = true;
                        break;
                    }
                }
                //We'd overwrite images
                if (overwriteRequired)
                {
                    lblFilenameStatus.ForeColor = Colors.WarningColor;
                    lblFilenameStatus.Text = overwriteFilenameStatus;
                    lblFilenameStatus.Visible = true;
                    txtFilename.BackColor = Colors.WarningBGColor;
                    btnOK.Enabled = true;
                }
                //Filename is good to go
                else
                {
                    lblFilenameStatus.Visible = false;
                    txtFilename.BackColor = Colors.ValidBGColor;
                    btnOK.Enabled = true;
                }
            }
            //Filename is good to go, but directory isn't.
            else
            {
                lblFilenameStatus.Visible = false;
                txtFilename.BackColor = Colors.ValidBGColor;
                btnOK.Enabled = false;
            }
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            //Overwrite any needed images
            //Get all images in the directory that start with the name provided
            string[] dirFiles = Directory.GetFiles(SplitDir);

            //Get requested filename
            string fn = SplitPath;
                
            //Check if we need an overwrite
            List<string> filesToOverwrite = new List<string>();
            foreach (string file in dirFiles)
            {
                //The name format of merges is <name>.split<num>.png
                string test = Path.GetFileName(file);
                string[] tokens = Path.GetFileName(file).Split('.');
                if (tokens.Length == 3
                    && tokens[0].Equals(txtFilename.Text, StringComparison.InvariantCultureIgnoreCase)
                    && tokens[1].StartsWith(ImageSplitter.SplitKeyword)
                    && IsInt(tokens[1].Substring(ImageSplitter.SplitKeyword.Length))
                    && tokens[2].Equals("png", StringComparison.InvariantCultureIgnoreCase))
                {
                    filesToOverwrite.Add(file);
                }
            }
            foreach (string file in filesToOverwrite)
            {
                File.Delete(file);
            }

            DialogResult = System.Windows.Forms.DialogResult.OK;
            Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = System.Windows.Forms.DialogResult.Cancel;
            Close();
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
