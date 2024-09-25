using Microsoft.Win32;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Diagnostics.Eventing.Reader;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using System.Windows.Forms;
using static LSPtools.QuartusProject;
using static LSPtools.TodoTexts;

namespace LSPtools
{
  public partial class QCFormMain : Form
  {
    private readonly string? firstMRUFileOnStart = null;
    private const string WINDOW_TITLE = "Checker of VEEK-MT2 Project (Alpha version) ";
    private readonly QuartusModelSimInstallations qmsi = new QuartusModelSimInstallations();
    private readonly QuartusProject quartusProject = new QuartusProject();
    private readonly RichTBWriter qcReport;
    private readonly RichTBWriter vwfReport;
    private readonly RichTBWriter todo;
    private readonly StatusMessages displayStatusMessage;
    private VEEKMT2? veekmt2 = null;
    public QCFormMain()
    {
      InitializeComponent();
      firstMRUFileOnStart = IniSettings.MRUFilesQCheck.AddToMenuItem(recentToolStripMenuItem, recentToolStripMenuItem_Click, true);
      IniSettings.MRUFilesQCAdjustVWF.AddToMenuItem(vwfRecentToolStripMenuItem, vwfRecentToolStripMenuItem_Click);
      qcReport = new RichTBWriter(reportsRTB);
      vwfReport = new RichTBWriter(vwfRtb);
      todo = new RichTBWriter(todoRTB);
      displayStatusMessage = new StatusMessages(10, tsslMessage);
      progressBar1.Minimum = 0; progressBar1.Maximum = 100; progressBar1.Value = 0;
      timerReload.Enabled = false;
    }

    private void recentToolStripMenuItem_Click(object sender, EventArgs e)
    {
      ToolStripMenuItem? mi = sender as ToolStripMenuItem;
      if (mi != null)
      {
        try
        {
          string filename = mi.ToolTipText;
          if (openQuartusProject(filename))
          {
            IniSettings.MRUFilesQCheck.AddFile(filename);
            IniSettings.MRUFilesQCheck.AddToMenuItem(recentToolStripMenuItem, recentToolStripMenuItem_Click);
          }
        }
        catch (Exception ex)
        {
          displayStatusMessage.Error(ex.Message);
        }

      }
    }

    private void vwfRecentToolStripMenuItem_Click(object sender, EventArgs e)
    {
      ToolStripMenuItem? mi = sender as ToolStripMenuItem;
      if (mi != null)
      {
        try
        {
          string filename = mi.ToolTipText;
          if (adjustQuartusVWF(filename))
          {
            IniSettings.MRUFilesQCAdjustVWF.AddFile(filename);
            IniSettings.MRUFilesQCAdjustVWF.AddToMenuItem(recentToolStripMenuItem, vwfRecentToolStripMenuItem_Click);
          }
        }
        catch (Exception ex)
        {
          displayStatusMessage.Error(ex.Message);
        }

      }
    }


    private bool was_shown = false;

    const string alteraUniversitySubkey = "SOFTWARE\\Altera Corporation\\University Program";
    const string alteraQuartusSubkey = "SOFTWARE\\Altera Corporation\\Quartus";
    const string intelSubkey = "SOFTWARE\\Intel Corporation";

    private void QCFormMain_Shown(object sender, EventArgs e)
    {
      if (was_shown) return;
      was_shown = true;
      IniSettings.GeometryQCheck.ApplyGeometryToForm(this);
      Cursor cursor = this.Cursor;
      quartusProject.Clear();
      try
      {
        this.Cursor = Cursors.WaitCursor;

        qcReport.Clear();
        qcReport.WrLine("============= Searching Quartus Installations Registry Keys =============");
        qmsi.Clear();
        qmsi.SearchLocalMachine(alteraUniversitySubkey);
        qmsi.SearchLocalMachine(alteraQuartusSubkey);
        qmsi.SearchLocalMachine(intelSubkey);
        qmsi.Sort();
        bool error = false;
        if (qmsi.Count == 0)
        {
          qcReport.WrLineB("No Quartus installation was found!");
          error = true;
        }
        else
        {
          QuartusModelSim qi = qmsi.Versions[0];
          QuartusModelSim quartusModelSim = qi;
          if (qmsi.Count > 0)
          {
            int vcnt = 0;
            qcReport.WrI("Installed Quartus versions: ");
            for (int j = 0; j < qmsi.Count; j++)
            {
              QuartusModelSim? qi1 = qmsi[j];
              if (qi1 == null) continue;
              vcnt++;
              qcReport.WrI(String.Format("{0};  ", qmsi.Versions[j]));
            }
            qcReport.WrLine("");
            if (qi.Version >= 21)
            {
              for (int i = 1; i < qmsi.Count; i++)
              {
                QuartusModelSim? qi1 = qmsi[i]; if (qi1 == null) continue;
                if (qi1.Version < qi.Version) { qi = qi1; break; }
              }
            }
            if (qi.Version < 21)
              qcReport.WrI("The latest version with ModelSim: ");
            else
              qcReport.WrB("Located version: ");
            qcReport.WrLineB(qi.VersionVerbose);
            quartusModelSim = qi;
          }
          else
          {
            qcReport.WrLineI(String.Format("Found one Quartus installation: "));
            qi = qmsi.Versions[0];
            qcReport.WrLineB(qi.VersionVerbose);
          }
          if (qi.Version >= 23)
            qcReport.WrLineB("Checker does not know this version yet. The results can be unpredictable.");

          qcReport.WrLineI("Its root directory: " + quartusModelSim.Root);
          int errors = 0;
          if (String.IsNullOrEmpty(quartusModelSim.QuartusDirectory))
          { qcReport.WrB("Error: No Quartus "); errors++; }
          if (String.IsNullOrEmpty(quartusModelSim.ModelSimDirectory))
          {
            if (errors == 0) qcReport.WrB("Error: ");
            qcReport.WrB(" + ");
            qcReport.WrLineB(" No ModelSim or Questa !!!"); errors |= 2;
          }
        }

        //if (firstMRUFileOnStart != null)
        //{
        //  if (!openQuartusProject(firstMRUFileOnStart))
        //  {
        //    IniSettings.MRUFilesQCheck.RemoveFile(firstMRUFileOnStart, recentToolStripMenuItem);
        //  }
        //}
        if (error)
        {
          displayStatusMessage.Error("Tool supposes a local Quartus installation.");
          qmsi.Clear();
        }
        else
        {
          displayStatusMessage.Info("Open a Quartus project...");
        }
        setWindowTitle();
      }
      catch (Exception ex)
      {
        Trace.WriteLine(ex.ToString());
        tsslMessage.Text = ex.Message;
      }
      finally
      {
        this.Cursor = cursor;
        timerUpdate.Enabled = true;
        qcReport.WrLine("=========================================================================");
      }
    }

