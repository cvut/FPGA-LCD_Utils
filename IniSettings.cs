using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Reflection.Emit;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace LSPtools
{
  /// <summary>
  /// Collection of all settings
  /// </summary>
  class IniSettings
  {
    public static IniGeometry GeometryStart = new IniGeometry("Start");
    public static IniGeometry GeometryBMP = new IniGeometry("Bitmap");
    public static IniGeometry GeometryQCheck = new IniGeometry("QCheck");
    public static IniGeometry GeometryRulers = new IniGeometry("Rules");
    public static IniGeometry GeometryViewer = new IniGeometry("Viewer");
    public static IniMRUList MRUFilesBitmap = new IniMRUList("Bitmap");
    public static IniMRUList MRUFilesRulers = new IniMRUList("Rulers");
    public static IniMRUList MRUFilesViewer = new IniMRUList("Viewer");
    public static IniMRUList MRUFilesQCheck = new IniMRUList("QCheck");
    public static IniMRUList MRUFilesQCAdjustVWF = new IniMRUList("QCAvwf");
    public static IniSettingsData RRData = new IniSettingsData("RRuler");
    public static IniSettingsData LRData = new IniSettingsData("LRuler");
    public static IniSettingsData ERData = new IniSettingsData("ERuler");

    private static readonly RootSettings[] rootSettings = new RootSettings[]
    { GeometryStart, GeometryBMP, GeometryRulers, GeometryQCheck, GeometryViewer,
      MRUFilesBitmap, MRUFilesRulers, MRUFilesQCheck, MRUFilesQCAdjustVWF, MRUFilesViewer,
      RRData, LRData, ERData};


    public static bool Test(string name, string token)
    {
      token = token.Trim();
      name = name.Trim();
      bool OK = false;

      for (int i = 0; i < rootSettings.Length; i++)
      {
        OK = rootSettings[i].TokenWasStored(name, token);
        if (OK) return true;
      }
      return OK;
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="writer"></param>
    internal static void WriteToXml(System.Xml.XmlTextWriter writer)
    {
      for (int i = 0; i < rootSettings.Length; i++)
      {
        rootSettings[i].WriteToXml(writer);
      }
    }
  }

  /// <summary>
  /// Common base class of all settings
  /// </summary>
  public abstract class RootSettings
  {
    public readonly string Root;
    public readonly string SettingsName;
    private readonly string _rootSettingsName;
    public RootSettings(string root, string settingsName)
    {
      if (root == null || root.Length == 0) throw new ArgumentNullException("root in RootSettings");
      if (settingsName == null || settingsName.Length == 0) throw new ArgumentNullException("settingsName in RootSettings");
      this.Root = root;
      this.SettingsName = settingsName; this._rootSettingsName = root + settingsName;
    }
    public string RootSettingsName { get { return _rootSettingsName; } }
    public bool CompareNameToken(string name, string token)
    {
      return CompareNameToken(name, token, true);
    }

    public bool CompareNameToken(string name, string token, bool onlyEqual)
    {
      if (String.IsNullOrEmpty(name) || String.IsNullOrEmpty(token)) return false;
      if (!name.StartsWith(_rootSettingsName, StringComparison.InvariantCultureIgnoreCase)) return false;
      return !onlyEqual || _rootSettingsName.Length == name.Length;
    }
    /// <summary>
    /// It tests if xml attribute name belong to processing parameter
    /// </summary>
    /// <param name="name"></param>
    /// <param name="token"></param>
    /// <returns></returns>
    internal abstract bool TokenWasStored(string name, string token);
    internal abstract void WriteToXml(System.Xml.XmlTextWriter writer);
  }


  public class IniGeometry : RootSettings
  {
    public Point WinLocation;
    public Size WinSize;
    public bool Shown = false;
    private FormWindowState initialState;
    private bool _wasApplyCall = false;
    public bool WasApplyCall { get { return _wasApplyCall; } }

    public static Rectangle PrimaryMonitor = new Rectangle();
    public static Rectangle SecondaryMonitor = new Rectangle();
    public static void LoadMonitors()
    {
      Screen primary = Screen.PrimaryScreen;
      Screen? secondary = null;
      for (int i = 0; i < Screen.AllScreens.Length; i++)
      {
        if (Screen.AllScreens[i] != primary) { secondary = Screen.AllScreens[i]; break; }
      }
      IniGeometry.PrimaryMonitor = primary.Bounds;
      IniGeometry.SecondaryMonitor = secondary != null ? secondary.Bounds : new Rectangle();
    }


    public IniGeometry(string settingsName) : base("GEO", settingsName)
    {

    }
    public void ApplyGeometryToForm(Form formIn)
    {
      ApplyGeometryToForm(formIn, false);
    }

    public void ApplyGeometryToForm(Form formIn, bool forceNormal)
    {
      _wasApplyCall = true;
      if (WinSize.Height < formIn.MinimumSize.Height) WinSize.Height = formIn.MinimumSize.Height;
      if(WinSize.Width< formIn.MinimumSize.Width) WinSize.Width=formIn.MinimumSize.Width;
      if (initialState == FormWindowState.Normal || initialState == FormWindowState.Minimized)
      {
        bool locOkay = GeometryIsBizarreLocation(WinLocation, ref WinSize);
        bool sizeOkay = GeometryIsBizarreSize(WinSize);

        if (locOkay == true && sizeOkay == true)
        {
          formIn.Location = WinLocation;
          if (formIn.FormBorderStyle == FormBorderStyle.Sizable)
            formIn.Size = WinSize;
          formIn.StartPosition = FormStartPosition.Manual;
          formIn.WindowState = forceNormal ? FormWindowState.Normal : initialState;
        }
        else
        {
          if (sizeOkay == true)
          {
            if (formIn.FormBorderStyle == FormBorderStyle.Sizable)
              formIn.Size = WinSize;
            formIn.WindowState = initialState;
          }
        }
      }
      else
      {
        if (initialState == FormWindowState.Maximized)
        {
          formIn.Location = new Point(100, 100);
          formIn.StartPosition = FormStartPosition.Manual;
          formIn.WindowState = FormWindowState.Maximized;
        }
      }
    }
    private static bool GeometryIsBizarreLocation(Point loc, ref Size size)
    {
      bool locOkay = true;
      if (loc.X < 0 || loc.Y < 0)
      {
        locOkay = false;
      }
      else
      {
        if (loc.X + size.Width > PrimaryMonitor.Width + SecondaryMonitor.Width)
        {
          int width = PrimaryMonitor.Width + SecondaryMonitor.Width - loc.X;
          if (width > 100) size.Width = width;
          else locOkay = false;
        }
        if (loc.Y + size.Height > Screen.PrimaryScreen.Bounds.Height)
        {
          int height = Screen.PrimaryScreen.Bounds.Height - loc.X;
          if (height > 100) size.Height = height;
          else locOkay = false;
        }
      }
      return locOkay;
    }
    /*
     * Basically, the size is okay whenever it is smaller than the screen. 
     * If the window is positioned partly off-screen, but it is small enough to fit on the screen,
     * it is simply relocated, and the size doesn't change.
     */
    private static bool GeometryIsBizarreSize(Size size)
    {
      return (size.Height <= Screen.PrimaryScreen.WorkingArea.Height &&
          size.Width <= Screen.PrimaryScreen.WorkingArea.Width);
    }

    public override string ToString()
    {

      string thisWindowGeometry = WinLocation.X.ToString() + "|" +
          WinLocation.Y.ToString() + "|" +
          WinSize.Width.ToString() + "|" +
          WinSize.Height.ToString() + "|" +
          initialState.ToString() + (Shown ? "|Shown" : "");
      return thisWindowGeometry;
    }

    public void StoreGeometry(Form mainForm)
    {
      //   if (!WasApplyCall) return;
      WinSize = mainForm.Size;
      WinLocation = mainForm.Location;
      initialState = mainForm.WindowState;
      _wasApplyCall = true;
    }
    public void StoreShown(Form mainForm)
    {
      Shown = (mainForm != null && !mainForm.IsDisposed && mainForm.Visible);
      if (Shown && mainForm != null) StoreGeometry(mainForm);
    }
    internal override bool TokenWasStored(string name, string token)
    {
      try
      {
        if (CompareNameToken(name, token))
        {
          string hisWindowGeometry = token;
          string[] numbers = token.Split('|');
          if (numbers.Length < 5) return false;
          string windowString = numbers[4];
          WinLocation = new Point(int.Parse(numbers[0]),
              int.Parse(numbers[1]));
          WinSize = new Size(int.Parse(numbers[2]),
              int.Parse(numbers[3]));
          switch (numbers[4].ToLower())
          {
            case "minimized": initialState = FormWindowState.Minimized; break;
            case "maximized": initialState = FormWindowState.Maximized; break;
            default: initialState = FormWindowState.Normal; break;
          }
          Shown = (numbers.Length > 5 && numbers[5].StartsWith("Shown", StringComparison.InvariantCultureIgnoreCase));
        }
      }
      catch (Exception) { }
      return false;
    }

    internal override void WriteToXml(System.Xml.XmlTextWriter writer)
    {
      if (this.WinSize.Width >= 10 && this.WinSize.Height >= 10)
      {
        writer.WriteStartElement(Root);
        writer.WriteAttributeString(RootSettingsName, this.ToString());
        writer.WriteEndElement();
      }

    }
  }



  class IniMRUList : RootSettings
  {
    const int MRUnumber = 6;
    private readonly List<string> MRUlist = new List<string>();

    public string Top { get { return MRUlist.Count > 0 ? MRUlist[0] : String.Empty; } }

    public IniMRUList(string settingsName) : base("MRU", settingsName)
    {
    }
    public string? AddToMenuItem(ToolStripMenuItem recentToolStripMenuItem, EventHandler eventHandler)
    {
      return AddToMenuItem(recentToolStripMenuItem, eventHandler, false);
    }
    public string? AddToMenuItem(ToolStripMenuItem recentToolStripMenuItem, EventHandler eventHandler, bool addWithFullPath)
    {
      recentToolStripMenuItem.DropDownItems.Clear(); //clear all recent list from menu
      string? first = null;
      for (int i = 0; i < MRUlist.Count; i++)
      {
        string fullFilename = MRUlist[i];
        if (File.Exists(fullFilename))
        {
          string sadd = addWithFullPath ? fullFilename : Path.GetFileName(fullFilename);
          ToolStripMenuItem menuItemRecentFile = new ToolStripMenuItem(sadd, null, eventHandler);  //create new menu for each fullFilename in list
          menuItemRecentFile.ToolTipText = fullFilename;
          recentToolStripMenuItem.DropDownItems.Add(menuItemRecentFile); //add the menu to "recent" submenu
          first ??= fullFilename; //if (first == null) first = fullFilename;
        }
      } //for
      return first;
    }

    public void AddFile(string filename)
    {
      int ix = MRUlist.IndexOf(filename); //prevent duplication on recent list
      if (ix == 0) return;
      if (ix > 0) MRUlist.Remove(filename);
      MRUlist.Insert(0, filename); //insert given path into list
      while (MRUlist.Count > MRUnumber) //keep list number not exceeded given value
      {
        MRUlist.RemoveAt(MRUlist.Count - 1);
      }

    }


    internal override bool TokenWasStored(string name, string token)
    {
      if (CompareNameToken(name, token, false))
      {
        MRUlist.Add(token); return true;

      }
      return false;
    }

    internal override void WriteToXml(System.Xml.XmlTextWriter writer)
    {
      if (MRUlist.Count > 0)
      {
        writer.WriteStartElement(Root);
        for (int i = 0; i < MRUlist.Count; i++)
        {
          writer.WriteAttributeString(RootSettingsName + (i + 1).ToString(), MRUlist[i]);
        }
        writer.WriteEndElement();
      }

    }
    /// <summary>
    /// If file exists in MRU file list, it will be deleted.
    /// </summary>
    /// <param name="sf"></param>
    /// <param name="mRUFiles_ToolStripMenuItem"></param>
    internal void RemoveFile(string sf, ToolStripMenuItem mRUFiles_ToolStripMenuItem)
    {
      int ix = MRUlist.IndexOf(sf);
      if (ix < 0) return; // not in list
      MRUlist.RemoveAt(ix);
      for (int i = 0; i < mRUFiles_ToolStripMenuItem.DropDownItems.Count; i++)
      {
        if (mRUFiles_ToolStripMenuItem.DropDownItems[i].ToolTipText == sf)
        {
          mRUFiles_ToolStripMenuItem.DropDownItems.RemoveAt(i);
          break;
        }
      }


    }
  }


  class IniSettingsData : RootSettings
  {
    public List<float> FloatList = new List<float>(8);

    int GetInt(int ix)
    {
      if (ix >= 0 && ix < FloatList.Count) return (int)Math.Round(FloatList[ix], 0, MidpointRounding.AwayFromZero);
      else return 0;
    }
    float GetFloat(int ix)
    {
      if (ix >= 0 && ix < FloatList.Count) return FloatList[ix];
      else return 0f;
    }

    public IniSettingsData(string settingsName) : base("Data", settingsName)
    {

    }

    internal override bool TokenWasStored(string name, string token)
    {
      if (CompareNameToken(name, token))
      {
        string[] numbers = token.Split('|');
        for (int i = 0; i < numbers.Length; i++)
        {
          float f;
          if (float.TryParse(numbers[i], out f)) FloatList.Add(f); else FloatList.Add(0.0f);
        }
        return true;
      }
      return false;
    }

    internal override void WriteToXml(System.Xml.XmlTextWriter writer)
    {
      if (FloatList.Count > 0)
      {
        writer.WriteStartElement(Root);
        StringBuilder sb = new StringBuilder();
        for (int i = 0; i < FloatList.Count; i++)
        {
          if (sb.Length > 0) sb.Append(" | ");
          sb.AppendFormat("{0:0.##}", FloatList[i]);
        }
        writer.WriteAttributeString(RootSettingsName, sb.ToString());
        writer.WriteEndElement();
      }

    }
  }
  class IniSettingsFiles : RootSettings
  {
    public string Filename = String.Empty;

    public IniSettingsFiles(string settingsName) : base("Path", settingsName)
    {

    }
    public IniSettingsFiles(string settingsName, string initialNameValue) : this(settingsName)
    {
      Filename = initialNameValue;
    }

    internal override bool TokenWasStored(string name, string token)
    {
      if (CompareNameToken(name, token)) { this.Filename = token; return true; }
      return false;
    }

    internal override void WriteToXml(System.Xml.XmlTextWriter writer)
    {
      if (!String.IsNullOrEmpty(Filename))
      {
        writer.WriteStartElement(Root);
        writer.WriteAttributeString(RootSettingsName, Filename);
        writer.WriteEndElement();
      }
    }
  }
}

