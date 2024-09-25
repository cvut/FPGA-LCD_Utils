using System;
using System.Diagnostics;
using System.Globalization;
using System.Text;

namespace LSPtools
{
  public partial class TBFormMain : Form
  {
    private RGBInfoArray? rgbArray = null; //LCD pixel information

    private const string WINDOW_TITLE = "Viewer of LCD Testbench V3.3 ";
    private const string OUT_OF_IMAGE_MESSAGE = "## out of loaded image";
    private readonly Cursor defaultGraphCursor = Cursor.Current; // cursor in panelGraph
    private static StringBuilder debugTestbench = new StringBuilder();
    private Thread threadLoadFrame;
    private string currentFilename = String.Empty;
    string? firstMRUFileOnStart = null;
    private TBFormNote? formNote = null;

    const string AutoReloadSuspendedText = "Automatic reloading is suppressed during playback!";
    const string AutoReloadNormalText = "Automatic reloading of the testbench after its change";
    static bool wasDBGThreadEvent = false;
    static bool timerLoadFrameMode = false;
    public static void DBG(string text)
    {
      if (text != null && text.Length > 0) debugTestbench.AppendLine(text);
    }

    public TBFormMain()
    {
      debugTestbench = new StringBuilder(); wasDBGThreadEvent = false; timerLoadFrameMode = false;
      Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
      InitializeComponent();



      firstMRUFileOnStart = IniSettings.MRUFilesViewer.AddToMenuItem(recentTestbenchToolStripMenuItem, recentTestbenchToolStripMenuItem_Click);
      DBG("MRUtestbench=" + firstMRUFileOnStart);

      rgbArray = new RGBInfoArray();
      this.Text = WINDOW_TITLE;
      SetHScrollBar(0, imageWidth = RGBInfoArray.WIDTH_VISIBLE);
      SetVScrollBar(0, imageHeight = RGBInfoArray.HEIGHT_VISIBLE);
      ZoomText();
      if (panelGraph != null) { panelGraph.MouseWheel += panelGraph_MouseWheel; defaultGraphCursor = panelGraph.Cursor; }
      ThreadLoadFrame.FlagStopThread = false;
      threadLoadFrame = new Thread(ThreadLoadFrame.Start);
      if (cbAutoReload != null) // safety test
      {
        cbAutoReload.Checked = true;
        toolTip1.SetToolTip(cbAutoReload, AutoReloadNormalText);
      }
    }
    private void FormMain_Resize(object sender, EventArgs e)
    {
      Rectangle scroll = GetScrollBarRectangle();
      panelGraph.Invalidate();
    }

    private Font fontTextBox = SystemFonts.DefaultFont;
    private Font fontTextBoxItalic = SystemFonts.DefaultFont;
    private bool was_shown = false;

    private void FormMain_Shown(object sender, EventArgs e)
    {
      if (was_shown) return;
      was_shown = true;
      setPlayControls(0, 0);
      ThreadLoadFrame.Wait4CompleteFrame = waitForCompleteFrameToolStripMenuItem.Checked;
      IniSettings.GeometryViewer.ApplyGeometryToForm(this);
      if (firstMRUFileOnStart != null)
      {
        if (!testIfAccessible(firstMRUFileOnStart))
        {
          IniSettings.MRUFilesBitmap.RemoveFile(firstMRUFileOnStart, recentTestbenchToolStripMenuItem);
          firstMRUFileOnStart = String.Empty;
        }
      }

      Cursor cursor = this.Cursor;
      const string SPS = @"C:\SPS";
      try
      {
        SetHScrollBar(0, imageWidth = RGBInfoArray.WIDTH_VISIBLE);
        SetVScrollBar(0, imageHeight = RGBInfoArray.HEIGHT_VISIBLE);
        showVisibleToolStripMenuItem.Checked = true;
        fontTextBox = xyCoordinatesTextBox.Font;
        fontTextBoxItalic = new Font(fontTextBox, FontStyle.Italic);
        this.Cursor = Cursors.WaitCursor;

        string directory = SPS;
        if (!Directory.Exists(directory)) directory = Path.GetDirectoryName(Application.ExecutablePath);
        string filename = directory + @"\testbenchLCD.txt";
        if (firstMRUFileOnStart != null && firstMRUFileOnStart.Length > 0)
        {
          displayMessage("Opening the last testbench file " + firstMRUFileOnStart, MessageSeverity.Info);
          filename = firstMRUFileOnStart; // reloadTestbenchButton_Click(sender, e);
          displayMessage("Opening last testbench file.", MessageSeverity.Info);
        }
        if (!File.Exists(filename))
        {
          string[] files = Directory.GetFiles(directory, "*.txt", SearchOption.TopDirectoryOnly);
          if (files == null || files.Length == 0)
          {
            if (directory != SPS) return;
            directory = Path.GetDirectoryName(Application.ExecutablePath);
            files = Directory.GetFiles(directory, "*.txt", SearchOption.TopDirectoryOnly);
            if (files == null || files.Length == 0) return;
          }
          bool found = false;
          for (int i = 0; i < files.Length; i++)
          {
            using (TextReader rd = File.OpenText(files[i]))
            {
              string line = rd.ReadLine();
              if (String.IsNullOrEmpty(line)) continue;
              if (line.IndexOf("## LCD Testbench") == 0)
              {
                found = true; filename = files[i];
                break;
              }
            }
          }
          if (!found) return;
        }
        openLCDImageFile.FileName = currentFilename = filename;
        threadLoadFrame.Start();
        int watchDog = 10;
        // We wait for the beginning of threadLoadFrame activity
        do { Thread.Sleep(50); }
        while (--watchDog > 0 && ThreadLoadFrame.activityCounters.MainLoop == 0);
        applyChangesOfCurrentFilename();
      }
      catch (Exception ex)
      {
        messageToolStripStatusLabel.Text = ex.Message;
      }
      finally
      {
        this.Cursor = cursor;
      }
    }