    private void timerUpdate_Tick(object sender, EventArgs e)
    {
      if (displayStatusMessage == null) return; // not initialize yet
      displayStatusMessage.timerUpdate_Tick(sender, e);
    }



    private void QCFormMain_FormClosing(object sender, FormClosingEventArgs e)
    {
      try
      {
        IniSettings.GeometryQCheck.StoreGeometry(this);
      }
      catch (Exception ex)
      {
        Trace.WriteLine(ex.ToString());
      }
    }

    private void setWindowTitle()
    {
      if (quartusProject.IsValidFilename && qmsi.Versions.Count > 0)
      {
        this.Text = WINDOW_TITLE + " - " + quartusProject.Filename;
      }
      else
      {
        this.Text = WINDOW_TITLE + " - " + QuartusProject.INVALID;
        displayStatusMessage.Info("Open a Quartus project");
      }
    }


    private void openQuartusProjectToolStripMenuItem_Click(object sender, EventArgs e)
    {
      if (qmsi.Versions.Count == 0)
      { displayStatusMessage.Error("Quartus installation was not found."); }
      if (openFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
      {
        openQuartusProject(openFileDialog1.FileName);
      }
    }
    private readonly Dictionary<string, string> keyWords = new Dictionary<string, string>();

    private void copyToClipboard_Click(object sender, EventArgs e)
    {
      Clipboard.SetText(reportsRTB.Rtf, TextDataFormat.Rtf);
    }
#if TABLE_CREATION
    private bool openQuartusProject(string filename)
    {
      string? s = QuartusProject.ValidateFilename(openFileDialog1.FileName);
      if (s != null && s.Length > 0) qpf.AddError("Quartus project path!", s, TodoTexts.TT.WRONG_PATH);

      try
      {
        if (File.Exists(filename))
        {
          currentFilename = filename;

          using (FileStream fileStream = new FileStream(filename, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
          {
            Regex rg = new Regex(@"\s*(\w+)\s*=\s*\""(.+)\""", RegexOptions.Compiled);
            using (StreamReader sr = new StreamReader(fileStream))
            {
              string? line = null;
              while ((line = sr.ReadLine()) != null)
              {
                if (line.Length == 0 || line[0] == '#') continue; // empty or comment
                Match m = rg.Match(line);
                if (!m.Success || m.Groups.Count < 3) continue;
                string key = m.Groups[1].Value;
                if (String.IsNullOrEmpty(key)) continue;
                string value = m.Groups[2].Value;
                if (String.IsNullOrEmpty(value)) continue;
                qcr.AppendLine(String.Format("{0}={1}", key, value));
                switch (key.ToUpper())
                {
                  // active revision is the first in list
                  case "PROJECT_REVISION":
                    if (qpf.activeRevisionName == null) qpf.activeRevisionName = value;
                    break;
                  case "QUARTUS_VERSION":
                    if (qpf.activeVersion == null) qpf.activeVersion = value;
                    break;
                  case "QUARTUS_DATE":
                    if (qpf.activeDate == null) qpf.activeDate = value;
                    break;
                }
              }
            }
          }
          if (qpf.activeRevisionName == null)
          {
            qcr.AppendLineB("Unknown format of the file: " + qpf);
            displayStatusMessage.Info("Open valid Quartus project file");
          }
          string qsf = qpf.activeRevisionName + ".qsf";
          string qsfull = Path.Combine(Path.GetDirectoryName(filename), qpf.activeRevisionName + ".qsf");
          if (!File.Exists(qsfull))
          {
            qpf.AddError(qsf, "Failure to read the setting file of active project revision: " + qsfull,
                         TodoTexts.TT.CHANGE_REVISION);
          }
          else
          {
            qpf.locAss.Clear();
            qpf.globAss.Clear();
           Regex rgxInst = new Regex(@"(\w+)\s+("".+"")\s+-to\s+(.*)", RegexOptions.Compiled);
            Regex rgxInst2 = new Regex(@"(\w+)\s+(\w+)\s+-to\s+(.*)", RegexOptions.Compiled);
            Regex rgxLoc = new Regex(@"\s*-to\s+(.+)", RegexOptions.Compiled);
            using (FileStream fileStream = new FileStream(qsfull, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
              if (fileStream == null || fileStream.Length == 0) throw new FileNotFoundException(filename);
              long size = fileStream.Length;
              Regex rg = new Regex(@"\s*(\w+)\s+([-\w\d]+)\s+(.*)", RegexOptions.Compiled);
              int lineCount = 0;
              double ps = progressBar1.Maximum - progressBar1.Minimum;
              progressBar1.Value = progressBar1.Minimum; using (StreamReader sr = new StreamReader(fileStream))
              {
                string? line = null;
                while ((line = sr.ReadLine()) != null)
                {
                  if (++lineCount > 15 && ps > 0)
                  {
                    lineCount = 0;
                    int pos = unchecked((int)((ps * fileStream.Position) / size));
                    if (pos < progressBar1.Minimum) pos = progressBar1.Minimum;
                    else
                    {
                      if (pos > progressBar1.Maximum) pos = progressBar1.Maximum;
                    }
                    progressBar1.Value = pos; Application.DoEvents();
                  }
                  if (line.Length == 0 || line[0] == '#') continue; // empty or comment
                  Match m = rg.Match(line);
                  if (!m.Success || m.Groups.Count < 3)
                  {
                    Debugger.Break(); continue;
                  }
                  string key = m.Groups[1].Value;
                  string option = m.Groups[2].Value;
                  string value = m.Groups[3].Value;
                  string ko = key;
                  if (option.Length > 0 && option[0] == '-') ko = key + option;
                  if (!keyWords.ContainsKey(ko)) keyWords.Add(ko, value);
                  switch (ko.ToLower())
                  {
                    case "set_global_assignment-name":
                      QCParser parser = new QCParser(value);
                      string symbol = parser.ReadToWhiteSpace();
                      string data = parser.ReadToWhiteSpace();
                      string optionpar = parser.ReadToEndLine();
                      qpf.AddGlob(symbol, data, optionpar);
                      break;
                    case "set_location_assignment":
                      Match mloc = rgxLoc.Match(value);
                      if (mloc.Success && mloc.Groups.Count > 1)
                      {
                        qpf.AddLoc(mloc.Groups[1].Value, option, null);
                      }
                      else
                      { Debugger.Break(); }
                      break;
                    case "set_instance_assignment-name":
                      Match minst = rgxInst.Match(value);

                      if (value.IndexOf("IO_STANDARD", 0, StringComparison.InvariantCultureIgnoreCase) < 0)
                      { continue;  /* Partition assignments are not checked*/ }
                      if (minst.Success && minst.Groups.Count > 3)
                      {

                        qpf.AddLoc(minst.Groups[3].Value, null, minst.Groups[2].Value);
                      }
                      else
                      {
                        minst = rgxInst2.Match(value);
                        if (minst.Success && minst.Groups.Count > 3)
                        {
                          if (minst.Groups[3].Value.StartsWith("HSMC_R"))
                          { }
                          qpf.AddLoc(minst.Groups[3].Value, null, minst.Groups[2].Value);
                        }
                        else
                        { Debugger.Break(); }
                      }

                      break;
                    default:
                      qcr.AppendLine(String.Format("Unknown {0} {1} {2}", key, option, value), QCReport.FT.regular);
                      Debugger.Break();
                      break;
                  }
                }
              }
            }
          }
          string sloc = qpf.DebugGetLocations();
          string sglob = qpf.DebugGetGlobals();
          File.WriteAllText("E:\\@VS_PRG\\@Vyuka\\LSPtools\\Debug.txt", sloc);
          File.WriteAllText("E:\\@VS_PRG\\@Vyuka\\LSPtools\\Debug2.txt", sglob);
          setWindowTitle();
          IniSettings.MRUFilesRulers.AddFile(currentFilename);
          IniSettings.MRUFilesRulers.AddToMenuItem(recentToolStripMenuItem, recentToolStripMenuItem_Click);

          return true;
        }
      }
      catch (Exception ex)
      {
        Trace.WriteLine(ex.ToString());
        tsslMessage.Text = ex.Message;
        setWindowTitle();
      }
      try
      {
        IniSettings.MRUFilesRulers.RemoveFile(filename, recentToolStripMenuItem);
      }
      catch (Exception) { }
      return false;
    }
#else // TABLE_CREATION
    bool _openQuartusProjectLock = false;

    private bool openQuartusProject(string filenameOfProject)
    {
      return openQuartusProject(filenameOfProject, false);
    }

    private bool openQuartusProject(string filenameOfProject, bool isAutoReload)
    {
      if (_openQuartusProjectLock)
      {
        if (isAutoReload) return false;
        int count = 5;
        do { Application.DoEvents(); }
        while (0 < count-- && _openQuartusProjectLock);
      }
      if (!File.Exists(filenameOfProject)) return false;
      try
      {
        _openQuartusProjectLock = true;
        timerReload.Enabled = false;
        try
        {
          filenameOfProject = Path.GetFullPath(filenameOfProject);
          int isDir;
          string? s = QuartusProject.ValidateFilename(filenameOfProject, out isDir);
          if (s != null && s.Length > 0)
            quartusProject.AddError("Quartus project path!", s, TodoTexts.TT.WRONG_PATH);
          if (!isAutoReload)
          {
            string path = Path.GetDirectoryName(filenameOfProject);
            path = Path.Combine(path, "db");
            if (Directory.Exists(path))
            {
              // The Quartus opens this file in none-sharing mode to disable renaming of project directory.
              string[] flocks = Directory.GetFiles(path, "*.flock");
              if (flocks.Length > 0)
              {
                using (QCFormSaveQuestion qcfs = new QCFormSaveQuestion())
                {
                  qcfs.StartPosition = FormStartPosition.CenterParent;
                  qcfs.ShowDialog();
                }
              }
            }
          }
        }
        catch (Exception ex) { Trace.WriteLine(ex.ToString()); }
        try
        {
          quartusProject.Clear();
          quartusProject.Filename = filenameOfProject;

          FileInfo fi = new FileInfo(quartusProject.Filename);
          lastFilenameAccess = fi.LastWriteTime;

          using (FileStream fileStream = new FileStream(filenameOfProject, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
          {

            Regex rg = new Regex(@"\s*(\w+)\s*=\s*\""(.+)\""", RegexOptions.Compiled);
            using (StreamReader sr = new StreamReader(fileStream))
            {
              string? line = null;
              while ((line = sr.ReadLine()) != null)
              {
                if (line.Length == 0 || line[0] == '#') continue; // empty or comment
                Match m = rg.Match(line);
                if (!m.Success || m.Groups.Count < 3) continue;
                string key = m.Groups[1].Value;
                if (String.IsNullOrEmpty(key)) continue;
                string value = m.Groups[2].Value;
                if (String.IsNullOrEmpty(value)) continue;
                qcReport.WrLine(String.Format("{0}={1}", key, value));
                switch (key.ToUpper())
                {
                  // active revision is the first in the list
                  case "PROJECT_REVISION":
                    quartusProject.ActiveRevisionName = value;
                    break;
                  case "QUARTUS_VERSION":
                    if (string.IsNullOrEmpty(quartusProject.ProjectCreatedInQuartusVersion))
                      quartusProject.ProjectCreatedInQuartusVersion = value;
                    break;
                  case "DATE":
                    if (string.IsNullOrEmpty(quartusProject.ActiveDate))
                      quartusProject.ActiveDate = value;
                    break;
                }
              }
            }
          }
          if (quartusProject.ActiveRevisionName == null)
          {
            qcReport.WrLineB("Unknown format of the file: " + filenameOfProject);
            displayStatusMessage.Info("Open valid Quartus project file");
            quartusProject.Clear();
            return false;
          }
          string quartusRevisionFile = quartusProject.ActiveRevisionName + ".qsf";
          string qsfull = Path.Combine(Path.GetDirectoryName(filenameOfProject), quartusRevisionFile);
          if (!File.Exists(qsfull))
          {
            quartusProject.AddError(quartusRevisionFile, "Failure to read the setting file of active project revision: " + qsfull,
                         TodoTexts.TT.CHANGE_REVISION);
            return false;
          }
          veekmt2 ??= new VEEKMT2(); //if (veekmt2 == null) veekmt2 = new VEEKMT2();
          if (veekmt2 == null)
            throw new NullReferenceException("new VEEKMT2() returned null. Try restarting.");
          veekmt2.Clear();
          Regex rgxInst = new Regex(@"(\w+)\s+("".+"")\s+-to\s+(.*)", RegexOptions.Compiled);
          Regex rgxInst2 = new Regex(@"(\w+)\s+(\w+)\s+-to\s+(.*)", RegexOptions.Compiled);
          Regex rgxLoc = new Regex(@"\s*-to\s+(.+)", RegexOptions.Compiled);
          using (FileStream fileStream = new FileStream(qsfull, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
          {
            if (fileStream == null || fileStream.Length == 0) throw new FileNotFoundException(filenameOfProject);
            long size = fileStream.Length;
            Regex rg = new Regex(@"\s*(\w+)\s+([-\w\d]+)\s+(.*)", RegexOptions.Compiled);
            int lineCount = 0;
            progressBar1.Value = progressBar1.Minimum; using (StreamReader sr = new StreamReader(fileStream))
            {
              string? line = null;
              while ((line = sr.ReadLine()) != null)
              {
                if (++lineCount > 15)
                {
                  lineCount = 0;
                  int pos = unchecked((int)((67 * fileStream.Position) / size));
                  if (pos < 0) pos = 0;
                  else
                  {
                    if (pos > progressBar1.Maximum) pos = progressBar1.Maximum;
                  }
                  progressBar1.Value = pos; Application.DoEvents();
                }
                if (line.Length == 0 || line[0] == '#') continue; // empty or comment
                Match m = rg.Match(line);
                if (!m.Success || m.Groups.Count < 3)
                {
                  Debugger.Break(); continue;
                }
                string key = m.Groups[1].Value;
                string option = m.Groups[2].Value;
                string value = m.Groups[3].Value;
                string ko = key;
                if (option.Length > 0 && option[0] == '-') ko = key + option;
                if (!keyWords.ContainsKey(ko)) keyWords.Add(ko, value);
                switch (ko.ToLower())
                {
                  case "set_global_assignment-name":
                    QCParser parser = new QCParser(value);
                    string symbol = parser.ReadToWhiteSpace();
                    string data = parser.ReadToWhiteSpace();
                    string optionpar = parser.ReadToEndLine();
                    quartusProject.AddGlob(symbol, data, optionpar);
                    break;
                  case "set_location_assignment":
                    Match mloc = rgxLoc.Match(value);
                    if (mloc.Success && mloc.Groups.Count > 1)
                    {
                      if (veekmt2.VerifyLoc(mloc.Groups[1].Value, option, null)) break;
                      else qcReport.WrLineR(String.Format("?Assignment {0}", veekmt2.Report));
                    }
                    else
                    {
                      qcReport.WrI("??? Unknown line in settings file: " + line);
                      /* wrong format */
                      Debugger.Break();
                    }
                    break;
                  case "set_instance_assignment-name":
                    Match minst = rgxInst.Match(value);

                    if (value.IndexOf("IO_STANDARD", 0, StringComparison.InvariantCultureIgnoreCase) < 0)
                    { continue;  /* Partition assignments are not checked*/ }
                    if (minst.Success && minst.Groups.Count > 3)
                    {

                      if (veekmt2.VerifyLoc(minst.Groups[3].Value, null, minst.Groups[2].Value)) break;
                      qcReport.WrLineR(String.Format("?Assignment {0}", veekmt2.Report));
                    }
                    else
                    {
                      minst = rgxInst2.Match(value);
                      if (minst.Success && minst.Groups.Count > 3)
                      {
                        if (LAssignment.GetIOstandard(minst.Groups[2].Value) != LAssignment.IO.Unknown
                          ? veekmt2.VerifyLoc(minst.Groups[3].Value, null, minst.Groups[2].Value)
                          : veekmt2.VerifyLoc(minst.Groups[3].Value, minst.Groups[2].Value, null))
                          break;
                        else qcReport.WrLineR(String.Format("?Assignment {0}", veekmt2.Report));
                      }
                      else
                      {
                        qcReport.WrI("??? Unknown line in settings file: " + line);
                        Debugger.Break();
                      }

                    }
                    break;

                  default:
                    qcReport.WrLineR(String.Format("Unknown {0} {1} {2}", key, option, value));
                    Debugger.Break();
                    break;
                }
              }
            }
          }

          GAssignment lgv;
          if (quartusProject.globAss.TryGetValue("LAST_QUARTUS_VERSION", out lgv))
          {
            string slgv = lgv.GetTopValue(); // we read revision
            if (slgv != null) quartusProject.RevisionLastQuartusVersion = slgv;
          }
          setWindowTitle();
          string sy;
          VEEKMT2.LAErrorsStatistic ler = veekmt2.GetResultsOfVerifications(out sy);
          if (ler.CountOK == 0)
          {
            qcReport.WrB("Zero FPGA pin assignments! Circuits can be only simulated.");
            quartusProject.AddError("Pin assignment", "No assignments!", TodoTexts.TT.IMPORT_ASSIGNMENTS, sy);
          }
          else
          {
            if (ler.CountPinErrors == 0)
            {
              qcReport.WrLineR(String.Format("All {0} used pin assignments are correct", veekmt2.CountLoc));
            }
            else
            {
              string sx = String.Format("Found {0} wrong pin assignments", ler.CountPinErrors);
              qcReport.WrLineB(sx);

              //qcr.AppendLineI(String.Format(" - {0} essential, {1} useful, and {2} optional",
              //                              ler.Top, ler.Second, ler.X));
              //qcr.AppendLineB(String.Format("Wrong {0} IO standard definitions", ler.CountIosErrors));
              //qcr.AppendLineI(String.Format(" - {0}  essential, {1} useful, and {2} optional",
              //                              ler.TopIO, ler.SecondIO, ler.XIO));
              //if (ler.Top + ler.Second + ler.TopIO + ler.SecondIO > 0)
              quartusProject.AddError("Pin assignment", sx, TodoTexts.TT.IMPORT_ASSIGNMENTS, sy);
              //else
              //{
              //  qpf.AddError("Pin assignment", "Some optional assignment are missing", TodoTexts.TT.IMPORT_ASSIGNMENTS);
              //}

            }
          }
          progressBar1.Value = 75;
          bool NCE = false, SDC = false, VER_VHDL = false;
          foreach (KeyValuePair<string, QuartusProject.GAssignment> kv in quartusProject.globAss)
          {
            string key = kv.Key; QuartusProject.GAValue[] ga = kv.Value.GetValues();

            // we search on in dictionary for parameter and its expected value
            string? expecterValueOfSettings = veekmt2.GetGlob(key);
            if (expecterValueOfSettings == null) continue; // we do not check this parameter

            string gaTopValue = ga != null && ga.Length > 0 ? ga[0].Value : String.Empty;

            //Length=1 signals variable strings content as  * filenames, $ versions, ? directories
            if (expecterValueOfSettings.Length == 1 && ga != null)
            {

              switch (expecterValueOfSettings[0])
              {
                case '*': // file name character specified in LSPtools gassDefinitions as the mark for filename
                  for (int i = 0; i < ga.Length; i++)
                  {
                    string vf = ga[i].Value;
                    string vfilename = quartusProject.GetAbsolutePath(vf);
                    if (vfilename.Length == 0) continue;
                    if (File.Exists(vfilename)) quartusProject.AddProjectFile(vfilename);
                    else
                    {
                      string stx = String.Format("Project file {0} was not found,", vfilename);
                      qcReport.WrLineB(stx);
                      quartusProject.AddError("FILE WAS NOT FOUND", stx, TodoTexts.TT.FILE_NOTFOUND);
                      continue;
                    }
                    if (key == "SDC_FILE")
                    {
                      SDC = true;
                      if (Path.GetExtension(vfilename).ToUpper() != ".SDC")
                      {
                        string stx = String.Format("SDC file {0} should have the extension .sdc,", vfilename);
                        qcReport.WrLineB(stx);
                        quartusProject.AddError("SDC EXTENSION", stx, TodoTexts.TT.SDC_EXT);
                      }
                    }

                    if (!vfilename.StartsWith(quartusProject.RootDir, StringComparison.OrdinalIgnoreCase))
                    {
                      string stx = String.Format("{0} is outside of the project directory", vfilename);
                      qcReport.WrLineB(stx);
                      quartusProject.AddError("FILE OUTSIDE OF PROJECT", stx, TodoTexts.TT.FILE_OUTSIDE);
                      continue;
                    }
                    int partCount;
                    string? result = QuartusProject.ValidateFilename(vfilename, out partCount);
                    if (result != null)
                    {
                      string stx = String.Format("Error {0} in project filename {1}", result, vfilename);
                      qcReport.WrLineB(stx);
                      quartusProject.AddError("FILENAME", stx, TodoTexts.TT.WRONG_FILENAME);
                      continue;
                    }
                  }
                  continue;
                case '$': // specialties
                  switch (key)
                  {
                    case "LAST_QUARTUS_VERSION":
                      quartusProject.RevisionLastQuartusVersion = gaTopValue;
                      break;
                    case "ORIGINAL_QUARTUS_VERSION":
                      quartusProject.RevisionOriginalQuartusVersion = gaTopValue;
                      break;
                    case "EDA_SIMULATION_TOOL":
                      int ixst = gaTopValue.IndexOf(quartusProject.SimulationTool, StringComparison.OrdinalIgnoreCase);
                      if (ixst < 0)
                        quartusProject.AddError("SIMULATION TOOL", gaTopValue,
                          quartusProject.IsModelSimVersion ? TodoTexts.TT.SIMULATION_MODELSIM : TodoTexts.TT.SIMULATION_QUESTA);

                      break;
                  }
                  continue;

                case '?': // directories
                  switch (key)
                  { // not checked yet
                    case "EDA_NETLIST_WRITER_OUTPUT_DIR":
                    case "PROJECT_OUTPUT_DIRECTORY":
                      quartusProject.OutputDir = gaTopValue;
                      break;

                    case "SEARCH_PATH":
                    case "TOP_LEVEL_ENTITY":
                      break;
                  }
                  continue;

                  //  for (int i = 0; i < ga.Length; i++)
                  //  {
                  //    string vdir = ga[i].Value;

                  //    int isDir;
                  //    string? result = QuartusProject.ValidateFilename(vdir, out isDir);
                  //    if (result != null)
                  //    {
                  //      qcr.AppendLineB(String.Format("Error {0} in project directory: {1}", result, vdir));
                  //      sbFiles.Append(vdir); continue;
                  //    }
                  //  }
                  //  break;
                  //  continue;
              }

            }
            int ixOfExpectedValue = gaTopValue.IndexOf(expecterValueOfSettings, StringComparison.OrdinalIgnoreCase);
            switch (key) //set_global_assignment -name
            {
              case "CYCLONEII_RESERVE_NCEO_AFTER_CONFIGURATION":
                if (ixOfExpectedValue >= 0)
                {
                  NCE = true;
                  qcReport.WrLineR("Found correct configuration for dual pin nCEO.");
                }
                continue;
              case "DEVICE":
                if (ixOfExpectedValue < 0)
                  quartusProject.AddError(key, "Wrong device " + gaTopValue + " instead of required EP4CE115F29C7", TodoTexts.TT.WRONG_DEVICE);
                continue;
              case "EDA_OUTPUT_DATA_FORMAT":
                if (ixOfExpectedValue < 0)
                  quartusProject.AddError(key, "Wrong internal simulation setting " + key, TodoTexts.TT.SIMULATION_MODELSIM);
                continue;
              case "LAST_QUARTUS_VERSION":
                quartusProject.RevisionLastQuartusVersion = expecterValueOfSettings;
                break;
              case "ORIGINAL_QUARTUS_VERSION":
                quartusProject.RevisionOriginalQuartusVersion = expecterValueOfSettings;
                break;
              case "SMART_RECOMPILE":
                if (ixOfExpectedValue < 0)
                  quartusProject.AddError(key, "No smart recompile!", TodoTexts.TT.SMART_RECOMPILE_OFF);
                break;
              case "FLOW_DISABLE_ASSEMBLER":
                if (ixOfExpectedValue < 0)
                  quartusProject.AddError(key, "Disabled assembler!", TodoTexts.TT.FLOW_DISABLE_ASSEMBLER);
                break;
              case "VHDL_INPUT_VERSION":
                VER_VHDL = true;
                if (ixOfExpectedValue < 0)
                  quartusProject.AddError(key, "Wrong VHDL version " + gaTopValue, TodoTexts.TT.VHDL2008);
                continue;
              case "TIMING_ANALYZER_MULTICORNER_ANALYSIS":
                qcReport.WrLineB($"Project contains {key} Quartus V20 assignment that prevents its opening in V13.");
                continue;
            }
          } // foreach-----------------------------------------------------------------------------------
          if (!NCE)
            quartusProject.AddError("Dual pin nCEO", "VEEK-MT2 requires nCEO pin as regular I/O", TodoTexts.TT.nCEO);
          if (!SDC)
          {
            quartusProject.AddError("MISSING SDC", "TimeQuest Timing Analyzer misses *.sdc file(s).", TodoTexts.TT.MISSING_SDC);
          }
          if (!VER_VHDL)
            quartusProject.AddError("Wrong VHDL version", "You should set VHDL version to VHDL2008", TodoTexts.TT.VHDL2008);
          if (quartusProject.ProjectCreatedInQuartusVersion != null)
            qcReport.WrLineR($"Project created in {quartusProject.ProjectCreatedInQuartusVersion}");
          if (quartusProject.ActiveRevisionName != null && quartusProject.RevisionLastQuartusVersion != null)
          {
            qcReport.WrR($"Its active revision {quartusProject.ActiveRevisionName} stored in ");
            qcReport.WrLineB($"{quartusProject.RevisionLastQuartusVersion}");
          }
          string? simdirn = quartusProject.GetSimulationDir();
          string simdir = simdirn==null ?String.Empty : simdirn;
          if (quartusProject.RevisionLastVersionNumber < 21)
          {
            if ( simdir.Length==0 || !simdir.Equals(QuartusProject.SIMDIR))
            {
              quartusProject.AddError("Warning: Wrong simulation directory",
              "The simulation directory is not defined correctly.", TodoTexts.TT.SIMULATION_DIR);
            }
          }
          else
          { // empty directory is default one
            if (simdir.Length>0 && !simdir.Equals(QuartusProject.SIMDIRQUESTA))
            {
              quartusProject.AddError("Wrong simulation directory",
              "The simulation directory is not defined correctly.", TodoTexts.TT.SIMULATION_DIRQUESTA);
            }

          }
          ReportList rlist = quartusProject.GetReports();
          if (rlist.Count == 0)
          {
            quartusProject.AddError("Info: No reports to analyze",
              "The project is not compiled so the checker cannot analyze reports.", TodoTexts.TT.NOREPORTS);
          }
          else
          {
            bool wasLoop = false, wasLatch = false, wasSensitivity = false, wasDivide = false;
            Regex rgrep = new Regex(@"\b(?:loop|latch|sensitivity|lpm_divide)\b", RegexOptions.IgnoreCase | RegexOptions.Compiled);
            for (int ixrep = 0; ixrep < rlist.Count; ixrep++)
            {
              string[] lines = File.ReadAllLines(rlist.Items[ixrep].Filename);
              for (int iline = 0; iline < lines.Length; iline++)
              {
                string line = lines[iline];
                Match match = rgrep.Match(line);
                if (match.Success)
                {
                  bool isVhd = line.IndexOf(".vhd", StringComparison.OrdinalIgnoreCase) >= 0;
                  switch (match.Value.ToLower())
                  {
                    case "lpm_divide":
                      quartusProject.AddError("LPM DIVIDE", "The project creates disallowed divider",
                        TodoTexts.TT.WARNING_DIVIDE, isVhd ? line : null);
                      wasDivide = true; break;
                    case "sensitivity":
                      if (line.IndexOf("Process Statement", StringComparison.OrdinalIgnoreCase) >= 0)
                      {
                        quartusProject.AddError("Incomplete process", "A process statement has incomplete statement list",
                          TodoTexts.TT.WARNING_SENSITIVITY, isVhd ? line : null);

                        wasSensitivity = true;
                      }
                      break;
                    case "latch":
                      if (line.IndexOf("inferring latch", StringComparison.OrdinalIgnoreCase) >= 0) // we narrow search
                      {

                        quartusProject.AddError("LATCH", "The project creates a forbidden latch",
                          TodoTexts.TT.WARNING_LATCH, line);

                        wasLatch = true;
                      }
                      break;
                    case "loop":
                      if (line.IndexOf("combinational loop", StringComparison.OrdinalIgnoreCase) >= 0)
                      {
                        quartusProject.AddError("LOOP", "The project creates a forbidden combinational loop",
                                TodoTexts.TT.WARNING_LOOP, line);


                        wasLoop = true;
                      }
                      break;
                  }
                }
              }
            }
          }

          FillListView();
          IniSettings.MRUFilesQCheck.AddFile(quartusProject.Filename);
          IniSettings.MRUFilesQCheck.AddToMenuItem(recentToolStripMenuItem, recentToolStripMenuItem_Click);
          progressBar1.Value = progressBar1.Maximum;


          return true;

        }
        catch (Exception ex)
        {
          Trace.WriteLine(ex.ToString());
          tsslMessage.Text = ex.Message;
          setWindowTitle(); quartusProject.Clear();
        }
        try
        {
          IniSettings.MRUFilesQCheck.RemoveFile(filenameOfProject, recentToolStripMenuItem);
        }
        catch (Exception) { }
        return false;
      }
      finally
      {
        timerReload.Enabled = true; _openQuartusProjectLock = false;
        qcReport.WrLine("-------------------------------------------------------------------------");
      }
    }
    private void clearLogs_Click(object sender, EventArgs e)
    {
      qcReport.Clear();
    }

    private void FillListView()
    {
      try
      {
        listViewReports.BeginUpdate(); listViewReports.Items.Clear(); todo.Clear();
        TTItem[] tta = quartusProject.ToDoList.Values.ToArray();
        if (tta.Length == 0)
        {
          ListViewItem lvi = new ListViewItem("Info");
          lvi.Tag = new TTItem();
          ListViewItem.ListViewSubItem lvisub0 = new ListViewItem.ListViewSubItem(lvi, "The checker found 0 problems.");
          lvi.SubItems.Add(lvisub0);
          ListViewItem.ListViewSubItem lvisub01 = new ListViewItem.ListViewSubItem(lvi, "0");
          lvi.SubItems.Add(lvisub01);
          listViewReports.Items.Add(lvi);
        }
        else
        {
          for (int i = 0; i < tta.Length; i++)
          {
            TTItem tti = tta[i];

            ListViewItem lvi = new ListViewItem(TodoTexts.GetCategory(tti.tt));
            lvi.Tag = tti;
            ListViewItem.ListViewSubItem lvisub1 = new ListViewItem.ListViewSubItem(lvi, tta[i].tt.ToString());
            lvi.SubItems.Add(lvisub1);
            ListViewItem.ListViewSubItem lvisub11 = new ListViewItem.ListViewSubItem(lvi, tta[i].Count.ToString());
            lvi.SubItems.Add(lvisub11);
            listViewReports.Items.Add(lvi);
          }
        }
        descriptionRTB.Text= " "; // Clear() of assigning String.Empty clears also ZoomFactor
        todoRTB.Text = " "; // Clear() of assigning String.Empty clears also ZoomFactor
      }
      finally
      {
        listViewReports.EndUpdate();
      }
      foreach (ColumnHeader ch in this.listViewReports.Columns)
      {
        ch.Width = -2;
      }
    }

    private void listView1_SelectedIndexChanged(object sender, EventArgs e)
    {
      ListView.SelectedListViewItemCollection lsc = listViewReports.SelectedItems;
      descriptionRTB.Text = " "; // Clear() clears also font size;
      todo.Clear();
      if (lsc == null || lsc.Count == 0) return;
      try
      {
        ListViewItem? lvi = lsc[0];
        if (lvi == null || lvi.Tag == null) return;
        TTItem? tt = lvi.Tag as TTItem;
        if (tt == null || tt.Count == 0) return;
        string[] stt = tt.GetItems();

        StringBuilder sb = new StringBuilder();
        for (int i = 0; i < stt.Length; i++)
        {
          string s = stt[i];
          if (s == null || s.Length == 0) continue;
          sb.AppendLine(s);
        }
        descriptionRTB.Text = sb.ToString();

        string eng = TodoTexts.Get(tt.tt, out string cz);
        if (czRadioButton.Checked) todo.Wr(cz); else todo.Wr(eng);
        todo.ShowFirstLine();
      }
      catch (Exception ex) { Trace.WriteLine(ex.ToString()); }
    }

    private void czRadioButton_CheckedChanged(object sender, EventArgs e)
    {
      ListView.SelectedListViewItemCollection lsc = listViewReports.SelectedItems;
      todo.Clear();
      if (lsc == null || lsc.Count == 0) return;
      try
      {
        ListViewItem? lvi = lsc[0];
        if (lvi == null || lvi.Tag == null) return;
        TTItem? tt = lvi.Tag as TTItem;
        if (tt == null) return;
        string eng = TodoTexts.Get(tt.tt, out string cz);
        if (czRadioButton.Checked) todo.Wr(cz); else todo.Wr(eng);
        todo.ShowFirstLine();
      }
      catch (Exception ex)
      {
        Trace.WriteLine(ex.ToString());
      }
    }

    private void vwfAdjustToolStripMenuItem_Click(object sender, EventArgs e)
    {
      tabControl1.SelectedTab = tabPageVWF;
      if (qmsi.Versions.Count == 0)
      {
        vwfReport.WrB("Error: Quartus installation was not found.");
      }
      if (String.IsNullOrEmpty(openFileDialog2.InitialDirectory))
      {
        if (quartusProject.IsValidFilename) openFileDialog2.InitialDirectory = Path.GetDirectoryName(quartusProject.Filename);
        else openFileDialog2.InitialDirectory = openFileDialog1.InitialDirectory;
      }
      string vwf = String.Empty;
      if (openFileDialog2.ShowDialog() == DialogResult.OK)
      {
        adjustQuartusVWF(vwf = openFileDialog2.FileName);
        IniSettings.MRUFilesQCAdjustVWF.AddFile(vwf);
        IniSettings.MRUFilesQCAdjustVWF.AddToMenuItem(recentToolStripMenuItem, vwfAdjustToolStripMenuItem_Click);
      }
    }

    VWFdata? vwfdata = null;
    private bool adjustQuartusVWF(string vwfFilename)
    {
      if (String.IsNullOrEmpty(vwfFilename) || !File.Exists(vwfFilename))
        return false;
      try
      {
        tabControl1.SelectedTab = tabPageVWF;
        vwfReport.Clear(); vwfQuartusProjectTextBox.Clear(); vwfTopLevelEntity.Clear();
        vwfFilename = Path.GetFullPath(vwfFilename); vwfdata = null; vwfAdjustButton.Enabled = false;
        vwfOpenedFilenameTextBox.Text = vwfFilename;

        if (!quartusProject.IsValidFilename || quartusProject.RootDir != Path.GetDirectoryName(vwfFilename))
        {
          string dv = Path.GetDirectoryName(vwfFilename);
          string[] pfiles = Directory.GetFiles(dv, "*.qpf", SearchOption.TopDirectoryOnly);
          if (pfiles == null || pfiles.Length != 1 || pfiles[0] == null)
          {

            vwfReport.WrB("Error: A valid Quartus project was not found in VWF file directory."); return false;

          }
          //if (MessageBox.Show("Tool needs to open Quartus project\r\nto which VWF file belongs.\r\nOpen " + pfiles[0],
          // "Open Quartus Project?", MessageBoxButtons.OKCancel, MessageBoxIcon.Question)
          //   != DialogResult.OK) return false;
          if (!openQuartusProject(pfiles[0]))
          {
            vwfReport.WrB("Error: The corresponding Quartus project contains errors. Correct them first.");
            return false;
          }
          if (quartusProject.RevisionLastVersionNumber < 18)
          {
            vwfQuartusProjectTextBox.Text = quartusProject.VerboseInfo();
            vwfQuartusProjectTextBox.SelectionStart = 0; vwfQuartusProjectTextBox.SelectionLength = 0;
            vwfReport.WrB("VWF adjusting is only required for Quartus major versions from 18 to 20.");
            return false;
          }
        }
      }
      catch (Exception ex) { Trace.WriteLine(ex.ToString()); }
      try
      {
        vwfQuartusProjectTextBox.Text = quartusProject.VerboseInfo();
        vwfQuartusProjectTextBox.SelectionStart = 0; vwfQuartusProjectTextBox.SelectionLength = 0;
        vwfTopLevelEntity.Text = quartusProject.GetTopLevelEntity();
        vwfdata = new VWFdata(vwfFilename);
        string error = vwfdata.CreateForQuartusProject(quartusProject);
        if (error == null || error.Length == 0)
        {
          vwfAdjustButton.Enabled = true;
          vwfReport.WrLineI("After pressing [Overwrite VWF] button, the original VWF file will modified.");
          vwfReport.WrLineI("If it is opened in Quartus, close it and reopen to propagate its correction.");
          vwfAdjustButton.Focus();
          return true;
        }
        vwfReport.WrB(error);
      }
      catch (Exception ex)
      {
        Trace.WriteLine(ex.ToString());
      }
      vwfdata = null; vwfAdjustButton.Enabled = false;

      return false;

#endif // TABLE_CREATION

    }
    DateTime lastFilenameAccess = DateTime.MinValue;
    private void timerReload_Tick(object sender, EventArgs e)
    {
      if (!quartusProject.IsValidFilename) return;
      FileInfo fi = new FileInfo(quartusProject.Filename);
      if (lastFilenameAccess < fi.LastWriteTime)
      {
        if (!_openQuartusProjectLock)
        {
          openQuartusProject(fi.FullName);
          lastFilenameAccess = fi.LastWriteTime;
        }
      }

    }

    private void vwfAdjustButton_Click(object sender, EventArgs e)
    {
      if (vwfdata != null)
      {
        vwfReport.WrLineR(vwfdata.WriteVWF());
      }
      else
      {
        vwfAdjustButton.Enabled = false;
      }
    }

    private void panelToDoEngCz_Resize(object sender, EventArgs e)
    {
      Rectangle r = panelToDoEngCz.ClientRectangle;
      int h = labelToDo.Height;
      labelToDo.Location = new Point(labelToDo.Location.X, r.Height / 2 - h / 2);
    }


  }
}
