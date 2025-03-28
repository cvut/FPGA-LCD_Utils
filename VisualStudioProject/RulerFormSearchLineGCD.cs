using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static FpgaLcdUtils.QuartusProject;
using static RGBInfoArray;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace FpgaLcdUtils
{
  public partial class RulerFormSearchLineGCD : Form
  {
    RulerListViewColumnSorter columnSorter;
    private const int SHORT_QLINE_WARNING = 128 * 128;
    private const int INIT_DX = 0;
    private const int INIT_DY = 1;
    private const int INIT_DANGLE = 2;
    private const int INIT_DFONT = 3;

    public struct OrgLineData
    {
      public bool isDX2DY;
      public int X1, Y1, X2, Y2;
      public int DX { get { return X2 - X1; } }
      public int DY { get { return Y2 - Y1; } }

      public int NewX2(int dx) { return X2 + dx; }
      public int NewY2(int dy) { return Y2 + dy; }

      public int NewDX(int dx) { return X2 + dx - X1; }
      public int NewDY(int dy) { return Y2 + dy - Y1; }


      public int Angle100(int dx, int dy)
      {
        int x2n = X2 + dx, y2n = Y2 + dy;
        if (x2n == X1) return 90;
        if (y2n == Y1) return 0;
        double tan = isDX2DY ? (x2n - X1) / (double)(y2n - Y1) : (y2n - Y1) / (double)(x2n - X1);
        double rad = tan < 0.4 ? tan : Math.Atan(tan); // small angles approximated
        int angle100 = (int)Math.Round((18000 / Math.PI) * rad, 0);
        if (angle100 > 9000) angle100 -= 18000; // we select line angle in positive plane
        if (angle100 < -9000) angle100 += 18000;
        return angle100;
      }

      public int QSIZE2 { get { return DX * DX + DY * DY; } }
    }
    static OrgLineData orgLineData;
    static int _orgAngle100 = 0;
    RulerFormMain.AdjustLinearRulerDelegate? adjustLinearRuler = null;

    private const int COLUMN_ANGLE = 0, COLUMN_EQ = 1, COLUMN_X2 = 2, COLUMN_Y2 = 3,
      COLUMN_DX = 4, COLUMN_DY = 5, COLUMN_GCD = 6;
    public RulerFormSearchLineGCD(OrgLineData lineData, RulerFormMain.AdjustLinearRulerDelegate adjustlinearRuler)
    {
      InitializeComponent();
      this.adjustLinearRuler = adjustlinearRuler;
      orgLineData = lineData;
      orgX1TextBox.Text = orgLineData.X1.ToString();
      orgY1TextBox.Text = orgLineData.Y1.ToString();
      orgX2TextBox.Text = orgLineData.X2.ToString();
      orgY2TextBox.Text = orgLineData.Y2.ToString();
      lineDXTextBox.Text = (orgLineData.X2 - orgLineData.X1).ToString();
      lineDYTextBox.Text = (orgLineData.Y2 - orgLineData.Y1).ToString();
      gcdDXDYTextBox.Text = CalculateGCD(0, 0).ToString();
      degreesNUD.Maximum = 30.0M;
      degreesNUD.Increment = 0.5M;
      degreesNUD.Minimum = 0.1M;

      orgLineData.isDX2DY = (Math.Abs(orgLineData.DX) <= Math.Abs(orgLineData.DY));

      if (orgLineData.DX == 0) _orgAngle100 = 9000;
      else
      {
        if (orgLineData.DY == 0) _orgAngle100 = 0;
        else
          _orgAngle100 = orgLineData.Angle100(0, 0);
      }
      columnSorter = new RulerListViewColumnSorter(this.listViewReports.Columns.Count);
      this.listViewReports.ListViewItemSorter = columnSorter;
      columnSorter.SetSort(COLUMN_EQ, true);
      this.listViewReports.MouseWheel += ListViewReports_MouseWheel;

      if (IniSettings.LSearchData.ExistNonZeroValue())
      {
        IniData.AssignNUD(deltaDXNUD, IniSettings.LSearchData.GetInt(INIT_DX));
        IniData.AssignNUD(deltaDYNUD, IniSettings.LSearchData.GetInt(INIT_DY));
        IniData.AssignNUD(degreesNUD, IniSettings.LSearchData.GetInt(INIT_DANGLE) / 10.0);
        float fsize = IniSettings.LSearchData.GetInt(INIT_DFONT);
        if (fsize > 50 && listViewReports != null)
        {
          listViewReports.Font = new Font(listViewReports.Font.FontFamily, fsize / 100);
        }
      }
      if (orgLineData.DX == 0 || orgLineData.DY == 0)
      {
        messageToolStripStatusLabel.Text = "Equations of vertical or horizontal lines cannot be simplified!";
        runButton.Enabled = false;
        messageToolStripStatusLabel.BackColor = Color.LightCyan;
        return;
      }
      if (orgLineData.QSIZE2 < SHORT_QLINE_WARNING)
      {
        messageToolStripStatusLabel.Text = "Longer line segments can sometimes give better results!";
        messageToolStripStatusLabel.BackColor = Color.LightPink;
      }
      runButton.Enabled = true;
    }

    bool wasShown = false;
    private void RulerFormSearchGCD_Shown(object sender, EventArgs e)
    {
      if (!wasShown)
      {
        wasShown = true;
        timerUpdate.Enabled = true;
        IniSettings.GeometryRLSearch.ApplyGeometryToForm(this);
        if (runButton.Enabled) runButton_Click(this, EventArgs.Empty);
      }
    }
    private void RulerFormSearchLineGCD_FormClosing(object sender, FormClosingEventArgs e)
    {
      bool OK = true;
      IniSettings.LSearchData.IntList.Clear();
      if (IniSettings.LSearchData.Count != INIT_DX) { OK = false; }
      IniSettings.LSearchData.IntList.Add((int)deltaDXNUD.Value);
      if (IniSettings.LSearchData.Count != INIT_DY) { OK = false; }
      IniSettings.LSearchData.IntList.Add((int)deltaDYNUD.Value);
      if (IniSettings.LSearchData.Count != INIT_DANGLE) { OK = false; }
      IniSettings.LSearchData.IntList.Add((int)(Math.Round(10 * degreesNUD.Value, 0)));
      if (IniSettings.LSearchData.Count != INIT_DFONT) { OK = false; }
      IniSettings.LSearchData.IntList.Add((int)Math.Round(listViewReports.Font.SizeInPoints * 100, 0)); //INIT_DFONT
      IniSettings.GeometryRLSearch.StoreGeometry(this);
      if (!OK) Trace.WriteLine("DEBUG:RulerFormSearchLineGCD: Wrong a constant: INIT_WIDTH,DY,DANGLE,DFONT");
    }
    private void RulerFormSearchLineGCD_Resize(object sender, EventArgs e)
    {
      if (wasShown)
      {
        for (int i = 0; i < listViewReports.Columns.Count; i++) // auto arrange
        {
          listViewReports.Columns[i].Width = -2;
        }
      }
    }

    private class FoundLineGCD : RulerListViewColumnSorter.ILVColumns
    {
      public readonly int AngleDiff100;
      public readonly int X2, Y2, DXG, DYG;
      public readonly int GCD;
      public FoundLineGCD(int dx, int dy, int gcdDXDY, int angleDiff)
      {
        if (gcdDXDY < 1) gcdDXDY = 1; // check for safety only
        this.X2 = orgLineData.NewX2(dx);
        this.Y2 = orgLineData.NewY2(dy);
        this.DXG = orgLineData.NewDX(dx) / gcdDXDY;
        this.DYG = orgLineData.NewDY(dy) / gcdDXDY;
        this.GCD = gcdDXDY;
        this.AngleDiff100 = angleDiff;
      }

      public int GetColumnValue(int index)
      {
        switch (index)
        {
          case 0: return Math.Abs(AngleDiff100);
          case 1: return Math.Abs(DYG); // sort for equation, its 1st cooficient
          case 2: return X2;
          case 3: return Y2;
          case 4: return X2 - orgLineData.X1;
          case 5: return Y2 - orgLineData.X2;
          default: return GCD;
        };
      }
    }
    private long _orgGDC = 0;
    private long _maxDegree = 0;
    private List<FoundLineGCD> listFound = new List<FoundLineGCD>();

    long CalculateGCD(int dx, int dy)
    {
      long DX = (orgLineData.DX + dx);
      long DY = (orgLineData.DY + dy);
      if (DX == 0 || DY == 0) return 1;
      return RulerData.GCD(Math.Abs(DX), Math.Abs(DY));
    }


    private void Search(int dx, int dy)
    {
      long gcd = CalculateGCD(dx, dy);
      if (gcd <= _orgGDC) return;
      int angleDiff100 = RulerFormSearchLineGCD.orgLineData.Angle100(dx, dy) - RulerFormSearchLineGCD._orgAngle100;
      if (Math.Abs(angleDiff100) > _maxDegree) return;
      listFound.Add(new FoundLineGCD(dx, dy, (int)gcd, angleDiff100));
    }
    private void runButton_Click(object sender, EventArgs e)
    {
      Cursor cursor = this.Cursor;
      try
      {
        this.Cursor = Cursors.WaitCursor;
        const int MAX_RESULTS = 500;
        const int PROGRESS_MAX = 10;
        int dxval = (int)deltaDXNUD.Value;
        int dyval = (int)deltaDYNUD.Value;
        _maxDegree = (int)(degreesNUD.Value * 100);

        int progressStep = (2 * dxval + 1) * (2 * dyval + 1) / 10;
        int counter = 0, progressBV = 0;
        progressBar1.Minimum = 0; progressBar1.Maximum = PROGRESS_MAX; progressBar1.Value = 0;
        _orgGDC = CalculateGCD(0, 0);
        listViewReports.Items.Clear(); listFound.Clear();
        Application.DoEvents();
        for (int ixw = -dxval; ixw <= +dxval; ixw++)
        {
          for (int iyh = -dyval; iyh <= +dyval; iyh++)
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
        int listFoundCount = listFound.Count;
        // we delete same angle with less
        listFound.Sort(listFoundSorter);
        List<FoundLineGCD> list1 = new List<FoundLineGCD>();
        int signAngle = 0, lastAbsAngle = int.MaxValue;
        bool plusMinusAngle = false;
        for (int ilist = 0; ilist < listFoundCount; ilist++)
        {
          FoundLineGCD fg = listFound[ilist];
          int absAngle = Math.Abs(fg.AngleDiff100);
          // lower GDC and different angle, do not insert the same angle for GCD
          if (absAngle != lastAbsAngle)
          {
            signAngle = Math.Sign(fg.AngleDiff100); plusMinusAngle = false; lastAbsAngle = absAngle;
            list1.Add(fg);
            continue;
          }
          else
          {  // we add value for oposite angle
            if (plusMinusAngle || Math.Sign(fg.AngleDiff100) == signAngle) continue;
            list1.Add(fg); plusMinusAngle = true;
          }
        }
        // listFound = list1;

        // we delete same GCD with bigger angle
        list1.Sort(listFoundSorter2);
        listFound.Clear();
        int lastXDG = int.MinValue;
        lastAbsAngle = int.MaxValue;
        for (int ilist = 0; ilist < list1.Count && listFound.Count < MAX_RESULTS; ilist++)
        {
          FoundLineGCD fg = list1[ilist];
          int absAngle = Math.Abs(fg.AngleDiff100);
          // lower GDC and different angle, do not insert the same angle for GCD
          if (lastXDG != fg.DXG && absAngle < lastAbsAngle)
          {
            lastXDG = fg.DXG; lastAbsAngle = absAngle;
            listFound.Add(fg); continue;
          }
        }

        try
        {
          listViewReports.BeginUpdate();
          for (int i = 0; i < listFound.Count; i++)
          {
            FoundLineGCD fg = listFound[i];
            ListViewItem lvi = new ListViewItem((fg.AngleDiff100 / 100.0).ToString());
            lvi.Tag = fg;
            string eq = LinRuler.ToLineEquation(orgLineData.X1, orgLineData.Y1, fg.X2, fg.Y2);
            ListViewItem.ListViewSubItem lvisub0 = new ListViewItem.ListViewSubItem(lvi, eq);
            lvi.SubItems.Add(lvisub0);
            ListViewItem.ListViewSubItem lvisub01x = new ListViewItem.ListViewSubItem(lvi, fg.X2.ToString());
            lvi.SubItems.Add(lvisub01x);
            ListViewItem.ListViewSubItem lvisub01y = new ListViewItem.ListViewSubItem(lvi, fg.Y2.ToString());
            lvi.SubItems.Add(lvisub01y);
            ListViewItem.ListViewSubItem lvisub02 = new ListViewItem.ListViewSubItem(lvi, (fg.X2 - orgLineData.X1).ToString());
            lvi.SubItems.Add(lvisub02);
            ListViewItem.ListViewSubItem lvisub03 = new ListViewItem.ListViewSubItem(lvi, (fg.Y2 - orgLineData.Y1).ToString());
            lvi.SubItems.Add(lvisub03);
            ListViewItem.ListViewSubItem lvisub04 = new ListViewItem.ListViewSubItem(lvi, fg.GCD.ToString());
            lvi.SubItems.Add(lvisub04);
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
        if (orgLineData.QSIZE2 < SHORT_QLINE_WARNING)
        {
          messageToolStripStatusLabel.Text = srep + " Longer line segments can give better results!";
          messageToolStripStatusLabel.BackColor = Color.LightPink;
        }
        else
        {
          messageToolStripStatusLabel.Text = srep;
          messageToolStripStatusLabel.BackColor = SystemColors.ControlLight;
        }
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

    static int listFoundSorter(FoundLineGCD item1, FoundLineGCD item2)
    {
      int xa = Math.Abs(item1.AngleDiff100), xb = Math.Abs(item2.AngleDiff100);
      if (xa < xb) return -1; // low to high
      if (xa > xb) return 1;
      int dxg1 = Math.Abs(item1.DXG), dxg2 = Math.Abs(item2.DXG);
      if (dxg1 < dxg2) return -1;
      if (dxg1 > dxg2) return 1;
      int dyg1 = Math.Abs(item1.DYG), dyg2 = Math.Abs(item2.DYG);
      if (dyg1 < dyg2) return -1;
      if (dyg1 > dyg2) return 1;
      return 0;
    }
    static int listFoundSorter2(FoundLineGCD item1, FoundLineGCD item2)
    {
      int dxg1 = Math.Abs(item1.DXG), dxg2 = Math.Abs(item2.DXG);
      if (dxg1 < dxg2) return -1;
      if (dxg1 > dxg2) return 1;
      int xa = Math.Abs(item1.AngleDiff100), xb = Math.Abs(item2.AngleDiff100);
      if (xa < xb) return -1; // low to high
      if (xa > xb) return 1;
      int dyg1 = Math.Abs(item1.DYG), dyg2 = Math.Abs(item2.DYG);
      if (dyg1 < dyg2) return -1;
      if (dyg1 > dyg2) return 1;
      if (item1.GCD < item2.GCD) return 1; // from high to low
      if (item1.GCD > item2.GCD) return -1;
      return 0;
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



    private void sendSelectedRowButton_Click(object sender, EventArgs e)
    {
      if (listViewReports.SelectedItems.Count == 0) { displayMessage("No selected row"); return; }
      ListViewItem lvi = listViewReports.SelectedItems[0];
      if (lvi == null) { displayMessage("No valid row"); return; };
      FoundLineGCD? fgcd = lvi.Tag as FoundLineGCD;
      if (fgcd == null) { displayMessage("No valid data"); return; }; ;
      if (adjustLinearRuler == null) return;
      adjustLinearRuler(fgcd.X2, fgcd.Y2);
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


    private void LineInitialCoordinatesButton_Click(object sender, EventArgs e)
    {
      if (adjustLinearRuler == null) return;
      adjustLinearRuler(orgLineData.X2, orgLineData.Y2);

    }


    private void equationHelpButton_Click(object sender, EventArgs e)
    {
      RulerFormMain.ShowEquationHelp(RulerFormEquations.LINE_TAB);
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

    private void listViewReport_ColumnClick(object sender, ColumnClickEventArgs e)
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


