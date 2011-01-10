namespace MDump
{
    partial class frmSplitDest
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmSplitDest));
            this.dlgSplitDir = new System.Windows.Forms.FolderBrowserDialog();
            this.btnOK = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.lblSelectDir = new System.Windows.Forms.Label();
            this.txtDir = new System.Windows.Forms.TextBox();
            this.btnBrowse = new System.Windows.Forms.Button();
            this.lblSelectFilename = new System.Windows.Forms.Label();
            this.txtFilename = new System.Windows.Forms.TextBox();
            this.lblFilenameStatus = new System.Windows.Forms.Label();
            this.lblDirStatus = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // btnOK
            // 
            this.btnOK.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.btnOK.Enabled = false;
            this.btnOK.Location = new System.Drawing.Point(99, 174);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 23);
            this.btnOK.TabIndex = 0;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(180, 174);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 1;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // lblSelectDir
            // 
            this.lblSelectDir.AutoSize = true;
            this.lblSelectDir.Location = new System.Drawing.Point(12, 9);
            this.lblSelectDir.Name = "lblSelectDir";
            this.lblSelectDir.Size = new System.Drawing.Size(268, 26);
            this.lblSelectDir.TabIndex = 2;
            this.lblSelectDir.Text = "Select a folder to split the merged images to,\r\nor leave blank to split images to" +
                " same folder as this exe:";
            // 
            // txtDir
            // 
            this.txtDir.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.txtDir.Location = new System.Drawing.Point(12, 37);
            this.txtDir.Name = "txtDir";
            this.txtDir.Size = new System.Drawing.Size(248, 20);
            this.txtDir.TabIndex = 3;
            this.txtDir.TextChanged += new System.EventHandler(this.txtDir_TextChanged);
            // 
            // btnBrowse
            // 
            this.btnBrowse.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.btnBrowse.Location = new System.Drawing.Point(266, 35);
            this.btnBrowse.Name = "btnBrowse";
            this.btnBrowse.Size = new System.Drawing.Size(76, 23);
            this.btnBrowse.TabIndex = 4;
            this.btnBrowse.Text = "Browse...";
            this.btnBrowse.UseVisualStyleBackColor = true;
            this.btnBrowse.Click += new System.EventHandler(this.btnBrowse_Click);
            // 
            // lblSelectFilename
            // 
            this.lblSelectFilename.AutoSize = true;
            this.lblSelectFilename.Location = new System.Drawing.Point(12, 93);
            this.lblSelectFilename.Name = "lblSelectFilename";
            this.lblSelectFilename.Size = new System.Drawing.Size(177, 13);
            this.lblSelectFilename.TabIndex = 5;
            this.lblSelectFilename.Text = "Set at runtime based on split options";
            // 
            // txtFilename
            // 
            this.txtFilename.Location = new System.Drawing.Point(12, 109);
            this.txtFilename.Name = "txtFilename";
            this.txtFilename.Size = new System.Drawing.Size(238, 20);
            this.txtFilename.TabIndex = 6;
            this.txtFilename.TextChanged += new System.EventHandler(this.txtFilename_TextChanged);
            // 
            // lblFilenameStatus
            // 
            this.lblFilenameStatus.AutoSize = true;
            this.lblFilenameStatus.Location = new System.Drawing.Point(12, 132);
            this.lblFilenameStatus.Name = "lblFilenameStatus";
            this.lblFilenameStatus.Size = new System.Drawing.Size(87, 13);
            this.lblFilenameStatus.TabIndex = 7;
            this.lblFilenameStatus.Text = "Status goes here";
            this.lblFilenameStatus.Visible = false;
            // 
            // lblDirStatus
            // 
            this.lblDirStatus.AutoSize = true;
            this.lblDirStatus.Location = new System.Drawing.Point(12, 61);
            this.lblDirStatus.Name = "lblDirStatus";
            this.lblDirStatus.Size = new System.Drawing.Size(87, 13);
            this.lblDirStatus.TabIndex = 8;
            this.lblDirStatus.Text = "Status goes here";
            this.lblDirStatus.Visible = false;
            // 
            // frmSplitDest
            // 
            this.AcceptButton = this.btnOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(354, 207);
            this.Controls.Add(this.lblDirStatus);
            this.Controls.Add(this.lblFilenameStatus);
            this.Controls.Add(this.txtFilename);
            this.Controls.Add(this.lblSelectFilename);
            this.Controls.Add(this.btnBrowse);
            this.Controls.Add(this.txtDir);
            this.Controls.Add(this.lblSelectDir);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmSplitDest";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Select a split destination...";
            this.Load += new System.EventHandler(this.frmSplitDest_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.FolderBrowserDialog dlgSplitDir;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Label lblSelectDir;
        private System.Windows.Forms.TextBox txtDir;
        private System.Windows.Forms.Button btnBrowse;
        private System.Windows.Forms.Label lblSelectFilename;
        private System.Windows.Forms.TextBox txtFilename;
        private System.Windows.Forms.Label lblFilenameStatus;
        private System.Windows.Forms.Label lblDirStatus;
    }
}