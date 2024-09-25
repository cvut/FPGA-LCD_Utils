using Microsoft.Win32;
using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Xml.Serialization.Configuration;
using static LSPtools.QuartusProject;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ToolTip;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Window;

namespace LSPtools
{
  public class QuartusModelSim
  {
    public readonly int Version = 0;
    public string VersionVerbose = String.Empty;
    public string Root = String.Empty;
    public string QuartusDirectory = String.Empty;
    public string ModelSimDirectory = String.Empty;
    public bool IsQuesta = false;
    private readonly List<string> revisions = new List<string>();

    QuartusModelSim(int version) { this.Version = version; }
    internal static QuartusModelSim? GetFromDir(string directory, QuartusModelSimInstallations qmsi, string keyWithVersion)
    {
      string path = String.Empty;
      try
      {
        if (String.IsNullOrEmpty(directory)) return null;
        directory = directory.Trim();
        if (Directory.Exists(directory)) { path = directory; }
        else
        {
          path = Path.GetDirectoryName(directory); // try to remove file if present
          if (!Directory.Exists(path)) return null; // wrong parameter
        }
        int ix = path.IndexOf("\\1");
        if (ix < 0) { ix = path.IndexOf("\\2"); }
        if (ix < 1) return null;
        char[] ch = new char[2] { ' ', ' ' }; char c; int ixc = 0;
        while (++ix < path.Length && (c = path[ix]) != '\\')
        { if (ixc < ch.Length && Char.IsDigit(c)) ch[ixc++] = c; }
        string ns = new string(ch).Trim();
        int version = 0;
        if (!int.TryParse(ns, out version)) { version = 0; }
        if (qmsi != null && qmsi.Exists(version)) return null;

        if (ix != path.Length) path = path.Substring(0, ix);
        string qfilename = Path.Combine(path, "quartus\\bin64\\quartus.exe");
        if (!File.Exists(qfilename)) qfilename = String.Empty;
        string mfilename = Path.Combine(path, "modelsim_ase\\win32aloem\\modelsim.exe");
        bool isQuesta = false;
        if (!File.Exists(mfilename))
        {
          mfilename = Path.Combine(path, "questa_fse\\win64\\questasim.exe");
          if (!File.Exists(mfilename)) mfilename = String.Empty;
          else isQuesta = true;
        }
        if (mfilename.Length == 0) return null;

        QuartusModelSim qm = new QuartusModelSim(version);
        qm.Root = path;
        qm.QuartusDirectory = qfilename;
        qm.ModelSimDirectory = mfilename;
        qm.IsQuesta = isQuesta;
        string vs = version.ToString();
        if (keyWithVersion != null && keyWithVersion.Length > 0 && keyWithVersion.Contains(vs))
          qm.VersionVerbose = keyWithVersion;
        else
        {
          int n = qfilename.LastIndexOf(vs);
          if (n >= 0)
          {
            StringBuilder sbx = new StringBuilder(); char cx;
            while (n < qfilename.Length && (cx = qfilename[n]) != '\\' && cx != '/')
            { sbx.Append(cx); n++; }
            if (sbx.Length > 0) { vs = sbx.ToString(); }
            qm.VersionVerbose = vs;
          }
        }
        return qm;
      }
      catch (Exception) { return null; }
    }
    public override string ToString()
    {
      return this.Root;
    }


  }

  public class QuartusProject
  {
    private string? activeRevisionName = null;
    private string? activeDate = null;
    private string? projectCreatedInQuartusVersion = null;
    private int projectCreatedInQuartusVersionNumber = 0;

