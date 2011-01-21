using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.IO;
using System.Windows.Forms;

namespace MDump
{
    public partial class frmMain : Form
    {
        #region String Constants
        private const string notSetActionText = "Split/Merge Images";
        private const string notSetActionTooltip = "This button will split or merge images"
                + " based on what is added to the above list.  Add merged images to split them"
                + " or individual images to merge them.";
        private const string mergeActionText = "Merge Images";
        private const string mergeActionTooltip = "Merge the images in the list above in to"
                            + " a smaller set of images to be uploaded to an image board.";
        private const string splitActionText = "Split images";
        private const string splitActionTooltip = "Split the merged images in the list above"
                            + " back into the original images.";
        private const string incorrectModeMsg = "MDump cannot split merged images at the same time it"
                        + " merges individual images.  Please add either inidividual images to merge"
                        + " or MDump merged images to split back up, but not both at once.";
        private const string incorrectModeTitle = "Cannot split and merge at the same time";
        private const string duplicateInDirTitle = "Duplicate in folder";
        private const string errorLoadingImageMsg = " could not be loaded.  It will not be added to the list.";
        private const string errorLoadingImageTitle = "Problem loading image";
        private const string dllNotFoundMsg = PNGOps.DllName + " could not be found."
                    + " Make sure it is in the same folder as this program.";
        private const string dllNotFoundTitle = "Couldn't find DLL";

        private const string noSuchPathMsg = "The path entered does not exist.";
        private const string noSuchPathTitle = "No such path";
        #endregion

        private readonly string[] supportedImageFormats = { "bmp", "gif", "exif", "jpg",
                                                              "jpeg", "png", "tif", "tiff" };

        /// <summary>
        /// Used to track the current mode of the app (not set, merging images, splitting images)
        /// </summary>
        public enum Mode
        {
            NotSet,
            Merge,
            Split
        }

        private ImageDirectoryManager dirMan;

        /// <summary>
        /// Gets or sets the current options object
        /// </summary>
        private MDumpOptions Opts { get; set; }
        private frmSplitDest dlgSplitDest;

        Mode __currentMode;
        /// <summary>
        /// Gets or sets the current mode (not set, merging images, splitting images).
        /// On set, update the UI to reflect the change.
        /// </summary>
        private Mode CurrentMode
        {
            get
            {
                return __currentMode;
            }
            set
            {
                __currentMode = value;
                switch (value)
                {
                    case Mode.NotSet:
                        //Disable merge directory fun
                        DirectoryUIEnabled = false;
                        btnAction.Enabled = false;
                        btnAction.Text = notSetActionText;
                        ttpMain.SetToolTip(btnAction, notSetActionTooltip);
                        break;

                    case Mode.Merge:
                        DirectoryUIEnabled = Opts.MergePathOpts == MDumpOptions.PathOptions.PreservePath;
                        btnAction.Enabled = true;
                        btnAction.Text = mergeActionText;
                        ttpMain.SetToolTip(btnAction, mergeActionTooltip);
                        break;

                    case Mode.Split:
                        DirectoryUIEnabled = false;
                        btnAction.Enabled = true;
                        btnAction.Text = splitActionText;
                        ttpMain.SetToolTip(btnAction, splitActionTooltip);
                        break;
                }
            }
        }

        bool _dirUIEnabled;
        /// <summary>
        /// Gets or sets the enable status of the directory browsing UI
        /// On set, enables or disables the directory browsing UI.
        /// </summary>
        private bool DirectoryUIEnabled
        {
            get { return _dirUIEnabled; }
            set
            {
               btnAddFolder.Enabled = lblRoot.Enabled
                   = txtPath.Enabled = _dirUIEnabled = value;
                if (_dirUIEnabled == false)
                {
                    dirMan.MoveAllToRoot();
                }
                else
                {
                    //This may not enable if we're in the root
                    btnUpFolder.Enabled = dirMan.ActiveHasParent();
                }
            }
        }

        /// <summary>
        /// Clears the list view and repopulates it with the current directory's items.
        /// </summary>
        private void RepopulateListView()
        {
            lvImages.Items.Clear();
            foreach (ListViewItem item in dirMan.CreateListViewItems())
            {
                lvImages.Items.Add(item);
            }
        }

