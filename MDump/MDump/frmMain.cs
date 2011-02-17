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
        /// <summary>
        /// Gets the incorrect mode message based on the current mode.
        /// </summary>
        private string IncorrectModeMsg
        {
            get
            {
                if (CurrentMode == Mode.Merge)
                {
                    return "MDump is current in merge mode, and therefore cannot split merged images."
                        + " Please add only individual images to merge.";
                }
                else if (CurrentMode == Mode.Split)
                {
                    return "MDump is currently in split mode, and therefore cannot merge individual images."
                        + " Please add only merged images to split.";
                }
                else
                {
                    throw new InvalidOperationException("incorrectModeMsg should not be used when no mode is set to begin with");
                }
            }
        }
        private const string incorrectModeTitle = "Cannot split and merge at the same time";
        private const string duplicateInDirTitle = "Duplicate in folder";
        private const string errorLoadingImageMsg = " could not be loaded.  It will not be added to the list.";
        private const string errorLoadingImageTitle = "Problem loading image";
        private const string incorrectModeForPaths = "Images with path information cannot be added unless we are in merge mode"
            + " with settings configured to preserve the path.";
        private const string badBaseDir =
            "One of the images provided to AddImagesWithPaths does not come from the specified base directory";
        private const string revertingToDefaultsMsg = "An error occurred while trying to load settings."
            + " Reverting to defaults.";
        private const string revertingToDefaultsTitle = "Error loading settings";
        private const string bothTypesInSelectionMsg = "Both merged and individual images are in the selected"
            + " files and folders. MDump cannot split and merge images at the same time";
        private const string bothTypesInSelectionTitle = "Merged and individual images present";
        private const string noImagesInSelectionMsg = "There are no images in the dragged files and folders";
        private const string noImagesInSelectionTitle = "No images present";
        private const string dllNotFoundMsg = PNGHandler.DllName + " could not be found."
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

        #region Vars and Properties
        private ImageDirectoryManager dirMan;
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
                        btnCreateFolder.Enabled = MDumpOptions.Instance.MergePathOpts == MDumpOptions.PathOptions.PreservePath;
                        DirectoryUIEnabled = false;
                        btnAction.Enabled = false;
                        btnAction.Text = notSetActionText;
                        ttpMain.SetToolTip(btnAction, notSetActionTooltip);
                        lvImages.LabelEdit = false;
                        break;

                    case Mode.Merge:
                        btnCreateFolder.Enabled = DirectoryUIEnabled
                            = MDumpOptions.Instance.MergePathOpts == MDumpOptions.PathOptions.PreservePath;
                        btnAction.Enabled = true;
                        btnAction.Text = mergeActionText;
                        ttpMain.SetToolTip(btnAction, mergeActionTooltip);
                        lvImages.LabelEdit = true;
                        break;

                    case Mode.Split:
                        btnCreateFolder.Enabled = DirectoryUIEnabled = false;
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
                    RefreshDirUI();
                }
                else
                {
                    //This may not enable if we're in the root
                    btnUpFolder.Enabled = dirMan.ActiveHasParent();
                }
            }
        }

        #endregion

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
                AddImages(clArgs, false);
            }
            //Load up any settings (if they exist)
            if (File.Exists(MDumpOptions.fileName))
            {
                try
                {
                    MDumpOptions.Instance = MDumpOptions.FromFile(PathManager.AppDir + MDumpOptions.fileName);
                }
                catch
                {
                    MessageBox.Show(revertingToDefaultsMsg, revertingToDefaultsTitle,
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    MDumpOptions.Instance = new MDumpOptions();
                }
            }
            else
            {
                MDumpOptions.Instance = new MDumpOptions();
            }
            dlgSplitDest = new frmSplitDest();
            CurrentMode = Mode.NotSet;
        }

        //This region contains functions used to split common behavior from the GUI itself.
        //For example, moving up a directory can be done in multiple ways through the GUI,
        //so common functions to do so are provided here.
        #region Common Functionality
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
        /// If the mode is not set, it will be set based on whether the provided images are all merged
        /// or all individual. If they are a combination of the two, the mode will not be set and the function
        /// will return false.
        /// </summary>
        /// <param name="paths">Collection of files (assumably images)</param>
        /// <returns>True if the images are all of one type and the mode is either set or maintained</returns>
        private bool SetModeFromImages(IEnumerable<string> paths)
        {
            bool hasMerged = false;
            bool hasIndividual = false;

            foreach (string path in paths)
            {
                if (MasterFormatHandler.Instance.SupportsMergedImage(path))
                {
                    hasMerged = true;
                }
                else
                {
                    hasIndividual = true;
                }
            }

            if (hasIndividual && hasMerged)
            {
                MessageBox.Show(bothTypesInSelectionMsg, bothTypesInSelectionTitle,
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            if (CurrentMode == Mode.NotSet)
            {
                if (hasIndividual)
                {
                    CurrentMode = Mode.Merge;
                    return true;
                }
                else
                {
                    CurrentMode = Mode.Split;
                    return true;
                }
            }
            else
            {
                if ((hasIndividual && CurrentMode == Mode.Split)
                    || hasMerged && CurrentMode == Mode.Merge)
                {
                    MessageBox.Show(IncorrectModeMsg, incorrectModeTitle,
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }

                return true;
            }
        }

        /// <summary>
        /// Add Images to the current directory
        /// </summary>
        /// <param name="paths">paths of each file (assumably images) to add</param>
        /// <param name="modeAlreadySet">
        /// true if SetModeFromImages has already been called, i.e., when files are dragged into the list view.
        /// false if SetModeFromImages has not already been called, i.e., when images are added from the file open dialog.
        /// </param>
        private void AddImages(IEnumerable<string> paths, bool modeAlreadySet) 
        {
            List<string> images = new List<string>();
            foreach (string path in paths)
            {
                if (PathManager.IsSupportedImage(path))
                {
                    images.Add(path);
                }
            }

            //If we haven't done this already, set the mode from the images
            if (!modeAlreadySet)
            {
                //Make sure all of the provided images are either merged or individual
                if (!SetModeFromImages(images))
                {
                    return;
                }
            }

           foreach (string filepath in images)
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
                    
                    //Based on the current mode, properly tag our image and add it.
                    if (CurrentMode == Mode.Merge)
                    {

                        bmp.Tag = new MergeImageTag(Path.GetFileNameWithoutExtension(filepath),
                            dirMan.CurrentPath);
                    }
                    else
                    {
                        bmp.Tag = new SplitImageTag(Path.GetFileName(filepath),
                            MasterFormatHandler.Instance.LoadMergedImageData(filepath));
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
            }
        }

        /// <summary>
        /// When merging with path data, add an image with a given path from the current directory
        /// </summary>
        /// <param name="paths">Paths of the images</param>
        /// <param name="baseDir">The path of the first directory you want to add to the merge's directory structure</param>
        private void AddImagesWithPaths(IEnumerable<string> paths, string baseDir)
        {
            if (!(CurrentMode == Mode.Merge && MDumpOptions.Instance.MergePathOpts == MDumpOptions.PathOptions.PreservePath))
            {
                throw new InvalidOperationException(incorrectModeForPaths);
            }
            foreach (string filepath in paths)
            {
                if (!filepath.StartsWith(baseDir, StringComparison.InvariantCultureIgnoreCase))
                {
                    throw new ArgumentException(badBaseDir);
                }
            }

            foreach (string filepath in paths)
            {
                Bitmap bmp = new Bitmap(filepath);
                //Make sure we're in 32-bpp argb format
                if (bmp.PixelFormat != System.Drawing.Imaging.PixelFormat.Format32bppArgb)
                {
                    bmp = bmp.Clone(new Rectangle(0, 0, bmp.Width, bmp.Height),
                        System.Drawing.Imaging.PixelFormat.Format32bppArgb);
                }

                string path = filepath.Substring(baseDir.Length);
                path = path.Substring(0, path.LastIndexOf(Path.DirectorySeparatorChar));

                bmp.Tag = new MergeImageTag(Path.GetFileNameWithoutExtension(filepath),
                    dirMan.CurrentPath + path);

                try
                {
                    ListViewItem dmRet = dirMan.AddImagePath(bmp, path);
                    if (dmRet != null)
                    {
                        lvImages.Items.Add(dmRet);
                    }
                }
                catch (ArgumentException)
                {
                    //This is thrown by AddImagePath if we have duplicates. Since this may happen
                    //if say, we added a sub-directory, just ignore it.
                }
            }
        }

        /// <summary>
        /// Deletes selected items when the delete key is pressed or
        /// the context menu item is clicked
        /// </summary>
        private void DeleteSelectedItems()
        {
            if (lvImages.SelectedItems.Count > 0)
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
        }

        /// <summary>
        /// Performs Add Folder action when the button or context menu item is clicked
        /// </summary>
        private void CreateFolder()
        {
            if (CurrentMode == Mode.NotSet)
            {
                CurrentMode = Mode.Merge;
            }

            lvImages.SelectedItems.Clear();

            ListViewItem newItem = dirMan.AddChildDirectory();
            lvImages.Items.Add(newItem);
            newItem.BeginEdit();
        }

        /// <summary>
        /// Returns true if string can be converted to integer
        /// </summary>
        /// <param name="str">string representation of an integer</param>
        /// <returns>true if str can be converted to an Integer</returns>
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
        #endregion

        #region Event Handlers
        private void lvImages_DragDrop(object sender, DragEventArgs e)
        {
            List<string> rootImages = new List<string>(); //Images not contained in a folder
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
                        rootImages.Add(token);
                    }
                }
            }

            //Used to set the mode from all iamges at the same time
            List<string> allImages = new List<string>(rootImages.Count);
            allImages.AddRange(rootImages);

            //Add all images in the directories to allImages
            foreach (string dir in dirs)
            {
                string[] files = Directory.GetFiles(dir, "*.*", SearchOption.AllDirectories);
                foreach (string file in files)
                {
                    if (PathManager.IsSupportedImage(file))
                    {
                        allImages.Add(file);
                    }
                }
            }

            //Set the correct mode
            if (!SetModeFromImages(allImages))
            {
                return;
            }

            //If we're keeping track of directories, we have some work to do
            if (CurrentMode == Mode.Merge
                && MDumpOptions.Instance.MergePathOpts == MDumpOptions.PathOptions.PreservePath)
            {
                AddImages(rootImages, false);

                List<string> dirImages = new List<string>();
                foreach (string dir in dirs)
                {
                    dirImages.Clear();
                    string[] files = Directory.GetFiles(dir, "*.*", SearchOption.AllDirectories);
                    foreach(string file in files)
                    {
                        if(PathManager.IsSupportedImage(file))
                        {
                            dirImages.Add(file);
                        }
                    }
                    AddImagesWithPaths(dirImages, Directory.GetParent(dir).FullName);
                }
            }
            //Otherwise chuck all the images into AddImages
            else
            {
                AddImages(allImages, true);
            }
        }

        private void lvImages_DragEnter(object sender, DragEventArgs e)
        {
            //Only allow files and folders to be dropped in
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
                 AddImages(dlgOpenImg.FileNames, false);
            }
        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            //TODO: Implement
        }

        private void lvImages_KeyUp(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Delete:
                    DeleteSelectedItems();
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

                case Keys.Back:
                    //Go up a directory
                    if (btnUpFolder.Enabled)
                    {
                        btnUpFolder.PerformClick();
                    }
                    break;
            }
        }

        private void btnOptions_Click(object sender, EventArgs e)
        {
            frmOptions optsDlg = new frmOptions(MDumpOptions.Instance);
            if (optsDlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                MDumpOptions newOpts = optsDlg.GetOptions();
                if (newOpts.IsDefaultOptions())
                {
                    File.Delete(PathManager.AppDir + MDumpOptions.fileName);
                }
                else if (!newOpts.Equals(MDumpOptions.Instance))
                {
                    newOpts.SaveToFile(PathManager.AppDir + MDumpOptions.fileName);
                }
                MDumpOptions.Instance = newOpts;
                if (CurrentMode == Mode.Merge)
                {
                    btnCreateFolder.Enabled = DirectoryUIEnabled
                        = MDumpOptions.Instance.MergePathOpts == MDumpOptions.PathOptions.PreservePath;
                }
                else if (CurrentMode == Mode.NotSet)
                {
                    btnCreateFolder.Enabled = MDumpOptions.Instance.MergePathOpts
                        == MDumpOptions.PathOptions.PreservePath;
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
            new frmWait(CurrentMode, dirMan.ImageList, path).ShowDialog();
        }

        private void dlgMerge_FileOk(object sender, CancelEventArgs e)
        {
            //Get all images in the directory that start with the name provided
            string[] dirFiles = Directory.GetFiles(Path.GetDirectoryName(dlgMerge.FileName));

            //Keep track of merge images (we'll be deleting these if the user wants to overwrite)
            List<string> mergeFiles = new List<string>();
            
            //Get requested filepath
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

        private void btnUpFolder_Click(object sender, EventArgs e)
        {
            dirMan.MoveUpDirectory();
            RefreshDirUI();
        }

        private void btnCreateFolder_Click(object sender, EventArgs e)
        {
            CreateFolder();
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

        private void conImages_Opening(object sender, CancelEventArgs e)
        {
            //Reset all items to insisible, then make the ones we need visible
            foreach (ToolStripItem tsi in conImages.Items)
            {
                tsi.Visible = false;
            }

            switch (lvImages.SelectedItems.Count)
            {
                case 0:
                    tsiAddImages.Visible = true;
                    tsiCreateFolder.Visible = CurrentMode != Mode.Split
                        && MDumpOptions.Instance.MergePathOpts == MDumpOptions.PathOptions.PreservePath;
                    break;

                case 1:
                    tsiRename.Visible = true;
                    tsiDelete.Visible = true;
                    break;

                default:
                    tsiDelete.Visible = true;
                    break;
            }
        }

        private void tsiAddImages_Click(object sender, EventArgs e)
        {
            if (dlgOpenImg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                AddImages(dlgOpenImg.FileNames, false);
            }
        }

        private void tsiCreateFolder_Click(object sender, EventArgs e)
        {
            CreateFolder();
        }

        private void tsiDelete_Click(object sender, EventArgs e)
        {
            DeleteSelectedItems();
        }

        private void tsiRename_Click(object sender, EventArgs e)
        {
            lvImages.SelectedItems[0].BeginEdit();
        }
        #endregion
    }
}
