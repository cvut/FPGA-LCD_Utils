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
    public partial class TBFormNote : Form
    {
        private StringBuilder sbNotes = new StringBuilder();
        
        public TBFormNote(string text)
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
        }
    }
}
