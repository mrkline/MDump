namespace MDump
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
            this.btnOptions = new System.Windows.Forms.Button();
            this.btnAction = new System.Windows.Forms.Button();
            this.ttpMain = new System.Windows.Forms.ToolTip(this.components);
            this.btnAddImage = new System.Windows.Forms.Button();
            this.btnHowWork = new System.Windows.Forms.Button();
            this.btnInfo = new System.Windows.Forms.Button();
            this.btnDelete = new System.Windows.Forms.Button();
            this.dlgOpenImg = new System.Windows.Forms.OpenFileDialog();
            this.dlgMerge = new System.Windows.Forms.SaveFileDialog();
            this.txtPath = new System.Windows.Forms.TextBox();
            this.lblRoot = new System.Windows.Forms.Label();
            this.btnUpFolder = new System.Windows.Forms.Button();
            this.btnAddFolder = new System.Windows.Forms.Button();
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
            this.lvImages.Location = new System.Drawing.Point(12, 42);
            this.lvImages.Name = "lvImages";
            this.lvImages.Size = new System.Drawing.Size(460, 236);
            this.lvImages.Sorting = System.Windows.Forms.SortOrder.Ascending;
            this.lvImages.TabIndex = 0;
            this.lvImages.UseCompatibleStateImageBehavior = false;
            this.lvImages.View = System.Windows.Forms.View.Details;
            this.lvImages.DragDrop += new System.Windows.Forms.DragEventHandler(this.lvImages_DragDrop);
            this.lvImages.DragEnter += new System.Windows.Forms.DragEventHandler(this.lvImages_DragEnter);
            this.lvImages.KeyUp += new System.Windows.Forms.KeyEventHandler(this.lvImages_KeyUp);
            this.lvImages.Resize += new System.EventHandler(this.lvImages_Resize);
            // 
            // clmImages
            // 
            this.clmImages.Text = "SetAtRuntime";
            this.clmImages.Width = 250;
            // 
            // btnOptions
            // 
            this.btnOptions.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOptions.Location = new System.Drawing.Point(361, 284);
            this.btnOptions.Name = "btnOptions";
            this.btnOptions.Size = new System.Drawing.Size(75, 30);
            this.btnOptions.TabIndex = 5;
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
            this.btnAction.TabIndex = 6;
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
            this.btnAddImage.TabIndex = 8;
            this.btnAddImage.Text = "Add Images";
            this.ttpMain.SetToolTip(this.btnAddImage, "Add an Image to the list");
            this.btnAddImage.UseVisualStyleBackColor = true;
            this.btnAddImage.Click += new System.EventHandler(this.btnAdd_Click);
            // 
            // btnHowWork
            // 
            this.btnHowWork.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnHowWork.Location = new System.Drawing.Point(245, 284);
            this.btnHowWork.Name = "btnHowWork";
            this.btnHowWork.Size = new System.Drawing.Size(110, 30);
            this.btnHowWork.TabIndex = 9;
            this.btnHowWork.Text = "How does it work?";
            this.ttpMain.SetToolTip(this.btnHowWork, "Click to learn how MDump works");
            this.btnHowWork.UseVisualStyleBackColor = true;
            this.btnHowWork.Click += new System.EventHandler(this.btnHowWork_Click);
            // 
            // btnInfo
            // 
            this.btnInfo.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnInfo.Image = global::MDump.Properties.Resources.InfoIcon;
            this.btnInfo.Location = new System.Drawing.Point(442, 284);
            this.btnInfo.Name = "btnInfo";
            this.btnInfo.Size = new System.Drawing.Size(30, 30);
            this.btnInfo.TabIndex = 7;
            this.ttpMain.SetToolTip(this.btnInfo, "Info about MDump");
            this.btnInfo.UseVisualStyleBackColor = true;
            this.btnInfo.Click += new System.EventHandler(this.btnInfo_Click);
            // 
            // btnDelete
            // 
            this.btnDelete.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnDelete.Enabled = false;
            this.btnDelete.Image = global::MDump.Properties.Resources.XIcon;
            this.btnDelete.Location = new System.Drawing.Point(174, 284);
            this.btnDelete.Name = "btnDelete";
            this.btnDelete.Size = new System.Drawing.Size(30, 30);
            this.btnDelete.TabIndex = 4;
            this.ttpMain.SetToolTip(this.btnDelete, "Remove the selected image from the list");
            this.btnDelete.UseVisualStyleBackColor = true;
            this.btnDelete.Click += new System.EventHandler(this.btnDelete_Click);
            // 
            // dlgOpenImg
            // 
            this.dlgOpenImg.Filter = "Images|*gif;*jpg;*jpeg;*wmf;*bmp;*png";
            this.dlgOpenImg.Multiselect = true;
            // 
            // dlgMerge
            // 
            this.dlgMerge.AddExtension = false;
            this.dlgMerge.Filter = "MDump Merges|*.png";
            this.dlgMerge.OverwritePrompt = false;
            this.dlgMerge.SupportMultiDottedExtensions = true;
            this.dlgMerge.FileOk += new System.ComponentModel.CancelEventHandler(this.dlgMerge_FileOk);
            // 
            // txtPath
            // 
            this.txtPath.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.txtPath.Location = new System.Drawing.Point(53, 16);
            this.txtPath.Name = "txtPath";
            this.txtPath.Size = new System.Drawing.Size(367, 20);
            this.txtPath.TabIndex = 10;
            // 
            // lblRoot
            // 
            this.lblRoot.AutoSize = true;
            this.lblRoot.Location = new System.Drawing.Point(12, 19);
            this.lblRoot.Name = "lblRoot";
            this.lblRoot.Size = new System.Drawing.Size(35, 13);
            this.lblRoot.TabIndex = 11;
            this.lblRoot.Text = "/root/";
            // 
            // btnUpFolder
            // 
            this.btnUpFolder.Enabled = false;
            this.btnUpFolder.Image = global::MDump.Properties.Resources.UpFolder;
            this.btnUpFolder.Location = new System.Drawing.Point(426, 14);
            this.btnUpFolder.Name = "btnUpFolder";
            this.btnUpFolder.Size = new System.Drawing.Size(46, 24);
            this.btnUpFolder.TabIndex = 12;
            this.btnUpFolder.UseVisualStyleBackColor = true;
            // 
            // btnAddFolder
            // 
            this.btnAddFolder.Location = new System.Drawing.Point(93, 284);
            this.btnAddFolder.Name = "btnAddFolder";
            this.btnAddFolder.Size = new System.Drawing.Size(75, 30);
            this.btnAddFolder.TabIndex = 13;
            this.btnAddFolder.Text = "Add Folder";
            this.btnAddFolder.UseVisualStyleBackColor = true;
            // 
            // frmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(484, 362);
            this.Controls.Add(this.btnAddFolder);
            this.Controls.Add(this.btnUpFolder);
            this.Controls.Add(this.lblRoot);
            this.Controls.Add(this.txtPath);
            this.Controls.Add(this.btnHowWork);
            this.Controls.Add(this.btnAddImage);
            this.Controls.Add(this.btnInfo);
            this.Controls.Add(this.btnAction);
            this.Controls.Add(this.btnOptions);
            this.Controls.Add(this.btnDelete);
            this.Controls.Add(this.lvImages);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MinimumSize = new System.Drawing.Size(460, 200);
            this.Name = "frmMain";
            this.Text = "MDump";
            this.Load += new System.EventHandler(this.frmMain_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListView lvImages;
        private System.Windows.Forms.ColumnHeader clmImages;
        private System.Windows.Forms.Button btnDelete;
        private System.Windows.Forms.Button btnOptions;
        private System.Windows.Forms.Button btnAction;
        private System.Windows.Forms.Button btnInfo;
        private System.Windows.Forms.ToolTip ttpMain;
        private System.Windows.Forms.Button btnAddImage;
        private System.Windows.Forms.OpenFileDialog dlgOpenImg;
        private System.Windows.Forms.SaveFileDialog dlgMerge;
        private System.Windows.Forms.Button btnHowWork;
        private System.Windows.Forms.TextBox txtPath;
        private System.Windows.Forms.Label lblRoot;
        private System.Windows.Forms.Button btnUpFolder;
        private System.Windows.Forms.Button btnAddFolder;

    }
}

