namespace MDump
{
    partial class frmOverwrite
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmOverwrite));
            this.lblFilename = new System.Windows.Forms.Label();
            this.lblPrompt = new System.Windows.Forms.Label();
            this.btnOverwrite = new System.Windows.Forms.Button();
            this.btnOverwriteAll = new System.Windows.Forms.Button();
            this.btnRename = new System.Windows.Forms.Button();
            this.btnRenameAll = new System.Windows.Forms.Button();
            this.btnSkip = new System.Windows.Forms.Button();
            this.btnSkipAll = new System.Windows.Forms.Button();
            this.btnAbort = new System.Windows.Forms.Button();
            this.tppOverwrite = new System.Windows.Forms.ToolTip(this.components);
            this.SuspendLayout();
            // 
            // lblFilename
            // 
            this.lblFilename.AutoSize = true;
            this.lblFilename.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblFilename.Location = new System.Drawing.Point(12, 9);
            this.lblFilename.Name = "lblFilename";
            this.lblFilename.Size = new System.Drawing.Size(126, 20);
            this.lblFilename.TabIndex = 0;
            this.lblFilename.Text = "Filename Here";
            // 
            // lblPrompt
            // 
            this.lblPrompt.AutoSize = true;
            this.lblPrompt.Location = new System.Drawing.Point(13, 39);
            this.lblPrompt.Name = "lblPrompt";
            this.lblPrompt.Size = new System.Drawing.Size(206, 13);
            this.lblPrompt.TabIndex = 1;
            this.lblPrompt.Text = "Already exists. What would you like to do?";
            // 
            // btnOverwrite
            // 
            this.btnOverwrite.Location = new System.Drawing.Point(17, 64);
            this.btnOverwrite.Name = "btnOverwrite";
            this.btnOverwrite.Size = new System.Drawing.Size(122, 23);
            this.btnOverwrite.TabIndex = 2;
            this.btnOverwrite.Text = "Overwrite";
            this.tppOverwrite.SetToolTip(this.btnOverwrite, "Overwrite the old image with the new one.");
            this.btnOverwrite.UseVisualStyleBackColor = true;
            this.btnOverwrite.Click += new System.EventHandler(this.btnOverwrite_Click);
            // 
            // btnOverwriteAll
            // 
            this.btnOverwriteAll.Location = new System.Drawing.Point(145, 64);
            this.btnOverwriteAll.Name = "btnOverwriteAll";
            this.btnOverwriteAll.Size = new System.Drawing.Size(122, 23);
            this.btnOverwriteAll.TabIndex = 3;
            this.btnOverwriteAll.Text = "Overwrite All";
            this.tppOverwrite.SetToolTip(this.btnOverwriteAll, "Overwrite all old images with the new ones.");
            this.btnOverwriteAll.UseVisualStyleBackColor = true;
            this.btnOverwriteAll.Click += new System.EventHandler(this.btnOverwriteAll_Click);
            // 
            // btnRename
            // 
            this.btnRename.Location = new System.Drawing.Point(17, 93);
            this.btnRename.Name = "btnRename";
            this.btnRename.Size = new System.Drawing.Size(122, 23);
            this.btnRename.TabIndex = 4;
            this.btnRename.Text = "Rename";
            this.tppOverwrite.SetToolTip(this.btnRename, "Rename the new image.");
            this.btnRename.UseVisualStyleBackColor = true;
            this.btnRename.Click += new System.EventHandler(this.btnRename_Click);
            // 
            // btnRenameAll
            // 
            this.btnRenameAll.Location = new System.Drawing.Point(145, 93);
            this.btnRenameAll.Name = "btnRenameAll";
            this.btnRenameAll.Size = new System.Drawing.Size(122, 23);
            this.btnRenameAll.TabIndex = 5;
            this.btnRenameAll.Text = "Rename All";
            this.tppOverwrite.SetToolTip(this.btnRenameAll, "Rename all new images.");
            this.btnRenameAll.UseVisualStyleBackColor = true;
            this.btnRenameAll.Click += new System.EventHandler(this.btnRenameAll_Click);
            // 
            // btnSkip
            // 
            this.btnSkip.Location = new System.Drawing.Point(17, 122);
            this.btnSkip.Name = "btnSkip";
            this.btnSkip.Size = new System.Drawing.Size(122, 23);
            this.btnSkip.TabIndex = 6;
            this.btnSkip.Text = "Skip";
            this.tppOverwrite.SetToolTip(this.btnSkip, "Discard the new image.");
            this.btnSkip.UseVisualStyleBackColor = true;
            this.btnSkip.Click += new System.EventHandler(this.btnSkip_Click);
            // 
            // btnSkipAll
            // 
            this.btnSkipAll.Location = new System.Drawing.Point(145, 122);
            this.btnSkipAll.Name = "btnSkipAll";
            this.btnSkipAll.Size = new System.Drawing.Size(122, 23);
            this.btnSkipAll.TabIndex = 7;
            this.btnSkipAll.Text = "Skip All";
            this.tppOverwrite.SetToolTip(this.btnSkipAll, "Skip the new image");
            this.btnSkipAll.UseVisualStyleBackColor = true;
            this.btnSkipAll.Click += new System.EventHandler(this.btnSkipAll_Click);
            // 
            // btnAbort
            // 
            this.btnAbort.Location = new System.Drawing.Point(17, 151);
            this.btnAbort.Name = "btnAbort";
            this.btnAbort.Size = new System.Drawing.Size(250, 23);
            this.btnAbort.TabIndex = 8;
            this.btnAbort.Text = "Abort Split";
            this.tppOverwrite.SetToolTip(this.btnAbort, "Stop splitting images.");
            this.btnAbort.UseVisualStyleBackColor = true;
            this.btnAbort.Click += new System.EventHandler(this.btnAbort_Click);
            // 
            // frmOverwrite
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 190);
            this.ControlBox = false;
            this.Controls.Add(this.btnAbort);
            this.Controls.Add(this.btnSkipAll);
            this.Controls.Add(this.btnSkip);
            this.Controls.Add(this.btnRenameAll);
            this.Controls.Add(this.btnRename);
            this.Controls.Add(this.btnOverwriteAll);
            this.Controls.Add(this.btnOverwrite);
            this.Controls.Add(this.lblPrompt);
            this.Controls.Add(this.lblFilename);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmOverwrite";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Duplicate Image";
            this.Load += new System.EventHandler(this.frmOverwrite_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblFilename;
        private System.Windows.Forms.Label lblPrompt;
        private System.Windows.Forms.Button btnOverwrite;
        private System.Windows.Forms.Button btnOverwriteAll;
        private System.Windows.Forms.Button btnRename;
        private System.Windows.Forms.Button btnRenameAll;
        private System.Windows.Forms.Button btnSkip;
        private System.Windows.Forms.Button btnSkipAll;
        private System.Windows.Forms.Button btnAbort;
        private System.Windows.Forms.ToolTip tppOverwrite;
    }
}