        /// <summary>
        /// Add Images to the current directory
        /// </summary>
        /// <param name="paths">paths of each image</param>
        private void AddImages(IEnumerable<string> paths) 
        {
            foreach (string filepath in paths)
            {
                //Ignore this file if it's not even a supported format
                string currExt = Path.GetExtension(filepath);
                bool matched = false;
                foreach (string extension in supportedImageFormats)
                {
                    if (currExt.Equals(extension, StringComparison.InvariantCultureIgnoreCase))
                    {
                        matched = true;
                        break;
                    }
                }
                if (!matched)
                {
                    continue;
                }

                //Get the filename sans the path
                string name = System.IO.Path.GetFileName(filepath);

                //Try to create an image out of the file
                try
                {
                    Bitmap bmp = new Bitmap(filepath);
                    //Make sure we're in 32-bpp argb format
                    if (bmp.PixelFormat != System.Drawing.Imaging.PixelFormat.Format32bppArgb)
                    {
                        bmp = bmp.Clone(new Rectangle(0, 0, bmp.Width, bmp.Height),
                            System.Drawing.Imaging.PixelFormat.Format32bppArgb);
                    }
                    //Check if the image is a merged image, and if we're in the right mode
                    //to handle it.
                    bool mergedImg = PNGOps.IsMDumpMergedImage(filepath);
                    if (mergedImg && CurrentMode == Mode.Merge
                        || !mergedImg && CurrentMode == Mode.Split)
                    {
                        //We're not in the mode to handle the image being added
                        MessageBox.Show(incorrectModeMsg, incorrectModeTitle,
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                        break;
                    }
                    else if (CurrentMode == Mode.NotSet)
                    {
                        CurrentMode = mergedImg ? Mode.Split : Mode.Merge;
                    }

                    bmp.Tag = name; //Save the filename for later comparison
                    lvImages.Items.Add(dirMan.AddImage(bmp));
                }
                catch (ArgumentException ex)
                {
                    //This gets thrown by dirMan.AddImage when a duplicate exists in the given folder
                    //We'll want to use the message contained in it.
                    MessageBox.Show(ex.Message, duplicateInDirTitle,
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                catch
                {
                    //File has an image extension but couldn't be loaded
                    MessageBox.Show(name + errorLoadingImageMsg, errorLoadingImageTitle,
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        public frmMain()
        {
            InitializeComponent();
            dirMan = new ImageDirectoryManager();
            CurrentMode = Mode.NotSet;
            //Try to add any images dragged onto program:
            string[] clArgs = Environment.GetCommandLineArgs();
            if (clArgs.Length > 1)
            {
                string[] toAdd = new string[clArgs.Length - 1];
                clArgs.CopyTo(toAdd, 1);
                AddImages(clArgs);
            }
            //Load up any settings (if they exist)
            if (File.Exists(MDumpOptions.fileName))
            {
                try
                {
                    Opts = MDumpOptions.FromFile(MDumpOptions.fileName);
                }
                catch (Exception)
                {
                    MessageBox.Show("An error occurred while trying to load settings."
                        + " Reverting to default settings.");
                    Opts = new MDumpOptions();
                }
            }
            else
            {
                Opts = new MDumpOptions();
            }
            dlgSplitDest = new frmSplitDest(Opts);
        }

        private void lvImages_DragDrop(object sender, DragEventArgs e)
        {
            //TODO: Update to directory system
            AddImages((string[])e.Data.GetData(DataFormats.FileDrop));
        }

        private void lvImages_DragEnter(object sender, DragEventArgs e)
        {
            //TODO: Update to directory system
            if (e.Data.GetDataPresent(DataFormats.FileDrop, false))
            {
                e.Effect = DragDropEffects.All;
            }
        }

        private void btnInfo_Click(object sender, EventArgs e)
        {
            new frmAbout().ShowDialog();
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            if (dlgOpenImg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                try
                {
                    AddImages(dlgOpenImg.FileNames);
                }
                catch (InvalidOperationException)
                {
                    MessageBox.Show("MDump cannot merge and split images at the same time.\n"
                       + "Select some images to merge or some merged images to split.", "Error",
                       MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            //TODO: Update to directory system
            foreach (ListViewItem lvi in lvImages.SelectedItems)
            {
                lvImages.Items.Remove(lvi);
            }
            if (lvImages.Items.Count == 0)
            {
                CurrentMode = Mode.NotSet;
            }
        }

        private void lvImages_KeyUp(object sender, KeyEventArgs e)
        {
            //TODO: Update to directory system
            if (e.KeyCode == Keys.Delete && lvImages.SelectedItems.Count > 0)
            {
                foreach(ListViewItem lvi in lvImages.SelectedItems)
                {
                    lvi.Remove();
                }
                if (lvImages.Items.Count == 0)
                {
                    CurrentMode = Mode.NotSet;
                }
            }
            else if (e.KeyCode == Keys.A && e.Control)
            {
                foreach (ListViewItem lvi in lvImages.Items)
                {
                    lvi.Selected = true;
                }
            }
        }

        private void btnOptions_Click(object sender, EventArgs e)
        {
            frmOptions optsDlg = new frmOptions(Opts);
            if (optsDlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                MDumpOptions newOpts = optsDlg.GetOptions();
                if (newOpts.IsDefaultOptions())
                {
                    Opts = newOpts;
                    File.Delete(MDumpOptions.fileName);
                }
                else if (!newOpts.Equals(Opts))
                {
                    Opts = newOpts;
                    Opts.SaveToFile(MDumpOptions.fileName);
                }
            }
        }

        private void btnAction_Click(object sender, EventArgs e)
        {
            string path = null;

            if (CurrentMode == Mode.Merge)
            {
                dlgMerge.FileName = String.Empty;
                if (dlgMerge.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    path = dlgMerge.FileName;
                }
                else
                {
                    return;
                }
            }
            else
            {
                if (dlgSplitDest.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    path = dlgSplitDest.SplitPath;
                }
                else
                {
                    return;
                }
            }
            new frmWait(CurrentMode, dirMan.GetAllImages(), Opts, path).ShowDialog();
        }

        private void dlgMerge_FileOk(object sender, CancelEventArgs e)
        {
            //Get all files in the directory that start with the name provided
            string[] dirFiles = Directory.GetFiles(Path.GetDirectoryName(dlgMerge.FileName));

            //Keep track of merge files (we'll be deleting these if the user wants to overwrite)
            List<string> mergeFiles = new List<string>();
            
            //Get requested filename
            string fn = dlgMerge.FileName;
            int fnIdx = fn.LastIndexOf(System.IO.Path.DirectorySeparatorChar) + 1;
            int extIdx = fn.IndexOf('.');
            int fnLen =  extIdx - fnIdx;
            string name = fn.Substring(fnIdx, fnLen);

            //Gather all merge files
            foreach (string file in dirFiles)
            {
                //The name format of merges is <name>.<num>.png
                string test = Path.GetFileName(file);
                string[] tokens = Path.GetFileName(file).Split('.');
                if(tokens.Length == 3
                    && tokens[0].Equals(name, StringComparison.InvariantCultureIgnoreCase)
                    && IsInt(tokens[1])
                    && tokens[2].Equals("png", StringComparison.InvariantCultureIgnoreCase))
                {
                    mergeFiles.Add(file);
                }
            }

            if (mergeFiles.Count > 0)
            {
                if (MessageBox.Show("Merge files with the name " + name + " already exist in this folder."
                    + " Overwrite?", "Confirm Overwrite", MessageBoxButtons.YesNo, MessageBoxIcon.Warning,
                    MessageBoxDefaultButton.Button2) == System.Windows.Forms.DialogResult.Yes)
                {
                    e.Cancel = false;
                    foreach (string file in mergeFiles)
                    {
                        File.Delete(file);
                    }
                    dlgMerge.FileName = fn.Substring(0, extIdx);
                }
                else
                {
                    e.Cancel = true;
                }
            }
            else
            {
                e.Cancel = false;
                dlgMerge.FileName = fn.Substring(0, extIdx);
            }
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

        private void btnHowWork_Click(object sender, EventArgs e)
        {
            new frmHowItWorks().ShowDialog();
        }

        private void frmMain_Load(object sender, EventArgs e)
        {
            if (!PNGOps.DllIsPresent)
            {
                MessageBox.Show(dllNotFoundMsg,
                   dllNotFoundTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
                Close();
            }
        }

        private void txtPath_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                try
                {
                    dirMan.SetActiveDirectory(txtPath.Text);
                }
                catch (ArgumentException)
                {
                    //A directory couldn't be resolved from the given path
                    dirMan.SetActiveToRoot();
                    RepopulateListView();
                    MessageBox.Show(noSuchPathMsg, noSuchPathTitle, MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                }
            }
        }

        private void btnUpFolder_Click(object sender, EventArgs e)
        {
            dirMan.MoveUpDirectory();
            btnUpFolder.Enabled = dirMan.ActiveHasParent();
            RepopulateListView();
        }
    }
}
