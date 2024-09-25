using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Common;
using System.Diagnostics;
using System.Diagnostics.Eventing.Reader;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime;
using System.Runtime.CompilerServices;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;

namespace LSPtools
{
  public partial class RulerFormMain : Form
  {
    private RGBInfoArray? rgbArray = null; //LCD pixel information
    private const string WINDOW_TITLE = "LCD Geometry Rulers ";
    private const string OUT_OF_IMAGE_MESSAGE = "## out of image";
    Cursor defaultGraphCursor = Cursor.Current; // cursor in panelGraph
    private static StringBuilder debugSB;
    public static StringWriter DebugWriter = new StringWriter(debugSB = new StringBuilder());
    private string currentFilename = String.Empty;

    private TBFormNote? formNote = null;
    RectRuler? rRuler = null;
    LinRuler? lRuler = null;
    EllipticRuler? eRuler = null;

    string? firstMRUFileOnStart = null;
    public RulerFormMain()
    {

      InitializeComponent();
      this.Text = WINDOW_TITLE;
      InitilizeScrollBars();
      ZoomText();
      if (panelGraph != null) { panelGraph.MouseWheel += panelGraph_MouseWheel; defaultGraphCursor = panelGraph.Cursor; }
      firstMRUFileOnStart = IniSettings.MRUFilesRulers.AddToMenuItem(recentToolStripMenuItem, recentToolStripMenuItem_Click);
    }

    private void RulerFormMain_Load(object sender, EventArgs e)
    {
      try
      {
        List<float> floatList = IniSettings.RRData.FloatList;
        rRuler = new RectRuler(rrTableLayoutPanelParams, rectTableLayoutReference, floatList);
        floatList = IniSettings.LRData.FloatList;
        lRuler = new LinRuler(lineTableLayoutParams, null, floatList);
        floatList = IniSettings.ERData.FloatList;
        eRuler = new EllipticRuler(ellipseTableLayoutPanelParams, ellipseTableLayoutPanelRef, floatList);
        setColorButtonText(visibleRRCheckBox.Checked, rrColorButton);
        setColorButtonText(visibleLineCheckBox.Checked, lineColorButton);
        setColorButtonText(visibleEllipseCheckBox.Checked, ellipseColorButton);
        if (!rRuler.IsValid) rectResetButton_Click(sender, e);
        if (!lRuler.IsValid) lineResetButton_Click(sender, e);
        if (!eRuler.IsValid) ellipseResetButton_Click(sender, e);
      }
      catch (Exception ex)
      { Trace.WriteLine(ex.ToString()); }
    }

    private void GeoFormMain_Resize(object sender, EventArgs e)
    {
      Rectangle scroll = GetScrollBarRectangle();
      panelGraph.Invalidate();
    }
    private bool was_shown = false;
    private Font fontTextBox = SystemFonts.DefaultFont;
    private Font fontTextBoxItalic = SystemFonts.DefaultFont;

    private void InitilizeScrollBars()
    {
      Rectangle sr = RulerData.transformCanvasToScrolls(0, 0, RulerData.WIDTH_VISIBLE, RulerData.HEIGHT_VISIBLE);
      SetHScrollBar(sr.Left, sr.Width); SetVScrollBar(sr.Top, sr.Width);
      setZoom(1.0);
      Application.DoEvents();
      centerScrollbars();
      panelGraph.Invalidate();
    }

    private void GeoFormMain_Shown(object sender, EventArgs e)
    {
      if (was_shown) return;
      was_shown = true;
      IniSettings.GeometryRulers.ApplyGeometryToForm(this);
      //      IniSettings.GeometryRulers.StoreShown(this);
      Cursor cursor = this.Cursor;
      //      const string SPS = @"C:\SPS";
      try
      {
         fontTextBox = xyCoordinatesTextBox.Font;
        fontTextBoxItalic = new Font(fontTextBox, FontStyle.Italic);
        this.Cursor = Cursors.WaitCursor;
        if (firstMRUFileOnStart != null)
        {
          if (!openLCDImage(firstMRUFileOnStart))
          {
            IniSettings.MRUFilesRulers.RemoveFile(firstMRUFileOnStart, recentToolStripMenuItem);
          }
        }
        //string directory = SPS;
        //if (!Directory.Exists(directory)) directory = Path.GetDirectoryName(Application.ExecutablePath);
        //string filename = directory + @"\testbenchLCD.txt";
        //if (!File.Exists(filename))
        //{
        //  string[] files = Directory.GetFiles(directory, "*.txt", SearchOption.TopDirectoryOnly);
        //  if (files == null || files.Length == 0)
        //  {
        //    if (directory != SPS) return;
        //    directory = Path.GetDirectoryName(Application.ExecutablePath);
        //    files = Directory.GetFiles(directory, "*.txt", SearchOption.TopDirectoryOnly);
        //    if (files == null || files.Length == 0) return;
        //  }
        //  bool found = false;
        //  for (int i = 0; i < files.Length; i++)
        //  {
        //    using (TextReader rd = File.OpenText(files[i]))
        //    {
        //      string line = rd.ReadLine();
        //      if (String.IsNullOrEmpty(line)) continue;
        //      if (line.IndexOf("## LCD Testbench") == 0)
        //      {
        //        found = true; filename = files[i];
        //        break;
        //      }
        //    }
        //  }
        //  if (!found) return;
        //}
        openLCDImageFile.FileName = currentFilename;
        setWindowTitle();
        InitilizeScrollBars();
      }
      catch (Exception ex)
      {
        Trace.WriteLine(ex.ToString());
        displayMessage(ex.Message, MessageSeverity.Error);
      }
      finally
      {
        this.Cursor = cursor;
      }
    }

    private void setWindowTitle()
    {
      if (!string.IsNullOrWhiteSpace(currentFilename) && File.Exists(currentFilename))
      {
        this.Text = WINDOW_TITLE + " - " + currentFilename;
        //ThreadLoadFrame.InputQueue.EnqueueRequestAlways(
        //    new ThreadLoadFrame.InputQueueItem(currentFilename, ThreadLoadFrame.InputQueueItem.CmdEnum.ChangeFilename));
      }
      else
      {
        this.Text = WINDOW_TITLE + " - no file";
      }
    }

    private bool openLCDImage(string filename)
    {
      try
      {
        Bitmap? openedBitmap = null;
        if (File.Exists(filename) && (openedBitmap = new Bitmap(filename)) != null)
        {
          currentFilename = filename;
          rgbArray = new RGBInfoArray();
          string message = rgbArray.LoadFromBitmap(openedBitmap, false);
          int nonBlack;
          screenBitmap = rgbArray.CreateBitmap(true, false, out nonBlack);
          if (nonBlack == 0) message += " Warning: All pixels have BLACK color!";
          displayMessage(message, MessageSeverity.Warning);
          panelGraph.Invalidate();
          setWindowTitle();
          IniSettings.MRUFilesRulers.AddFile(currentFilename);
          IniSettings.MRUFilesRulers.AddToMenuItem(recentToolStripMenuItem, recentToolStripMenuItem_Click);
          return true;
        }
      }
      catch (Exception ex)
      {
        Trace.WriteLine(ex.ToString());
        displayMessage(ex.Message, MessageSeverity.Error);
        screenBitmap = null;
        panelGraph.Invalidate();
        setWindowTitle();
      }
      try
      {
        IniSettings.MRUFilesRulers.RemoveFile(filename, recentToolStripMenuItem);
      }
      catch (Exception) { }
      return false;
    }
    private void openLCDImageToolStripMenuItem_Click(object sender, EventArgs e)
    {

      if (openLCDImageFile.ShowDialog() == System.Windows.Forms.DialogResult.OK)
      {
        openLCDImage(openLCDImageFile.FileName);
      }
    }