    private string? revisionLastQuartusVersion = null;
    private int revisionLastQuartusVersionNumber = 0;
    private string? projectRevisionOriginalQuartusVersion = null;
    private int projectRevisionOriginalQuartusVersionNumber = 0;
    private string filename = String.Empty;
    private string rootDirectory = String.Empty;
    private string? outputDirectory = null;
    private readonly List<string> filenamesInProject = new List<string>();
    public readonly SortedList<string, LAssignment> locAss = new SortedList<string, LAssignment>();
    public readonly SortedList<string, GAssignment> globAss = new SortedList<string, GAssignment>();
    private readonly StringBuilder sbDebugErrors = new StringBuilder();
    public readonly SortedList<TodoTexts.TT, TTItem> ToDoList = new SortedList<TodoTexts.TT, TTItem>();
    public const string SIMDIR = "simulation/qsim";
    public const string SIMDIRQUESTA = "simulation/questa";
    public const string MODELSIM = "ModelSim-Altera (Verilog)";
    public const string QUESTA = "Questa Intel FPGA (Verilog)";

    public bool IsModelSimVersion { get { return revisionLastQuartusVersionNumber < 21; } }

    public string SimulationDir { get { return IsModelSimVersion ? SIMDIR : SIMDIRQUESTA; } }
    public string SimulationTool { get { return IsModelSimVersion ? MODELSIM : QUESTA; } }


    public const string INVALID = "No Quartus project is loaded!";
    public QuartusProject() { }

    private string? removeLastBackslashIfAny(string? s)
    {
      if (s == null) return s;
      return (s.Length > 0 && s[s.Length - 1] == '\\') ? s.Substring(0, s.Length - 1) : s;
    }
    private string? removeFirstLastBackslashIfAny(string? s)
    {
      if (s == null) return s;
      string? s1 = removeLastBackslashIfAny(s);
      if (s1 != null && s1.Length > 0 && s1[0] == '\\') return s1.Substring(1);
      else return s1;
    }
    public string Filename
    {
      get { return filename; }
      set
      {
        if (value != null && value.Length > 0)
        {
          filename = value;
          string? r = removeLastBackslashIfAny(Path.GetDirectoryName(value));
          rootDirectory = (r != null) ? r : String.Empty;
        }
        else filename = rootDirectory = String.Empty;
      }
    }

    public string RootDir { get { return rootDirectory; } }
    public string? OutputDir
    {
      get { return outputDirectory; }
      set
      {
        if (value == null) return;
        //if (outputDirectory == null) // not assigned
        outputDirectory ??= removeFirstLastBackslashIfAny(value) ?? String.Empty;

      }
    }

    internal void AddProjectFile(string filename)
    {
      filenamesInProject.Add(filename);
    }
    internal int ProjectFilesCount()
    {
      return filenamesInProject.Count;
    }

    internal string[] GetProjectFiles()
    {
      return filenamesInProject.ToArray();
    }

    internal void Clear()
    {
      activeRevisionName = null; activeDate = null; RevisionLastQuartusVersion = null;
      revisionLastQuartusVersionNumber = 0;
      locAss.Clear(); globAss.Clear(); sbDebugErrors.Clear(); ToDoList.Clear();
      Filename = String.Empty;
    }

    public override string ToString()
    {
      return IsValidFilename ? String.Format("File {0} Version {1}", this.Filename, RevisionLastQuartusVersion)
                             : INVALID;
    }

    public string VerboseInfo()
    {
      if (!IsValidFilename) return INVALID;
      StringBuilder sb = new StringBuilder();
      sb.Append("Filename: \""); sb.Append(Path.GetFileName(this.Filename));
      sb.Append("\"  in dir: \""); sb.Append(this.RootDir);
      sb.AppendLine("\"");
      sb.Append("Its Quartus version: \""); sb.Append(this.projectCreatedInQuartusVersion);
      sb.Append("\" has active revision: \""); sb.Append(this.ActiveRevisionName);
      sb.AppendLine("\"");
      sb.Append("Date: "); sb.Append(this.activeDate);
      return sb.ToString();
    }


