using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace LSPtools
{
  public partial class BMFormAbout : Form
  {
    public BMFormAbout()
    {
      InitializeComponent();
    }

    private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
    {
      try
      {
        System.Diagnostics.Process.Start("https://susta.cz/");
      }
      catch (Exception) { }
    }
    private void linkLabel2_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
    {
      try
      {
        System.Diagnostics.Process.Start("https://dcenet.fel.cvut.cz/edu/fpga/");
      }
      catch (Exception) { }
    }

    private void button1_Click(object sender, EventArgs e)
    {
      this.Close();
    }

    private void FormAbout_FormClosing(object sender, FormClosingEventArgs e)
    {
      e.Cancel = false;
    }
  }
}