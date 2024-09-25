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
  public partial class QCToDoForm_notused : Form
  {
    private StringBuilder czNotes = new StringBuilder();
    private StringBuilder engNotes = new StringBuilder();


    public QCToDoForm_notused()
    {
      InitializeComponent();

    }



    public string GetNotes(string text)
    {

      return czNotes.ToString();
    }

    private void closeButton_Click(object sender, EventArgs e)
    {
      Close();
    }

    private void copyToClipboard_Click(object sender, EventArgs e)
    {
      Clipboard.SetText(czNotes.ToString());
    }
  }
}