    public bool IsValidFilename
    {
      get
      {
        return !String.IsNullOrEmpty(Filename) && File.Exists(Filename);
      }
    }
    /// <summary>
    /// Replace \ by / and removes starting or ending /
    /// </summary>
    /// <param name="filename"></param>
    /// <returns></returns>
    public static string ToUnix(string? filename)
    {
      if (filename == null || filename.Length == 0) return String.Empty;
      StringBuilder sb = new StringBuilder();
      int len = filename.Length;
      for (int i = 0; i < len; i++)
      {
        char c = filename[i];
        if (c == '\\') { if (i > 0 && i < len - 1) sb.Append('/'); }
        else { sb.Append(c); }
      }
      return sb.ToString();
    }

    public string? ProjectCreatedInQuartusVersion
    {
      get { return projectCreatedInQuartusVersion; }
      set
      {
        if (value == null) return;
        projectCreatedInQuartusVersion = QCParser.RemoveApostrophe(value);
        projectCreatedInQuartusVersionNumber = Version2Number(projectCreatedInQuartusVersion);
      }
    }
    public int ProjectCreatedInQuartusVersionNumber
    {
      get { return projectCreatedInQuartusVersionNumber; }

    }
    /// <summary>
    /// Quartus version in *.qsf revision file
    /// </summary>
    public string? RevisionLastQuartusVersion
    {
      get { return revisionLastQuartusVersion; }
      set
      {
        if (value == null) return;
        revisionLastQuartusVersion = QCParser.RemoveApostrophe(value);
        revisionLastQuartusVersionNumber = Version2Number(revisionLastQuartusVersion);
      }
    }
    /// <summary>
    /// Quartus version number inf *.qsf revision file
    /// </summary>
    public int RevisionLastVersionNumber
    {
      get { return revisionLastQuartusVersionNumber; }

    }

    public string? RevisionOriginalQuartusVersion
    {
      get { return projectRevisionOriginalQuartusVersion; }
      set
      {
        if (value == null) return;
        projectRevisionOriginalQuartusVersion = QCParser.RemoveApostrophe(value);
        projectRevisionOriginalQuartusVersionNumber = Version2Number(projectRevisionOriginalQuartusVersion);
      }
    }
    public int RevisionOriginalQuartusVersionNumber
    {
      get { return projectRevisionOriginalQuartusVersionNumber; }

    }
    // information pro *.qpf


    public string ActiveDate
    {
      get
      {
        return (activeDate == null) ? String.Empty : activeDate;
      }
      set
      {
        activeDate ??= value; //if (activeDate == null) activeDate = value;
      }
    }

    public string ActiveRevisionName
    {
      get
      {
        return (activeRevisionName == null) ? String.Empty : activeRevisionName;
      }
      set
      {
        activeRevisionName ??= value; // if (activeRevisionName == null)
      }
    }

    public static int Version2Number(string? s)
    {
      if (s == null || s.Length == 0) return 0;
      char c = s[0];
      for (int i = 0; i < s.Length - 1; i++)
      {
        char c1 = s[i + 1];
        if (c == '1' || c == '2' && Char.IsDigit(c1))
        {
          string x = s.Substring(i, 2);
          return int.Parse(x);
        }
      }
      return 0;
    }



    #region Internal classes
    public class TTItem
    {
      public readonly TodoTexts.TT tt;
      private readonly List<string> items = new List<string>();

      public TTItem(TodoTexts.TT tt, string? item)
      {
        this.tt = tt;
        if (item != null) { items.Add(item); }
      }
      public TTItem()
      {
        tt = TodoTexts.TT.NONE;
      }
      public void AddNoRepeat(string? item)
      {
        if (item == null) return;
        for (int i = 0; i < items.Count; i++)
        {
          if (items[i].Equals(item, StringComparison.InvariantCultureIgnoreCase)) return;
        }
        items.Add(item);

      }
      public int Count { get { return items.Count; } }
      public string[] GetItems() { return items.ToArray(); }

