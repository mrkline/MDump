using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace MDump
{
    /// <summary>
    /// Form containing a brief explanation of how the application works
    /// </summary>
    public partial class frmHowItWorks : Form
    {
        public frmHowItWorks()
        {
            InitializeComponent();
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