    private void recentToolStripMenuItem_Click(object sender, EventArgs e)
    {
      ToolStripMenuItem? mi = sender as ToolStripMenuItem;
      if (mi != null)
      {
        try
        {
          string filename = mi.ToolTipText;
          if (openLCDImage(filename))
          {
            IniSettings.MRUFilesRulers.AddFile(filename);
            IniSettings.MRUFilesRulers.AddToMenuItem(recentToolStripMenuItem, recentToolStripMenuItem_Click);
          }
        }
        catch (Exception ex)
        {
          displayMessage(ex.Message, MessageSeverity.Error);
        }

      }

    }

    /// <summary>
    /// Screen bitmap containing LCD image plus border
    /// </summary>
    Bitmap? screenBitmap = null;
    /// <summary>
    /// Create bitmap and paint it to screen
    /// </summary>
    /// <param name="g"></param>
    private void PaintImage(Graphics g)
    {
      if (rgbArray != null && screenBitmap != null)
      {
        DrawBitmap(g);
      }
    }
    private void panelGraph_Paint(object sender, PaintEventArgs e)
    {
      if (screenBitmap != null) DrawBitmap(e.Graphics);
    }
    /// <summary>
    /// Draw created bitmap using transformation
    /// </summary>
    /// <param name="g"></param>
    private void DrawBitmap(Graphics g)
    {
      if (screenBitmap == null || panelGraph.IsDisposed) return;
      try
      {
        BufferedGraphicsContext currentContext;
        BufferedGraphics myBuffer;
        // Gets a reference to the current BufferedGraphicsContext
        currentContext = BufferedGraphicsManager.Current;
        // Creates a BufferedGraphics instance associated with Form1, and with  
        // dimensions the same size as the drawing surface of Form1.
        myBuffer = currentContext.Allocate(g, panelGraph.DisplayRectangle);
        Rectangle imageRectangle = GetScrollBarRectangle();
        SetupTransformation(myBuffer.Graphics, imageRectangle, panelGraph.ClientRectangle);
        g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.None;
        g.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.None;
        g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBilinear;
        g.CompositingMode = System.Drawing.Drawing2D.CompositingMode.SourceOver;
        g.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
        myBuffer.Graphics.Clear(Color.Black);

 //       imageWidth = screenBitmap.Width;
 //       imageHeight = screenBitmap.Height;
        myBuffer.Graphics.DrawImage(screenBitmap, 0, 0);
        PointF[] pt1 = new PointF[] { new PointF(0, 0),
                             new PointF(RulerData.SELECT_PIXEL_SIZE, RulerData.SELECT_PIXEL_SIZE) };
        myBuffer.Graphics.TransformPoints(
          System.Drawing.Drawing2D.CoordinateSpace.World,
          System.Drawing.Drawing2D.CoordinateSpace.Device, pt1);
        float xDifferenceScreen = pt1[1].X - pt1[0].X, yDifferecneScreen = pt1[1].Y - pt1[0].Y;

        for (int i = 0; i < RulerData.MRURuler.Length; i++)
        {
          switch (i) // we draw in the order of Most Recently Used tab
          {
            case RulerData.ID_RECT:

              if (rRuler != null && rRuler.IsValid)
              {
                Rectangle rectDraw = rRuler.rect.GetDrawRectangle();
                myBuffer.Graphics.DrawRectangle(rRuler.lastPen, rectDraw);

                Point[] pt = new Point[] { rectDraw.Location, new Point(rectDraw.Right, rectDraw.Bottom) };
                myBuffer.Graphics.TransformPoints(
                  System.Drawing.Drawing2D.CoordinateSpace.Device,
                  System.Drawing.Drawing2D.CoordinateSpace.World, pt);
                // setting mouse squares
                rRuler.SetScreenPoints(pt);

                if (rRuler.isSelected)
                {
                  // calculating word coordinates
                  Rect[] selRectangles = rRuler.SetDrawPoints(new Rect(rectDraw), xDifferenceScreen, yDifferecneScreen);
                  if (selRectangles != null)
                  {
                    for (int j = 0; j < selRectangles.Length; j++)
                    {
                      Rect r = selRectangles[j];
                      // if (j != rRuler.selectedPointIndex)
                      myBuffer.Graphics.DrawRectangle(rRuler.lastPen, r.P11F.X, r.P11F.Y, r.WidthF, r.HeightF);
                      //else
                      //  myBuffer.Graphics.FillRectangle(new SolidBrush(rRuler.lastPen.Color), r.P11F.X, r.P11F.Y, r.WidthF, r.HeightF);
                    }
                  }
                }
              }
              break;
            case RulerData.ID_LINE:
              if (lRuler != null && lRuler.IsValid)
              {
                Point[] dp = lRuler.rect.GetDrawPoints();
                myBuffer.Graphics.DrawLine(lRuler.lastPen, dp[0], dp[1]);
                Point[] ptTransformed = new Point[] { dp[0], dp[1] };
                myBuffer.Graphics.TransformPoints(
                  System.Drawing.Drawing2D.CoordinateSpace.Device,
                  System.Drawing.Drawing2D.CoordinateSpace.World, ptTransformed);
                lRuler.SetScreenPoints(ptTransformed);
                if (lRuler.isSelected)
                {
                  myBuffer.Graphics.DrawEllipse(lRuler.lastPen,
                     dp[0].X - xDifferenceScreen / 2, dp[0].Y - yDifferecneScreen / 2, xDifferenceScreen, yDifferecneScreen);
                  myBuffer.Graphics.DrawEllipse(lRuler.lastPen,
                     dp[1].X - xDifferenceScreen / 2, dp[1].Y - yDifferecneScreen / 2, xDifferenceScreen, yDifferecneScreen);
                }
              }
              break;
            case RulerData.ID_ELLIPSE:

              if (eRuler != null && eRuler.IsValid)
              {
                Rectangle rectDraw = eRuler.rect.GetDrawRectangle();
                myBuffer.Graphics.DrawEllipse(eRuler.lastPen, rectDraw);
                Point[] pt = new Point[] { rectDraw.Location, new Point(rectDraw.Right, rectDraw.Bottom) };
                myBuffer.Graphics.TransformPoints(
                  System.Drawing.Drawing2D.CoordinateSpace.Device,
                  System.Drawing.Drawing2D.CoordinateSpace.World, pt);
                eRuler.SetScreenPoints(pt);

                if (eRuler.isSelected)
                {
                  // calculating word coordinates
                  Rect[] selRectangles = eRuler.SetDrawPoints(new Rect(rectDraw), xDifferenceScreen, yDifferecneScreen);
                  if (selRectangles != null)
                  {
                    for (int j = 0; j < selRectangles.Length; j++)
                    {
                      Rect r = selRectangles[j];
                      //                    if (j != eRuler.selectedPointIndex)
                      myBuffer.Graphics.DrawRectangle(eRuler.lastPen, r.P11F.X, r.P11F.Y, r.WidthF, r.HeightF);
                      //                    else
                      //                      myBuffer.Graphics.FillRectangle(new SolidBrush(eRuler.lastPen.Color), r.P11F.X, r.P11F.Y, r.WidthF, r.HeightF);
                    }
                  }
                }

              }
              break;
          }
        }

        // Renders the contents of the buffer to the specified drawing surface.
        myBuffer.Render(g);
        myBuffer.Dispose();
      }
      catch (Exception) { }
    }
    /// <summary>
    /// Set transformation of image
    /// </summary>
    /// <param name="g">Graphics</param>
    /// <param name="rectBitmap">display rectangle of bitmap</param>
    /// <param name="rectWindow"></param>
    private void SetupTransformation(Graphics g, Rectangle rectBitmap, Rectangle rectWindow)
    {
      float w = (float)rectWindow.Width / rectBitmap.Width;
      float h = (float)rectWindow.Height / rectBitmap.Height;
      //g.ScaleTransform(w, h);
      if (h < w) g.ScaleTransform(h, h);
      else g.ScaleTransform(w, w);
      g.TranslateTransform(-rectBitmap.X, -rectBitmap.Top);
    }
    /// <summary>
    /// Reload image from previous file
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnReload_Click(object sender, EventArgs e)
    {
      setWindowTitle();
    }