      public override string ToString()
      {
        return tt.ToString();
      }
    }
    public class LAssignment
    {
      public string SymbolicName = String.Empty;
      public string PinName = String.Empty;
      public enum IO { Unknown, V25, V33, LVDS };
      public IO IOstandard = IO.Unknown;
      public const string V25 = "2.5 V";
      public const string V33 = "3.3-V LVTTL";
      public const string LVDS = "LVDS";
      public const byte FLAG_PIN = 1;
      public const byte FLAG_IO = 2;
      public const byte FLAG_PINALIAS = 4;
      public const byte OTOP = (byte)'T';
      public const byte OSECOND = (byte)'S';
      public const byte OX = (byte)'X';
      public byte Order = (byte)'X';
      public byte Flag = 0;

      public override string ToString()
      {
        return String.Format("{0} = {1} {2}",
          SymbolicName, PinName == null ? String.Empty : PinName, IOstandard.ToString());
      }
      public void loadFromArray(string[] arr, int ix)
      {
        this.SymbolicName = arr[ix];
        this.PinName = arr[ix + 1];
        string s = arr[ix + 2];
        switch (s[0])
        {
          case '2': IOstandard = IO.V25; break;
          case '3': IOstandard = IO.V33; break;
          case 'L': IOstandard = IO.LVDS; break;
          default: IOstandard = IO.Unknown; break;
        }
        byte b;
        unchecked { b = (byte)s[1]; };

        switch (b)
        {
          case OTOP:
          case OSECOND: Order = b; break;
          default: Order = OX; break;
        }
      }

      /// <summary>
      /// Convert string to LAssignment.IO standard
      /// </summary>
      /// <param name="iost"></param>
      /// <returns></returns>
      public static LAssignment.IO GetIOstandard(string? iost)
      {
        LAssignment.IO ioenum = LAssignment.IO.Unknown;
        if (iost != null && iost.Length > 0)
        {
          if (iost.IndexOf(LAssignment.V25, StringComparison.InvariantCultureIgnoreCase) >= 0)
            ioenum = LAssignment.IO.V25;
          else
          {
            if (iost.IndexOf(LAssignment.V33, StringComparison.InvariantCultureIgnoreCase) >= 0)
              ioenum = LAssignment.IO.V33;
            else
            {
              if (iost.IndexOf(LAssignment.LVDS, StringComparison.InvariantCultureIgnoreCase) >= 0)
                ioenum = LAssignment.IO.LVDS;
            }
          }
        }
        return ioenum;
      }
      public string debugGetStringArrayItem()
      {
        char[] cdef = new char[] { ' ', 'X' };
        switch (IOstandard)
        {
          case IO.V25: cdef[0] = '2'; break;
          case IO.V33: cdef[0] = '3'; break;
          case IO.LVDS: cdef[0] = 'L'; break;
          default: cdef[0] = 'U'; break;
        }
        char prio = (char)OX;
        string[] test = new string[]
        { "LED", "LCD", "HEX", "SW", "CLO", "AUD", "TOU", "KEY","IRD","I2C" };
        for (int i = 0; i < test.Length; i++)
        {
          if (SymbolicName.StartsWith(test[i], StringComparison.InvariantCultureIgnoreCase))
            prio = (char)OTOP;
        }
        if (prio != 'T')
        {
          string[] test2 = new string[]
          { "CAM", "MIP" /* 2*camera */, "MPU" /* magnetometer+accelerometer*/,
            "PS2" /*port*/, "UAR" /* RS232*/, "VGA","LSE" /* light sensor*/,"I2C" /* bus*/};

          for (int i = 0; i < test2.Length; i++)
          {
            if (SymbolicName.StartsWith(test2[i], StringComparison.InvariantCultureIgnoreCase))
              prio = (char)OSECOND;
          }
        }
        cdef[1] = prio;
        //Note: X remains OTG (USB), SRAM, DRAM, ENET, EX_ (clock), FL_ (flash),
        //GPIO ext. connector1, HSMC ext. connector, SD_ (card), SMA extern clocks, TD_ (video modul)
        string sdef = new string(cdef);
        return String.Format("\"{0}\",\"{1}\",\"{2}\",",
          SymbolicName, PinName == null ? String.Empty : PinName, sdef);
      }
    }

