using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FpgaLcdUtils
{
  public partial class RulerFormEquations : Form
  {
    public const int LINE_TAB = 0;
    public const int ELLIPSE_TAB = 1;
    public RulerFormEquations(int ixTab)
    {
      InitializeComponent();
      OpenTab(ixTab);
    }

    internal void OpenTab(int ixTab)
    {
      try
      {
        if (ixTab == LINE_TAB) tabControl1.SelectedIndex = LINE_TAB;
        else tabControl1.SelectedIndex = ELLIPSE_TAB;
      }
      catch (Exception) { }
    }
    bool wasShown = false;
    private void RulerFormEquations_Shown(object sender, EventArgs e)
    {
      if (!wasShown)
      {
        IniSettings.GeometryREquations.ApplyGeometryToForm(this);
        wasShown = true;
      }
    }

    private void RulerFormEquations_FormClosing(object sender, FormClosingEventArgs e)
    {
      IniSettings.GeometryREquations.StoreGeometry(this);
    }

    private void closeButton_Click(object sender, EventArgs e)
    {
      this.Close();
    }
  }
}