    /// <summary>
    /// save content of bitmap to image
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void saveImageAsBitmapToolStripMenuItem_Click(object sender, EventArgs e)
    {
      if (screenBitmap == null || rgbArray == null)
      { displayMessage("No image loaded", MessageSeverity.Info); return; }
      try
      {
        if (saveLCDImageAs.ShowDialog() == System.Windows.Forms.DialogResult.OK)
        {
          // Create bitmap without border
          Bitmap bitmap = //showVisibleToolStripMenuItem.Checked ?
                    rgbArray.CreateVisibleBitmap(panelGraph.CreateGraphics(), Application.DoEvents);
               //   : rgbArray.CreateBitmap(false, panelGraph.CreateGraphics(), Application.DoEvents);
          string ext = Path.GetExtension(saveLCDImageAs.FileName).ToLower();
          switch (ext)
          {
            case ".bmp": bitmap.Save(saveLCDImageAs.FileName, System.Drawing.Imaging.ImageFormat.Bmp); break;
            case ".jpg": bitmap.Save(saveLCDImageAs.FileName, System.Drawing.Imaging.ImageFormat.Jpeg); break;
            case ".png": bitmap.Save(saveLCDImageAs.FileName, System.Drawing.Imaging.ImageFormat.Png); break;
            default:
              displayMessage("Unsupported extension " + ext, MessageSeverity.Error); return;
          }
          displayMessage("Image saved as " + saveLCDImageAs.FileName, MessageSeverity.Warning);
        }
      }
      catch (Exception ex)
      {
        displayMessage(ex.Message, MessageSeverity.Error);
        Trace.WriteLine(ex.ToString());
        return;
      }
    }

    private void helpToolStripMenuItem_Click(object sender, EventArgs e)
    {
      TBFormHelp formHelp = new TBFormHelp();
      formHelp.ShowDialog();
    }