    /// <summary>
    /// Global Assignment Value
    /// </summary>
    public readonly struct GAValue
    {
      public readonly string Value = String.Empty;
      public readonly string? Option = null;
      public GAValue(string value, string? option)
      { this.Value = value == null ? String.Empty : value; this.Option = option; }
      public override string ToString()
      {
        return String.Format("{0} {1}", Value, Option == null ? String.Empty : Option); ;
      }
      public string GetDebugValue()
      {
        return String.Format("\"{0}\"", QCParser.RemoveApostrophe(Value)); ;
      }
    }
    /// <summary>
    /// Glabal Assignment
    /// </summary>
    public class GAssignment
    {
      public string Name = String.Empty;
      private readonly List<GAValue> Items = new List<GAValue>();
      public GAValue[] GetValues() { return Items.ToArray(); }
      public string GetTopValue()
      { return Items.Count > 0 && Items[0].Value != null ? Items[0].Value : String.Empty; }
      public override string ToString()
      {
        return String.Format("{0} with {1} values", Name, Items.Count); ;
      }
      public GAssignment(string name)
      {
        this.Name = name == null ? String.Empty : name;
      }
      public void Add(string? value, string? option)
      {
        if (value != null && value.Length > 0)
        {
          Items.Add(new GAValue(value, option));
        }
      }

      internal string debugGetStringArrayItem()
      {
        StringBuilder sb = new StringBuilder();
        switch (Items.Count)
        {
          case 0: sb.Append("\"\""); break;
          case 1: sb.Append(Items[0].GetDebugValue()); break;
          default: sb.Append("\"*\""); ; break; // more items
        }
        return String.Format("\"{0}\",{1},", Name == null ? String.Empty : Name, sb.ToString());
      }
    }
    #endregion

#if TABLE_CREATION
    public void AddLoc(string symbolicName, string? pinName, string? iost)
    {
      if (symbolicName == null || symbolicName.Length == 0) return;
      LAssignment lAssignment;
      LAssignment.IO ioenum;
      bool add = false;
      if (!locAss.TryGetValue(symbolicName, out lAssignment))
      {
        add = true; lAssignment = new LAssignment();
      }
      if (pinName != null && pinName.Length > 0) lAssignment.PinName = pinName;
      ioenum = LAssignment.GetIOstandard(iost);
      if (ioenum != LAssignment.IO.Unknown) lAssignment.IOstandard = ioenum;
      if (symbolicName != null && symbolicName.Length > 0)
      {
        lAssignment.SymbolicName = symbolicName;
        if (add) locAss.Add(symbolicName, lAssignment);
      }
    }
#endif

    public void AddGlob(string key, string value, string option)
    {
      if (key == null || key.Length == 0) return;
      GAssignment gAssignment;
      if (globAss.TryGetValue(key, out gAssignment))
      {
        gAssignment.Add(value, option);
      }
      else
      {
        gAssignment = new GAssignment(key);
        gAssignment.Add(value, option);
        globAss.Add(key, gAssignment);
      }
    }
    public void AddToDo(string item, TodoTexts.TT tt)
    {
      if (tt == TodoTexts.TT.NONE) return;
      TTItem tti;
      if (ToDoList.TryGetValue(tt, out tti)) tti.AddNoRepeat(item);
      else ToDoList.Add(tt, new TTItem(tt, item));
    }
    public void AddError(string item, string? description)
    {
      AddError(item, description, TodoTexts.TT.NONE, null);
    }
    public void AddError(string item, string? description, TodoTexts.TT tt)
    {
      AddError(item, description, tt, null);
    }
    public void AddError(string item, string? description, TodoTexts.TT tt, string? longdescription)
    {
      if (description == null || description.Length == 0) return;
      string cc = String.Format("{0}:{1}", item, description);
      sbDebugErrors.Append(cc);
      AddToDo(description, tt);
      if (longdescription != null && longdescription.Length > 0)
      {
        AddToDo(longdescription, tt);

      }
    }

