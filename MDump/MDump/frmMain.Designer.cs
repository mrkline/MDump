﻿namespace MDump
{
    partial class frmMain
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmMain));
            this.lvImages = new System.Windows.Forms.ListView();
            this.clmImages = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.conImages = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.tsiAddImages = new System.Windows.Forms.ToolStripMenuItem();
            this.tsiCreateFolder = new System.Windows.Forms.ToolStripMenuItem();
            this.tsiDelete = new System.Windows.Forms.ToolStripMenuItem();
            this.tsiRename = new System.Windows.Forms.ToolStripMenuItem();
            this.imlLVIcons = new System.Windows.Forms.ImageList(this.components);
            this.btnOptions = new System.Windows.Forms.Button();
            this.btnAction = new System.Windows.Forms.Button();
            this.ttpMain = new System.Windows.Forms.ToolTip(this.components);
            this.btnAddImage = new System.Windows.Forms.Button();
            this.btnInfo = new System.Windows.Forms.Button();
            this.btnUpFolder = new System.Windows.Forms.Button();
            this.btnCreateFolder = new System.Windows.Forms.Button();
            this.btnReset = new System.Windows.Forms.Button();
            this.dlgOpenImg = new System.Windows.Forms.OpenFileDialog();
            this.dlgMerge = new System.Windows.Forms.SaveFileDialog();
            this.txtPath = new System.Windows.Forms.TextBox();
            this.lblRoot = new System.Windows.Forms.Label();
            this.conImages.SuspendLayout();
            this.SuspendLayout();
            // 
            // lvImages
            // 
            this.lvImages.AllowDrop = true;
            this.lvImages.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.lvImages.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.clmImages});
            this.lvImages.ContextMenuStrip = this.conImages;
            this.lvImages.LabelEdit = true;
            this.lvImages.Location = new System.Drawing.Point(12, 42);
            this.lvImages.Name = "lvImages";
            this.lvImages.Size = new System.Drawing.Size(460, 236);
            this.lvImages.SmallImageList = this.imlLVIcons;
            this.lvImages.Sorting = System.Windows.Forms.SortOrder.Ascending;
            this.lvImages.TabIndex = 3;
            this.lvImages.UseCompatibleStateImageBehavior = false;
            this.lvImages.View = System.Windows.Forms.View.List;
            this.lvImages.AfterLabelEdit += new System.Windows.Forms.LabelEditEventHandler(this.lvImages_AfterLabelEdit);
            this.lvImages.ItemActivate += new System.EventHandler(this.lvImages_ItemActivate);
            this.lvImages.DragDrop += new System.Windows.Forms.DragEventHandler(this.lvImages_DragDrop);
            this.lvImages.DragEnter += new System.Windows.Forms.DragEventHandler(this.lvImages_DragEnter);
            this.lvImages.KeyUp += new System.Windows.Forms.KeyEventHandler(this.lvImages_KeyUp);
            // 
            // clmImages
            // 
            this.clmImages.Text = "SetAtRuntime";
            this.clmImages.Width = 250;
            // 
            // conImages
            // 
            this.conImages.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsiAddImages,
            this.tsiCreateFolder,
            this.tsiDelete,
            this.tsiRename});
            this.conImages.Name = "conImages";
            this.conImages.Size = new System.Drawing.Size(188, 92);
            this.conImages.Opening += new System.ComponentModel.CancelEventHandler(this.conImages_Opening);
            // 
            // tsiAddImages
            // 
            this.tsiAddImages.Name = "tsiAddImages";
            this.tsiAddImages.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.I)));
            this.tsiAddImages.Size = new System.Drawing.Size(187, 22);
            this.tsiAddImages.Text = "Add &Images";
            this.tsiAddImages.Visible = false;
            this.tsiAddImages.Click += new System.EventHandler(this.tsiAddImages_Click);
            // 
            // tsiCreateFolder
            // 
            this.tsiCreateFolder.Name = "tsiCreateFolder";
            this.tsiCreateFolder.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.N)));
            this.tsiCreateFolder.Size = new System.Drawing.Size(187, 22);
            this.tsiCreateFolder.Text = "Create &Folder";
            this.tsiCreateFolder.Click += new System.EventHandler(this.tsiCreateFolder_Click);
            // 
            // tsiDelete
            // 
            this.tsiDelete.Name = "tsiDelete";
            this.tsiDelete.Size = new System.Drawing.Size(187, 22);
            this.tsiDelete.Text = "&Delete";
            this.tsiDelete.Click += new System.EventHandler(this.tsiDelete_Click);
            // 
            // tsiRename
            // 
            this.tsiRename.Name = "tsiRename";
            this.tsiRename.Size = new System.Drawing.Size(187, 22);
            this.tsiRename.Text = "&Rename";
            this.tsiRename.Click += new System.EventHandler(this.tsiRename_Click);
            // 
            // imlLVIcons
            // 
            this.imlLVIcons.ColorDepth = System.Windows.Forms.ColorDepth.Depth24Bit;
            this.imlLVIcons.ImageSize = new System.Drawing.Size(16, 16);
            this.imlLVIcons.TransparentColor = System.Drawing.Color.Transparent;
            // 
            // btnOptions
            // 
            this.btnOptions.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOptions.Location = new System.Drawing.Point(361, 284);
            this.btnOptions.Name = "btnOptions";
            this.btnOptions.Size = new System.Drawing.Size(75, 30);
            this.btnOptions.TabIndex = 8;
            this.btnOptions.Text = "Options...";
            this.ttpMain.SetToolTip(this.btnOptions, "Edit options and settings for MDump");
            this.btnOptions.UseVisualStyleBackColor = true;
            this.btnOptions.Click += new System.EventHandler(this.btnOptions_Click);
            // 
            // btnAction
            // 
            this.btnAction.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.btnAction.Location = new System.Drawing.Point(12, 320);
            this.btnAction.Name = "btnAction";
            this.btnAction.Size = new System.Drawing.Size(460, 30);
            this.btnAction.TabIndex = 4;
            this.btnAction.Text = "Set at runtime";
            this.btnAction.UseVisualStyleBackColor = true;
            this.btnAction.Click += new System.EventHandler(this.btnAction_Click);
            // 
            // btnAddImage
            // 
            this.btnAddImage.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnAddImage.Location = new System.Drawing.Point(12, 284);
            this.btnAddImage.Name = "btnAddImage";
            this.btnAddImage.Size = new System.Drawing.Size(75, 30);
            this.btnAddImage.TabIndex = 5;
            this.btnAddImage.Text = "Add Images";
            this.ttpMain.SetToolTip(this.btnAddImage, "Add an image to the list");
            this.btnAddImage.UseVisualStyleBackColor = true;
            this.btnAddImage.Click += new System.EventHandler(this.btnAdd_Click);
            // 
            // btnInfo
            // 
            this.btnInfo.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnInfo.Image = global::MDump.Properties.Resources.InfoIcon;
            this.btnInfo.Location = new System.Drawing.Point(442, 284);
            this.btnInfo.Name = "btnInfo";
            this.btnInfo.Size = new System.Drawing.Size(30, 30);
            this.btnInfo.TabIndex = 9;
            this.ttpMain.SetToolTip(this.btnInfo, "Info about MDump");
            this.btnInfo.UseVisualStyleBackColor = true;
            this.btnInfo.Click += new System.EventHandler(this.btnInfo_Click);
            // 
            // btnUpFolder
            // 
            this.btnUpFolder.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnUpFolder.Enabled = false;
            this.btnUpFolder.Image = global::MDump.Properties.Resources.UpFolder;
            this.btnUpFolder.Location = new System.Drawing.Point(426, 14);
            this.btnUpFolder.Name = "btnUpFolder";
            this.btnUpFolder.Size = new System.Drawing.Size(46, 24);
            this.btnUpFolder.TabIndex = 2;
            this.ttpMain.SetToolTip(this.btnUpFolder, "Move up one folder");
            this.btnUpFolder.UseVisualStyleBackColor = true;
            this.btnUpFolder.Click += new System.EventHandler(this.btnUpFolder_Click);
            // 
            // btnCreateFolder
            // 
            this.btnCreateFolder.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnCreateFolder.Location = new System.Drawing.Point(93, 284);
            this.btnCreateFolder.Name = "btnCreateFolder";
            this.btnCreateFolder.Size = new System.Drawing.Size(85, 30);
            this.btnCreateFolder.TabIndex = 6;
            this.btnCreateFolder.Text = "Create Folder";
            this.ttpMain.SetToolTip(this.btnCreateFolder, "Add a folder to put additional images in to");
            this.btnCreateFolder.UseVisualStyleBackColor = true;
            this.btnCreateFolder.Click += new System.EventHandler(this.btnCreateFolder_Click);
            // 
            // btnReset
            // 
            this.btnReset.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnReset.Location = new System.Drawing.Point(184, 284);
            this.btnReset.Name = "btnReset";
            this.btnReset.Size = new System.Drawing.Size(81, 30);
            this.btnReset.TabIndex = 7;
            this.btnReset.Text = "Reset";
            this.ttpMain.SetToolTip(this.btnReset, "Resets MDump, clearing out all images and/or folders");
            this.btnReset.UseVisualStyleBackColor = true;
            this.btnReset.Click += new System.EventHandler(this.btnReset_Click);
            // 
            // dlgOpenImg
            // 
            this.dlgOpenImg.Filter = "Images|*.gif;*.jpg;*.jpeg;*.wmf;*.bmp;*.png";
            this.dlgOpenImg.Multiselect = true;
            // 
            // dlgMerge
            // 
            this.dlgMerge.AddExtension = false;
            this.dlgMerge.Filter = "MDump Merges|*.png;*.jpg";
            this.dlgMerge.OverwritePrompt = false;
            this.dlgMerge.SupportMultiDottedExtensions = true;
            this.dlgMerge.FileOk += new System.ComponentModel.CancelEventHandler(this.dlgMerge_FileOk);
            // 
            // txtPath
            // 
            this.txtPath.AcceptsReturn = true;
            this.txtPath.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.txtPath.Location = new System.Drawing.Point(53, 16);
            this.txtPath.Name = "txtPath";
            this.txtPath.Size = new System.Drawing.Size(367, 20);
            this.txtPath.TabIndex = 1;
            this.txtPath.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtPath_KeyPress);
            this.txtPath.KeyUp += new System.Windows.Forms.KeyEventHandler(this.txtPath_KeyUp);
            // 
            // lblRoot
            // 
            this.lblRoot.AutoSize = true;
            this.lblRoot.Location = new System.Drawing.Point(12, 19);
            this.lblRoot.Name = "lblRoot";
            this.lblRoot.Size = new System.Drawing.Size(35, 13);
            this.lblRoot.TabIndex = 0;
            this.lblRoot.Text = "\\root\\";
            // 
            // frmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(484, 362);
            this.Controls.Add(this.btnCreateFolder);
            this.Controls.Add(this.btnUpFolder);
            this.Controls.Add(this.btnReset);
            this.Controls.Add(this.lblRoot);
            this.Controls.Add(this.txtPath);
            this.Controls.Add(this.btnAddImage);
            this.Controls.Add(this.btnInfo);
            this.Controls.Add(this.btnAction);
            this.Controls.Add(this.btnOptions);
            this.Controls.Add(this.lvImages);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MinimumSize = new System.Drawing.Size(460, 200);
            this.Name = "frmMain";
            this.Text = "MDump";
            this.conImages.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListView lvImages;
        private System.Windows.Forms.ColumnHeader clmImages;
        private System.Windows.Forms.Button btnOptions;
        private System.Windows.Forms.Button btnAction;
        private System.Windows.Forms.Button btnInfo;
        private System.Windows.Forms.ToolTip ttpMain;
        private System.Windows.Forms.Button btnAddImage;
        private System.Windows.Forms.OpenFileDialog dlgOpenImg;
        private System.Windows.Forms.SaveFileDialog dlgMerge;
        private System.Windows.Forms.TextBox txtPath;
        private System.Windows.Forms.Label lblRoot;
        private System.Windows.Forms.Button btnUpFolder;
        private System.Windows.Forms.Button btnCreateFolder;
        internal System.Windows.Forms.ImageList imlLVIcons;
        private System.Windows.Forms.ContextMenuStrip conImages;
        private System.Windows.Forms.ToolStripMenuItem tsiAddImages;
        private System.Windows.Forms.ToolStripMenuItem tsiCreateFolder;
        private System.Windows.Forms.ToolStripMenuItem tsiDelete;
        private System.Windows.Forms.ToolStripMenuItem tsiRename;
        private System.Windows.Forms.Button btnReset;

    }
}