    private void FormMain_FormClosing(object sender, FormClosingEventArgs e)
    {
      try
      {
        IniSettings.GeometryViewer.StoreGeometry(this);
        ThreadLoadFrame.FlagStopThread = true;
        int waitCount = 10;
        while (waitCount > 0) { Thread.Sleep(100); waitCount--; }
      }
      catch (Exception ex)
      {
        Trace.WriteLine(ex.ToString());
      }
      finally
      {
        if (wasDBGThreadEvent)
        {
          Trace.WriteLine("============ Testbench THREAD FAILURE - appending its messages ===========");
          Trace.WriteLine(debugTestbench.ToString());
          Trace.WriteLine("============ End of Testbench THREAD FAILURE messages ====================");
          debugTestbench.Clear(); // Not repeat 
        }
      }
    }

    /// <summary>
    /// Test if currentFilename is accessible, set windows title and reload testbench
    /// /// </summary>
    /// <returns>file was reloaded</returns>

    private bool applyChangesOfCurrentFilename()
    {
      if (testIfAccessible(currentFilename))
      {
        this.Text = WINDOW_TITLE + " - " + currentFilename;
        ThreadLoadFrame.AutoReloadTestbench = cbAutoReload.Checked;
        ThreadLoadFrame.Wait4CompleteFrame = waitForCompleteFrameToolStripMenuItem.Checked;
        reloadCurrentTestbenchFile();
        return true;
      }
      else
      {
        this.Text = WINDOW_TITLE + " - no file";
        currentFilename = String.Empty;
        return false;
      }
    }
    /// <summary>
    /// Set to InputQueue message to change file name 
    /// </summary>
    private void reloadCurrentTestbenchFile()
    {
      if (!timerLoadFrameMode)
        ThreadLoadFrame.InputQueue.EnqueueRequestAlways(
          new ThreadLoadFrame.InputQueueItem(currentFilename, ThreadLoadFrame.InputQueueItem.CmdEnum.ChangeFilename));
      else lastTimerFileAccess = DateTime.MinValue; // always reload
      timer1.Enabled = true;
    }

    private void openLCDImageToolStripMenuItem_Click(object sender, EventArgs e)
    {

      if (openLCDImageFile.ShowDialog() == System.Windows.Forms.DialogResult.OK)
      {
        if (testIfAccessible(openLCDImageFile.FileName))
        {
          currentFilename = openLCDImageFile.FileName;
          IniSettings.MRUFilesViewer.AddFile(currentFilename);
          IniSettings.MRUFilesViewer.AddToMenuItem(recentTestbenchToolStripMenuItem, recentTestbenchToolStripMenuItem_Click);
        }
        else
        {
          currentFilename = String.Empty;
          IniSettings.MRUFilesViewer.RemoveFile(openLCDImageFile.FileName, recentTestbenchToolStripMenuItem);
        }
        applyChangesOfCurrentFilename();
      }
    }

