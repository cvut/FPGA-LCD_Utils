using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LSPtools
{
  public partial class FormStartException : Form
  {
    private StringBuilder sbNotes = new StringBuilder();

    public FormStartException(string text)
    {
      InitializeComponent();
      if (text != null)
      {
        sbNotes.AppendLine(text);
        noteTextBox.Text = sbNotes.ToString();
      }
    }


    public void AddNote(string text)
    {
      sbNotes.AppendLine(text);
      noteTextBox.Text = sbNotes.ToString();
      noteTextBox.ScrollToCaret();
    }
    public string GetNotes(string text)
    {

      return sbNotes.ToString();
    }

    private void closeButton_Click(object sender, EventArgs e)
    {
      Close();
    }

    private void copyToClipboard_Click(object sender, EventArgs e)
    {
      Clipboard.SetText(sbNotes.ToString());
      confirmationTextBox.Text = "The error log was copied to clipboard!";
    }

    private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
    {
      try
      {
        Clipboard.SetText(linkLabel1.Text);
        confirmationTextBox.Text = "The email was copied to clipboard!!";
      }
      catch (Exception) { }
    }
  }
}