    public static string? ValidateFilename(string filename, out int partsCount)
    {
      partsCount = 0;
      if (filename == null || filename.Length == 0) return "Empty filename";
      try
      {
        int ix = filename.IndexOf(' ');
        if (ix >= 0) return "Whitespace is not allowed!";
        string[] parts = filename.Split(new char[] { '\\', '/' }, StringSplitOptions.RemoveEmptyEntries);
        partsCount = parts.Length;
        string ext = Path.GetExtension(filename);

        for (int i = 0; i < partsCount; i++)
        {
          if (i == partsCount - 1)
          {
            if (String.IsNullOrEmpty(ext) || ext.ToLower() == ".txt") continue;
          }
          int dotcount = 0;
          string part = parts[i];
          for (int j = 0; j < part.Length; j++)
          {
            char c = part[j], cn = j + 1 < part.Length ? part[j + 1] : '\0';
            UInt32 uc = (UInt32)c;
            if (uc < (UInt32)'.' || uc > (UInt32)'z')
              return uc <= (UInt32)' ' ? "Whitespace" : "Non-allowed " + c;
            if (Char.IsDigit(c) || Char.IsLetter(c)) continue;
            if (c == '_')
            {
              if (cn == '_') return "Not allowed __";
              else continue;
            }
            if (c == '.')
            {
              if (cn == '.') return "Not allowed ..";
              else
              {
                if (++dotcount < 2) continue;
                else return "Unrecommended two . extensions";
              }
            }
            if (c == ':')
            {
              if (i > 0) return "Wrong position of :";
              if (!Directory.Exists(part)) return "Directory " + part + "is not accessible";
              else continue;
            }
          }
        }
      }
      catch (Exception ex) { return ex.Message; }
      return null;
    }
    /// <summary>
    /// Debug only
    /// </summary>
    /// <returns></returns>
    public string DebugGetLocations()
    {
      KeyValuePair<string, LAssignment>[] pairs = locAss.ToArray();
      StringBuilder sb = new StringBuilder();
      int nlcount = 0;
      for (int i = 0; i < pairs.Length; i++)
      {
        LAssignment la = pairs[i].Value;
        sb.Append(la.debugGetStringArrayItem());
        if (++nlcount > 2) { nlcount = 0; sb.AppendLine(); }
      }
      return sb.ToString();
    }

    internal string DebugGetGlobals()
    {
      KeyValuePair<string, GAssignment>[] pairs = globAss.ToArray();
      StringBuilder sb = new StringBuilder();
      for (int i = 0; i < pairs.Length; i++)
      {
        GAssignment ga = pairs[i].Value;
        sb.AppendLine(ga.debugGetStringArrayItem());
      }
      return sb.ToString();
    }

    internal string GetTopLevelEntity()
    {
      GAssignment gAssignment;
      if (globAss.TryGetValue("TOP_LEVEL_ENTITY", out gAssignment))
      {
        return gAssignment.GetTopValue();
      }
      return String.Empty;
    }

