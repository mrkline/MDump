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
            this.prgMarquee = new System.Windows.Forms.ProgressBar();
            this.lblWaitStatus = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // prgMarquee
            // 
            this.prgMarquee.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.prgMarquee.Location = new System.Drawing.Point(46, 35);
            this.prgMarquee.Name = "prgMarquee";
            this.prgMarquee.Size = new System.Drawing.Size(200, 23);
            this.prgMarquee.Style = System.Windows.Forms.ProgressBarStyle.Marquee;
            this.prgMarquee.TabIndex = 0;
            // 
            // lblWaitStatus
            // 
            this.lblWaitStatus.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.lblWaitStatus.Location = new System.Drawing.Point(12, 9);
            this.lblWaitStatus.Name = "lblWaitStatus";
            this.lblWaitStatus.Size = new System.Drawing.Size(268, 23);
            this.lblWaitStatus.TabIndex = 1;
            this.lblWaitStatus.Text = "Starting up Merger";
            this.lblWaitStatus.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // frmWait
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.ClientSize = new System.Drawing.Size(292, 80);
            this.ControlBox = false;
            this.Controls.Add(this.lblWaitStatus);
            this.Controls.Add(this.prgMarquee);
            this.DoubleBuffered = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmWait";
            this.ShowInTaskbar = false;
            this.Text = "frmWait";
            this.Shown += new System.EventHandler(this.frmWait_Shown);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ProgressBar prgMarquee;
        private System.Windows.Forms.Label lblWaitStatus;


    }
}