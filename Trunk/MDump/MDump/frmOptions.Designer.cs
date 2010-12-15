namespace MDump
{
    partial class frmOptions
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmOptions));
            this.btnSaveOptions = new System.Windows.Forms.Button();
            this.btnCancelOptions = new System.Windows.Forms.Button();
            this.tabOptions = new System.Windows.Forms.TabControl();
            this.tbpMerging = new System.Windows.Forms.TabPage();
            this.lblKB = new System.Windows.Forms.Label();
            this.nudMaxMergeSize = new System.Windows.Forms.NumericUpDown();
            this.lblMaxMergeSize = new System.Windows.Forms.Label();
            this.grpImageNaming = new System.Windows.Forms.GroupBox();
            this.radSaveFilePath = new System.Windows.Forms.RadioButton();
            this.radSaveFilenames = new System.Windows.Forms.RadioButton();
            this.radDiscardFilenames = new System.Windows.Forms.RadioButton();
            this.tbpSplitting = new System.Windows.Forms.TabPage();
            this.grpSplitNames = new System.Windows.Forms.GroupBox();
            this.radUseFilename = new System.Windows.Forms.RadioButton();
            this.radUsePathInfo = new System.Windows.Forms.RadioButton();
            this.radIgnore = new System.Windows.Forms.RadioButton();
            this.grpSplitDestination = new System.Windows.Forms.GroupBox();
            this.radSplitAsk = new System.Windows.Forms.RadioButton();
            this.radSplitToFolder = new System.Windows.Forms.RadioButton();
            this.btnSplitBrowse = new System.Windows.Forms.Button();
            this.txtSplitToFolder = new System.Windows.Forms.TextBox();
            this.dlgFolderBrowse = new System.Windows.Forms.FolderBrowserDialog();
            this.btnDefaults = new System.Windows.Forms.Button();
            this.tabOptions.SuspendLayout();
            this.tbpMerging.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudMaxMergeSize)).BeginInit();
            this.grpImageNaming.SuspendLayout();
            this.tbpSplitting.SuspendLayout();
            this.grpSplitNames.SuspendLayout();
            this.grpSplitDestination.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnSaveOptions
            // 
            this.btnSaveOptions.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.btnSaveOptions.Location = new System.Drawing.Point(51, 258);
            this.btnSaveOptions.Name = "btnSaveOptions";
            this.btnSaveOptions.Size = new System.Drawing.Size(100, 23);
            this.btnSaveOptions.TabIndex = 1;
            this.btnSaveOptions.Text = "Save Changes";
            this.btnSaveOptions.UseVisualStyleBackColor = true;
            this.btnSaveOptions.Click += new System.EventHandler(this.btnSaveOptions_Click);
            // 
            // btnCancelOptions
            // 
            this.btnCancelOptions.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.btnCancelOptions.Location = new System.Drawing.Point(263, 258);
            this.btnCancelOptions.Name = "btnCancelOptions";
            this.btnCancelOptions.Size = new System.Drawing.Size(100, 23);
            this.btnCancelOptions.TabIndex = 2;
            this.btnCancelOptions.Text = "Cancel";
            this.btnCancelOptions.UseVisualStyleBackColor = true;
            this.btnCancelOptions.Click += new System.EventHandler(this.btnCancelOptions_Click);
            // 
            // tabOptions
            // 
            this.tabOptions.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.tabOptions.Controls.Add(this.tbpMerging);
            this.tabOptions.Controls.Add(this.tbpSplitting);
            this.tabOptions.Location = new System.Drawing.Point(12, 12);
            this.tabOptions.Name = "tabOptions";
            this.tabOptions.SelectedIndex = 0;
            this.tabOptions.Size = new System.Drawing.Size(390, 240);
            this.tabOptions.TabIndex = 5;
            // 
            // tbpMerging
            // 
            this.tbpMerging.Controls.Add(this.lblKB);
            this.tbpMerging.Controls.Add(this.nudMaxMergeSize);
            this.tbpMerging.Controls.Add(this.lblMaxMergeSize);
            this.tbpMerging.Controls.Add(this.grpImageNaming);
            this.tbpMerging.Location = new System.Drawing.Point(4, 22);
            this.tbpMerging.Name = "tbpMerging";
            this.tbpMerging.Padding = new System.Windows.Forms.Padding(3);
            this.tbpMerging.Size = new System.Drawing.Size(382, 214);
            this.tbpMerging.TabIndex = 0;
            this.tbpMerging.Text = "Merging Options";
            this.tbpMerging.UseVisualStyleBackColor = true;
            // 
            // lblKB
            // 
            this.lblKB.AutoSize = true;
            this.lblKB.Location = new System.Drawing.Point(138, 132);
            this.lblKB.Name = "lblKB";
            this.lblKB.Size = new System.Drawing.Size(72, 13);
            this.lblKB.TabIndex = 16;
            this.lblKB.Text = "Kilobytes (KB)";
            // 
            // nudMaxMergeSize
            // 
            this.nudMaxMergeSize.Location = new System.Drawing.Point(12, 130);
            this.nudMaxMergeSize.Maximum = new decimal(new int[] {
            1048576,
            0,
            0,
            0});
            this.nudMaxMergeSize.Minimum = new decimal(new int[] {
            100,
            0,
            0,
            0});
            this.nudMaxMergeSize.Name = "nudMaxMergeSize";
            this.nudMaxMergeSize.Size = new System.Drawing.Size(120, 20);
            this.nudMaxMergeSize.TabIndex = 15;
            this.nudMaxMergeSize.Value = new decimal(new int[] {
            100,
            0,
            0,
            0});
            // 
            // lblMaxMergeSize
            // 
            this.lblMaxMergeSize.AutoSize = true;
            this.lblMaxMergeSize.Location = new System.Drawing.Point(6, 114);
            this.lblMaxMergeSize.Name = "lblMaxMergeSize";
            this.lblMaxMergeSize.Size = new System.Drawing.Size(320, 13);
            this.lblMaxMergeSize.TabIndex = 14;
            this.lblMaxMergeSize.Text = "Maximum Size of merged images (max upload size of image board):";
            // 
            // grpImageNaming
            // 
            this.grpImageNaming.Controls.Add(this.radSaveFilePath);
            this.grpImageNaming.Controls.Add(this.radSaveFilenames);
            this.grpImageNaming.Controls.Add(this.radDiscardFilenames);
            this.grpImageNaming.Location = new System.Drawing.Point(6, 6);
            this.grpImageNaming.Name = "grpImageNaming";
            this.grpImageNaming.Size = new System.Drawing.Size(370, 93);
            this.grpImageNaming.TabIndex = 13;
            this.grpImageNaming.TabStop = false;
            this.grpImageNaming.Text = "Image Naming Options";
            // 
            // radSaveFilePath
            // 
            this.radSaveFilePath.AutoSize = true;
            this.radSaveFilePath.Location = new System.Drawing.Point(6, 19);
            this.radSaveFilePath.Name = "radSaveFilePath";
            this.radSaveFilePath.Size = new System.Drawing.Size(318, 17);
            this.radSaveFilePath.TabIndex = 12;
            this.radSaveFilePath.TabStop = true;
            this.radSaveFilePath.Text = "Save the names and folder layout of the images when merging";
            this.radSaveFilePath.UseVisualStyleBackColor = true;
            // 
            // radSaveFilenames
            // 
            this.radSaveFilenames.AutoSize = true;
            this.radSaveFilenames.Location = new System.Drawing.Point(6, 42);
            this.radSaveFilenames.Name = "radSaveFilenames";
            this.radSaveFilenames.Size = new System.Drawing.Size(343, 17);
            this.radSaveFilenames.TabIndex = 10;
            this.radSaveFilenames.TabStop = true;
            this.radSaveFilenames.Text = "Save the names of the images (ingoring folder layout) when merging\r\n";
            this.radSaveFilenames.UseVisualStyleBackColor = true;
            // 
            // radDiscardFilenames
            // 
            this.radDiscardFilenames.AutoSize = true;
            this.radDiscardFilenames.Location = new System.Drawing.Point(6, 65);
            this.radDiscardFilenames.Name = "radDiscardFilenames";
            this.radDiscardFilenames.Size = new System.Drawing.Size(270, 17);
            this.radDiscardFilenames.TabIndex = 11;
            this.radDiscardFilenames.TabStop = true;
            this.radDiscardFilenames.Text = "Do not save the names of the images when merging";
            this.radDiscardFilenames.UseVisualStyleBackColor = true;
            // 
            // tbpSplitting
            // 
            this.tbpSplitting.Controls.Add(this.grpSplitNames);
            this.tbpSplitting.Controls.Add(this.grpSplitDestination);
            this.tbpSplitting.Location = new System.Drawing.Point(4, 22);
            this.tbpSplitting.Name = "tbpSplitting";
            this.tbpSplitting.Padding = new System.Windows.Forms.Padding(3);
            this.tbpSplitting.Size = new System.Drawing.Size(382, 214);
            this.tbpSplitting.TabIndex = 1;
            this.tbpSplitting.Text = "Spliting Options";
            this.tbpSplitting.UseVisualStyleBackColor = true;
            // 
            // grpSplitNames
            // 
            this.grpSplitNames.Controls.Add(this.radUseFilename);
            this.grpSplitNames.Controls.Add(this.radUsePathInfo);
            this.grpSplitNames.Controls.Add(this.radIgnore);
            this.grpSplitNames.Location = new System.Drawing.Point(6, 112);
            this.grpSplitNames.Name = "grpSplitNames";
            this.grpSplitNames.Size = new System.Drawing.Size(370, 89);
            this.grpSplitNames.TabIndex = 1;
            this.grpSplitNames.TabStop = false;
            this.grpSplitNames.Text = "If merged images come with file names:";
            // 
            // radUseFilename
            // 
            this.radUseFilename.AutoSize = true;
            this.radUseFilename.Location = new System.Drawing.Point(6, 42);
            this.radUseFilename.Name = "radUseFilename";
            this.radUseFilename.Size = new System.Drawing.Size(218, 17);
            this.radUseFilename.TabIndex = 2;
            this.radUseFilename.TabStop = true;
            this.radUseFilename.Text = "Use file name only (ignore any folder info)";
            this.radUseFilename.UseVisualStyleBackColor = true;
            // 
            // radUsePathInfo
            // 
            this.radUsePathInfo.AutoSize = true;
            this.radUsePathInfo.Location = new System.Drawing.Point(6, 19);
            this.radUsePathInfo.Name = "radUsePathInfo";
            this.radUsePathInfo.Size = new System.Drawing.Size(203, 17);
            this.radUsePathInfo.TabIndex = 1;
            this.radUsePathInfo.TabStop = true;
            this.radUsePathInfo.Text = "Use provided folder and file name info";
            this.radUsePathInfo.UseVisualStyleBackColor = true;
            // 
            // radIgnore
            // 
            this.radIgnore.AutoSize = true;
            this.radIgnore.Location = new System.Drawing.Point(6, 65);
            this.radIgnore.Name = "radIgnore";
            this.radIgnore.Size = new System.Drawing.Size(263, 17);
            this.radIgnore.TabIndex = 0;
            this.radIgnore.TabStop = true;
            this.radIgnore.Text = "Ignore it and ask for a name for the images instead";
            this.radIgnore.UseVisualStyleBackColor = true;
            // 
            // grpSplitDestination
            // 
            this.grpSplitDestination.Controls.Add(this.radSplitAsk);
            this.grpSplitDestination.Controls.Add(this.radSplitToFolder);
            this.grpSplitDestination.Controls.Add(this.btnSplitBrowse);
            this.grpSplitDestination.Controls.Add(this.txtSplitToFolder);
            this.grpSplitDestination.Location = new System.Drawing.Point(6, 6);
            this.grpSplitDestination.Name = "grpSplitDestination";
            this.grpSplitDestination.Size = new System.Drawing.Size(370, 100);
            this.grpSplitDestination.TabIndex = 0;
            this.grpSplitDestination.TabStop = false;
            this.grpSplitDestination.Text = "Where to put split images:";
            // 
            // radSplitAsk
            // 
            this.radSplitAsk.AutoSize = true;
            this.radSplitAsk.Location = new System.Drawing.Point(6, 68);
            this.radSplitAsk.Name = "radSplitAsk";
            this.radSplitAsk.Size = new System.Drawing.Size(228, 17);
            this.radSplitAsk.TabIndex = 16;
            this.radSplitAsk.TabStop = true;
            this.radSplitAsk.Text = "Ask me where to put split images each time";
            this.radSplitAsk.UseVisualStyleBackColor = true;
            // 
            // radSplitToFolder
            // 
            this.radSplitToFolder.AutoSize = true;
            this.radSplitToFolder.Location = new System.Drawing.Point(6, 19);
            this.radSplitToFolder.Name = "radSplitToFolder";
            this.radSplitToFolder.Size = new System.Drawing.Size(215, 17);
            this.radSplitToFolder.TabIndex = 15;
            this.radSplitToFolder.TabStop = true;
            this.radSplitToFolder.Text = "Put split images in to the following folder:";
            this.radSplitToFolder.UseVisualStyleBackColor = true;
            this.radSplitToFolder.CheckedChanged += new System.EventHandler(this.radSplitToFolder_CheckedChanged);
            // 
            // btnSplitBrowse
            // 
            this.btnSplitBrowse.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSplitBrowse.Location = new System.Drawing.Point(289, 39);
            this.btnSplitBrowse.Name = "btnSplitBrowse";
            this.btnSplitBrowse.Size = new System.Drawing.Size(75, 23);
            this.btnSplitBrowse.TabIndex = 14;
            this.btnSplitBrowse.Text = "Browse...";
            this.btnSplitBrowse.UseVisualStyleBackColor = true;
            this.btnSplitBrowse.Click += new System.EventHandler(this.btnSplitBrowse_Click);
            // 
            // txtSplitToFolder
            // 
            this.txtSplitToFolder.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.txtSplitToFolder.Location = new System.Drawing.Point(6, 42);
            this.txtSplitToFolder.Name = "txtSplitToFolder";
            this.txtSplitToFolder.Size = new System.Drawing.Size(277, 20);
            this.txtSplitToFolder.TabIndex = 13;
            this.txtSplitToFolder.Leave += new System.EventHandler(this.txtSplitToFolder_Leave);
            // 
            // dlgFolderBrowse
            // 
            this.dlgFolderBrowse.RootFolder = System.Environment.SpecialFolder.MyComputer;
            // 
            // btnDefaults
            // 
            this.btnDefaults.Location = new System.Drawing.Point(157, 258);
            this.btnDefaults.Name = "btnDefaults";
            this.btnDefaults.Size = new System.Drawing.Size(100, 23);
            this.btnDefaults.TabIndex = 6;
            this.btnDefaults.Text = "Reset to Defaults";
            this.btnDefaults.UseVisualStyleBackColor = true;
            this.btnDefaults.Click += new System.EventHandler(this.btnDefaults_Click);
            // 
            // frmOptions
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(414, 293);
            this.Controls.Add(this.btnDefaults);
            this.Controls.Add(this.tabOptions);
            this.Controls.Add(this.btnCancelOptions);
            this.Controls.Add(this.btnSaveOptions);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmOptions";
            this.Text = "Options";
            this.tabOptions.ResumeLayout(false);
            this.tbpMerging.ResumeLayout(false);
            this.tbpMerging.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudMaxMergeSize)).EndInit();
            this.grpImageNaming.ResumeLayout(false);
            this.grpImageNaming.PerformLayout();
            this.tbpSplitting.ResumeLayout(false);
            this.grpSplitNames.ResumeLayout(false);
            this.grpSplitNames.PerformLayout();
            this.grpSplitDestination.ResumeLayout(false);
            this.grpSplitDestination.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnSaveOptions;
        private System.Windows.Forms.Button btnCancelOptions;
        private System.Windows.Forms.TabControl tabOptions;
        private System.Windows.Forms.TabPage tbpMerging;
        private System.Windows.Forms.GroupBox grpImageNaming;
        private System.Windows.Forms.RadioButton radSaveFilePath;
        private System.Windows.Forms.RadioButton radSaveFilenames;
        private System.Windows.Forms.RadioButton radDiscardFilenames;
        private System.Windows.Forms.TabPage tbpSplitting;
        private System.Windows.Forms.GroupBox grpSplitNames;
        private System.Windows.Forms.GroupBox grpSplitDestination;
        private System.Windows.Forms.RadioButton radSplitAsk;
        private System.Windows.Forms.RadioButton radSplitToFolder;
        private System.Windows.Forms.Button btnSplitBrowse;
        private System.Windows.Forms.TextBox txtSplitToFolder;
        private System.Windows.Forms.NumericUpDown nudMaxMergeSize;
        private System.Windows.Forms.Label lblMaxMergeSize;
        private System.Windows.Forms.RadioButton radUseFilename;
        private System.Windows.Forms.RadioButton radUsePathInfo;
        private System.Windows.Forms.RadioButton radIgnore;
        private System.Windows.Forms.Label lblKB;
        private System.Windows.Forms.FolderBrowserDialog dlgFolderBrowse;
        private System.Windows.Forms.Button btnDefaults;

    }
}