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
        private const string duplicateDirMsg = "A folder with the same name already exists";
        private const string duplicateDirTitle = "Duplicate Folder";
        private const string switchToMergeMsg = "Adding a folder will put MDump in merge mode"            
                    + " since splitting images doesn't use folder info. Continue?";
        private const string switchToMergeTitle = "Switch to merge mode?";
        private const string noSuchPathMsg = "The path entered does not exist.";
        private const string noSuchPathTitle = "No such path";
        #endregion

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
                        btnAddFolder.Enabled = Opts.MergePathOpts == MDumpOptions.PathOptions.PreservePath;
                        DirectoryUIEnabled = false;
                        btnAction.Enabled = false;
                        btnAction.Text = notSetActionText;
                        ttpMain.SetToolTip(btnAction, notSetActionTooltip);
                        lvImages.LabelEdit = false;
                        break;

                    case Mode.Merge:
                        btnAddFolder.Enabled = DirectoryUIEnabled 
                            = Opts.MergePathOpts == MDumpOptions.PathOptions.PreservePath;
                        btnAction.Enabled = true;
                        btnAction.Text = mergeActionText;
                        ttpMain.SetToolTip(btnAction, mergeActionTooltip);
                        lvImages.LabelEdit = true;
                        break;

                    case Mode.Split:
                        btnAddFolder.Enabled = DirectoryUIEnabled = false;
                        btnAction.Enabled = true;
                        btnAction.Text = splitActionText;
                        ttpMain.SetToolTip(btnAction, splitActionTooltip);
                        lvImages.LabelEdit = false;
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
                lblRoot.Enabled = txtPath.Enabled = _dirUIEnabled = value;
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
        private void RefreshDirUI()
        {
            lvImages.Items.Clear();
            foreach (ListViewItem item in dirMan.LVIRepresentation)
            {
                lvImages.Items.Add(item);
            }
            txtPath.Text = dirMan.CurrentPath;
            btnUpFolder.Enabled = dirMan.ActiveHasParent();
        }

        /// <summary>
        /// Add Images to the current directory
        /// </summary>
        /// <param name="paths">paths of each image</param>
        private void AddImages(IEnumerable<string> paths) 
        {
            foreach (string filepath in paths)
            {
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

                    if (CurrentMode == Mode.Merge)
                    {

                        bmp.Tag = new MergeImageTag(Path.GetFileNameWithoutExtension(filepath),
                            dirMan.CurrentPath);
                    }
                    else
                    {
                        bmp.Tag = new SplitImageTag(Path.GetFileName(filepath),
                            PNGOps.LoadMergedImageData(filepath));
                    }
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
                    MessageBox.Show(Path.GetFileName(filepath) + errorLoadingImageMsg,
                        errorLoadingImageTitle,
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
            imlLVIcons.Images.Add(Properties.Resources.ImageIcon);
            imlLVIcons.Images.Add(Properties.Resources.Folder);
            dirMan = new ImageDirectoryManager();
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
            CurrentMode = Mode.NotSet;
        }

        private void lvImages_DragDrop(object sender, DragEventArgs e)
        {
            List<string> images = new List<string>();
            //We want to add our directories after we add images so
            //the images can set the mode (if not set)
            List<string> dirs = new List<string>();

            //We don't know whether these are images or folders
            string[] tokens = (string[])e.Data.GetData(DataFormats.FileDrop);
            
            foreach (string token in tokens)
            {
                //If this is a directory, add all images in it. 
                if(Directory.Exists(token))
                {
                    dirs.Add(token);
                }
                else
                {
                    if (PathManager.IsSupportedImage(token))
                    {
                        images.Add(token);
                    }
                }
            }

            AddImages(images);

            switch (CurrentMode)
            {
                case Mode.NotSet:
                    //TODO: Check if we have just individual images or just merges. Set mode appropriately
                    break;

                case Mode.Merge:
                    if (Opts.MergePathOpts == MDumpOptions.PathOptions.PreservePath)
                    {
                        foreach (string dir in dirs)
                        {
                            //TODO: Implement some kind of dirMan.AddImageByPath and just
                            //add each image in the directory structure by their path
                        }
                    }
                    else
                    {
                        //Reuse images to hold on to images in the directory
                        images.Clear();
                        foreach (string dir in dirs)
                        {
                            //Dir structure doesn't have to be maintained
                            string[] files = Directory.GetFiles(dir, "*.*", SearchOption.AllDirectories);
                            foreach (string file in files)
                            {
                                if (PathManager.IsSupportedImage(file))
                                {
                                    images.Add(file);
                                }
                            }
                        }
                        AddImages(images);
                    }
                    break;

                case Mode.Split:
                    //Reuse images to hold on to images in the directory
                    images.Clear();
                    foreach (string dir in dirs)
                    {
                        //Dir structure doesn't have to be maintained
                        string[] files = Directory.GetFiles(dir, "*.*", SearchOption.AllDirectories);
                        foreach (string file in files)
                        {
                            if (PathManager.IsSupportedImage(file))
                            {
                                images.Add(file);
                            }
                        }
                    }
                    AddImages(images);
                    break;
            }
        }

        private void lvImages_DragEnter(object sender, DragEventArgs e)
        {
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
                    List<string> images = new List<string>();
                    foreach (string file in dlgOpenImg.FileNames)
                    {
                        if (PathManager.IsSupportedImage(file))
                        {
                            images.Add(file);
                        }
                    }
                    AddImages(images);
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
            foreach (ListViewItem lvi in lvImages.SelectedItems)
            {
                dirMan.RemoveItem(lvi);
                lvi.Remove();
            }
            if (dirMan.IsEmpty)
            {
                CurrentMode = Mode.NotSet;
            }
        }

        private void lvImages_KeyUp(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Delete:
                    //Delete selected items
                    if (lvImages.SelectedItems.Count > 0)
                    {
                        foreach(ListViewItem lvi in lvImages.SelectedItems)
                        {
                            dirMan.RemoveItem(lvi);
                            lvi.Remove();
                        }
                        if (dirMan.IsEmpty)
                        {
                            CurrentMode = Mode.NotSet;
                        }
                    }
                    break;

                case Keys.A:
                    //Select all
                    if (e.Control)
                    {
                        foreach (ListViewItem lvi in lvImages.Items)
                        {
                            lvi.Selected = true;
                        }
                    }
                    break;

                case Keys.F2:
                    //Rename selected item
                    if (lvImages.SelectedItems.Count == 1)
                    {
                        lvImages.SelectedItems[0].BeginEdit();
                    }
                    break;
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
                    File.Delete(MDumpOptions.fileName);
                }
                else if (!newOpts.Equals(Opts))
                {
                    newOpts.SaveToFile(MDumpOptions.fileName);
                }
                Opts = newOpts;
                if (CurrentMode == Mode.Merge)
                {
                    btnAddFolder.Enabled = DirectoryUIEnabled
                        = Opts.MergePathOpts == MDumpOptions.PathOptions.PreservePath;
                }
                else if (CurrentMode == Mode.NotSet)
                {
                    btnAddFolder.Enabled = Opts.MergePathOpts == MDumpOptions.PathOptions.PreservePath;
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
            new frmWait(CurrentMode, dirMan.ImageList, Opts, path).ShowDialog();
        }

        private void dlgMerge_FileOk(object sender, CancelEventArgs e)
        {
            //Get all images in the directory that start with the name provided
            string[] dirFiles = Directory.GetFiles(Path.GetDirectoryName(dlgMerge.FileName));

            //Keep track of merge images (we'll be deleting these if the user wants to overwrite)
            List<string> mergeFiles = new List<string>();
            
            //Get requested filename
            string fn = dlgMerge.FileName;
            int fnIdx = fn.LastIndexOf(System.IO.Path.DirectorySeparatorChar) + 1;
            int extIdx = fn.IndexOf('.');
            int fnLen =  extIdx - fnIdx;
            string name = fn.Substring(fnIdx, fnLen);

            //Gather all merge images
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
        private void btnUpFolder_Click(object sender, EventArgs e)
        {
            dirMan.MoveUpDirectory();
            RefreshDirUI();
        }

        private void btnAddFolder_Click(object sender, EventArgs e)
        {
            if (CurrentMode == Mode.NotSet)
            {
                if (MessageBox.Show(switchToMergeMsg, switchToMergeTitle,
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question)
                    == System.Windows.Forms.DialogResult.Yes)
                {
                    CurrentMode = Mode.Merge;
                }
                else
                {
                    return;
                } 
            }

            frmFolderName dlg = new frmFolderName();
            if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                try
                {
                    lvImages.Items.Add(dirMan.AddChildDirectory(dlg.FolderName));
                }
                catch (ArgumentException)
                {
                    //A duplicate directory exists
                    MessageBox.Show(duplicateDirMsg, duplicateDirTitle,
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
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
                    MessageBox.Show(noSuchPathMsg, noSuchPathTitle, MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                }
                finally
                {
                    RefreshDirUI();
                }
            }
        }

        private void txtPath_KeyPress(object sender, KeyPressEventArgs e)
        {
            //Stop the bleeding chime on hitting enter
            if (e.KeyChar == '\r')
            {
                e.Handled = true;
            }
        }

        private void lvImages_ItemActivate(object sender, EventArgs e)
        {
            if (dirMan.ItemRepresentsDirectory(lvImages.SelectedItems[0]))
            {
                dirMan.MoveToChildDirecory(lvImages.SelectedItems[0]);
                RefreshDirUI();
            }
        }

        private void lvImages_AfterLabelEdit(object sender, LabelEditEventArgs e)
        {
            //Apparently e.Label is null when no change has occurred
            if (e.Label == null)
            {
                return;
            }

            ListViewItem item = lvImages.Items[e.Item];
            string newName = e.Label;
            if (dirMan.ItemRepresentsDirectory(item))
            {
                if (PathManager.IsValidDirName(newName))
                {
                    dirMan.RenameItem(item, newName);
                }
                else
                {
                    e.CancelEdit = true;
                    MessageBox.Show(newName + PathManager.InvalidDirNameMsg, PathManager.InvalidDirNameTitle,
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            else
            {
                if (PathManager.IsValidMergeName(newName))
                {
                    dirMan.RenameItem(item, newName);
                }
                else
                {
                    e.CancelEdit = true;
                    MessageBox.Show(PathManager.InvalidBmpNameMsg, PathManager.InvalidBmpTagTitle,
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
    }
}
