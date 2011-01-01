﻿using System;
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
        public enum Mode
        {
            NotSet,
            Merge,
            Split
        }

        MDumpOptions opts;

        Mode __currentMode;
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
                        lvImages.Columns[0].Text = "Images to split/merge";
                        btnAction.Enabled = false;
                        btnAction.Text = "Split/Merge Images";
                        ttpMain.SetToolTip(btnAction, "This button will split or merge images"
                            + " based on what is added to the above list.  Add merged images to split them"
                            + " or individual images to merge them.");
                        break;

                    case Mode.Merge:
                        lvImages.Columns[0].Text = "Images to merge";
                        btnAction.Enabled = true;
                        btnAction.Text = "Merge Images";
                        ttpMain.SetToolTip(btnAction, "Merge the images in the list above into"
                            + " a smaller set of images to be uploaded to an image board.");
                        break;

                    case Mode.Split:
                        lvImages.Columns[0].Text = "Images to split";
                        btnAction.Enabled = true;
                        btnAction.Text = "Split images";
                        ttpMain.SetToolTip(btnAction, "Split the merged images in the list above"
                            + " back into the original images.");
                        break;
                }
            }
        }

        private void AddImages(IEnumerable<string> paths) 
        {
            foreach (string filepath in paths)
            {
                //Get the filename sans the path
                string name = System.IO.Path.GetFileName(filepath);

                //Make sure it's not already in our list (filename of the image is stored in its tag)
                bool unique = true;
                foreach (ListViewItem lvi in lvImages.Items)
                {
                    if (((string)((Bitmap)lvi.Tag).Tag) == filepath)
                    {
                        MessageBox.Show(name + " is already added to this list.",
                            "Image already added", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        unique = false;
                        break;
                    }
                }

                if (unique)
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
                        MergedCode mergedImg = ImageMerger.IsMergedImage(filepath);
                        if (mergedImg == MergedCode.MC_ERROR)
                        {
                            throw new IOException();
                        }
                        else if (mergedImg == MergedCode.MC_MERGED && CurrentMode == Mode.Merge
                            || mergedImg == MergedCode.MC_NOT_MERGED && CurrentMode == Mode.Split)
                        {
                            throw new InvalidOperationException();
                        }
                        else if (CurrentMode == Mode.NotSet)
                        {
                            CurrentMode = mergedImg == MergedCode.MC_MERGED ? Mode.Split : Mode.Merge;
                        }

                        bmp.Tag = filepath; //Save the filename for later comparison
                        ListViewItem lvi = new ListViewItem(name);
                        lvi.Tag = bmp; //Tag the image onto the list view item
                        lvImages.Items.Add(lvi);
                    }
                    catch (InvalidOperationException)
                    {

                        MessageBox.Show("MDump cannot split merged images at the same time it"
                            + " merges individual images.  Please add either inidividual images to merge"
                            + " or MDump merged images to split back up, but not both at once.",
                            "Cannot split and merge at the same time", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        break;
                    }
                    catch
                    {
                        MessageBox.Show(name + " could not be loaded.  It will not be added to the list.",
                            "Problem loading image", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        public frmMain()
        {
            InitializeComponent();
            CurrentMode = Mode.NotSet;
            //Try to add any images dragged onto program:
            string[] clArgs = Environment.GetCommandLineArgs();
            if (clArgs.Length > 1)
            {
                string[] toAdd = new string[clArgs.Length - 1];
                clArgs.CopyTo(toAdd, 1);
                try
                {
                    AddImages(clArgs);
                }
                catch (InvalidOperationException)
                {
                    MessageBox.Show("MDump cannot merge and split images at the same time.\n"
                        + "Select some images to merge or some merged images to split.\n"
                        + "Please try again", "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    Close();
                }
            }
            //Load up any settings (if they exist)
            if (File.Exists(MDumpOptions.fileName))
            {
                try
                {
                    opts = MDumpOptions.FromFile(MDumpOptions.fileName);
                }
                catch (Exception)
                {
                    MessageBox.Show("An error occurred while trying to load settings."
                        + " Reverting to default settings.");
                    opts = new MDumpOptions();
                }
            }
            else
            {
                opts = new MDumpOptions();
            }
        }

        private void lvImages_Resize(object sender, EventArgs e)
        {
            lvImages.Columns[0].Width = lvImages.Width - 10;
        }

        private void lvImages_DragDrop(object sender, DragEventArgs e)
        {
            AddImages((string[])e.Data.GetData(DataFormats.FileDrop));
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

        private void lvImages_ItemSelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e)
        {
            if (lvImages.SelectedItems.Count > 0)
            {
                int firstIndex = lvImages.SelectedItems[0].Index;
                int lastIndex = lvImages.SelectedItems[lvImages.SelectedItems.Count - 1].Index;
                btnUp.Enabled = firstIndex != 0;
                btnDown.Enabled = lastIndex != lvImages.Items.Count - 1;
                btnDelete.Enabled = true;
            }
            else
            {
                btnUp.Enabled = btnDown.Enabled = btnDelete.Enabled = false;
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            foreach (ListViewItem lvi in lvImages.SelectedItems)
            {
                lvImages.Items.Remove(lvi);
            }
            if (lvImages.Items.Count == 0)
            {
                CurrentMode = Mode.NotSet;
            }
        }

        private void btnUp_Click(object sender, EventArgs e)
        {
            int idx = lvImages.SelectedItems[0].Index - 1;
            foreach (ListViewItem lvi in lvImages.SelectedItems)
            {
                lvi.Remove();
                lvImages.Items.Insert(idx, lvi);
                lvi.Selected = true;
            }
            lvImages.Focus();
        }

        private void btnDown_Click(object sender, EventArgs e)
        {
            int idx = lvImages.SelectedItems[lvImages.SelectedItems.Count - 1].Index + 1;
            foreach (ListViewItem lvi in lvImages.SelectedItems)
            {
                lvi.Remove();
                lvImages.Items.Insert(idx, lvi);
                lvi.Selected = true;
            }
            lvImages.Focus();
        }

        private void lvImages_KeyUp(object sender, KeyEventArgs e)
        {
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
            frmOptions optsDlg = new frmOptions(opts);
            if (optsDlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                MDumpOptions newOpts = optsDlg.GetOptions();
                if (newOpts.IsDefaultOptions())
                {
                    opts = newOpts;
                    File.Delete(MDumpOptions.fileName);
                }
                else if (!newOpts.Equals(opts))
                {
                    opts = newOpts;
                    opts.SaveToFile(MDumpOptions.fileName);
                }
            }
        }

        private void btnAction_Click(object sender, EventArgs e)
        {
            string path = string.Empty;

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
                if (opts.SplitPathOpts == MDumpOptions.PathOptions.Discard)
                {
                    if (dlgSplitPath.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                    {
                        path = dlgSplitPath.FileName;
                    }
                    else
                    {
                        return;
                    }
                }
                else
                {
                    if (dlgSplitDir.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                    {
                        path = dlgSplitDir.SelectedPath;
                    }
                    else
                    {
                        return;
                    }
                }
            }

            //Compile a list of bitmaps to pass to our mergers or splitters
            List<Bitmap> bmpList = new List<Bitmap>();
            foreach(ListViewItem lvi in lvImages.Items)
            {
                bmpList.Add((Bitmap)lvi.Tag);
            }
            new frmWait(CurrentMode, bmpList, opts, path).ShowDialog();
        }
        
        private void dlgMerge_FileOk(object sender, CancelEventArgs e)
        {
            bool cancel;
            dlgMerge.FileName = StripExtAndOverwrite(dlgMerge.FileName, out cancel);
            e.Cancel = cancel;
        }

        private void dlgSplitPath_FileOk(object sender, CancelEventArgs e)
        {
            bool cancel;
            dlgSplitPath.FileName = StripExtAndOverwrite(dlgSplitPath.FileName, out cancel);
            e.Cancel = cancel;
        }

        /// <summary>
        /// For sets of files (such as merges or re-named splits), strip the filename
        /// and overwrite all images in the set
        /// </summary>
        /// <param name="filename">Filename from the dialog</param>
        /// <param name="cancelOk">upon exit, is true if the dialog shouldn't return OK yet</param>
        /// <returns>filename stripped of its extension(s)</returns>
        private string StripExtAndOverwrite(string filename, out bool cancelOk)
        {
            //Get all files in the directory that start with the name provided
            string[] dirFiles = Directory.GetFiles(Path.GetDirectoryName(filename));

            //Keep track of merge files (we'll be deleting these if the user wants to overwrite)
            List<string> mergeFiles = new List<string>();

            //Get requested filename
            string fn = dlgMerge.FileName;
            int fnIdx = fn.LastIndexOf(System.IO.Path.DirectorySeparatorChar) + 1;
            int extIdx = fn.IndexOf('.');
            int fnLen = extIdx - fnIdx;
            string name = fn.Substring(fnIdx, fnLen);
            string strippedPath = filename.Substring(0, extIdx);

            //Gather all merge files
            foreach (string file in dirFiles)
            {
                //The name format of merges is name.num.png
                string[] tokens = Path.GetFileName(file).Split('.');
                if (tokens.Length == 3
                    && tokens[0].Equals(name, StringComparison.InvariantCultureIgnoreCase)
                    && IsInt(tokens[1])
                    && tokens[2].Equals("png", StringComparison.InvariantCultureIgnoreCase))
                {
                    mergeFiles.Add(file);
                }
            }

            if (mergeFiles.Count > 0)
            {
                if (MessageBox.Show("Split files with the name " + name + " already exist in this folder."
                    + " Overwrite?", "Confirm Overwrite", MessageBoxButtons.YesNo, MessageBoxIcon.Warning,
                    MessageBoxDefaultButton.Button2) == System.Windows.Forms.DialogResult.Yes)
                {
                    cancelOk = false;
                    foreach (string file in mergeFiles)
                    {
                        File.Delete(file);
                    }
                }
                else
                {
                    cancelOk = true;
                }
            }
            else
            {
                cancelOk = false;
            }
            return strippedPath;
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
    }
}
