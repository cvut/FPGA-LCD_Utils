using System;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using TextAnalysis;
using static LSPtools.FormStart.DllImports;
using static System.Windows.Forms.AxHost;


namespace LSPtools
{

  public partial class FormStart : Form
  {
    static bool IsRestart = false;
    const string RESTART_ARG = "-restart";
    const string INFO_FORMAT = "V{0}.{1}";
    static string INFO = "V 1.4";
    const string TITLE = "LSP tools for FPGA";
    const string VERSION_FORMAT = "LSPtools {0}.{1}.{2}.{3}/{4} [Major.Minor.Build.MajRev/minRev]";
    static string VERSION = "LSPtools";
    private const string appGuid = "e27a0b79-5ea3-44ce-92f7-88cb60bcf970";
    [STAThread]
    static void Main()
    {
      string version = String.Empty;
      try
      {
        Assembly assembly = Assembly.GetExecutingAssembly();
        ProcessStartInfo startInfo = Process.GetCurrentProcess().StartInfo;
        IsRestart = startInfo.Arguments.Contains(RESTART_ARG);
        Version v = assembly.GetName().Version;
        VERSION = String.Format(VERSION_FORMAT, v.Major, v.Minor, v.Build, v.MajorRevision, v.MinorRevision);
        INFO = String.Format(INFO_FORMAT, v.Major, v.Minor);
      }
      catch (Exception ex) { IniData.DebugLogWriter.WriteLine(ex.ToString()); }
      // To customize application configuration such as set high DPI settings or default font,
      // see https://aka.ms/applicationconfiguration.
      //ApplicationConfiguration.Initialize();
      try
      {
        using (Mutex mutex = new Mutex(false, "Global\\" + appGuid))
        {
          if (!mutex.WaitOne(0, false))
          {
            MessageBox.Show("Instance already running", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
            DllImports.bringToFront(TITLE);
            return;
          }
          using (FormStart fs = new FormStart(true))
          {
            Application.Run(fs);
          }
          if (IsRestart)
          {
           
            {
              using (FormStart fs = new FormStart(false))
              {
                Application.Run(fs);
              }
            }
          }
        }
      }
      catch (Exception ex)
      {
        IniData.DebugLogWriter.WriteLine(ex.ToString());
      }

      try
      {
        IniData.DebugLogWriter.Flush();
        string text = IniData.DebugLog;
        if (text.Length > 0)
        {
          IniData.DebugLogWriter.WriteLine();
          IniData.DebugLogWriter.WriteLine(VERSION);
          text = IniData.DebugLog;
        }
        string? filename = IniData.GetSettingFilename();
        if (!String.IsNullOrEmpty(filename))
        {
          string log = Path.ChangeExtension(filename, ".log");
          File.WriteAllText(log, IniData.DebugLog); // we clear old file
          if (text.Length > 0)
          {
            using (FormStartException fse = new FormStartException(text))
            {
              fse.ShowDialog();
            }
          }
        }
      }
      catch (Exception) { }
    }



    static FormSplash? formSplash = null;
    TBFormMain? tbFormMain = null;
    BMForm? bmForm1 = null;
    RulerFormMain? geoFormMain = null;
    QCFormMain? checkerFormMain = null;
    FormStartAbout? formStartAbout = null;

    public FormStart(bool loadSettings)
    {
      /* Create a new text writer using the output stream, and add it to
      * the trace listeners. */
      TextWriterTraceListener myTextListener = new TextWriterTraceListener(IniData.DebugLogWriter);
      Trace.Listeners.Add(myTextListener);

      // Write output to the file.
      Trace.AutoFlush = true;
      Trace.Indent();


      try
      {
        bool settingsLoaded = false;
        if (loadSettings)
        {
          string? filename = IniData.GetSettingFilename();
          if (filename != null)
          {
            settingsLoaded = IniData.LoadSettingsFromFile(filename);
          }
          if (!settingsLoaded)
          {
            Trace.WriteLine("No setting file");
          }
        }
      }
      catch (Exception ex)
      {
        Trace.WriteLine("FormStart-constructor-settings: " + ex.ToString());
      }

      InitializeComponent();
      this.Text = TITLE;
      this.infoButton.Text = INFO;

      if (!loadSettings)
      {
        emergencyRestart.Enabled = false;
        emergencyRestart.ToolTipText = "LSP tools were already restarted";
      }


    }


    private void FormStart_FormClosing(object sender, FormClosingEventArgs e)
    {
      try
      {
        saveSettingToFile();
        if (bmForm1 != null || tbFormMain != null || geoFormMain != null || checkerFormMain != null)
        {
          using (FormStartCloseQuestion fsc = new FormStartCloseQuestion())
          {
            fsc.StartPosition = FormStartPosition.CenterParent;
            if (fsc.ShowDialog(this) == DialogResult.Yes)
            {
              if (bmForm1 != null) { bmForm1.Close(); Application.DoEvents(); }
              if (tbFormMain != null) { tbFormMain.Close(); Application.DoEvents(); }
              if (geoFormMain != null) { geoFormMain.Close(); Application.DoEvents(); }
              if (checkerFormMain != null) { checkerFormMain.Close(); Application.DoEvents(); }
            }
            else
              e.Cancel = true;
          }
        }
        Trace.Listeners.Clear();
      }
      catch (Exception) { }
    }

    private bool wasAlreadyShown = false;
    private void FormStart_Shown(object sender, EventArgs e)
    {
      if (wasAlreadyShown) return;
      wasAlreadyShown = true;

      string source = String.Empty;
      try
      {
        IniSettings.GeometryStart.Shown = true; // override stored value
        IniSettings.GeometryStart.ApplyGeometryToForm(this, true); // we must call here
        Application.DoEvents();

        this.WindowState = FormWindowState.Normal;
        this.Show();
        this.BringToFront();
        Application.DoEvents();
        formSplash = new FormSplash();
        if (formSplash != null)
        {
          formSplash.StartPosition = FormStartPosition.Manual;
          formSplash.Top = this.Top;
          formSplash.Left = this.Left;
          formSplash.FormClosed += delegate (object s1, FormClosedEventArgs e1)
           {
             formSplash = null;
           };
          formSplash.AllowTransparency = true;
          formSplash.Show(this);
        }
      }
      catch (Exception ex) { Trace.WriteLine(ex.ToString()); }

#if DEBUG
      if (formSplash != null) formSplash.TopMost = false;
#else
      //  formSplash.TopMost = true;
#endif

    }

    private void FormStart_Resize(object sender, EventArgs e)
    {
      if (this.WindowState == FormWindowState.Minimized)
      {
        Hide();
        notifyIconMain.Visible = true;
      }
    }

    private void saveSettingToFile()
    {
      string? filename = null;
      try
      {
        IniSettings.GeometryStart.StoreGeometry(this);
        filename = IniData.GetSettingFilename();
        if (filename != null && filename.Length > 0)
        {
          try
          {
            IniData.SaveSettingToFile(filename);
          }
          catch (Exception ex)
          {
            Trace.WriteLine("Saving settings" + ex.Message);
          }

        }
        else Trace.WriteLine("IniData.GetSettingFilename(): return 0 parameters");
      }
      catch (Exception ex)
      {
        Trace.WriteLine("saveSettingToFile(): " + ex.Message);
      }
      IniData.DebugLogWriter.Flush();

      if (!String.IsNullOrEmpty(filename) && IniData.DebugLogLength > 0)
      {
        try
        {
          string log = Path.ChangeExtension(filename, ".log");
          File.WriteAllText(log, IniData.DebugLog);
        }
        catch (Exception ex)
        {
          Trace.WriteLine(ex.ToString()); ;
        }
      }
    }

    private void minimizeButton_Click(object sender, EventArgs e)
    {
      this.WindowState = FormWindowState.Minimized;
    }

    private void showToolWindows()
    {
      Show();
      this.WindowState = FormWindowState.Normal;
      notifyIconMain.Visible = true;
      this.BringToFront();
    }

    private void notifyIcon_MouseDoubleClick(object sender, MouseEventArgs e)
    {
      if (e.Button == MouseButtons.Left) showToolWindows();
    }

    private void fullScreenTool_Click(object sender, EventArgs e)
    {
      showToolWindows();
    }

    private void runBitmap2MIF_Click(object sender, EventArgs e)
    {
      if (bmForm1 == null)
      {
        bmForm1 = new LSPtools.BMForm();
        bmForm1.FormClosed += delegate (object s1, FormClosedEventArgs e1)
        {
          bmForm1 = null; saveSettingToFile();
        };
      }
      if (!bmForm1.Visible) bmForm1.Show();
      if (bmForm1.WindowState == FormWindowState.Minimized)
        bmForm1.WindowState = FormWindowState.Normal;
      bmForm1.BringToFront();


    }

    private void runTestbenchViewer_Click(object sender, EventArgs e)
    {
      if (tbFormMain == null)
      {
        tbFormMain = new TBFormMain();
        tbFormMain.FormClosed += delegate (object s1, FormClosedEventArgs e1)
        {
          tbFormMain = null; saveSettingToFile();
        };
      }
      if (!tbFormMain.Visible) tbFormMain.Show();
      if (tbFormMain.WindowState == FormWindowState.Minimized)
        tbFormMain.WindowState = FormWindowState.Normal;
      tbFormMain.BringToFront();


    }

    private void runLCDGeometry_Click(object sender, EventArgs e)
    {
      if (geoFormMain == null)
      {
        geoFormMain = new RulerFormMain();
        geoFormMain.FormClosed += delegate (object s1, FormClosedEventArgs e1)
        {
          geoFormMain = null; saveSettingToFile();
        };
      }
      if (!geoFormMain.Visible) geoFormMain.Show();
      if (geoFormMain.WindowState == FormWindowState.Minimized)
        geoFormMain.WindowState = FormWindowState.Normal;
      geoFormMain.BringToFront();
    }

    private void runChecker_Click(object sender, EventArgs e)
    {
      if (checkerFormMain == null)
      {
        checkerFormMain = new QCFormMain();
        checkerFormMain.FormClosed += delegate (object s1, FormClosedEventArgs e1)
        {
          checkerFormMain = null; saveSettingToFile();
        };
      }
      if (!checkerFormMain.Visible) checkerFormMain.Show();
      if (checkerFormMain.WindowState == FormWindowState.Minimized)
        checkerFormMain.WindowState = FormWindowState.Normal;
      checkerFormMain.BringToFront();
    }

    private void infoButton_Click(object sender, EventArgs e)
    {
      if (formStartAbout == null)
      {
        formStartAbout = new LSPtools.FormStartAbout();
        formStartAbout.FormClosed += delegate (object s1, FormClosedEventArgs e1) { formStartAbout = null; };
      }
      if (!formStartAbout.Visible) formStartAbout.Show();
      if (formStartAbout.WindowState == FormWindowState.Minimized)
        formStartAbout.WindowState = FormWindowState.Normal;
      formStartAbout.BringToFront();
    }


    private void closeToolStripMenuItem_Click(object sender, EventArgs e)
    {
      Close();
    }

    #region System Menu
    //   IntPtr? sysMenuHandle = null;
    public const Int32 WM_SYSCOMMAND = 0x112;
    public const Int32 MF_SEPARATOR = 0x800;
    public const Int32 MF_BYPOSITION = 0x400;
    public const Int32 MF_BYCOMMAND = 0x0;
    public const Int32 MF_STRING = 0x0;
    public const Int32 IDM_BITMAP = 1000;
    public const Int32 IDM_MEASURE = 1001;
    public const Int32 IDM_CHECKER = 1002;
    public const Int32 IDM_VIEWER = 1003;
    // https://learn.microsoft.com/en-us/windows/win32/api/winuser/ns-winuser-menuiteminfoa
    public const Int32 MIIM_ID = 2;  // Retrieves or sets the wID member.
    public const Int32 MIIM_STRING = 0x40;  // RRetrieves or sets the dwTypeData member.
    public const Int32 MIIM_SUBMENU = 0x0;  // RRetrieves or sets the dwTypeData member.
    public const Int32 SC_RESTORE = 0xF10;

    protected override void WndProc(ref Message m)
    {
      try
      {
        if (m.Msg == WM_SYSCOMMAND && m.WParam == (IntPtr)SC_RESTORE) showToolWindows();
      }
      catch (Exception) { }
      base.WndProc(ref m);
    }

    public class DllImports
    {
      [StructLayout(LayoutKind.Sequential)]
      public struct MENUITEMINFO
      {
        public uint cbSize;
        public uint fMask;
        public uint fType;
        public uint fState;
        public uint wID;
        public IntPtr hSubMenu;
        public IntPtr hbmpChecked;
        public IntPtr hbmpUnchecked;
        public IntPtr dwItemData;
        public string dwTypeData;
        public uint cch;
        public IntPtr hbmpItem;
      }
      [DllImport("user32.dll")]
      public static extern IntPtr GetSystemMenu(IntPtr hwnd, bool bRevert);

      [DllImport("user32.dll")]
      public static extern bool InsertMenuItem(IntPtr hMenu, uint uPosition, uint uFlags, [In] ref MENUITEMINFO mii);

      [DllImport("user32")]
      public static extern UInt32 SetMenuItemBitmaps(IntPtr hMenu, uint uPosition, uint uFlags, IntPtr hBitmapUnchecked, IntPtr hBitmapChecked);

      [DllImport("USER32.DLL", CharSet = CharSet.Unicode)]
      public static extern IntPtr FindWindow(String? lpClassName, String lpWindowName);

      [DllImport("USER32.DLL")]
      public static extern bool SetForegroundWindow(IntPtr hWnd);

      [DllImport("user32.dll")]
      public static extern int SendMessage(IntPtr hWnd, int wMsg, IntPtr wParam, IntPtr lParam);
      public static void bringToFront(string title)
      {
        try
        {
          // Get a handle to the application.
          IntPtr handle = FindWindow(null, title);

          // Verify that is a running process.
          if (handle == IntPtr.Zero)
          {
            return;
          }
          SendMessage(handle, WM_SYSCOMMAND, (IntPtr)SC_RESTORE, (IntPtr)0);

          // Make  foreground application
          SetForegroundWindow(handle);
        }
        catch (Exception ex) { IniData.DebugLogWriter.WriteLine(ex.ToString()); }
      }
    }

    private void AddMenuItem(IntPtr hMenu, uint id, uint position, string text, Bitmap? icon, IntPtr? hSubMenu)
    {
      MENUITEMINFO mii = new MENUITEMINFO();
      mii.cbSize = (uint)Marshal.SizeOf(typeof(MENUITEMINFO)); //48;
      mii.fMask = (uint)MIIM_ID | (uint)MIIM_STRING | (uint)MIIM_SUBMENU;
      mii.wID = id;
      mii.dwTypeData = text;

      if (hSubMenu.HasValue)
      {
        mii.hSubMenu = hSubMenu.Value;
      }

      DllImports.InsertMenuItem(hMenu, position, (uint)MF_BYPOSITION, ref mii);

      if (icon != null)
      {
        DllImports.SetMenuItemBitmaps(hMenu, id, (uint)MF_BYCOMMAND, icon.GetHbitmap(), icon.GetHbitmap());
      }
    }
    private void FormStart_Load(object sender, EventArgs e)
    {
      try
      {
        IniGeometry.LoadMonitors();


        //IntPtr sysMenuHandle = DllImports.GetSystemMenu(this.Handle, false);
        //if (sysMenuHandle != IntPtr.Zero)
        //{
        //  //It would be better to find the position at run time of the 'Close' item, but...
        //  AddMenuItem(sysMenuHandle, IDM_BITMAP, 0, "Bitmap into VHDL", null, null);
        //  AddMenuItem(sysMenuHandle, IDM_MEASURE, 1, "Measure Rulers", null, null);
        //  AddMenuItem(sysMenuHandle, IDM_CHECKER, 2, "Checker of Quartus Project", null, null);
        //  AddMenuItem(sysMenuHandle, IDM_VIEWER, 3, "Testbench Viewer", null, null);
        //  AddMenuItem(sysMenuHandle, MF_SEPARATOR, 4, "-", null, null);
        //}
      }
      catch (Exception ex) { Trace.WriteLine(ex.ToString()); }
    }



    /*

        protected override void WndProc(ref Message m)
        {
          if (m.Msg == WM_SYSCOMMAND)
          {
            switch (m.WParam.ToInt32())
            {
              case IDM_BITMAP:
                runBitmap2MIF_Click(this, EventArgs.Empty);
                return;
              case IDM_MEASURE:
                runLCDGeometry_Click(this, EventArgs.Empty);
                return;
              case IDM_CHECKER:
                runChecker_Click(this, EventArgs.Empty);
                return;
              case IDM_VIEWER:
                runTestbenchViewer_Click(this, EventArgs.Empty);
                return;
              default:
                break;
            }
          }
          base.WndProc(ref m);
        }
    */
    #endregion System Menu

    private void emergencyRestart_Click(object sender, EventArgs e)
    {
      if (MessageBox.Show("The Emergency Restart rewrites\r\nall stored settings by default values.\r\nContinue anyway?", "Confirmation",
                          MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
      {
        IsRestart = true;
        this.Close();
      }
      //try
      //{
      //  ProcessStartInfo startInfo = Process.GetCurrentProcess().StartInfo;
      //  startInfo.FileName = Application.ExecutablePath;
      //  var exit = typeof(Application).GetMethod("ExitInternal",
      //                      System.Reflection.BindingFlags.NonPublic |
      //                      System.Reflection.BindingFlags.Static);
      //  exit.Invoke(null, null);

      //  //startInfo.Arguments = "-restart";
      //  //startInfo.UseShellExecute = true;
      //  //Process.Start(startInfo);
      //}
      //catch (Exception ex) { }
    }

  
  }
}