    bool SendNote(string text, bool bringFormToFront)
    {
      try
      {
        if (formNote == null)
        {
          formNote = new TBFormNote(text);
          formNote.FormClosed += delegate (object s1, FormClosedEventArgs e1)
          {
            formNote = null;
          };
        }
        else
        {
          formNote.AddNote(text);
        }
        if (bringFormToFront)
        {
          if (!formNote.Visible) formNote.Show(this);
          //    this.RemoveOwnedForm(formNote);
          if (formNote.WindowState == FormWindowState.Minimized)
            formNote.WindowState = FormWindowState.Normal;
          formNote.BringToFront();
        }
        return true;
      }
      catch (Exception ex)
      {
        displayMessage(ex.Message, MessageSeverity.Error);
        Trace.WriteLine(ex.ToString());
      }
      return false;
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

    private void FormMain_FormClosing(object sender, FormClosingEventArgs e)
    {
      try
      {
        IniSettings.GeometryRulers.StoreGeometry(this);
        if (rRuler != null) rRuler.StoreData(IniSettings.RRData.FloatList);
        if (lRuler != null) lRuler.StoreData(IniSettings.LRData.FloatList);
        if (eRuler != null) eRuler.StoreData(IniSettings.ERData.FloatList);
      }
      catch (Exception ex)
      {
        Trace.WriteLine(ex.ToString());
      }
    }

    private void exitToolStripMenuItem_Click(object sender, EventArgs e)
    {
      this.Close();
    }

    //private void showVisibleToolStripMenuItem_Click(object sender, EventArgs e)
    //{
    //  showVisibleToolStripMenuItem.Checked = true;
    //  showWholeFrameToolStripMenuItem.Checked = false;
    //  visibleInvisibleFrame();
    //}

    //private void showWholeFrameToolStripMenuItem_Click(object sender, EventArgs e)
    //{
    //  showVisibleToolStripMenuItem.Checked = false;
    //  showWholeFrameToolStripMenuItem.Checked = true;
    //  visibleInvisibleFrame();
    //}

    //private void visibleInvisibleFrame()
    //{
    //  if (showVisibleToolStripMenuItem.Checked)
    //  {
    //    SetHScrollBar(0, imageWidth = RulerData.WIDTH_VISIBLE);
    //    SetVScrollBar(0, imageHeight = RulerData.HEIGHT_VISIBLE);
    //  }
    //  else
    //  {
    //    SetHScrollBar(0, imageWidth = RulerData.WIDTH);
    //    SetVScrollBar(0, imageHeight = RulerData.HEIGHT);
    //  }
    //  PaintImage(panelGraph.CreateGraphics());
    //  setZoom(1);
    //}

    //private void lastFullFrameToolStripMenuItem_Click(object sender, EventArgs e)
    //{
    //  lastFullFrameToolStripMenuItem.Checked = true; ThreadLoadFrame.Wait4CompleteFrame = true;
    //  lastFrameToolStripMenuItem.Checked = false;
    //}

    //private void lastFrameToolStripMenuItem_Click(object sender, EventArgs e)
    //{
    //  lastFullFrameToolStripMenuItem.Checked = false; ThreadLoadFrame.Wait4CompleteFrame = false;
    //  lastFrameToolStripMenuItem.Checked = true;
    //}

    private void adjustWindowMenuItem_Click(object sender, EventArgs e)
    {
      if (screenBitmap == null) return;
      try
      {
        setZoom(1);
        Application.DoEvents();
        int wWin = this.Width;
        int hWin = this.Height;
        int wPG = panelGraph.ClientRectangle.Width;
        int hPG = panelGraph.ClientRectangle.Height;
        int frameW = wWin - wPG;
        int frameH = hWin - hPG;
        int wnewWin = RulerData.WIDTH_VISIBLE + frameW;
        int hnewWin = RulerData.HEIGHT_VISIBLE + frameH;
        if (wnewWin == wWin && hnewWin == hWin) return;
        this.Size = new Size(wnewWin, hnewWin);
        Application.DoEvents();
        double ratioH = (double)hWin / this.Height;
        double ratioW = (double)wWin / this.Width;
        double min = Math.Min(ratioH, ratioW);
        this.Size = new Size((int)((this.Width - frameW) * min + frameW),
                             (int)((this.Height - frameH) * min + frameH));
      }
      catch (Exception ex)
      {
        Trace.WriteLine(ex.ToString());
      }
    }


    /****************************************************************************************/
    #region scroll bars

    /// <summary>
    /// Return rectangle of screenBitmap set by scrollbars
    /// </summary>
    /// <returns></returns>
    Rectangle GetScrollBarRectangle()
    {
      return RulerData.transformScrollsToCanvas(hScrollBarGraph.Value, vScrollBarGraph.Value,
                                      hScrollBarGraph.LargeChange, vScrollBarGraph.LargeChange);
    }
    /// <summary>
    /// Check scrollbar values, modify them if they are out of range necessary and set results
    /// </summary>
    /// <param name="scrollBar">scrollbar</param>
    /// <param name="minimum">minimum value</param>
    /// <param name="start">required start</param>
    /// <param name="length">required length</param>
    /// <param name="maximum">maximum value</param>
    private void SetScrollbar(ScrollBar scrollBar,
        int minimum, int start, int length, int maximum)
    {
      const int MINDELKA = 10;
      const int LARGE2SMALL = 10;
      if (maximum < minimum + MINDELKA) return; // meaningless parameters
      if (length > maximum - minimum) length = (maximum - minimum);
      if (length < 0)
      {
        length = -length;
        if (length > maximum - minimum) length = maximum - minimum;
        start = start - length;
      }
      if (start < minimum) start = minimum;
      if (length < MINDELKA) length = MINDELKA;
      int end = start + length;
      if (end > maximum)
      {
        end = maximum; start = end - length;
        if (start < minimum)
        { start = minimum; length = end - start; }
      }
      scrollBar.Maximum = maximum;
      scrollBar.Minimum = minimum;
      scrollBar.Value = start;
      scrollBar.LargeChange = length;
      int change = length / LARGE2SMALL;
      scrollBar.SmallChange = change > 0 ? change : 1;
    }

    private void centerScrollbars()
    {
      int w = hScrollBarGraph.LargeChange;
      int h = vScrollBarGraph.LargeChange;
      int xval = RulerData.CANVAS_XIMGCENTER-w/2;
      if(xval<0) xval = 0;
      int yval = RulerData.CANVAS_YIMGCENTER-h/2;
      if (yval < 0) yval = 0;
      hScrollBarGraph.Minimum = 0;
      hScrollBarGraph.Maximum = RulerData.CANVAS_WIDTH-1;
      hScrollBarGraph.Value = xval;
      vScrollBarGraph.Minimum = 0;
      vScrollBarGraph.Maximum = RulerData.CANVAS_HEIGHT - 1;
      vScrollBarGraph.Value = yval;
    }

    private void centerImageToolStripMenuItem_Click(object sender, EventArgs e)
    {
      centerScrollbars();
      panelGraph.Invalidate();
    }

    /// <summary>
    /// Set position of horizontal scroll bar
    /// </summary>
    /// <param name="xstart"></param>
    /// <param name="xlength"></param>
    private void SetHScrollBar(int xstart, int xlength)
    {
      SetScrollbar(hScrollBarGraph, 0, xstart, xlength, imageWidth - 1);
    }

    /// <summary>
    /// Set position of vertical scrollbar
    /// </summary>
    /// <param name="ystart"></param>
    /// <param name="ylength"></param>
    private void SetVScrollBar(int ystart, int ylength)
    {
      SetScrollbar(vScrollBarGraph, 0, ystart, ylength, imageHeight - 1);
    }
    /// <summary>
    /// Calculate zoom-coefficient for scrollbars
    /// </summary>
    /// <param name="orgLength">original size for adjusting zoom</param>
    /// <param name="zoomcoef">required zoom</param>
    /// <param name="fixpointratio">fix position on image</param>
    /// <param name="start">start of image</param>
    /// <param name="length">length</param>
    private void ZoomChange(int orgLength, double zoomcoef, double fixpointratio,
                            ref int start, ref int length)
    {
      if (zoomcoef <= 1e-3) return;
      if (fixpointratio < 0) fixpointratio = 0;
      if (fixpointratio > 1) fixpointratio = 1;
      int xdif0 = length;
      length = (int)(length * zoomcoef);
      start = start + (int)((xdif0 - length) * fixpointratio);
    }
    private void ZoomHScrollBar(double zoomcoef, double fixpointratio)
    {
      int xstart = hScrollBarGraph.Value;
      int xlength = hScrollBarGraph.LargeChange;
      ZoomChange(imageWidth, zoomcoef, fixpointratio, ref xstart, ref xlength);
      SetScrollbar(hScrollBarGraph, 0, xstart, xlength, imageWidth - 1);
    }
    private void ZoomVScrollBar(double zoomcoef, double fixpointratio)
    {
      int ystart = vScrollBarGraph.Value;
      int ylength = vScrollBarGraph.LargeChange;
      ZoomChange(imageHeight, zoomcoef, fixpointratio, ref ystart, ref ylength);
      SetScrollbar(vScrollBarGraph, 0, ystart, ylength, imageHeight - 1);
    }
    private double colZoom = 1, rowZoom = 1;
    private void ZoomText()
    {
      colZoom = (double)RulerData.WIDTH_VISIBLE / hScrollBarGraph.LargeChange;
      rowZoom = (double)RulerData.HEIGHT_VISIBLE / vScrollBarGraph.LargeChange;
      zoomTextBox.Text = String.Format("{0:###}% {1:###}%", 100 * colZoom, 100 * rowZoom);
    }

    private void hScrollBarGraph_Scroll(object sender, ScrollEventArgs e)
    {
      PaintImage(panelGraph.CreateGraphics());
    }

    private void vScrollBarGraph_Scroll(object sender, ScrollEventArgs e)
    {
      PaintImage(panelGraph.CreateGraphics());
    }
    #endregion

    #region Mouse Events
    /// <summary>
    /// It contains data for drag operation
    /// </summary>
    public class DragData
    {
      /// <summary>
      /// reversible selection is drawn
      /// </summary>
      internal bool isDownRect = false;
      /// <summary>
      /// is move operation
      /// </summary>
      internal bool isHand = false;
      /// <summary>
      /// start and end point of selection
      /// </summary>
      internal Point downStart = new Point(), downEnd = new Point();
      /// <summary>
      /// Last mouse coordinate
      /// </summary>
      internal Point mousePoint = new Point(-1, -1);
      /// <summary>
      /// enumeration of selection for mouse point
      /// </summary>
      internal RulerData.DragEnum mousePointDragEnum = RulerData.DragEnum.NONE;
      /// <summary>
      /// memory of Ruler selection on mouse down button
      /// </summary>
      internal RulerData.DragEnum dragEnumOnDown = RulerData.DragEnum.NONE;
      /// <summary>
      /// dragged mouse rectangle initial when mouse down, and current
      /// </summary>
      internal Rect mouseDownRectInital, mouseDownRect;
      /// <summary>
      /// specification of selected ruler
      /// </summary>
      internal RulerData.DraggedRuler draggedRuler = RulerData.DraggedRuler.NONE;
      internal Rect rulerOrgRectangle;
      /// <summary>
      /// for string message
      /// </summary>
      public int columnDisplay = 0, rowDisplay = 0;

      internal int idSelectedRuler = -1;

    }

    private DragData dd = new DragData();

    private void panelGraph_MouseWheel(object sender, MouseEventArgs e)
    {
      if (Control.ModifierKeys == Keys.Control)
      {
        const double WHEEL_DELTA = 120; // Default value in Windows
        const double ZOOMSTEP = 0.95;
        double delta = e.Delta;
        if (delta == 0) return;
        double zoom = Math.Pow(ZOOMSTEP, delta / WHEEL_DELTA);
        //if (zoom < 0)
        //{
        //    zoom = lastZoom - 0.25; if (zoom < 0.25) zoom = 0.25;
        //}
        //else
        //{
        //    zoom = lastZoom + 0.25; if (zoom > 16) zoom = 16;
        //}
        //zoom=Math.Round(zoom * 4, 0) / 4; lastZoom = zoom;
        //   setZoom(zoom);
        ZoomHScrollBar(zoom, (double)e.X / panelGraph.ClientRectangle.Width);
        ZoomVScrollBar(zoom, 1 - (double)e.Y / panelGraph.ClientRectangle.Height);
        ZoomText();
        PaintImage(panelGraph.CreateGraphics());
      }
    }

    private void panelGraph_MouseEnter(object sender, EventArgs e)
    {
      panelGraph.Focus();
    }
    private void vScrollBarGraph_MouseEnter(object sender, EventArgs e)
    {
      //  vScrollBarGraph.Focus();
    }
    private void hScrollBarGraph_MouseEnter(object sender, EventArgs e)
    {
      //  hScrollBarGraph.Focus();
    }

    private void setPanelGrafCursor(RulerData.DragEnum mp)
    {
      switch (mp)
      {
        case RulerData.DragEnum.NONE:
        default:
          panelGraph.Cursor = Cursors.Cross;
          break;
        case RulerData.DragEnum.SIZEALLSTART:
        case RulerData.DragEnum.SIZEALLEND:
          panelGraph.Cursor = Cursors.SizeAll;
          break;
        case RulerData.DragEnum.TOPLEFT:
        case RulerData.DragEnum.BOTTOMRIGHT:
          panelGraph.Cursor = Cursors.SizeNWSE;
          break;
        case RulerData.DragEnum.TOPRIGHT:
        case RulerData.DragEnum.BOTTOMLEFT:
          panelGraph.Cursor = Cursors.SizeNESW;
          break;

        case RulerData.DragEnum.RIGHT:
        case RulerData.DragEnum.LEFT:
          panelGraph.Cursor = Cursors.SizeWE;
          break;
        case RulerData.DragEnum.TOP:
        case RulerData.DragEnum.BOTTOM:
          panelGraph.Cursor = Cursors.SizeNS;
          break;
        case RulerData.DragEnum.HAND:
          panelGraph.Cursor = Cursors.Hand;
          break;
      }
    }

    private void panelGraf_MouseDown(object sender, MouseEventArgs e)
    {
      _mouseMoveProcessing = false;
      if (e.Button == MouseButtons.Left)
      {
        if (rRuler == null || lRuler == null || eRuler == null) return; // not initialized yet
        if (dd.mousePointDragEnum != RulerData.DragEnum.NONE
          || dd.draggedRuler == RulerData.DraggedRuler.NONE)
        {
          dd.dragEnumOnDown = dd.mousePointDragEnum;
          int idSel = dd.mousePointDragEnum == RulerData.DragEnum.NONE ? RulerData.ID_SELECTIONS : -1;
          switch (dd.draggedRuler)
          {
            case RulerData.DraggedRuler.RECTANGLE:
              rulersTabControl.SelectedIndex = idSel = RulerData.ID_RECT;
              dd.rulerOrgRectangle = rRuler.rect;
              goto default;
            case RulerData.DraggedRuler.LINE:
              rulersTabControl.SelectedIndex = idSel = RulerData.ID_LINE;
              dd.rulerOrgRectangle = lRuler.rect;
              goto default;
            case RulerData.DraggedRuler.ELLIPSE:
              rulersTabControl.SelectedIndex = idSel = RulerData.ID_ELLIPSE;
              dd.rulerOrgRectangle = eRuler.rect;
              goto default;
            default:
              dd.idSelectedRuler = idSel;
              if (idSel >= 0) selectRulerByNumber(idSel, RulerData.SelRuler_enum.True);
              break;
          }

          //if (Control.ModifierKeys == Keys.Shift) 
          //  _lastMouseDownPositionEnum = MeasurementRuler.MousePosition.INSIDE;
          if (!dd.isDownRect)
          {
            Control control = (Control)sender;
            dd.downStart = control.PointToScreen(e.Location);
            dd.downEnd = dd.downStart;
            dd.isHand = (dd.dragEnumOnDown == RulerData.DragEnum.HAND);
            setPanelGrafCursor(dd.dragEnumOnDown);
            using (Graphics g = panelGraph.CreateGraphics())
            {
              Rectangle rect = GetScrollBarRectangle();
              SetupTransformation(g, rect, panelGraph.ClientRectangle);
              Point[] pt;
              switch (dd.draggedRuler)
              {
                default:
                case RulerData.DraggedRuler.RECTANGLE:
                  pt = rRuler.rect.GetDrawPoints();
                  break;
                case RulerData.DraggedRuler.LINE:
                  pt = lRuler.rect.GetDrawPoints();
                  break;
                case RulerData.DraggedRuler.ELLIPSE:
                  pt = eRuler.rect.GetDrawPoints();
                  break;
              }
              g.TransformPoints(
                   System.Drawing.Drawing2D.CoordinateSpace.Device,
                   System.Drawing.Drawing2D.CoordinateSpace.World, pt);
              dd.mouseDownRectInital = new Rect(control.PointToScreen(pt[0]), control.PointToScreen(pt[1]), dd.draggedRuler);
              dd.mouseDownRect = dd.mouseDownRectInital;
              g.TransformPoints(
                  System.Drawing.Drawing2D.CoordinateSpace.World,
                  System.Drawing.Drawing2D.CoordinateSpace.Device, pt);
            }
            return;
          }
        }
        else
        {
          dd.isDownRect = false; dd.isHand = false;
          panelGraph.Cursor = Cursors.Cross;
        }
      }
    }


    bool _mouseMoveProcessing = false;
    private void panelGraf_MouseMove(object sender, MouseEventArgs e)
    {
      if (_mouseMoveProcessing) return;
      try
      {
        _mouseMoveProcessing = true;

        bool doTransformation4Display = true; // if the transformations are not active yet.
                                              // safety check
        if (screenBitmap == null || rRuler == null || lRuler == null || eRuler == null)
        {
          if (panelGraph.Cursor != Cursors.Cross) panelGraph.Cursor = Cursors.Cross;
          return;
        }

        if (e.Button != MouseButtons.Left)
        {
          int i = 0; dd.mousePointDragEnum = RulerData.DragEnum.NONE;
          dd.draggedRuler = RulerData.DraggedRuler.NONE;
          do
          {
            switch (RulerData.MRURuler[i])
            {
              case 0:
                dd.mousePointDragEnum = rRuler.testMousePosition(e.Location);
                if (dd.mousePointDragEnum != RulerData.DragEnum.NONE)
                  dd.draggedRuler = RulerData.DraggedRuler.RECTANGLE;
                break;
              case 1:
                dd.mousePointDragEnum = lRuler.testMousePosition(e.Location);
                if (dd.mousePointDragEnum != RulerData.DragEnum.NONE)
                  dd.draggedRuler = RulerData.DraggedRuler.LINE;
                break;
              case 2:
                dd.mousePointDragEnum = eRuler.testMousePosition(e.Location);
                if (dd.mousePointDragEnum != RulerData.DragEnum.NONE)
                  dd.draggedRuler = RulerData.DraggedRuler.ELLIPSE;
                break;
            }
            i++;
          } while (i < RulerData.MRURuler.Length
                    && dd.mousePointDragEnum == RulerData.DragEnum.NONE);

          setPanelGrafCursor(dd.mousePointDragEnum);
        }
        else
        {
          // clear previous
          if (dd.isDownRect && dd.mouseDownRect.IsValid)
          {
            switch (dd.draggedRuler)
            {
              case RulerData.DraggedRuler.ELLIPSE:
              case RulerData.DraggedRuler.RECTANGLE:
                ControlPaint.DrawReversibleFrame(dd.mouseDownRect.GetRectangle(), Color.White, FrameStyle.Dashed);
                break;
              case RulerData.DraggedRuler.LINE:
                // clear previous
                ControlPaint.DrawReversibleLine(dd.mouseDownRect.P11, dd.mouseDownRect.P33, Color.White);
                break;
            }
            dd.isDownRect = false;
          }

          dd.isHand = dd.mousePointDragEnum == RectRuler.DragEnum.HAND;
          Control control = (Control)sender;
          // Calculate the startPoint by using the PointToScreen         // method.
          dd.downEnd = control.PointToScreen(e.Location);
          int xDistance = dd.downEnd.X - dd.downStart.X;
          int yDistance = dd.downEnd.Y - dd.downStart.Y;
          switch (dd.dragEnumOnDown)
          {
            case RulerData.DragEnum.LEFT:
            case RulerData.DragEnum.RIGHT: yDistance = 0; break;
            case RulerData.DragEnum.TOP:
            case RulerData.DragEnum.BOTTOM: xDistance = 0; break;
          }
          dd.mouseDownRect.ApplyChange(dd, xDistance, yDistance, Control.ModifierKeys);
          if (dd.mouseDownRect.IsValid)
          {
            switch (dd.draggedRuler)
            {
              case RulerData.DraggedRuler.ELLIPSE:
              case RulerData.DraggedRuler.RECTANGLE:
                // clear previous
                ControlPaint.DrawReversibleFrame(dd.mouseDownRect.GetRectangle(), Color.White, FrameStyle.Dashed);
                break;
              case RulerData.DraggedRuler.LINE:
                // clear previous
                ControlPaint.DrawReversibleLine(dd.mouseDownRect.P11, dd.mouseDownRect.P33, Color.White);
                break;
            }
            dd.isDownRect = true;
          }
          PointF[] pt = new PointF[3]
            { control.PointToClient(dd.mouseDownRect.P11), control.PointToClient(dd.mouseDownRect.P33),
            e.Location };
          using (Graphics g = panelGraph.CreateGraphics())
          {
            Rectangle rect = GetScrollBarRectangle();
            SetupTransformation(g, rect, panelGraph.ClientRectangle);
            g.TransformPoints(
               System.Drawing.Drawing2D.CoordinateSpace.World,
               System.Drawing.Drawing2D.CoordinateSpace.Device, pt);
            switch (dd.draggedRuler)
            {
              case RulerData.DraggedRuler.RECTANGLE:
                rRuler.RedefineDrawing(pt, dd.draggedRuler);
                if (dd.isHand) rRuler.rect.AdjustWidthHeight(dd.rulerOrgRectangle);
                break;
              case RulerData.DraggedRuler.LINE:
                lRuler.RedefineDrawing(pt, dd.draggedRuler);
                if (dd.isHand) lRuler.rect.AdjustWidthHeight(dd.rulerOrgRectangle);
                break;
              case RulerData.DraggedRuler.ELLIPSE:
                eRuler.RedefineDrawing(pt, dd.draggedRuler);
                if (dd.isHand) eRuler.rect.AdjustWidthHeight(dd.rulerOrgRectangle);
                break;
            }
          }
          dd.columnDisplay = Rect.toInt(pt[2].X);
          dd.rowDisplay = Rect.toInt(pt[2].Y);
          doTransformation4Display = false;
        }

        if (doTransformation4Display)
        {
          using (Graphics g = panelGraph.CreateGraphics())
          {
            SetupTransformation(g, GetScrollBarRectangle(), panelGraph.ClientRectangle);
            Point[] pt = new Point[1] { e.Location };
            g.TransformPoints(
             System.Drawing.Drawing2D.CoordinateSpace.World,
             System.Drawing.Drawing2D.CoordinateSpace.Device, pt);
            dd.columnDisplay = pt[0].X;
            dd.rowDisplay = pt[0].Y;

          }
        }
        //     if (RulerData.IsLCDImageIndex(columnDisplay, rowDisplay))

        xyCoordinatesTextBox.Text = String.Format("{0:000}, {1:000}",
                           RulerData.Index2LCDImage(dd.columnDisplay), RulerData.Index2LCDImage(dd.rowDisplay));
        rgbTextBox.Text = rgbArray?[dd.columnDisplay, dd.rowDisplay].ToStringReduced();
      }
      catch (Exception) { }
      finally { _mouseMoveProcessing = false; }
    }

    private void panelGraf_MouseUp(object sender, MouseEventArgs e)
    {
      _mouseMoveProcessing = false; // safety clear
      if (dd.isDownRect && dd.mouseDownRect.IsValid)
      {
        bool isHand = dd.isHand;
        switch (dd.draggedRuler)
        {
          case RulerData.DraggedRuler.ELLIPSE:
          case RulerData.DraggedRuler.RECTANGLE:
            // clear previous
            ControlPaint.DrawReversibleFrame(dd.mouseDownRect.GetRectangle(), Color.White, FrameStyle.Dashed);
            break;
          case RulerData.DraggedRuler.LINE:
            // clear previous
            ControlPaint.DrawReversibleLine(dd.mouseDownRect.P11, dd.mouseDownRect.P33, Color.White);
            break;
        }
        dd.isDownRect = false;
      }
      selectRulerByNumber(dd.idSelectedRuler, RulerData.SelRuler_enum.True);
      panelGraph.Invalidate();
      panelGraph.Cursor = Cursors.Default;
    }  // MouseUp


    private void copyPixelInfoToClipboardToolStripMenuItem_Click(object sender, EventArgs e)
    {
      string s = verboseMouseCoordinates();
      Clipboard.SetText(s);
    }

    private void sendPixelInfoToInfoWindowToolStripMenuItem_Click(object sender, EventArgs e)
    {
      SendNote(verboseMouseCoordinates(), true);
    }

    private string verboseMouseCoordinates()
    {
      if (rgbArray == null) return String.Empty;
      StringBuilder sb = new StringBuilder();
      int column = RulerData.Index2LCDImage(dd.columnDisplay);
      int row = RulerData.Index2LCDImage(dd.rowDisplay);
      if (!RulerData.IsLCDImageIndex(column, row))
        sb.AppendFormat("Screen point x,y={0},{1} is outside of LCD image frame", column, row);
      else
      {
        RGBInfoArray.TB tb = rgbArray[column, row];
        tb.SetDXY(column, row);
        sb.AppendFormat("xcolumn={0}, yrow={1} ", column, row);
        tb.VerboseColorInfo(ref sb);
      }
      return sb.ToString();
    }


    double lastZoom = 1.0;
    private void zoomSet_Click(object sender, EventArgs e)
    {
      ToolStripMenuItem? tsmi = sender as ToolStripMenuItem;
      double zoom = 1;
      if (tsmi != null)
      {
        string? s = tsmi.Tag as string;
        if (s != null && s.Length > 0)
        {
          switch (s[0])
          {
            default: zoom = 1; break;
            case '3': zoom = 0.5; break;
            case '4': zoom = 0.75; break;
            case '5': zoom = 1.00; break;
            case '6': zoom = 1.25; break;
            case '7': zoom = 1.5; break;
            case '8': zoom = 2; break;
          }
        }

      }
      setZoom(zoom);
      Application.DoEvents();
      centerScrollbars();
      panelGraph.Invalidate();
    }
    private const int imageWidth = RulerData.CANVAS_WIDTH;
    private int imageHeight = RulerData.CANVAS_HEIGHT;

    void setZoom(double zoom)
    {
      if (screenBitmap == null) return;
      if (zoom < 0.25) zoom = 0.25;
      int xstart = hScrollBarGraph.Value;
      int xlength = (int)(RulerData.WIDTH_VISIBLE / zoom);
      SetScrollbar(hScrollBarGraph, 0, xstart, xlength, imageWidth - 1);
      int ystart = vScrollBarGraph.Value;
      int ylength = (int)(RulerData.HEIGHT_VISIBLE / zoom);
      SetScrollbar(vScrollBarGraph, 0, ystart, ylength, imageHeight - 1);
      lastZoom = zoom;
      ZoomText();
      panelGraph.Invalidate();
    }

    #endregion

    //int TryConvertToNumber(string s, out bool error)
    //{
    //  CultureInfo provider = CultureInfo.InvariantCulture;
    //  int n; error = false;
    //  if (s == null || s.Length == 0) return 0;
    //  s = s.Trim();
    //  if (s.StartsWith("0x", StringComparison.OrdinalIgnoreCase)
    //      || s.StartsWith("X\"", StringComparison.OrdinalIgnoreCase))
    //  {
    //    s = s.Substring(2);
    //    if (s.Length == 0) return 0;
    //    if (int.TryParse(s, NumberStyles.HexNumber, provider, out n)) return n;
    //    else { error = true; return 0; }
    //  }
    //  if (s[0] == '#' || s[0] == 'X')
    //  {
    //    s = s.Substring(1);
    //    if (s.Length == 0) return 0;
    //    if (int.TryParse(s, NumberStyles.HexNumber, provider, out n)) return n;
    //    else { error = true; return 0; }
    //  }
    //  if (int.TryParse(s, NumberStyles.Integer, provider, out n)) return n;
    //  else
    //  {
    //    error = true; return 0;
    //  }
    //}
    #region Square

    private void rectNUD_ValueChanged(object sender, EventArgs e)
    {
      if (rRuler != null)
      {
        if (rRuler.NumericUpDown_ValueChanged(sender, e))
          this.panelGraph.Invalidate();
      }
    }

    private void rectRB_CheckedChanged(object sender, EventArgs e)
    {
      if (rRuler != null)
      {
        if (rRuler.RB_CheckedChanged(sender, e))
          this.panelGraph.Invalidate();
      }
    }


    private void setColorButtonText(bool isChecked, Button colorButton)
    {
      if (!isChecked) colorButton.Text = "HIDDEN";
      else colorButton.Text = "";

      Application.DoEvents(); // before Invalidate(), we need updating colorButton and visibleCheckBox
      panelGraph.Invalidate();
    }

    private void lineNUP_ValueChanged(object sender, EventArgs e)
    {
      if (lRuler != null)
      {
        if (lRuler.NumericUpDown_ValueChanged(sender, e))
          this.panelGraph.Invalidate();
      }
    }



    private void visibleRRCheckBox_CheckedChanged(object sender, EventArgs e)
    {
      setColorButtonText(visibleRRCheckBox.Checked, rrColorButton);
    }

    private void visibleLineCheckBox_CheckedChanged(object sender, EventArgs e)
    {
      setColorButtonText(visibleLineCheckBox.Checked, lineColorButton);
    }
    private void visibleEllipseCheckBox_CheckedChanged(object sender, EventArgs e)
    {
      setColorButtonText(visibleEllipseCheckBox.Checked, ellipseColorButton);
    }

    private void rulersTabControl_SelectedIndexChanged(object sender, EventArgs e)
    {
      int ix = rulersTabControl.SelectedIndex;
      int[] order = new int[RulerData.MRU_LENGTH];
      int top = RulerData.MRURuler[0];
      if (top == ix) return; // no change
      order[0] = ix;
      // We create MRU by IF, it is faster and more robust.
      switch (ix)
      {
        case RulerData.ID_RECT:
          if (top == RulerData.ID_LINE)
          { order[1] = RulerData.ID_LINE; order[2] = RulerData.ID_ELLIPSE; }
          else
          {
            order[1] = RulerData.ID_ELLIPSE; order[2] = RulerData.ID_LINE;
          }
          break;
        case RulerData.ID_LINE:
          if (top == RulerData.ID_RECT)
          { order[1] = RulerData.ID_RECT; order[2] = RulerData.ID_ELLIPSE; }
          else
          {
            order[1] = RulerData.ID_ELLIPSE; order[2] = RulerData.ID_RECT;
          }
          break;
        case RulerData.ID_ELLIPSE:
          if (top == RulerData.ID_RECT)
          { order[1] = RulerData.ID_RECT; order[2] = RulerData.ID_LINE; }
          else
          {
            order[1] = RulerData.ID_LINE; order[2] = RulerData.ID_RECT;
          }
          break;
      }
      order.CopyTo(RulerData.MRURuler, 0);
    }

    private void rrColorButton_Click(object sender, EventArgs e)
    {
      if (rRuler == null) return;
      colorDialog1.Color = rrColorButton.BackColor;
      if (colorDialog1.ShowDialog(this) == DialogResult.OK)
      {
        rRuler.setColorButtonPen(colorDialog1.Color);
        this.panelGraph.Invalidate();
      }
    }
    private void rrCropButton_Click(object sender, EventArgs e)
    {
      if (rRuler == null || !rRuler.rect.IsValid)
      {
        displayMessage("Nothing to do - no rectangle specified!", MessageSeverity.Warning);
        return;
      }
      if (rgbArray == null || rgbArray.Count == 0)
      {
        displayMessage("No bitmap background loaded!", MessageSeverity.Warning);
        return;
      }
      int imem, jmem;
      try
      {
        rRuler.rect.Normalize();
        int x0 = rRuler.rect.P11.X, y0 = rRuler.rect.P11.Y;
        int x1 = rRuler.rect.P13.X, y1 = rRuler.rect.P33.Y;
        int bw, bh;
        Bitmap crop = new Bitmap(bw = x1 - x0, bh = y1 - y0, PixelFormat.Format24bppRgb);
        if (crop != null)
        {
          for (int j = 0; j < bh; j++)
          {
            jmem = j;
            for (int i = 0; i < bw; i++)
            {
              imem = i;
              RGBInfoArray.TB tb = rgbArray[i + x0, j + y0];
              crop.SetPixel(i, j, tb.ToColor());
            }
          }
          if (saveLCDImageAs.ShowDialog() == DialogResult.OK)
          {
            string ext = Path.GetExtension(saveLCDImageAs.FileName).ToLower();

            switch (ext)
            {
              case ".bmp":
                crop.Save(saveLCDImageAs.FileName, System.Drawing.Imaging.ImageFormat.Bmp);
                break;
              case ".jpg":
                crop.Save(saveLCDImageAs.FileName, System.Drawing.Imaging.ImageFormat.Jpeg);
                break;
              case ".png":
                crop.Save(saveLCDImageAs.FileName, System.Drawing.Imaging.ImageFormat.Png);
                break;
              default:
                displayMessage("Unsupported extension: " + ext, MessageSeverity.Error);
                return;
            }
            displayMessage("Image saved as " + saveLCDImageAs.FileName, MessageSeverity.Info);
          }
        }


      }
      catch (Exception ex)
      {
        Trace.WriteLine(ex.ToString());
        displayMessage(ex.Message, MessageSeverity.Error);
      }
    }

    private void rectResetButton_Click(object sender, EventArgs e)
    {
      if (rRuler == null) return;
      rrRB11.Checked = true;
      for (int i = 0; i <= RulerData.MRH; i++)
      {
        rRuler.numericUpDowns[i].Value = 100;
      }
    }

    private void lineResetButton_Click(object sender, EventArgs e)
    {
      if (lRuler == null) return;
      lRuler.numericUpDowns[RulerData.MRX].Value = 200;
      lRuler.numericUpDowns[RulerData.MRY].Value = 100;
      lRuler.numericUpDowns[RulerData.MRXEND].Value = 300;
      lRuler.numericUpDowns[RulerData.MRYEND].Value = 200;
    }



    private void lineColorButton_Click(object sender, EventArgs e)
    {
      if (lRuler == null) return;
      colorDialog1.Color = lineColorButton.BackColor;
      if (colorDialog1.ShowDialog(this) == DialogResult.OK)
      {
        lRuler.setColorButtonPen(colorDialog1.Color);
        this.panelGraph.Invalidate();
      }
    }

    private void ellipseRB_CheckedChanged(object sender, EventArgs e)
    {
      if (eRuler == null) return; // not initialized yet
      eRuler.RB_CheckedChanged(sender, e);
    }

    private void ellipseResetButton_Click(object sender, EventArgs e)
    {
      if (eRuler == null) return; // not initialized yet
      eRuler.numericUpDowns[RulerData.MRX].Value = 240;
      eRuler.numericUpDowns[RulerData.MRY].Value = 400;
      eRuler.numericUpDowns[RulerData.MRXEND].Value = 100;
      eRuler.numericUpDowns[RulerData.MRYEND].Value = 100;
    }

    private void ellipseColorButton_Click(object sender, EventArgs e)
    {
      if (eRuler == null) return;
      colorDialog1.Color = ellipseColorButton.BackColor;
      if (colorDialog1.ShowDialog(this) == DialogResult.OK)
      {
        eRuler.setColorButtonPen(colorDialog1.Color);
        this.panelGraph.Invalidate();
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


    private void selectRulerByNumber(int idRuler, RulerData.SelRuler_enum setState)
    {

      for (int i = RulerData.ID_RECT; i <= RulerData.ID_ELLIPSE; i++)
      {
        if (i == idRuler)
        {
          switch (i)
          {
            case RulerData.ID_RECT:
              if (rRuler != null && rRuler.IsValid) rRuler.selectButton_Clicked(setState);
              break;
            case RulerData.ID_LINE:
              if (lRuler != null && lRuler.IsValid) lRuler.selectButton_Clicked(setState);
              break;
            case RulerData.ID_ELLIPSE:
              if (eRuler != null && eRuler.IsValid) eRuler.selectButton_Clicked(setState);
              break;
          }
        }
        else
        {
          switch (i)
          {
            case RulerData.ID_RECT:
              if (rRuler != null && rRuler.IsValid) rRuler.selectButton_Clicked(RulerData.SelRuler_enum.False);
              break;
            case RulerData.ID_LINE:
              if (lRuler != null && lRuler.IsValid) lRuler.selectButton_Clicked(RulerData.SelRuler_enum.False);
              break;
            case RulerData.ID_ELLIPSE:
              if (eRuler != null && eRuler.IsValid) eRuler.selectButton_Clicked(RulerData.SelRuler_enum.False);
              break;
          }

        }
      }
      panelGraph.Invalidate();
    }

    private void rrSelectButton_Click(object sender, EventArgs e)
    {
      selectRulerByNumber(RulerData.ID_RECT, RulerData.SelRuler_enum.Toggle);
    }

    private void lineSelectButton_Click(object sender, EventArgs e)
    {
      selectRulerByNumber(RulerData.ID_LINE, RulerData.SelRuler_enum.Toggle);
    }

    private void ellipseSelectButton_Click(object sender, EventArgs e)
    {
      selectRulerByNumber(RulerData.ID_ELLIPSE, RulerData.SelRuler_enum.Toggle);
    }



    private void ellipseNUD_ValueChanged(object sender, EventArgs e)
    {
      if (eRuler != null)
      {
        if (eRuler.NumericUpDown_ValueChanged(sender, e))
          this.panelGraph.Invalidate();
      }
    }
    #endregion
    /****************************************************************************************/

  }
}