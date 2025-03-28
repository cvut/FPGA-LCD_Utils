using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;

namespace FpgaLcdUtils
{
  internal static class IniData
  {
    private static string? _lspTools_IniFileName = null;

    private static StringBuilder debugSB;
    public static StringWriter DebugLogWriter = new StringWriter(debugSB = new StringBuilder());
    public static int DebugLogLength { get { return debugSB.Length; } }
    public static string DebugLog { get { return debugSB.ToString(); } }
    public static bool IsFileRdWr(string filename)
    {
      bool OK = false;
      try
      {
        FileStream? fs = null;
        if (!File.Exists(_lspTools_IniFileName)) fs = File.Create(_lspTools_IniFileName);
        else fs = new FileStream(_lspTools_IniFileName, FileMode.Open);
        if (fs != null)
        {
          OK = fs.CanRead && fs.CanWrite;
          fs.Close(); fs.Dispose();
        }
      }
      catch { }
      return OK;
    }
    public static string? GetSettingFilename()
    {
      if (IniData._lspTools_IniFileName == null) // not loaded yet
      {
        IniData._lspTools_IniFileName = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                                         "FpgaLcdUtils.ini"); // application was renamed
        if (!File.Exists(IniData._lspTools_IniFileName))
        {
          string sold = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "LSPTools.ini");
          if (!File.Exists(sold))
          {
            // old version
            sold = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "LSPTools.ini");
          }
          if (File.Exists(sold))
          {
            try
            {
              File.Copy(sold, IniData._lspTools_IniFileName);
              File.Delete(sold);
            }
            catch (Exception) { }
          }
        }
      }
      if (IsFileRdWr(_lspTools_IniFileName)) return _lspTools_IniFileName;
      return null;
    }

    /// <summary>
    /// Create XmlTextReader and processes attributes by calling TASetting.Test
    /// </summary>
    /// <param name="settingsFileName"></param>
    /// <returns></returns>
    public static bool LoadSettingsFromFile(string settingsFileName)
    {
      if (String.IsNullOrEmpty(settingsFileName) || !System.IO.File.Exists(settingsFileName)) return false;
      string cd = Environment.CurrentDirectory;
      try
      {
        Environment.CurrentDirectory = System.IO.Path.GetDirectoryName(settingsFileName);
        using (XmlTextReader reader = new XmlTextReader(settingsFileName))
        {
          string predesly = "";
          while (reader.Read())
          {
            switch (reader.NodeType)
            {
              case XmlNodeType.Element:
                if (reader.HasAttributes)
                {
                  predesly = reader.Name;
                  while (reader.MoveToNextAttribute())
                  {
                    IniSettings.Test(reader.Name, reader.Value);
                  }
                }
                break;

              case XmlNodeType.Text:

                break;
              case XmlNodeType.EndElement:
                break;
            }
          }
          reader.Close();
        }
        return true;
      }
      catch (Exception ex)
      {
        Trace.WriteLine(ex.ToString());
        return false;
      }
      finally { Environment.CurrentDirectory = cd; }
    }

    public static void SaveSettingToFile(string settingsFileName)
    {
      string cd = Environment.CurrentDirectory;
      try
      {
        Environment.CurrentDirectory = System.IO.Path.GetDirectoryName(settingsFileName);
        XmlTextWriter writer = new XmlTextWriter(settingsFileName, null);
        writer.Formatting = Formatting.Indented;
        writer.Indentation = 4;
        writer.WriteStartElement("Settings");
        IniSettings.WriteToXml(writer);
        writer.WriteEndElement();
        writer.Close();
      }
      catch (Exception ex)
      {
        Trace.WriteLine(ex.ToString());
      }
      finally { Environment.CurrentDirectory = cd; }
    }

    /// <summary>
    /// It perform [for i=filename.Count-2 to 0]  filename[i] -> filename[i+1]
    /// </summary>
    /// <param name="filenames">full file names</param>
    /// <param name="moveOrg">true - filename[0] is renamed to filename[1], otherwise [0] is copied to [1]</param>
    public static void CreateBackups(string[] filenames, bool moveOrg)
    {
      for (int i = filenames.Length - 2; i >= 0; i--)
      {
        try
        {
          if (File.Exists(filenames[i]))
          {

            if (File.Exists(filenames[i + 1])) File.Delete(filenames[i + 1]);
            if (i == 0 && !moveOrg) File.Copy(filenames[0], filenames[1]);
            else File.Move(filenames[i], filenames[i + 1]);
          }
        }
        catch (Exception ex)
        {
          Trace.WriteLine("CreateBackups:" + ex.ToString());
        }

      }
    }

    public static void AssignNUD(NumericUpDown nud, int n)
    {
      AssignNUD(nud, (decimal)n);
    }
    public static void AssignNUD(NumericUpDown nud, double d)
    {
      AssignNUD(nud, (decimal)d);
    }

    public static void AssignNUD(NumericUpDown nud, decimal ndecimal)
    {
      decimal min = nud.Minimum, max = nud.Maximum;
      if (ndecimal < nud.Minimum) ndecimal = nud.Minimum;
      if (ndecimal > nud.Maximum) ndecimal = nud.Maximum;
      nud.Value = ndecimal;

    }

    public static string CheckReadAccessToFile(string filename)
    {
      try
      {
        FileInfo fi = new FileInfo(filename);
        if (!fi.Exists)
        {
          return "Not found or access deinied: " + filename;
        }
        using (FileStream fs = new FileStream(filename, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
        {
          if (!fs.CanRead)
          {
            return "No permission to read file " + filename;
          }
          using (BinaryReader br = new BinaryReader(fs))
          {
            if (fi.Length > 0)
            {
              byte[] test = br.ReadBytes(128);
            }
          }
        }
        return String.Empty;
      }
      catch (Exception ex)
      {
        return ex.Message;
      }
    }

 /*  An attempt to remove modality of the open/save file dialog
  *  ***ToDo create own open file dialog
  private class ThreadDialogCall
    {
      public bool IsDialogResultOk = false;
      System.Windows.Forms.FileDialog? fileDialog = null;
      public ThreadDialogCall(System.Windows.Forms.FileDialog? OpenSaveFileDialog)
      {
        fileDialog = OpenSaveFileDialog;
      }

      public void ThreadProc()
      {
        try
        {
          if (fileDialog != null)
          {
            System.Windows.Forms.OpenFileDialog? openFileDialog = fileDialog as System.Windows.Forms.OpenFileDialog;
            if (openFileDialog != null)
            {
              IsDialogResultOk = openFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK;
              return;
            }
            System.Windows.Forms.SaveFileDialog? saveFileDialog = fileDialog as System.Windows.Forms.SaveFileDialog;
            if (saveFileDialog != null)
            {
              IsDialogResultOk = saveFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK;
              return;
            }
          }
        }
        catch (Exception) { }
      }
    }

    public static bool RunFileDialog(System.Windows.Forms.FileDialog openSaveDialog)
    {
      ThreadDialogCall tdc = new ThreadDialogCall(openSaveDialog);
      Thread t = new Thread(new ThreadStart(tdc.ThreadProc));
      t.SetApartmentState(ApartmentState.STA);
      t.Start();
      t.Join();
      return tdc.IsDialogResultOk;
    }
 */

  }
}



