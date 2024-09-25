using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace LSPtools
{
    public partial class BMFormHelp : Form
    {
        public BMFormHelp()
        {
            InitializeComponent();
        }
        private void FormHelp_Load(object sender, EventArgs e)
        {
            try
            {
                richTextBox1.Rtf = LSPtools.Properties.Resources.BMHelp;
            }
            catch (Exception ex)
            {

              richTextBox1.Text = ex.Message;
            }
        }

        private void buttonClose_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}