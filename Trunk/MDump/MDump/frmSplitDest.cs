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
        private const string ignoreInfoLabel = "Select a name to save all split images as:";
        private const string useInfoLabel = "Select a name to use for any merges that didn't save file info:";
        private const string overwriteFilenameStatus = "Split files with this name already exist in this folder. They will be overwritten.";
        private const string hasExtensionFilenameStatus = "Do not add an extension to the file name.  It will be done automatically";
        private const string invalidFilenameStatus = "This is not a valid file name.";

        private readonly Color validColor = Color.Green;
        private readonly Color warningColor = Color.Goldenrod;
        private readonly Color invalidColor = Color.Red;
        private readonly Color validBGColor = Color.LightGreen;
        private readonly Color warningBGColor = Color.Yellow;
        private readonly Color invalidBGColor = Color.PaleVioletRed;
        private readonly MDumpOptions opts;

        public frmSplitDest(MDumpOptions options)
        {
            opts = options;
            InitializeComponent();
            lblDirStatus.ForeColor = invalidColor;
        }

        public string SplitPath
        {
            get
            {
                if (txtDir.Text.EndsWith(Path.DirectorySeparatorChar.ToString())
                    || txtDir.Text.EndsWith(Path.AltDirectorySeparatorChar.ToString()))
                {
                    return txtDir.Text + txtFilename.Text;
                }
                else
                {
                    return txtDir.Text + Path.DirectorySeparatorChar + txtFilename.Text;
                }
            }
        }

        private void frmSplitDest_Load(object sender, EventArgs e)
        {
            lblSelectFilename.Text = string.Empty;
            lblSelectFilename.BackColor = DefaultBackColor;
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
                txtDir.BackColor = DefaultBackColor;
            }
            else if (Directory.Exists(txtDir.Text))
            {
                lblDirStatus.Visible = false;
                txtDir.BackColor = validBGColor;
            }
            else
            {
                lblDirStatus.Visible = true;
                txtDir.BackColor = invalidBGColor;
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
                txtFilename.BackColor = DefaultBackColor;
                btnOK.Enabled = false;
            }
            //File name has invalid characters
            else if (txtFilename.Text.IndexOfAny(Path.GetInvalidFileNameChars()) != -1)
            {
                lblFilenameStatus.ForeColor = invalidColor;
                lblFilenameStatus.Text = invalidFilenameStatus;
                lblFilenameStatus.Visible = true;
                txtFilename.BackColor = invalidBGColor;
                btnOK.Enabled = false;
            }
            //Filename has extension (which we don't want them to do)
            else if (txtFilename.Text.IndexOf('.') != -1)
            {
                lblFilenameStatus.ForeColor = invalidColor;
                lblFilenameStatus.Text = hasExtensionFilenameStatus;
                lblFilenameStatus.Visible = true;
                txtFilename.BackColor = invalidBGColor;
                btnOK.Enabled = false;
            }
            //File is valid and directory is nonexistant.  This is a valid case.
            else if (txtDir.Text.Length == 0)
            {
                lblFilenameStatus.Visible = false;
                txtFilename.BackColor = validBGColor;
                btnOK.Enabled = true;
            }
            //Check if directory is valid and check if an overwrite would be needed
            else if (Directory.Exists(txtDir.Text))
            {
                //Get all files in the directory that start with the name provided
                string[] dirFiles = Directory.GetFiles(txtDir.Text);

                //Get requested filename
                string fn = SplitPath;
                
                //Check if we need an overwrite
                bool overwriteRequired = false;
                foreach (string file in dirFiles)
                {
                    //The name format of merges is <name>.split<num>.png
                    string test = Path.GetFileName(file);
                    string[] tokens = Path.GetFileName(file).Split('.');
                    if (tokens.Length == 4
                        && tokens[0].Equals(txtFilename.Text, StringComparison.InvariantCultureIgnoreCase)
                        && tokens[1].StartsWith(ImageSplitter.SplitKeyword)
                        && IsInt(tokens[1].Substring(ImageSplitter.SplitKeyword.Length))
                        && tokens[2].Equals("png", StringComparison.InvariantCultureIgnoreCase))
                    {
                        overwriteRequired = true;
                        break;
                    }
                }
                //We'd overwrite files
                if (overwriteRequired)
                {
                    lblFilenameStatus.ForeColor = warningColor;
                    lblFilenameStatus.Text = overwriteFilenameStatus;
                    lblFilenameStatus.Visible = true;
                    txtFilename.BackColor = warningBGColor;
                    btnOK.Enabled = false;
                }
                //Filename is good to go
                else
                {
                    lblFilenameStatus.Visible = false;
                    txtFilename.BackColor = validBGColor;
                    btnOK.Enabled = true;
                }
            }
            //Filename is good to go, but directory isn't.
            else
            {
                lblFilenameStatus.Visible = false;
                txtFilename.BackColor = validBGColor;
                btnOK.Enabled = false;
            }
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            //TODO: Overwrite code here

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
