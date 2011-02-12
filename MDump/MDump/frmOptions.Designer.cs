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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmOptions));
            this.btnSaveOptions = new System.Windows.Forms.Button();
            this.btnCancelOptions = new System.Windows.Forms.Button();
            this.tabOptions = new System.Windows.Forms.TabControl();
            this.tbpMerging = new System.Windows.Forms.TabPage();
            this.cmbFormat = new System.Windows.Forms.ComboBox();
            this.lblFormat = new System.Windows.Forms.Label();
            this.picExplainAddTitleBar = new System.Windows.Forms.PictureBox();
            this.picExplainMaxSize = new System.Windows.Forms.PictureBox();
            this.picExplainCompression = new System.Windows.Forms.PictureBox();
            this.lblCompression = new System.Windows.Forms.Label();
            this.lblMoreComp = new System.Windows.Forms.Label();
            this.lblLessComp = new System.Windows.Forms.Label();
            this.trkCompression = new System.Windows.Forms.TrackBar();
            this.chkAddTitleBar = new System.Windows.Forms.CheckBox();
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
            this.dlgFolderBrowse = new System.Windows.Forms.FolderBrowserDialog();
            this.btnDefaults = new System.Windows.Forms.Button();
            this.ttpExplanations = new System.Windows.Forms.ToolTip(this.components);
            this.tabOptions.SuspendLayout();
            this.tbpMerging.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picExplainAddTitleBar)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picExplainMaxSize)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picExplainCompression)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.trkCompression)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudMaxMergeSize)).BeginInit();
            this.grpImageNaming.SuspendLayout();
            this.tbpSplitting.SuspendLayout();
            this.grpSplitNames.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnSaveOptions
            // 
            this.btnSaveOptions.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.btnSaveOptions.Location = new System.Drawing.Point(51, 341);
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
            this.btnCancelOptions.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancelOptions.Location = new System.Drawing.Point(263, 341);
            this.btnCancelOptions.Name = "btnCancelOptions";
            this.btnCancelOptions.Size = new System.Drawing.Size(100, 23);
            this.btnCancelOptions.TabIndex = 3;
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
            this.tabOptions.Size = new System.Drawing.Size(390, 323);
            this.tabOptions.TabIndex = 0;
            // 
            // tbpMerging
            // 
            this.tbpMerging.Controls.Add(this.cmbFormat);
            this.tbpMerging.Controls.Add(this.lblFormat);
            this.tbpMerging.Controls.Add(this.picExplainAddTitleBar);
            this.tbpMerging.Controls.Add(this.picExplainMaxSize);
            this.tbpMerging.Controls.Add(this.picExplainCompression);
            this.tbpMerging.Controls.Add(this.lblCompression);
            this.tbpMerging.Controls.Add(this.lblMoreComp);
            this.tbpMerging.Controls.Add(this.lblLessComp);
            this.tbpMerging.Controls.Add(this.trkCompression);
            this.tbpMerging.Controls.Add(this.chkAddTitleBar);
            this.tbpMerging.Controls.Add(this.lblKB);
            this.tbpMerging.Controls.Add(this.nudMaxMergeSize);
            this.tbpMerging.Controls.Add(this.lblMaxMergeSize);
            this.tbpMerging.Controls.Add(this.grpImageNaming);
            this.tbpMerging.Location = new System.Drawing.Point(4, 22);
            this.tbpMerging.Name = "tbpMerging";
            this.tbpMerging.Padding = new System.Windows.Forms.Padding(3);
            this.tbpMerging.Size = new System.Drawing.Size(382, 297);
            this.tbpMerging.TabIndex = 0;
            this.tbpMerging.Text = "Merging Options";
            this.tbpMerging.UseVisualStyleBackColor = true;
            // 
            // cmbFormat
            // 
            this.cmbFormat.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbFormat.FormattingEnabled = true;
            this.cmbFormat.Location = new System.Drawing.Point(87, 108);
            this.cmbFormat.Name = "cmbFormat";
            this.cmbFormat.Size = new System.Drawing.Size(104, 21);
            this.cmbFormat.TabIndex = 26;
            // 
            // lblFormat
            // 
            this.lblFormat.AutoSize = true;
            this.lblFormat.Location = new System.Drawing.Point(6, 111);
            this.lblFormat.Name = "lblFormat";
            this.lblFormat.Size = new System.Drawing.Size(75, 13);
            this.lblFormat.TabIndex = 25;
            this.lblFormat.Text = "Merge Format:";
            // 
            // picExplainAddTitleBar
            // 
            this.picExplainAddTitleBar.Image = global::MDump.Properties.Resources.QuestionIcon;
            this.picExplainAddTitleBar.Location = new System.Drawing.Point(279, 272);
            this.picExplainAddTitleBar.Name = "picExplainAddTitleBar";
            this.picExplainAddTitleBar.Size = new System.Drawing.Size(20, 20);
            this.picExplainAddTitleBar.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.picExplainAddTitleBar.TabIndex = 24;
            this.picExplainAddTitleBar.TabStop = false;
            this.ttpExplanations.SetToolTip(this.picExplainAddTitleBar, "Adding a title bar to the merged images\r\nwill make it easier for people to recogn" +
                    "ize\r\nthem as MDump ones so they know\r\nto use the program to split them back up.");
            // 
            // picExplainMaxSize
            // 
            this.picExplainMaxSize.Image = global::MDump.Properties.Resources.QuestionIcon;
            this.picExplainMaxSize.Location = new System.Drawing.Point(216, 239);
            this.picExplainMaxSize.Name = "picExplainMaxSize";
            this.picExplainMaxSize.Size = new System.Drawing.Size(20, 20);
            this.picExplainMaxSize.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.picExplainMaxSize.TabIndex = 23;
            this.picExplainMaxSize.TabStop = false;
            this.ttpExplanations.SetToolTip(this.picExplainMaxSize, "Most image boards have a maximum allowed size per picture.\r\nSet this value to tha" +
                    "t maximum size.\r\n\r\nThe default value is 2048 KB, the 4chan max size.\r\nThe max va" +
                    "lue is 2 GB.");
            // 
            // picExplainCompression
            // 
            this.picExplainCompression.Image = global::MDump.Properties.Resources.QuestionIcon;
            this.picExplainCompression.Location = new System.Drawing.Point(108, 140);
            this.picExplainCompression.Name = "picExplainCompression";
            this.picExplainCompression.Size = new System.Drawing.Size(20, 20);
            this.picExplainCompression.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.picExplainCompression.TabIndex = 22;
            this.picExplainCompression.TabStop = false;
            this.ttpExplanations.SetToolTip(this.picExplainCompression, resources.GetString("picExplainCompression.ToolTip"));
            // 
            // lblCompression
            // 
            this.lblCompression.AutoSize = true;
            this.lblCompression.Location = new System.Drawing.Point(3, 144);
            this.lblCompression.Name = "lblCompression";
            this.lblCompression.Size = new System.Drawing.Size(99, 13);
            this.lblCompression.TabIndex = 1;
            this.lblCompression.Text = "Compression Level:";
            // 
            // lblMoreComp
            // 
            this.lblMoreComp.AutoSize = true;
            this.lblMoreComp.Location = new System.Drawing.Point(229, 166);
            this.lblMoreComp.Name = "lblMoreComp";
            this.lblMoreComp.Size = new System.Drawing.Size(112, 39);
            this.lblMoreComp.TabIndex = 4;
            this.lblMoreComp.Text = "More Compression\r\nSlower\r\nFewer Merged Images";
            this.lblMoreComp.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblLessComp
            // 
            this.lblLessComp.AutoSize = true;
            this.lblLessComp.Location = new System.Drawing.Point(6, 166);
            this.lblLessComp.Name = "lblLessComp";
            this.lblLessComp.Size = new System.Drawing.Size(107, 39);
            this.lblLessComp.TabIndex = 2;
            this.lblLessComp.Text = "Less Compression\r\nFaster\r\nMore Merged Images";
            this.lblLessComp.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // trkCompression
            // 
            this.trkCompression.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.trkCompression.LargeChange = 1;
            this.trkCompression.Location = new System.Drawing.Point(119, 163);
            this.trkCompression.Maximum = 1;
            this.trkCompression.Name = "trkCompression";
            this.trkCompression.Size = new System.Drawing.Size(104, 45);
            this.trkCompression.TabIndex = 3;
            this.trkCompression.Value = 1;
            // 
            // chkAddTitleBar
            // 
            this.chkAddTitleBar.AutoSize = true;
            this.chkAddTitleBar.Location = new System.Drawing.Point(12, 274);
            this.chkAddTitleBar.Name = "chkAddTitleBar";
            this.chkAddTitleBar.Size = new System.Drawing.Size(261, 17);
            this.chkAddTitleBar.TabIndex = 8;
            this.chkAddTitleBar.Text = "Add \"Created with MDump\" bar to merged images";
            this.chkAddTitleBar.UseVisualStyleBackColor = true;
            // 
            // lblKB
            // 
            this.lblKB.AutoSize = true;
            this.lblKB.Location = new System.Drawing.Point(138, 241);
            this.lblKB.Name = "lblKB";
            this.lblKB.Size = new System.Drawing.Size(72, 13);
            this.lblKB.TabIndex = 7;
            this.lblKB.Text = "Kilobytes (KB)";
            // 
            // nudMaxMergeSize
            // 
            this.nudMaxMergeSize.Location = new System.Drawing.Point(12, 239);
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
            this.nudMaxMergeSize.TabIndex = 6;
            this.nudMaxMergeSize.Value = new decimal(new int[] {
            100,
            0,
            0,
            0});
            // 
            // lblMaxMergeSize
            // 
            this.lblMaxMergeSize.AutoSize = true;
            this.lblMaxMergeSize.Location = new System.Drawing.Point(6, 223);
            this.lblMaxMergeSize.Name = "lblMaxMergeSize";
            this.lblMaxMergeSize.Size = new System.Drawing.Size(340, 13);
            this.lblMaxMergeSize.TabIndex = 5;
            this.lblMaxMergeSize.Text = "Maximum size of each merged image (max upload size of image board):";
            // 
            // grpImageNaming
            // 
            this.grpImageNaming.Controls.Add(this.radSaveFilePath);
            this.grpImageNaming.Controls.Add(this.radSaveFilenames);
            this.grpImageNaming.Controls.Add(this.radDiscardFilenames);
            this.grpImageNaming.Location = new System.Drawing.Point(6, 6);
            this.grpImageNaming.Name = "grpImageNaming";
            this.grpImageNaming.Size = new System.Drawing.Size(370, 93);
            this.grpImageNaming.TabIndex = 0;
            this.grpImageNaming.TabStop = false;
            this.grpImageNaming.Text = "Image Naming Options";
            // 
            // radSaveFilePath
            // 
            this.radSaveFilePath.AutoSize = true;
            this.radSaveFilePath.Location = new System.Drawing.Point(6, 19);
            this.radSaveFilePath.Name = "radSaveFilePath";
            this.radSaveFilePath.Size = new System.Drawing.Size(318, 17);
            this.radSaveFilePath.TabIndex = 0;
            this.radSaveFilePath.TabStop = true;
            this.radSaveFilePath.Text = "Save the names and folder layout of the images when merging";
            this.radSaveFilePath.UseVisualStyleBackColor = true;
            // 
            // radSaveFilenames
            // 
            this.radSaveFilenames.AutoSize = true;
            this.radSaveFilenames.Location = new System.Drawing.Point(6, 42);
            this.radSaveFilenames.Name = "radSaveFilenames";
            this.radSaveFilenames.Size = new System.Drawing.Size(292, 17);
            this.radSaveFilenames.TabIndex = 1;
            this.radSaveFilenames.TabStop = true;
            this.radSaveFilenames.Text = "Save the names of the images (no folders) when merging\r\n";
            this.radSaveFilenames.UseVisualStyleBackColor = true;
            // 
            // radDiscardFilenames
            // 
            this.radDiscardFilenames.AutoSize = true;
            this.radDiscardFilenames.Location = new System.Drawing.Point(6, 65);
            this.radDiscardFilenames.Name = "radDiscardFilenames";
            this.radDiscardFilenames.Size = new System.Drawing.Size(270, 17);
            this.radDiscardFilenames.TabIndex = 2;
            this.radDiscardFilenames.TabStop = true;
            this.radDiscardFilenames.Text = "Do not save the names of the images when merging";
            this.radDiscardFilenames.UseVisualStyleBackColor = true;
            // 
            // tbpSplitting
            // 
            this.tbpSplitting.Controls.Add(this.grpSplitNames);
            this.tbpSplitting.Location = new System.Drawing.Point(4, 22);
            this.tbpSplitting.Name = "tbpSplitting";
            this.tbpSplitting.Padding = new System.Windows.Forms.Padding(3);
            this.tbpSplitting.Size = new System.Drawing.Size(382, 297);
            this.tbpSplitting.TabIndex = 1;
            this.tbpSplitting.Text = "Spliting Options";
            this.tbpSplitting.UseVisualStyleBackColor = true;
            // 
            // grpSplitNames
            // 
            this.grpSplitNames.Controls.Add(this.radUseFilename);
            this.grpSplitNames.Controls.Add(this.radUsePathInfo);
            this.grpSplitNames.Controls.Add(this.radIgnore);
            this.grpSplitNames.Location = new System.Drawing.Point(6, 6);
            this.grpSplitNames.Name = "grpSplitNames";
            this.grpSplitNames.Size = new System.Drawing.Size(370, 89);
            this.grpSplitNames.TabIndex = 1;
            this.grpSplitNames.TabStop = false;
            this.grpSplitNames.Text = "If merged images come with file name or folder info:";
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
            this.radIgnore.Size = new System.Drawing.Size(281, 17);
            this.radIgnore.TabIndex = 0;
            this.radIgnore.TabStop = true;
            this.radIgnore.Text = "Ignore image names and use the provided one instead";
            this.radIgnore.UseVisualStyleBackColor = true;
            // 
            // dlgFolderBrowse
            // 
            this.dlgFolderBrowse.RootFolder = System.Environment.SpecialFolder.MyPictures;
            // 
            // btnDefaults
            // 
            this.btnDefaults.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.btnDefaults.Location = new System.Drawing.Point(157, 341);
            this.btnDefaults.Name = "btnDefaults";
            this.btnDefaults.Size = new System.Drawing.Size(100, 23);
            this.btnDefaults.TabIndex = 2;
            this.btnDefaults.Text = "Reset to Defaults";
            this.btnDefaults.UseVisualStyleBackColor = true;
            this.btnDefaults.Click += new System.EventHandler(this.btnDefaults_Click);
            // 
            // ttpExplanations
            // 
            this.ttpExplanations.AutomaticDelay = 0;
            this.ttpExplanations.AutoPopDelay = 600000;
            this.ttpExplanations.InitialDelay = 500;
            this.ttpExplanations.ReshowDelay = 100;
            this.ttpExplanations.Popup += new System.Windows.Forms.PopupEventHandler(this.ttpExplanations_Popup);
            // 
            // frmOptions
            // 
            this.AcceptButton = this.btnSaveOptions;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancelOptions;
            this.ClientSize = new System.Drawing.Size(414, 376);
            this.Controls.Add(this.btnDefaults);
            this.Controls.Add(this.tabOptions);
            this.Controls.Add(this.btnCancelOptions);
            this.Controls.Add(this.btnSaveOptions);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmOptions";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Options";
            this.tabOptions.ResumeLayout(false);
            this.tbpMerging.ResumeLayout(false);
            this.tbpMerging.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picExplainAddTitleBar)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picExplainMaxSize)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picExplainCompression)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.trkCompression)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudMaxMergeSize)).EndInit();
            this.grpImageNaming.ResumeLayout(false);
            this.grpImageNaming.PerformLayout();
            this.tbpSplitting.ResumeLayout(false);
            this.grpSplitNames.ResumeLayout(false);
            this.grpSplitNames.PerformLayout();
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
        private System.Windows.Forms.NumericUpDown nudMaxMergeSize;
        private System.Windows.Forms.Label lblMaxMergeSize;
        private System.Windows.Forms.RadioButton radUseFilename;
        private System.Windows.Forms.RadioButton radUsePathInfo;
        private System.Windows.Forms.RadioButton radIgnore;
        private System.Windows.Forms.Label lblKB;
        private System.Windows.Forms.FolderBrowserDialog dlgFolderBrowse;
        private System.Windows.Forms.Button btnDefaults;
        private System.Windows.Forms.CheckBox chkAddTitleBar;
        private System.Windows.Forms.Label lblMoreComp;
        private System.Windows.Forms.Label lblLessComp;
        private System.Windows.Forms.TrackBar trkCompression;
        private System.Windows.Forms.Label lblCompression;
        private System.Windows.Forms.PictureBox picExplainCompression;
        private System.Windows.Forms.ToolTip ttpExplanations;
        private System.Windows.Forms.PictureBox picExplainAddTitleBar;
        private System.Windows.Forms.PictureBox picExplainMaxSize;
        private System.Windows.Forms.ComboBox cmbFormat;
        private System.Windows.Forms.Label lblFormat;

    }
}