    private void recentTestbenchToolStripMenuItem_Click(object sender, EventArgs e)
    {
      ToolStripMenuItem? mi = sender as ToolStripMenuItem;
      if (mi != null)
      {
        try
        {
          string filename = mi.ToolTipText;
          if (testIfAccessible(filename))
          {
            IniSettings.MRUFilesViewer.AddFile(filename);
            IniSettings.MRUFilesViewer.AddToMenuItem(recentTestbenchToolStripMenuItem, recentTestbenchToolStripMenuItem_Click);
            currentFilename = filename;
            applyChangesOfCurrentFilename();
          }
          else
          {
            IniSettings.MRUFilesViewer.RemoveFile(filename, recentTestbenchToolStripMenuItem);
          }
        }
        catch (Exception ex)
        {
          displayMessage(ex.Message, MessageSeverity.Error);
        }

      }
    }
    /// <summary>
    /// Return true if attempt to open filename file was successful
    /// </summary>
    /// <param name="filename"></param>
    /// <returns></returns>
    bool testIfAccessible(string filename)
    {
      if (filename == null || filename.Length == 0)
      {
        displayMessage(">> open a testbench <<");
        return false;
      }
      if (!File.Exists(filename))
      {
        displayMessage("File was not found: " + filename, MessageSeverity.Error);
        return false;
      }
      try
      {
        using (FileStream fs = File.Open(filename, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
        {
          if (fs.CanRead) return true;
        }
      }
      //   T:System.IO.IOException:
      //     An I/O error occurred while performing the operation.
      catch (IOException)
      {
        displayMessage("!!! Access denied by another process that does not allow sharing: " + filename, MessageSeverity.Error);
      }
      catch (UnauthorizedAccessException)
      {
        displayMessage("!!! You do not have the required permission to access: " + filename, MessageSeverity.Error);
      }
      catch (Exception ex)
      {
        displayMessage(ex.Message, MessageSeverity.Error);
      }
      return false;
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
        if (showVisibleToolStripMenuItem.Checked)
        {
          Rectangle srcRectangle = new Rectangle(0, 0,
              imageWidth = RGBInfoArray.WIDTH_VISIBLE,
              imageHeight = RGBInfoArray.HEIGHT_VISIBLE);
          myBuffer.Graphics.DrawImage(screenBitmap, 0, 0, srcRectangle, GraphicsUnit.Pixel);
        }
        else
        {
          imageWidth = screenBitmap.Width;
          imageHeight = screenBitmap.Height;
          myBuffer.Graphics.DrawImage(screenBitmap, 0, 0);
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
      if (rectBitmap == null || rectBitmap.Width == 0 || rectBitmap.Height == 0) return;
      float w = (float)rectWindow.Width / rectBitmap.Width;
      float h = (float)rectWindow.Height / rectBitmap.Height;
      //g.ScaleTransform(w, h);
      if (w <= 0 || h <= 0) return; // not initialized yet
      if (h < w) g.ScaleTransform(h, h);
      else g.ScaleTransform(w, w);
      g.TranslateTransform(-rectBitmap.X, -rectBitmap.Top);
    }
    /// <summary>
    /// Assign ThreadLoadFrame.PlayMode and change color of text box
    /// </summary>
    /// <param name="state"></param>
    private void SetThreadPlayMode(bool state, bool force)
    {
      bool oldmode = ThreadLoadFrame.PlayMode;
      if (force || oldmode != state)
      {
        ThreadLoadFrame.SetPlayMode(state);
        cbAutoReload.ForeColor = state ? Color.Gray : SystemColors.ControlText;
        toolTip1.SetToolTip(cbAutoReload, state ? AutoReloadSuspendedText : AutoReloadNormalText);
      }
    }
    private void SetThreadPlayMode(bool state) { SetThreadPlayMode(state, false); }

    /// <summary>
    /// Reload image from previous file
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void reloadTestbenchButton_Click(object sender, EventArgs e)
    {
      try
      {
        setPauseOnPlayPauseButton(); SetThreadPlayMode(playActive, true);
        if (!applyChangesOfCurrentFilename()) return;
        if (timerLoadFrameMode)
        {
          directLoadOfFrame();
          return;
        }
        if (threadLoadFrame == null || !threadLoadFrame.IsAlive)
        {
          DBG(threadLoadFrame == null ? "threadLoadFrame==null" : "threadLoadFrame is not active");
          DBG(ThreadLoadFrame.activityCounters.ToString());
          playTimerFrameIndex = 1;
          directLoadOfFrame();
          if (!wasDBGThreadEvent) // one-time only
          {
            wasDBGThreadEvent = true;
            DBG("++ Attempt to recover thread");
            threadLoadFrame = new Thread(ThreadLoadFrame.Start);
            threadLoadFrame.Start();
            Application.DoEvents();
            int watchDog = 10;
            // We wait for the beginning of threadLoadFrame activity
            do { Thread.Sleep(50); }
            while (--watchDog > 0 && ThreadLoadFrame.activityCounters.MainLoop == 0);
            ThreadLoadFrame.AutoReloadTestbench = cbAutoReload.Checked;
            ThreadLoadFrame.Wait4CompleteFrame = waitForCompleteFrameToolStripMenuItem.Checked;
            DBG("++ Activity counters after attempt to create new thread");
            DBG(ThreadLoadFrame.activityCounters.ToString());
            applyChangesOfCurrentFilename();
            watchDog = 10;
            // We wait for thethreadLoadFrame reading input queue
            do { Thread.Sleep(10); }
            while (--watchDog > 0 && ThreadLoadFrame.InputQueue.GetCount() > 0);
            if (watchDog <= 0)
            {
              DBG("!! Activity counters after failure to create new thread");
              DBG(ThreadLoadFrame.activityCounters.ToString());
              timerLoadFrameMode = true;
            }
          }
          else
          {
            timerLoadFrameMode = true;
          }
          if (timerLoadFrameMode)
          {
            MessageBox.Show(
@"The testbench viewer cannot start its load thread.
This bug occurs only rarely on some computers,
but the cause is unknown at this time.
The program has switched to a slower timer mode.",
"Thread Failure", MessageBoxButtons.OK, MessageBoxIcon.Warning);
          }
        }
      }
      catch (Exception ex)
      {
        Trace.WriteLine(ex.ToString());
        displayMessage(ex.Message, MessageSeverity.Error);
      }
    }

    private int updateCount = 0;
    private const int UPDATE_MESSAGE = 4;

    private int playTimerFrameIndex = 1;
    DateTime lastTimerFileAccess = DateTime.MinValue;
    private int playModeResetCounter = 0;
    private void timer1_Tick(object sender, EventArgs e)
    {
      try
      {
        if (playActive) playModeResetCounter = 20;
        else
        {
          if (playModeResetCounter <= 0) SetThreadPlayMode(false);
          else playModeResetCounter--;
        }
        int watchDogCounter = 50;
        timer1.Enabled = false;
        if (timerLoadFrameMode)
        {
          FileInfo? fix = null;
          try { fix = new FileInfo(currentFilename); } catch (Exception) { }
          if (fix != null)
          {
            if (lastTimerFileAccess < fix.LastWriteTime || playActive) directLoadOfFrame();
            lastTimerFileAccess = fix.LastWriteTime;
          }
          else lastTimerFileAccess = DateTime.Now; // we do not repeat reading of wrong file
        }
        else
        {
          ThreadLoadFrame.OutputQueueItem? item = null;
          do // get newest item
          {
            item = ThreadLoadFrame.OutputQueue.NextToProcessing();
            if (item == null)
            { if (playActive) break; else return; }
          }
          while (--watchDogCounter > 0 && ThreadLoadFrame.OutputQueue.IsNextToProcessing);
          if (item != null)
          {
            rgbArray = item.RGBarray;
            screenBitmap = item.Image;
          }
          else { if (rgbArray == null) return; }

          int frameCount = rgbArray.fileDescriptor.FrameCount;
          setPlayControls(frameCount, rgbArray.FrameIndex);


          if (playActive)
          {
            if (playTimerFrameIndex <= frameCount)
            {
              if (!timerLoadFrameMode)
              {
                if (!ThreadLoadFrame.InputQueue.IsNextToProcessing && !ThreadLoadFrame.OutputQueue.IsNextToProcessing)
                {
                  //            test.AddNoRepeat(rgbArray);
                  int ok = ThreadLoadFrame.InputQueue.EnqueueRequest(
                    new ThreadLoadFrame.InputQueueItem(currentFilename, ThreadLoadFrame.InputQueueItem.CmdEnum.LoadFrame,
                                                       playTimerFrameIndex));
                  if (ok >= 0) playTimerFrameIndex++;


                }
              }
              else
              {
                playTimerFrameIndex++; directLoadOfFrame();
              }
            }
            else  setPauseOnPlayPauseButton();
            // stopping is tested according data received from the output queue 
          }
          else
          {
            string txt = item?.Message ?? String.Empty;
            if (txt.IndexOf("Warning:") > 0)
              displayMessage(txt, MessageSeverity.Warning);
            else
              displayMessage(txt, MessageSeverity.Info);
          }

          PaintImage(panelGraph.CreateGraphics());

        } // else of if (timerLoadFrameMode) -------------------------------------------------------------

        updateCount++;
        if (updateCount > UPDATE_MESSAGE)
        {
          updateCount = 0;
          if (_messageHasText)
          {
            DateTime dt = DateTime.Now;
            if ((dt - _messageStartTime) >= _messageDuration)
              displayMessage(null);
          }
        }
      }
      finally
      {
        timer1.Enabled = true;
        ThreadLoadFrame.AutoReloadTestbench = cbAutoReload.Checked;
      }

    }

    private void directLoadOfFrame()
    {
      string txt = ThreadLoadFrame.LoadTestbenchFile(currentFilename, playTimerFrameIndex, false, out rgbArray, out screenBitmap);
      if (rgbArray != null)
      {
        int frameCount = rgbArray.fileDescriptor.FrameCount;
        setPlayControls(frameCount, rgbArray.FrameIndex);
        if (playActive)
        {
          if (playTimerFrameIndex <= frameCount) playTimerFrameIndex++;
        }
        else setPauseOnPlayPauseButton();
      }
      else
      {
        if (txt.IndexOf("Warning:") > 0)
          displayMessage(txt, MessageSeverity.Warning);
        else
          displayMessage(txt, MessageSeverity.Info);
      }
      PaintImage(panelGraph.CreateGraphics());
    }


    private void cbAutoReload_CheckedChanged(object sender, EventArgs e)
    {
      ThreadLoadFrame.AutoReloadTestbench = cbAutoReload.Checked;
      if (cbAutoReload.Checked) { setPauseOnPlayPauseButton(); SetThreadPlayMode(playActive); }
    }
    /// <summary>
    /// save content of bitmap to image
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void saveImageAsBitmapToolStripMenuItem_Click(object sender, EventArgs e)
    {
      if (screenBitmap == null) { messageToolStripStatusLabel.Text = "No image loaded"; }
      try
      {
        if (saveLCDImageAs.ShowDialog() == System.Windows.Forms.DialogResult.OK && rgbArray != null)
        {
          // Create bitmap without border
          Bitmap bitmap = showVisibleToolStripMenuItem.Checked ?
                    rgbArray.CreateVisibleBitmap(panelGraph.CreateGraphics(), Application.DoEvents)
                  : rgbArray.CreateBitmap(false, panelGraph.CreateGraphics(), Application.DoEvents);
          string ext = Path.GetExtension(saveLCDImageAs.FileName).ToLower();
          switch (ext)
          {
            case ".bmp": bitmap.Save(saveLCDImageAs.FileName, System.Drawing.Imaging.ImageFormat.Bmp); break;
            case ".jpg": bitmap.Save(saveLCDImageAs.FileName, System.Drawing.Imaging.ImageFormat.Jpeg); break;
            case ".png": bitmap.Save(saveLCDImageAs.FileName, System.Drawing.Imaging.ImageFormat.Png); break;
            default: messageToolStripStatusLabel.Text = "Unsupported extension"; return;
          }
          messageToolStripStatusLabel.Text = "Image saved as " + saveLCDImageAs.FileName;
        }
      }
      catch (Exception ex)
      {
        messageToolStripStatusLabel.Text = ex.Message;
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


    private void exitToolStripMenuItem_Click(object sender, EventArgs e)
    {
      this.Close();
    }

    private void showVisibleToolStripMenuItem_Click(object sender, EventArgs e)
    {
      showVisibleToolStripMenuItem.Checked = true;
      showWholeFrameToolStripMenuItem.Checked = false;
      visibleInvisibleFrame();
    }

    private void showWholeFrameToolStripMenuItem_Click(object sender, EventArgs e)
    {
      showVisibleToolStripMenuItem.Checked = false;
      showWholeFrameToolStripMenuItem.Checked = true;
      visibleInvisibleFrame();
    }

    private void visibleInvisibleFrame()
    {
      if (showVisibleToolStripMenuItem.Checked)
      {
        SetHScrollBar(0, imageWidth = RGBInfoArray.WIDTH_VISIBLE);
        SetVScrollBar(0, imageHeight = RGBInfoArray.HEIGHT_VISIBLE);
      }
      else
      {
        SetHScrollBar(0, imageWidth = RGBInfoArray.WIDTH);
        SetVScrollBar(0, imageHeight = RGBInfoArray.HEIGHT);
      }
      PaintImage(panelGraph.CreateGraphics());
      setZoom(1);
    }

    private void lastFullFrameToolStripMenuItem_Click(object sender, EventArgs e)
    {
      waitForCompleteFrameToolStripMenuItem.Checked = true; ThreadLoadFrame.Wait4CompleteFrame = true;
      lastFrameToolStripMenuItem.Checked = false;
    }

    private void lastFrameToolStripMenuItem_Click(object sender, EventArgs e)
    {
      waitForCompleteFrameToolStripMenuItem.Checked = false; ThreadLoadFrame.Wait4CompleteFrame = false;
      lastFrameToolStripMenuItem.Checked = true;
    }

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
        int wnewWin = imageWidth + frameW;
        int hnewWin = imageHeight + frameH;
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

    #region play

    private void setPlayControls(int frameCount, int currentFrame)
    {
      try
      {
        if (frameCount == 0)
        {
          frameEndTextBox.Text = "0";
          frameNumericUpDown.Minimum = 0; frameNumericUpDown.Maximum = 0;
          frameNumericUpDown.Value = lastNumericUpDownvalue = 0;
          playProgressBar.Minimum = 0; playProgressBar.Maximum = 0; playProgressBar.Value = 0;
        }
        else
        {
          if (currentFrame > frameCount) currentFrame = frameCount;
          if (currentFrame < 1) currentFrame = 1;
          frameEndTextBox.Text = frameCount.ToString();
          frameNumericUpDown.Minimum = 1; frameNumericUpDown.Maximum = frameCount;
          frameNumericUpDown.Value = (lastNumericUpDownvalue = currentFrame);
          playProgressBar.Minimum = 1; playProgressBar.Maximum = frameCount; playProgressBar.Value = currentFrame;

        }
      }
      catch (Exception ex)
      {
        Trace.WriteLine(ex.ToString());
      }
    }
    const int PLAY_IMAGE_INDEX = 1;
    const int PAUSE_IMAGE_INDEX = 2;

    private bool playActive = false;

    private void placePlayRequestToInputQueue(int frameIndex)
    {
      SetThreadPlayMode(true);
      ThreadLoadFrame.InputQueue.EnqueueRequestAlways(
        new ThreadLoadFrame.InputQueueItem(currentFilename, ThreadLoadFrame.InputQueueItem.CmdEnum.LoadFrame,
                                           frameIndex));
    }


    private void playPauseButton_Click(object sender, EventArgs e)
    {
      if (frameNumericUpDown.Maximum <= 1) return;
      ThreadLoadFrame.Wait4CompleteFrame = waitForCompleteFrameToolStripMenuItem.Checked;
      if (playPauseButton.ImageIndex == PLAY_IMAGE_INDEX)
      {
        playPauseButton.ImageIndex = PAUSE_IMAGE_INDEX; toolTip1.SetToolTip(playPauseButton, "Play->Pause");
        playActive = true; SetThreadPlayMode(true);
        if (frameNumericUpDown.Value >= frameNumericUpDown.Maximum)
          setPlayControls((int)frameNumericUpDown.Maximum, playTimerFrameIndex = 1);

      }
      else setPauseOnPlayPauseButton();
    }
    /// <summary>
    /// Stop play and set controls
    /// </summary>
    private void setPauseOnPlayPauseButton()
    {
      playPauseButton.ImageIndex = PLAY_IMAGE_INDEX; toolTip1.SetToolTip(playPauseButton, "Pause->Play");
      playActive = false; // we will call SetThreadPlayMode(false) in timer with time delay;
    }

    private void startPlayButton_Click(object sender, EventArgs e)
    {
      if (frameNumericUpDown.Maximum <= 1) return;
      SetThreadPlayMode(true);
      placePlayRequestToInputQueue(playTimerFrameIndex = 1);
    }

    private void endPlayButton_Click(object sender, EventArgs e)
    {
      if (frameNumericUpDown.Maximum <= 1 || rgbArray == null) return;
      SetThreadPlayMode(true);
      playTimerFrameIndex = rgbArray.fileDescriptor.FrameCount;
      placePlayRequestToInputQueue(-1);
    }

    private int lastNumericUpDownvalue = 0;
    private void frameNumericUpDown_ValueChanged(object sender, EventArgs e)
    {
      if (frameNumericUpDown.Maximum <= 1) return;
      int frameIx = (int)frameNumericUpDown.Value;
      if (frameIx < 1 || frameIx == lastNumericUpDownvalue) return;
      setPauseOnPlayPauseButton();
      ThreadLoadFrame.InputQueue.ClearQueue();
      ThreadLoadFrame.OutputQueue.ClearQueue();
      placePlayRequestToInputQueue(playTimerFrameIndex = frameIx);
    }

    #endregion

    /****************************************************************************************/
    #region scroll bars

    /// <summary>
    /// Return rectangle of screenBitmap set by scrollbars
    /// </summary>
    /// <returns></returns>
    Rectangle GetScrollBarRectangle()
    {
      return new Rectangle(hScrollBarGraph.Value,
                           vScrollBarGraph.Value,
                           hScrollBarGraph.LargeChange,
                           vScrollBarGraph.LargeChange);
    }
    /// <summary>
    /// Check scrollbar values, modify them if they are out of range necessary and set results
    /// </summary>
    /// <param name="scrollBar">scrollbar</param>
    /// <param name="minimum">minimum value</param>
    /// <param name="start">required start</param>
    /// <param name="length">required length</param>
    /// <param name="maximum">maximum value</param>
    private void SetScrollbar(System.Windows.Forms.ScrollBar scrollBar,
        int minimum, int start, int length, int maximum)
    {
      const int MINDELKA = 10;
      const int LARGE2SMALL = 100;
      if (maximum < minimum + MINDELKA) return; // meaningless parameters
      if (length > maximum - minimum) length = (maximum - minimum);
      if (length < 0)
      {
        length = -length;
        if (length > maximum - minimum) length = maximum - minimum;
        start -= length;
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
      start += (int)((xdif0 - length) * fixpointratio);
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
      colZoom = (double)imageWidth / hScrollBarGraph.LargeChange;
      rowZoom = (double)imageHeight / vScrollBarGraph.LargeChange;
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
    private bool isMouseZoomRectangle = false;
    private bool isMouseHand = false;
    private int mouseZoomX0;
    private int mouseZoomY0;
    private int mouseZoomX1;
    private int mouseZoomY1;

    private void panelGraph_MouseWheel(object sender, MouseEventArgs e)
    {
      if (Control.ModifierKeys == Keys.Control)
      {
        const double WHEEL_DELTA = 120; // Vychozi hodnota ve windows
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

    private void panelGraf_MouseDown(object sender, MouseEventArgs e)
    {
      if (e.Button == MouseButtons.Left)
      {
        mouseZoomX0 = mouseZoomX1 = e.X;
        mouseZoomY0 = mouseZoomY1 = e.Y;
        if (Control.ModifierKeys != Keys.Control && Control.ModifierKeys != Keys.Shift)
        {
          if (!isMouseZoomRectangle)
          {
            mouseZoomX0 = mouseZoomX1 = e.X;
            mouseZoomY0 = mouseZoomY1 = e.Y;
            isMouseZoomRectangle = true; isMouseHand = false;
            panelGraph.Cursor = defaultGraphCursor;
            return;
          }
        }
        else
        {
          isMouseZoomRectangle = false; isMouseHand = true;
          panelGraph.Cursor = Cursors.Hand;
        }

      }
      //if (e.Button == MouseButtons.Right)
      //{
      //    Point ptTB = getXYcoordinatesFromMouse(e);
      //    lastMousePoint.X = RGBInfoArray.Index2LCDImage(ptTB.X); lastMousePoint.Y = RGBInfoArray.Index2LCDImage(ptTB.Y);
      //}
    }

    private Point lastMousePoint = new Point(-1, -1);
    private void panelGraf_MouseMove(object sender, MouseEventArgs e)
    {
      int xcolumn = 0, yrow = 0;
      bool doTransformation4Display = true;

      if (screenBitmap == null) return;
      if (e.Button == MouseButtons.Left)
      {
        if (isMouseZoomRectangle)
        {
          if (mouseZoomX0 != mouseZoomX1 || mouseZoomY0 != mouseZoomY1)
            // we delete the previous square 
            ControlPaint.DrawReversibleFrame(
                new Rectangle(panelGraph.PointToScreen(new Point(mouseZoomX0, mouseZoomY0)),
                new Size(mouseZoomX1 - mouseZoomX0,
                            mouseZoomY1 - mouseZoomY0)),
                Color.White, FrameStyle.Dashed);
          mouseZoomX1 = e.X; mouseZoomY1 = e.Y;
          if (mouseZoomX0 != mouseZoomX1 || mouseZoomY0 != mouseZoomY1)
            // drawing new square 
            ControlPaint.DrawReversibleFrame(
                    new Rectangle(panelGraph.PointToScreen(new Point(mouseZoomX0, mouseZoomY0)),
                    new Size(mouseZoomX1 - mouseZoomX0,
                    mouseZoomY1 - mouseZoomY0)),
                    Color.White, FrameStyle.Dashed);
        }
        else
        {
          if (isMouseHand)
          {
            mouseZoomX1 = e.X; mouseZoomY1 = e.Y;

            if (Math.Abs(mouseZoomX1 - mouseZoomX0) < 2
                || Math.Abs(mouseZoomY1 - mouseZoomY0) < 2)
              return;
            using (Graphics g = panelGraph.CreateGraphics())
            {
              Rectangle rect = GetScrollBarRectangle();
              SetupTransformation(g, rect, panelGraph.ClientRectangle);
              Point[] pt = new Point[2] {
                        new Point(mouseZoomX0, mouseZoomY0),
                        new Point(mouseZoomX1, mouseZoomY1) };
              g.TransformPoints(
                  System.Drawing.Drawing2D.CoordinateSpace.World,
                  System.Drawing.Drawing2D.CoordinateSpace.Device, pt
              );
              int xMove = e.X - mouseZoomX0;
              int yMove = e.Y - mouseZoomY0;
              SetVScrollBar(rect.Top - (int)(pt[1].Y - pt[0].Y), rect.Height);
              SetHScrollBar(rect.Left - (pt[1].X - pt[0].X), rect.Width);
              mouseZoomX0 = e.X;
              mouseZoomY0 = e.Y;
              xcolumn = RGBInfoArray.Index2LCDImage(pt[0].X);
              yrow = RGBInfoArray.Index2LCDImage(pt[0].Y);
              panelGraph.Invalidate();
              doTransformation4Display = false;

            }
          }
        }
      }
      if (e.Button != MouseButtons.Right)
      {
        if (doTransformation4Display)
        {
          Point ptTB = getXYcoordinatesFromMouse(e);
          xcolumn = RGBInfoArray.Index2LCDImage(ptTB.X); yrow = RGBInfoArray.Index2LCDImage(ptTB.Y);

        }
        if (RGBInfoArray.IsLCDImageIndex(xcolumn, yrow))
        {
          xyCoordinatesTextBox.Font = fontTextBox;
          rgbTextBox.Font = fontTextBox;
          xyCoordinatesTextBox.Text = String.Format("{0:000}, {1:000}", xcolumn, yrow);
          rgbTextBox.Text = rgbArray?[xcolumn, yrow].ToString();
          lastMousePoint.X = xcolumn; lastMousePoint.Y = yrow;
        }
        else
        {
          rgbTextBox.Font = fontTextBoxItalic;// = OUT_OF_IMAGE_MESSAGE; //lastMousePoint.X = -1; lastMousePoint.Y = -1;
          xyCoordinatesTextBox.Font = fontTextBoxItalic;
        }
      }

    }
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
      int column = lastMousePoint.X, row = lastMousePoint.Y;
      if (!RGBInfoArray.IsLCDImageIndex(column, row))
        sb.AppendFormat("Screen point x,y={0},{1} is outside of LCD image frame", column, row);
      else
      {
        RGBInfoArray.TB tb = rgbArray[column, row];
        tb.SetDXY(column, row);
        sb.AppendFormat("xcolumn={0}, yrow={1}, LCD_DEN='{2}', XEND='{3}', YEND='{4}'\r\n",
            column, row, tb.DE ? 1 : 0, tb.XEND ? 1 : 0, tb.YEND ? 1 : 0);
        tb.VerboseColorInfo(ref sb);
      }
      return sb.ToString();
    }

    /// <summary>
    /// Convert screen point in panelGraph  to image coordinates xcolumn, yrow
    /// </summary>
    /// <param name="e">mouse position</param>
    /// <returns></returns>
    private Point getXYcoordinatesFromMouse(MouseEventArgs e)
    {
      using (Graphics g = panelGraph.CreateGraphics())
      {
        SetupTransformation(g, GetScrollBarRectangle(), panelGraph.ClientRectangle);
        // použijeme pro přepočet
        // sem vložíme výpočet parametrů nové transformace


        /* přepočteme body v klientských souřadnicích okna (Device) na náš souřadnicový systém (World) */
        Point[] pt = new Point[1] { new Point(e.X, e.Y) };
        g.TransformPoints(
        // cílové souřadnice
        System.Drawing.Drawing2D.CoordinateSpace.World,
        // zdrojové souřadnice
        System.Drawing.Drawing2D.CoordinateSpace.Device, pt  // pole bodů pro přepočet
        );

        return pt[0];
      }
    }


    private void panelGraf_MouseUp(object sender, MouseEventArgs e)
    {
      if (screenBitmap == null) return;
      if (e.Button == MouseButtons.Left)
      {
        if (isMouseZoomRectangle)
        {
          isMouseZoomRectangle = false;
          if (Math.Abs(mouseZoomX1 - mouseZoomX0) < 2
                  || Math.Abs(mouseZoomY1 - mouseZoomY0) < 2)
            return;
          using (Graphics g = panelGraph.CreateGraphics())
          {
            SetupTransformation(g, GetScrollBarRectangle(), panelGraph.ClientRectangle);
            // We calculate parameters of the new transformation.
            // First, we adjust the parameters so that they make sense and are not too small
            if (mouseZoomX0 > mouseZoomX1)
            {
              (mouseZoomX0, mouseZoomX1) = (mouseZoomX1, mouseZoomX0); // tupple
              //int swp = mouseZoomX0; // switch
              //mouseZoomX0 = mouseZoomX1; mouseZoomX1 = swp;
            }
            if (mouseZoomY0 > mouseZoomY1)
            {
              (mouseZoomY0, mouseZoomY1) = (mouseZoomY1, mouseZoomY0);
              //int swp = mouseZoomY0; // switch
              //mouseZoomY0 = mouseZoomY1; mouseZoomY1 = swp;
            }

            if (mouseZoomX0 + 10 >= mouseZoomX1)  // limit pro max. zoom
              mouseZoomX1 = mouseZoomX0 + 10;
            if (mouseZoomY0 + 10 >= mouseZoomY1) // limit pro max. zoom
              mouseZoomY1 = mouseZoomY0 + 10;

            /* přepočteme body v klientských souřadnicích okna (Device) na náš souřadnicový systém (World) */
            Point[] pt = new Point[2] {
                        new Point(mouseZoomX0, mouseZoomY0),
                        new Point(mouseZoomX1, mouseZoomY1) };
            g.TransformPoints(
            // cílové souřadnice
            System.Drawing.Drawing2D.CoordinateSpace.World,
            // zdrojové souřadnice
            System.Drawing.Drawing2D.CoordinateSpace.Device, pt  // pole bodů pro přepočet
            );

            // nastavíme ScrollBar podle nové transformace
            SetHScrollBar(pt[0].X, pt[1].X - pt[0].X);
            SetVScrollBar(pt[0].Y, pt[1].Y - pt[0].Y);
            ZoomText();
            panelGraph.Invalidate();
          }
        }
        else
        {
          if (isMouseHand)
          {
            isMouseHand = false;
          }
        }
        panelGraph.Cursor = defaultGraphCursor;

      }
    }  // MouseUp

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
    }
    private int imageWidth = RGBInfoArray.WIDTH_VISIBLE;
    private int imageHeight = RGBInfoArray.HEIGHT_VISIBLE;



    void setZoom(double zoom)
    {
      if (screenBitmap == null) return;
      if (zoom < 1) zoom = 1;
      int xstart = hScrollBarGraph.Value;
      int xlength = (int)(imageWidth / zoom);
      SetScrollbar(hScrollBarGraph, 0, xstart, xlength, imageWidth - 1);
      int ystart = vScrollBarGraph.Value;
      int ylength = (int)(imageHeight / zoom);
      SetScrollbar(vScrollBarGraph, 0, ystart, ylength, imageHeight - 1);
      lastZoom = zoom;
      ZoomText();

      panelGraph.Invalidate();
    }



    #endregion
    /****************************************************************************************/

  }
}
