using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;

namespace LSPtools
{
  internal static class IniData
  {
    private static string? _lspTools_IniFileName = null;

    private static StringBuilder debugSB;
    public static  StringWriter DebugLogWriter = new StringWriter(debugSB = new StringBuilder());
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
        IniData._lspTools_IniFileName = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "LSPTools.ini");
        if (!File.Exists(IniData._lspTools_IniFileName))
        {
          // old version
          string s = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "LSPTools.ini");
          if (File.Exists(s))
          {
            try
            {
              File.Copy(s, IniData._lspTools_IniFileName);
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
      finally { Environment.CurrentDirectory=cd; }
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

  }
}