    internal string? GetSimulationDir()
    {
      GAssignment gAssignment;
      if (globAss.TryGetValue("EDA_NETLIST_WRITER_OUTPUT_DIR", out gAssignment))
      {
        string s = gAssignment.GetTopValue();
        return s == null ? String.Empty : s;
      }
      return String.Empty;
    }
    /// <summary>
    /// Test if not absolute path, relative is expanded by root directory 
    /// </summary>
    /// <param name="projectFilename"></param>
    /// <returns></returns>
    public String GetAbsolutePath(String? projectFilename)
    {
      if (projectFilename == null || projectFilename.Length == 0) return String.Empty;
      try
      {
        string basePath = this.RootDir;
        projectFilename = projectFilename.Replace("/", "\\").Trim(); // Linux names to Windows
        string fullPath;
        if (!Path.IsPathRooted(projectFilename))
        {
          if (projectFilename.StartsWith(Path.DirectorySeparatorChar.ToString()))
            fullPath = Path.Combine(Path.GetPathRoot(basePath), projectFilename.TrimStart(Path.DirectorySeparatorChar));
          else
            fullPath = Path.Combine(this.RootDir, projectFilename);
        }
        else
          fullPath = projectFilename;
        // resolving internal specialties as "..\" 
        return Path.GetFullPath(fullPath);
      }
      catch (Exception ex) { Trace.WriteLine(ex.ToString()); }
      return String.Empty;
    }

    internal ReportList GetReports()
    {
      try
      {
        return new ReportList(RootDir, OutputDir, ActiveRevisionName);
      }
      catch (Exception ex)
      {
        Trace.WriteLine(ex.ToString());
        return new ReportList();
      }
    }
  }
  internal class QuartusModelSimInstallations
  {
    public List<QuartusModelSim> Versions = new List<QuartusModelSim>();

    public void Clear() { Versions.Clear(); }

    public void Sort()
    {
      Versions.Sort(sorterOfItems);
    }

    private int sorterOfItems(QuartusModelSim q1, QuartusModelSim q2)
    {
      if (q1 != null)
      {
        if (q2 != null)
        {
          if (q1.Version > q2.Version) return -1; // descending sorter
          if (q1.Version < q2.Version) return 1;
          return 0;
        }
        else return -1;
      }
      return q2 == null ? 0 : 1;
    }

    public bool Exists(int version)
    {
      for (int i = 0; i < Versions.Count; i++)
      {
        if (Versions[i].Version == version) return true;
      }
      return false;
    }
    public int Count { get { return Versions.Count; } }

    public QuartusModelSim? this[int ix] { get { return ix >= 0 && ix < Count ? Versions[ix] : null; } }

    public void Add(QuartusModelSim? qm)
    {
      if (qm != null && qm.Version > 0 && !Exists(qm.Version))
        Versions.Add(qm);

    }

    public QuartusModelSim? GetLatest()
    {
      int n = -1;
      for (int i = 0; i < Versions.Count; i++)
      {
        if (Versions[i].Version > n) n = i;
      }
      return n > 0 ? Versions[n] : null;
    }
    private int _depthSearchLocalMachine = 0;
    public void SearchLocalMachine(string? localMachineSubkey)
    {
      if (localMachineSubkey == null) return;
      _depthSearchLocalMachine = 0;
      try
      {
        openSubKey(localMachineSubkey);
      }
      catch (Exception ex) { Trace.WriteLine(ex.ToString()); }
    }


    private void openSubKey(string? localMachineSubkey)
    {
      if (localMachineSubkey == null) return;
      using (RegistryKey key = Registry.LocalMachine.OpenSubKey(localMachineSubkey))
      {
        string textKey = String.Empty;

        if (localMachineSubkey.IndexOf("ModelSim", StringComparison.InvariantCultureIgnoreCase) > 0)
          return; // we search first for Quartus
        if (localMachineSubkey.IndexOf("Quartus", StringComparison.InvariantCultureIgnoreCase) >= 0)
        {

          textKey = Path.GetFileName(localMachineSubkey);
        }
        if (key != null)
        {
          if (key.ValueCount > 0)
          {
            foreach (string ValueOfName in key.GetValueNames())
            {
              string? s13 = key.GetValue(ValueOfName) as string;
              if (s13 != null)
              {
                QuartusModelSim? qm = QuartusModelSim.GetFromDir(s13, this, textKey);
                Add(qm);
              }
            }
          }
          _depthSearchLocalMachine++;
          if (_depthSearchLocalMachine < 2)
          {
            if (key.SubKeyCount > 0)
            {
              foreach (string Subkey in key.GetSubKeyNames())
              {
                openSubKey(localMachineSubkey + '\\' + Subkey);
              }
            }
          }
        }
      }
    }

  }

