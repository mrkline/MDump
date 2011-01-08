namespace MDump
{
    partial class frmWait
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmWait));
            this.prgIndividual = new System.Windows.Forms.ProgressBar();
            this.lblWaitStatus = new System.Windows.Forms.Label();
            this.lblIndividualProgress = new System.Windows.Forms.Label();
            this.prgOverall = new System.Windows.Forms.ProgressBar();
            this.lblOverallProgress = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // prgIndividual
            // 
            this.prgIndividual.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.prgIndividual.Location = new System.Drawing.Point(156, 111);
            this.prgIndividual.Name = "prgIndividual";
            this.prgIndividual.Size = new System.Drawing.Size(200, 23);
            this.prgIndividual.TabIndex = 0;
            // 
            // lblWaitStatus
            // 
            this.lblWaitStatus.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.lblWaitStatus.Location = new System.Drawing.Point(12, 9);
            this.lblWaitStatus.Name = "lblWaitStatus";
            this.lblWaitStatus.Size = new System.Drawing.Size(350, 99);
            this.lblWaitStatus.TabIndex = 1;
            this.lblWaitStatus.Text = "Fill at runtime";
            this.lblWaitStatus.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // lblIndividualProgress
            // 
            this.lblIndividualProgress.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.lblIndividualProgress.AutoSize = true;
            this.lblIndividualProgress.Location = new System.Drawing.Point(19, 116);
            this.lblIndividualProgress.Name = "lblIndividualProgress";
            this.lblIndividualProgress.Size = new System.Drawing.Size(131, 13);
            this.lblIndividualProgress.TabIndex = 2;
            this.lblIndividualProgress.Text = "Individual Image Progress:";
            // 
            // prgOverall
            // 
            this.prgOverall.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.prgOverall.Location = new System.Drawing.Point(156, 140);
            this.prgOverall.Name = "prgOverall";
            this.prgOverall.Size = new System.Drawing.Size(200, 23);
            this.prgOverall.TabIndex = 3;
            // 
            // lblOverallProgress
            // 
            this.lblOverallProgress.AutoSize = true;
            this.lblOverallProgress.Location = new System.Drawing.Point(63, 170);
            this.lblOverallProgress.Name = "lblOverallProgress";
            this.lblOverallProgress.Size = new System.Drawing.Size(87, 13);
            this.lblOverallProgress.TabIndex = 4;
            this.lblOverallProgress.Text = "Overall Progress:";
            // 
            // frmWait
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.ClientSize = new System.Drawing.Size(374, 172);
            this.ControlBox = false;
            this.Controls.Add(this.lblOverallProgress);
            this.Controls.Add(this.prgOverall);
            this.Controls.Add(this.lblIndividualProgress);
            this.Controls.Add(this.lblWaitStatus);
            this.Controls.Add(this.prgIndividual);
            this.DoubleBuffered = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmWait";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "frmWait";
            this.Shown += new System.EventHandler(this.frmWait_Shown);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ProgressBar prgIndividual;
        private System.Windows.Forms.Label lblWaitStatus;
        private System.Windows.Forms.Label lblIndividualProgress;
        private System.Windows.Forms.ProgressBar prgOverall;
        private System.Windows.Forms.Label lblOverallProgress;


    }
}