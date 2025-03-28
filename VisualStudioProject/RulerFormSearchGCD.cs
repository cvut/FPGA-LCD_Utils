using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static FpgaLcdUtils.QuartusProject;
using static FpgaLcdUtils.RulerFormSearchLineGCD;
using static RGBInfoArray;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace FpgaLcdUtils
{
  public partial class RulerFormSearchGCD : Form
  {
    public struct OrgData
    {
      public int Xcenter, Ycenter;
      public int Xwidth, Yheight;
    }
    RulerListViewColumnSorter columnSorter;
    static OrgData orgData;

    private const int INIT_WIDTH = 0;
    private const int INIT_HEIGHT = 1;
    private const int INIT_DFONT = 2;

    RulerFormMain.AdjustEllipticRulerDelegate? adjustEllipticRuler = null;

    private const int COLUMN_DISTANCE = 0, COLUMN_AGDC = 1, COLUMN_BGDC = 2,
                      COLUMN_GCD = 3, COLUMN_WIDTH = 4, COLUMN_HEIGHT = 5;

    public RulerFormSearchGCD(OrgData orgdata, RulerFormMain.AdjustEllipticRulerDelegate adjustEllipticRuler)
    {
      InitializeComponent();
      this.adjustEllipticRuler = adjustEllipticRuler;
      orgData = orgdata;
      xCenterTextBox.Text = orgData.Xcenter.ToString();
      yCenterTextTextBox.Text = orgData.Ycenter.ToString();
      widthTextBox.Text = orgData.Xwidth.ToString();
      heightTextBox.Text = orgData.Yheight.ToString();
      ellipseASemiTxb.Text = (orgData.Xwidth / 2).ToString();
      ellipseBSemiTxb.Text = (orgData.Yheight / 2).ToString();
      gcdABTextBox.Text = CalculateGCD(0, 0).ToString();
      columnSorter = new RulerListViewColumnSorter(this.listViewReports.Columns.Count);
      this.listViewReports.ListViewItemSorter = columnSorter;
      columnSorter.SetSort(COLUMN_DISTANCE, true);
      this.listViewReports.MouseWheel += ListViewReports_MouseWheel;

      if (IniSettings.ESearchData.ExistNonZeroValue())
      {
        IniData.AssignNUD(deltaWidthNUD, IniSettings.ESearchData.GetInt(INIT_WIDTH));
        IniData.AssignNUD(deltaHeightNUD, IniSettings.ESearchData.GetInt(INIT_HEIGHT));
        float fsize = IniSettings.ESearchData.GetInt(INIT_DFONT);
        if (fsize > 50 && listViewReports != null)
        {
          listViewReports.Font = new Font(listViewReports.Font.FontFamily, fsize / 100);
        }
      }
      else
      {
        IniData.AssignNUD(deltaWidthNUD, 32);
        IniData.AssignNUD(deltaHeightNUD, 32);
      }
      if (orgData.Xwidth == 0 || orgData.Xwidth == 0)
      {
        messageToolStripStatusLabel.Text = "Ellipse with zero Width or Height cannot be simplified!";
        runButton.Enabled = false;
        messageToolStripStatusLabel.BackColor = Color.LightCyan;
        return;
      }

    }
    bool wasShown = false;
    private void RulerFormSearchGCD_Shown(object sender, EventArgs e)
    {
      if (!wasShown)
      {
        wasShown = true;
        timerUpdate.Enabled = true;
        IniSettings.GeometryRESearch.ApplyGeometryToForm(this);
        if (runButton.Enabled) runButton_Click(this, EventArgs.Empty);
      }
    }

    private void RulerFormSearchGCD_FormClosing(object sender, FormClosingEventArgs e)
    {
      bool OK = true;
      IniSettings.ESearchData.IntList.Clear();
      if (IniSettings.ESearchData.Count != INIT_WIDTH) { OK = false; }
      IniSettings.ESearchData.IntList.Add((int)deltaWidthNUD.Value);
      if (IniSettings.ESearchData.Count != INIT_HEIGHT) { OK = false; }
      IniSettings.ESearchData.IntList.Add((int)deltaHeightNUD.Value);
      if (IniSettings.ESearchData.Count != INIT_DFONT) { OK = false; }
      IniSettings.ESearchData.IntList.Add((int)Math.Round(listViewReports.Font.SizeInPoints * 100, 0)); //INIT_DFONT
      IniSettings.GeometryRESearch.StoreGeometry(this);
      if (!OK) Trace.WriteLine("DEBUG:RulerFormSearchLineGCD: Wrong a constant: INIT_WIDTH,HEIGHT,DFONT");

    }

    private void RulerFormSearchGCD_Resize(object sender, EventArgs e)
    {
      if (wasShown)
      {
        for (int i = 0; i < listViewReports.Columns.Count; i++) // auto arrange
        {
          listViewReports.Columns[i].Width = -2;
        }
      }

    }

    private class FoundGCD : RulerListViewColumnSorter.ILVColumns
    {
      public readonly int Distance;
      public readonly int Agdc, Bgdc;
      public readonly int W, H;
      public readonly int GCD;
      public FoundGCD(int dw, int dh, int gcdAB)
      {
        GCD = gcdAB;
        W = orgData.Xwidth + dw; H = orgData.Yheight + dh;
        Agdc = W / (2 * gcdAB); Bgdc = H / (2 * gcdAB);
        Distance = (int)Math.Round(Math.Sqrt(dw * dw + dh * dh) * 10, 0);
      }

      public int GetColumnValue(int index)
      {
        switch (index)
        {
          case 0: return Math.Abs(Distance);
          case 1: return Agdc; // sort for equation, its 1st cooficient
          case 2: return Bgdc;
          case 3: return GCD;
          case 4: return W;
          default: return H;
        };
      }
    }
    private int dwidth = 0;
    private int dheight = 0;
    private long _orgGDC = 0;
    private List<FoundGCD> listFound = new List<FoundGCD>();

    long CalculateGCD(int dw, int dh)
    {
      long xaxis = (orgData.Xwidth + dw) / 2;
      long yaxis = (orgData.Yheight + dh) / 2;
      if (xaxis == 0 || yaxis == 0) return 1;
      return RulerData.GCD(Math.Abs(xaxis), Math.Abs(yaxis));

    }
    long CalculateGCDnoOdd(int dw, int dh)
    {
      long width = (orgData.Xwidth + dw);
      long height = (orgData.Yheight + dh);
      if (width <= 0 || height <= 0) return 1;
      if(width==800 && height==480)
      { }
      if (width % 2 == 1 || height % 2 == 1) return 0; // rounded to lower number
      return RulerData.GCD(Math.Abs(width) / 2, Math.Abs(height) / 2);
    }

    private void Search(int dw, int dh)
    {
      long gcd = CalculateGCDnoOdd(dw, dh);
      if (gcd <= _orgGDC) return;
      listFound.Add(new FoundGCD(dw, dh, (int)gcd));
    }
    private void runButton_Click(object sender, EventArgs e)
    {
      Cursor cursor = this.Cursor;
      try
      {
        const int MAX_RESULTS = 500;
        const int PROGRESS_MAX = 10;
        dwidth = (int)deltaWidthNUD.Value;
        dheight = (int)deltaHeightNUD.Value;
        int max = Math.Max(dwidth, dheight);
        int progressStep = (2 * dwidth + 1) * (2 * dheight + 1) / 10;
        int counter = 0, progressBV = 0;
        progressBar1.Minimum = 0; progressBar1.Maximum = PROGRESS_MAX; progressBar1.Value = 0;
        _orgGDC = CalculateGCD(0, 0);
        listViewReports.Items.Clear(); listFound.Clear();
        Application.DoEvents();
        // search run in squares
        for (int ixw = -dwidth; ixw <= dwidth; ixw++)
        {
          for (int iyh = -dheight; iyh <= +dheight; iyh++)
          {
            Search(ixw, iyh);
            if (++counter >= progressStep)
            {
              counter = 0;
              if (progressBV < PROGRESS_MAX)
              {
                progressBV++; progressBar1.Value = progressBV; Application.DoEvents();
              }
            }

          }
        }
        listFound.Sort(listFoundSorter);
        int listFoundCount = listFound.Count;
        List<FoundGCD> list1 = new List<FoundGCD>();
        int lastSemiA = int.MaxValue, minDistance = int.MaxValue;

        for (int ilist = 0; ilist < listFoundCount; ilist++)
        {
          FoundGCD fg = listFound[ilist];
          if(fg.H==480 && fg.W==800)
          { }
          int semiA = fg.Agdc;
          // lower GDC and different angle, do not insert the same angle for GCD
          if (semiA != lastSemiA && fg.Distance<minDistance)
          {
            lastSemiA = semiA; minDistance = fg.Distance;
            list1.Add(fg);
            continue;
          }
          else
          {  // we add value for oposite angle
            if (fg.Distance >= minDistance) continue;
            list1.Add(fg); minDistance = fg.Distance;
          }
        }
        listFound = list1;

        try
        {
          listViewReports.BeginUpdate();
          for (int i = 0; i < listFound.Count; i++)
          {
            FoundGCD fg = listFound[i];
            ListViewItem lvi = new ListViewItem((fg.Distance / 10.0).ToString());
            lvi.Tag = fg;
            ListViewItem.ListViewSubItem lvisubx = new ListViewItem.ListViewSubItem(lvi, fg.Agdc.ToString());
            lvi.SubItems.Add(lvisubx);
            ListViewItem.ListViewSubItem lvisuby = new ListViewItem.ListViewSubItem(lvi, fg.Bgdc.ToString());
            lvi.SubItems.Add(lvisuby);
            ListViewItem.ListViewSubItem lvisubg = new ListViewItem.ListViewSubItem(lvi, fg.GCD.ToString());
            lvi.SubItems.Add(lvisubg);
            ListViewItem.ListViewSubItem lvisub01 = new ListViewItem.ListViewSubItem(lvi, fg.W.ToString());
            lvi.SubItems.Add(lvisub01);
            ListViewItem.ListViewSubItem lvisub02 = new ListViewItem.ListViewSubItem(lvi, fg.H.ToString());
            lvi.SubItems.Add(lvisub02);
            listViewReports.Items.Add(lvi);
          }
          listViewReports_InitialSort();
        }
        finally
        {
          listViewReports.EndUpdate();
        }
        for (int i = 0; i < listViewReports.Columns.Count; i++) // auto arrange
        {
          listViewReports.Columns[i].DisplayIndex = i; listViewReports.Columns[i].Width = -2;
        }
        string srep = listFoundCount > listFound.Count
  ? String.Format("{0} results. Showing only the best {1}.", listFoundCount, listFound.Count)
  : String.Format("{0} results", listFoundCount);
        messageToolStripStatusLabel.Text = srep;


        listViewReports.Invalidate();
      }
      catch (Exception ex)
      {
        Trace.WriteLine(ex.ToString());
        messageToolStripStatusLabel.Text = ex.Message;
      }
      finally
      {
        this.Cursor = cursor;
        progressBar1.Value = progressBar1.Minimum = 0;
      }
    }
    static int listFoundSorter(FoundGCD item1, FoundGCD item2)
    {
      if (item1.Agdc < item2.Agdc) return -1; // from low to high
      if (item1.Agdc > item2.Agdc) return 1;
      if (item1.Bgdc < item2.Bgdc) return -1; // from low to high
      if (item1.Bgdc > item2.Bgdc) return 1;
      if (item1.Distance < item2.Distance) return -1;
      if (item1.Distance > item2.Distance) return 1;
      return 0;
    }

    private void sendSelectedRowButton_Click(object sender, EventArgs e)
    {
      if (listViewReports.SelectedItems.Count == 0) { displayMessage("No selected row"); return; }
      ListViewItem lvi = listViewReports.SelectedItems[0];
      if (lvi == null) { displayMessage("No valid row"); return; };
      FoundGCD? fgcd = lvi.Tag as FoundGCD;
      if (fgcd == null) { displayMessage("No valid data"); return; }; ;
      if (adjustEllipticRuler == null) return;
      adjustEllipticRuler(fgcd.W, fgcd.H);
    }
    private readonly TimeSpan _messageDuration = new TimeSpan(0, 0, 10); // 10 seconds
    public enum MessageSeverity { Info, Warning, Error };
    private bool _messageHasText = false;
    private DateTime _messageStartTime = DateTime.MinValue; // 10 seconds
    private void displayMessage(string? text)
    {
      if (text == null || text.Length == 0)
      {
        _messageHasText = false; messageToolStripStatusLabel.Text = String.Empty;
        return;
      }
      displayMessage(text, MessageSeverity.Info);
    }
    private void displayMessage(string? text, MessageSeverity severity)
    {
      Color foreColor = SystemColors.WindowText;
      switch (severity)
      {
        case MessageSeverity.Error: foreColor = Color.Red; break;
        case MessageSeverity.Warning: foreColor = Color.Green; break;
      }
      messageToolStripStatusLabel.ForeColor = foreColor;
      if (text == null || text.Trim().Length == 0)
      {
        messageToolStripStatusLabel.Text = String.Empty; _messageHasText = false;
      }
      else
      {
        messageToolStripStatusLabel.Text = text; _messageHasText = true;
        _messageStartTime = DateTime.Now;
      }
    }



    private void timerUpdate_Tick(object sender, EventArgs e)
    {
      if (_messageHasText)
      {
        DateTime dt = DateTime.Now;
        if ((dt - _messageStartTime) >= _messageDuration)
          displayMessage(String.Empty);
      }

    }
    private void EllipseEquationInfoButton_Click(object sender, EventArgs e)
    {
      if (adjustEllipticRuler == null) return;
      adjustEllipticRuler(orgData.Xwidth, orgData.Yheight);

    }

    private void listViewReports_MouseDoubleClick(object sender, MouseEventArgs e)
    {
      try
      {
        ListViewHitTestInfo info = listViewReports.HitTest(e.X, e.Y);
        ListViewItem item = info.Item; item.Selected = true;
        sendSelectedRowButton_Click(sender, EventArgs.Empty);
      }
      catch (Exception) { }
    }

    private void ListViewReports_MouseWheel(object sender, MouseEventArgs e)
    {
      if (Control.ModifierKeys == Keys.Control)
      {
        try
        {
          Font f = listViewReports.Font;
          float size = f.Size + Math.Sign(e.Delta);
          listViewReports.Font = new Font(f.FontFamily, size);
          foreach (ColumnHeader ch in this.listViewReports.Columns)
          {
            ch.Width = -2;
          }

          ((HandledMouseEventArgs)e).Handled = true;
        }
        catch (Exception ex) { Trace.WriteLine(ex.Message); }
      }
    }

    private void equationHelpButton_Click(object sender, EventArgs e)
    {
      RulerFormMain.ShowEquationHelp(RulerFormEquations.ELLIPSE_TAB);

    }

    private void listViewReports_ColumnClick(object sender, ColumnClickEventArgs e)
    {
      // Determine if clicked column is already the column that is being sorted.
      int ixColumn = e.Column;

      columnSorter.SetSort(ixColumn);


      // Perform the sort with these new sort options.
      this.listViewReports.Sort();
      for (int i = 0; i < listViewReports.Columns.Count; i++)
      {
        listViewReports.Columns[i].ImageKey = null;
        listViewReports.Columns[i].ImageIndex = columnSorter.GetImageIx(i);
      }
      this.listViewReports.Invalidate();
    }

    private void listViewReports_InitialSort()
    {
      if (listViewReports == null) return;
      listViewReports.Sort();
      for (int i = 0; i < listViewReports.Columns.Count; i++)
      {
        listViewReports.Columns[i].ImageKey = null;
        listViewReports.Columns[i].ImageIndex = columnSorter.GetImageIx(i);
      }
      listViewReports.Invalidate();
    }
  }
}