  internal class Report
  {
    public readonly string Filename;
    public readonly DateTime LastAccessTime;
    public readonly long Length;
    public readonly ReportType RT;
    public enum ReportType { UNK, MAP, FIT, ASM, STA, EDA, FLOW }
    public Report(string filename, DateTime lastAccessTime, long length)
    {
      Filename = filename; LastAccessTime = lastAccessTime; Length = length;
      string s = Path.GetFileNameWithoutExtension(Filename);
      RT = ReportType.UNK;
      if (s.Length == 0) return;
      // Reports have 2 extensions
      string s1 = Path.GetExtension(s);
      if (s1.Length == 0) return;
      switch (s1.ToLower())
      {
        case ".map": RT = ReportType.MAP; return;
        case ".fit": RT = ReportType.FIT; return;
        case ".asm": RT = ReportType.ASM; return;
        case ".sta": RT = ReportType.STA; return;
        case ".eda": RT = ReportType.EDA; return;
        case ".flow": RT = ReportType.FLOW; return;
      }
    }
    public override string ToString()
    {
      return String.Format("{0} {1:hh:mm:ss} {2}", RT.ToString(), LastAccessTime, Filename);
    }
  }

  internal class ReportList
  {
    public readonly List<Report> Items;
    public int Count { get { return Items.Count; } }

    public ReportList()
    {
      Items = new List<Report>();
    }

    public ReportList(string rootDir, string? outputDir, string? activeRevision) : this()
    {
      string searchPattern = (activeRevision != null) ? activeRevision + ".*.rpt" : "*.rpt";
      if (outputDir != null)
      {
        string startDir = rootDir + '\\' + outputDir;
        if (Directory.Exists(startDir))
        {
          ProcessDirectory(startDir, searchPattern);
        }
      }
      if (Items.Count == 0)
      {
        if (rootDir != null && Directory.Exists(rootDir))
        {
          ProcessDirectory(rootDir, searchPattern);
        }
      }
      if (Items.Count > 1)
      {
        Items.Sort(compareReport);
      }
    }

    private int compareReport(Report x, Report y)
    {
      return x.LastAccessTime.CompareTo(y.LastAccessTime);
    }


    // Process all files in the directory passed in, recurse on any directories 
    // that are found, and process the files they contain.
    private void ProcessDirectory(string targetDirectory, string searchPattern)
    {
      // Process the list of files found in the directory.
      string[] fileEntries = Directory.GetFiles(targetDirectory, searchPattern, SearchOption.TopDirectoryOnly);
      if (fileEntries.Length > 0)
      {
        foreach (string fileName in fileEntries) ProcessFile(fileName);
        if (Items.Count != 0) return; // we have found
      }
      // Recurse into subdirectories of this directory.
      string[] subdirectoryEntries = Directory.GetDirectories(targetDirectory);
      foreach (string subdirectory in subdirectoryEntries)
        ProcessDirectory(subdirectory, searchPattern);
    }

    private void ProcessFile(string path)
    {
      try
      {
        FileInfo fi = new FileInfo(path);
        if (fi == null) return;
        DateTime last = fi.LastAccessTime; long len = fi.Length;
        using (FileStream fs = File.Open(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
        {
          if (!fs.CanRead) return;
        }
        Report r = new Report(path, last, len);
        this.Items.Add(r);
      }
      catch (Exception) { /* File.Open failed */ }
    }

  }
}

