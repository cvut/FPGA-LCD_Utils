using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Dynamic;
using System.Globalization;
using System.Linq;
using System.Reflection.Emit;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Windows.Forms;
using System.Xml.Linq;

namespace FpgaLcdUtils
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
    public static IniGeometry GeometryRLSearch = new IniGeometry("RLSearch");
    public static IniGeometry GeometryRESearch = new IniGeometry("RESearch");
    public static IniGeometry GeometryREquations = new IniGeometry("REquations");
    public static IniGeometry GeometryViewer = new IniGeometry("Viewer");
    public static IniGeometry GeometryViewerInfo = new IniGeometry("ViewerInfo");
    public static IniMRUList MRUFilesBitmap = new IniMRUList("BmpMRU");
    public static IniTxtList FilesBitmap = new IniTxtList("BmpTxt");
    public static IniMRUList MRUFilesRulers = new IniMRUList("Rulers");
    public static IniTxtList FilesRulers = new IniTxtList("RulersTxt");
    public static IniMRUList MRUFilesTBViewer = new IniMRUList("Viewer");
    public static IniTxtList FilesTBViewer = new IniTxtList("ViewerTxt");
    public static IniMRUList MRUFilesQCheck = new IniMRUList("QCheck");
    public static IniTxtList FilesQCheck = new IniTxtList("QCheckTxt");
    public static IniMRUList MRUFilesQCAdjustVWF = new IniMRUList("QCAvwf");
    public static IniFloatList RRData = new IniFloatList("RRuler");
    public static IniFloatList LRData = new IniFloatList("LRuler");
    public static IniIntList LSearchData = new IniIntList("LSRuler");
    public static IniFloatList ERData = new IniFloatList("ERuler");
    public static IniIntList ESearchData = new IniIntList("ESRuler");

    private static readonly RootSettings[] rootSettings = new RootSettings[]
    { GeometryStart, GeometryBMP, GeometryRulers, GeometryRLSearch, 
      GeometryRESearch, GeometryREquations, GeometryQCheck, GeometryViewer,
      MRUFilesBitmap, FilesBitmap, MRUFilesRulers, FilesRulers,
      RRData, LRData, LSearchData, ERData, ESearchData,
      MRUFilesTBViewer, FilesTBViewer,
      MRUFilesQCheck, FilesQCheck, MRUFilesQCAdjustVWF};


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
      if (WinSize.Width < formIn.MinimumSize.Width) WinSize.Width = formIn.MinimumSize.Width;
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
    private int MRUnumber = 6;
    private readonly List<string> MRUlist = new List<string>();

    public string Top { get { return MRUlist.Count > 0 ? MRUlist[0] : String.Empty; } }

    public IniMRUList(string settingsName) : this(settingsName, 0) { } // 0-keep default

    public IniMRUList(string settingsName, int limit) : base("MRU", settingsName)
    {
      if (limit > 0) MRUnumber = limit;
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

  class IniTxtList : RootSettings
  {
    private readonly SortedList<string, string> list = new SortedList<string, string>(StringComparer.CurrentCultureIgnoreCase);

    public string this[string key]
    {
      get
      {
        if (list.TryGetValue(key, out string val)) return val;
        else return String.Empty;
      }
      set
      {
        if (key == null) return;
        key = key.Trim();
        if (key.Length == 0) return;
        int ix = list.IndexOfKey(key);
        if (ix >= 0) list.RemoveAt(ix); 
        list.Add(key, value);
        
      }
    }
    /// <summary>
    /// Search ini setting for keys
    /// </summary>
    /// <param name="keys">list of keys in order of the search</param>
    /// <param name="index">index of found key, or -1 if no key was found </param>
    /// <returns></returns>
    public string GetFirstNonEmpty(string[] keys, out int index)
    {
      index = -1;
      if (keys == null) return String.Empty;
      for (int i = 0; i < keys.Length; i++)
      {
        string param = this[keys[i]];
        if (!string.IsNullOrEmpty(param)) { index = i; return param; }
      }
      return string.Empty;
    }
    /// <summary>
    /// Process ini filenames, set found file extension and show SaveDialog
    /// </summary>
    /// <param name="saveFileDialog">save dialog</param>
    /// <param name="keys">search keys in initilized stored filenames</param>
    /// <param name="relatedFilename">derive store filename from this name </param>
    /// <param name="extension">use extension</param>
    /// <returns>true if dialog succesful</returns>
    public bool ShowSaveDialog(SaveFileDialog saveFileDialog, string[] keys, string? relatedFilename, string extension)
    {
      return ShowSaveDialog(saveFileDialog, keys, relatedFilename, extension, false);
    }
    /// <summary>
    /// Process ini filenames and show SaveDialog
    /// </summary>
    /// <param name="saveFileDialog">SaveFileDialog instance</param>
    /// <param name="keys">search keys in IniSettings stored filenames</param>
    /// <param name="relatedFilename">derive store filename from this name </param>
    /// <param name="extension">use this extension</param>
    /// <param name="replaceOnlyEmptyByRelatedFilename">if true then reletated file name is used only when stored is empty</param>
    /// <returns>true if dialog succesful</returns>
    public bool ShowSaveDialog(SaveFileDialog saveFileDialog, string[] keys,
                               string? relatedFilename, string extension, bool replaceOnlyEmptyByRelatedFilename)
    {
      try
      {
        int ixFound = -1;
        string lastFile = IniSettings.FilesBitmap.GetFirstNonEmpty(keys, out ixFound);
        bool useRelated = true;
        if (!String.IsNullOrEmpty(lastFile))
        {
          try
          {
            saveFileDialog.InitialDirectory = Path.GetDirectoryName(lastFile);
            string filename = Path.GetFileName(lastFile);
            string ext = Path.GetExtension(filename);
            if (ixFound > 0) // not the main index
              filename = Path.ChangeExtension(filename, extension);
            saveFileDialog.FileName = filename;
            if (replaceOnlyEmptyByRelatedFilename) useRelated = false;
          }
          catch (Exception) { }
        }
        if (useRelated && !String.IsNullOrEmpty(relatedFilename))
        {
          string s = Path.ChangeExtension(relatedFilename, extension);
          saveFileDialog.FileName = Path.GetFileName(s);
        }
      }
      catch (Exception ex)
      {
        Trace.WriteLine(ex.ToString());
      }
      if (saveFileDialog.ShowDialog() != System.Windows.Forms.DialogResult.OK)
        return false;
      IniSettings.FilesBitmap[keys[0]] = saveFileDialog.FileName;
      return true;

    }

    public IniTxtList(string settingsName) : base("Txt", settingsName) { }


    internal override bool TokenWasStored(string name, string token)
    {
      if (CompareNameToken(name, token, false))
      {
        string[] fields = token.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
        if (fields.Length > 1)
        {
          list[fields[0]] = fields[1];
          return true;
        }

      }
      return false;
    }

    internal override void WriteToXml(System.Xml.XmlTextWriter writer)
    {
      if (list.Count > 0)
      {
        writer.WriteStartElement(Root);
        int i = 1;
        foreach (KeyValuePair<string, string> kw in list)
        {
          writer.WriteAttributeString(RootSettingsName + i.ToString(), String.Format("{0}|{1}", kw.Key, kw.Value));
          i++;
        }
        writer.WriteEndElement();
      }

    }
  }


  class IniIntList : RootSettings
  {
    public List<int> IntList = new List<int>(8);

    public int Count { get { return IntList.Count; } }
    public int GetInt(int ix)
    {
      if (ix >= 0 && ix < IntList.Count) return IntList[ix];
      else return 0;
    }

    public bool ExistNonZeroValue()
    {
      for (int i = 0; i < IntList.Count; i++)
      {
        if (IntList[i]!=0) return true;
      }
      return false;
    }


    public IniIntList(string settingsName) : base("Data", settingsName)
    {
    }

 
    internal override bool TokenWasStored(string name, string token)
    {
      if (CompareNameToken(name, token))
      {
        string[] numbers = token.Split('|');
        for (int i = 0; i < numbers.Length; i++)
        {
          int n;
          if (int.TryParse(numbers[i], out n)) IntList.Add(n); else IntList.Add(0);
        }
        return true;
      }
      return false;
    }

    internal override void WriteToXml(System.Xml.XmlTextWriter writer)
    {
      if (IntList.Count > 0)
      {
        writer.WriteStartElement(Root);
        StringBuilder sb = new StringBuilder();
        for (int i = 0; i < IntList.Count; i++)
        {
          if (sb.Length > 0) sb.Append(" | ");
          sb.AppendFormat("{0}", IntList[i]);
        }
        writer.WriteAttributeString(RootSettingsName, sb.ToString());
        writer.WriteEndElement();
      }

    }
  }

  class IniFloatList : RootSettings
  {
    public List<float> FloatList = new List<float>(8);
    public int Count { get { return FloatList.Count; } }

    public int GetInt(int ix)
    {
      if (ix >= 0 && ix < FloatList.Count) return (int)Math.Round(FloatList[ix], 0, MidpointRounding.AwayFromZero);
      else return 0;
    }
    public float GetFloat(int ix)
    {
      if (ix >= 0 && ix < FloatList.Count) return FloatList[ix];
      else return 0f;
    }

    public bool ExistNonZeroValue()
    {
      for (int i = 0; i < FloatList.Count; i++)
      {
        if (Math.Abs(FloatList[i]) > 1e-6) return true;
      }
      return false;
    }


    public IniFloatList(string settingsName) : base("Data", settingsName)
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
  class IniFilename : RootSettings
  {
    public string Filename = String.Empty;

    public IniFilename(string settingsName) : base("Path", settingsName)
    {

    }
    public IniFilename(string settingsName, string initialNameValue) : this(settingsName)
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

