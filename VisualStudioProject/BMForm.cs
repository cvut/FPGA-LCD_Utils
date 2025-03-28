using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Text;
using System.Windows.Forms;
using System.Xml.Linq;

namespace FpgaLcdUtils
{
  public partial class BMForm : Form
  {
    const int MAX_ADDRESS_WIDTH = 16;  // limit of Quartus
    /// <summary>
    /// previewed map with adjusted sized
    /// </summary>
    BmpItem? previewBitmap = null;
    /// <summary>
    /// original bitmap
    /// </summary>
    Bitmap? openedBitmap = null;
    String openedBitmapFile = String.Empty;
    const string messageOpenBitmap = ">> Open a bitmap <<";
    const string messageFailToLoadBitmap = "!!! Failed to load: ";
    const string messageAdjustBitmap = "** Adjust bitmap and stored its memory file **";
    string? firstMRUFileOnStart = null;
    readonly RichTBWriter infoMemory;
    public BMForm()
    {
      InitializeComponent(); this.ResizeRedraw = true;
      CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;
      panelExtendColor.BackColor = Color.Black;
      setNoBitmapLoaded();
      firstMRUFileOnStart = IniSettings.MRUFilesBitmap.AddToMenuItem(recentBitmapsToolStripMenuItem, recentBitmapsToolStripMenuItem_Click);
      infoMemory = new RichTBWriter(infoMemoryTypeRichTextBox);
    }

    bool isFormShown = false;
    private void BMForm_Shown(object sender, EventArgs e)
    {
      if (!isFormShown) { panelExtendColor.BackColor = Color.Black; timerUpdate.Enabled = true; }
      isFormShown = true;
      Font f = messagesErrorRichTextBox.Font;
      int pxFont = (int)Math.Ceiling(f.GetHeight(messagesErrorRichTextBox.CreateGraphics()));
      if (pxFont < 10) pxFont = 10;
      splitContainer2.Panel1MinSize = pxFont;
      splitContainer2.SplitterDistance = 2 * pxFont;
      IniSettings.GeometryBMP.ApplyGeometryToForm(this);
      if (firstMRUFileOnStart != null)
      {
        displayMessage("Opening the last bitmap file " + firstMRUFileOnStart, MessageSeverity.Info);
        if (!openBitmap(firstMRUFileOnStart))
        {
          IniSettings.MRUFilesBitmap.RemoveFile(firstMRUFileOnStart, recentBitmapsToolStripMenuItem);
        }
        else return;
      }
      displayMessage("Open a bitmap file.", MessageSeverity.Info);
    }

    private void BMForm_FormClosing(object sender, FormClosingEventArgs e)
    {
      timerUpdate.Enabled = false;
      try
      {
        IniSettings.GeometryBMP.StoreGeometry(this);
      }
      catch (Exception ex)
      {
        Trace.WriteLine("BMForm_FormClosing: " + ex.ToString());
      }
    }

    /// <summary>
    /// Check if x is a power of 2 or sum of two powers of 2 
    /// </summary>
    /// <param name="x">x>0</param>
    /// <returns>is 2^n or 2^n+2^m </returns>
    private bool isPower2orSum(int x)
    {
      if (x <= 0) return false;
      int bitCount = 0;
      while (x > 0) { bitCount += (x & 1); x >>= 1; }
      return (bitCount <= 2);
    }

    private void setNoBitmapLoaded()
    {
      messagesErrorRichTextBox.Text = " ";
      openedBitmap = null; previewBitmap = null;
      assignFilenameOfOpenedBitmap(null);
      txbLoadedSize.Text = "--";
      txbCountOfColors.Text = "--";
      previewBitmap = null;
      palettePanel.Invalidate();
      previewBitmapPanel.Invalidate();
    }


    private void openBitmap_Click(object sender, EventArgs e)
    {
      try
      {
        if (String.IsNullOrEmpty(openFileDialog1.InitialDirectory))
        {
          openFileDialog1.InitialDirectory = Path.GetDirectoryName(System.Windows.Forms.Application.ExecutablePath);
          //if (!openFileDialog1.InitialDirectory.EndsWith("\\")) openFileDialog1.InitialDirectory = openFileDialog1.InitialDirectory + "\\";
        }
        if (openFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
        {
          openBitmap(openFileDialog1.FileName);
        }
      }
      catch (Exception ex)
      {
        Trace.WriteLine(ex.ToString());
        displayStatusMessage(ex.Message, MessageSeverity.Error);
      }
    }
    /// <summary>
    /// Open bitmap and add it into MRU files list
    /// </summary>
    /// <param name="bitmapFilename"></param>
    /// <returns></returns>
    private bool openBitmap(string bitmapFilename)
    {
      try
      {
        if (openedBitmap != null) openedBitmap.Dispose();
        setNoBitmapLoaded();
        if (!testIfBitmapAccessible(bitmapFilename)) return false;
        // we open bitmap with share access
        using (FileStream rdtxt = new System.IO.FileStream(bitmapFilename, FileMode.Open,
                                                           FileAccess.Read, FileShare.ReadWrite))
        {
          openedBitmap = new Bitmap(rdtxt);
        }
        long mem = (long)(openedBitmap.Width) * openedBitmap.Height;
        if (mem > 0x10000)
        {
          MessageBox.Show(this, String.Format(
@"Bitmap has the sizes W x H = {0} x {1} = {2} pixels,
but the Quartus allows up to a maximum of 65536.
Split the bitmap first to smaller parts.", openedBitmap.Width, openedBitmap.Height, mem),
"Bitmap sizes exceed the address limit!",
          MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
          setNoBitmapLoaded();
          return false;
        }
        IniSettings.MRUFilesBitmap.AddFile(bitmapFilename);
        IniSettings.MRUFilesBitmap.AddToMenuItem(recentBitmapsToolStripMenuItem, recentBitmapsToolStripMenuItem_Click);
      }
      catch (Exception ex)
      {
        Trace.WriteLine(ex.ToString());
        displayStatusMessage(ex.Message, MessageSeverity.Error); return false;
      }
      try
      {
        Color2Byte.processBitmap(openedBitmap); _paletteBmp = null;

        int count = Color2Byte.ColorList.Count;

        //MessageBox.Show(String.Format("Opened BMP contains {0} colors.\r\nTry to reduce their count.",count),
        //                               "Too Many Colors",MessageBoxButtons.OK, MessageBoxIcon.Warning);
        txbCountOfColors.Text = count.ToString();
        if (count <= 16) displayMessage("Loaded bitmap is suitable for storing into ROM.");
        else
        {
          if (count > 256)
          {
            displayMessage(String.Format("Bitmap {0} was rejected! {1} colors make indexing unsuitable!",
                                          Path.GetFileName(bitmapFilename), count.ToString()),
                           MessageSeverity.Error);

            using (BMInfoForm info = new BMInfoForm(openedBitmap, Color2Byte.ListColorsSortedHSV.ToArray()))
            {
              info.ShowDialog(this);
              setNoBitmapLoaded();
            };
            IniSettings.MRUFilesBitmap.RemoveFile(bitmapFilename, recentBitmapsToolStripMenuItem);
            return false;
          }
          else
          {
            displayMessage("Bitmap has more colors than 16. Its ROM storage will be non optimal!", MessageSeverity.Warning);
          }

          //https://www.faststone.org/FSIVDownload.htm
        }
        displayStatusMessage("* Adjust the bitmap", MessageSeverity.Warning);
        nudX0.Value = 0; nudY0.Value = 0;
        nudReloadWidth.Value = openedBitmap.Width;
        nudReloadHeight.Value = openedBitmap.Height;
        openedBitmapFile = bitmapFilename;
        int size = openedBitmap.Width * openedBitmap.Height;
        txbLoadedSize.Text = String.Format("Width x Height = 0x{0:X} x 0x{1:X} = {0} x {1}; Size 0x{2:X} = {2} bits",
                                            openedBitmap.Width, openedBitmap.Height, size);
        int n = Color2Byte.ListColorsSortedHSV.Count;
        txbCountOfColors.Text = n.ToString();
        recreatePreviewBitmap();
        if (previewBitmap == null)
        {
          setNoBitmapLoaded(); openedBitmapFile = String.Empty;
          return false;
        }
        else
          return true;
      }
      catch (Exception ex)
      {
        Trace.WriteLine(ex.ToString());
        displayStatusMessage(ex.Message, MessageSeverity.Error);
        IniSettings.MRUFilesBitmap.RemoveFile(bitmapFilename, recentBitmapsToolStripMenuItem);
        setNoBitmapLoaded();
        return false;
      }
    }

    class CBI
    {
      public string Text;
      public string Info;
      public CBI(string text, string info) { this.Text = text; this.Info = info; }
      public override string ToString()
      {
        return Text;
      }
    }
    void assignFilenameOfOpenedBitmap(string? filename)
    {
      this.Text = "LSP tool Bitmap for Cyclone IV";
      if (!testIfBitmapAccessible(filename))
      {
        displayStatusMessage(messageOpenBitmap);
        previewBitmap = null;
        openedBitmap = null;
        return;
      }

      string sn;
      string s = String.Format("{0} [{1}]", sn = Path.GetFileName(filename), Path.GetFullPath(filename));
      this.Text = "LSPtool bitmap: " + s;
      openedFilenameTextBox.ForeColor = SystemColors.WindowText; openedFilenameTextBox.Text = sn;
    }

    bool testIfBitmapAccessible(string? filename)
    {
      if (filename == null || filename.Length == 0)
      {
        displayStatusMessage(messageOpenBitmap);
        openedFilenameTextBox.ForeColor = SystemColors.WindowText;
        openedFilenameTextBox.Text = messageOpenBitmap;
        return false;
      }
      if (!File.Exists(filename))
      {
        displayMessage("File was not found: " + filename, MessageSeverity.Error);
        openedFilenameTextBox.ForeColor = Color.Red;
        openedFilenameTextBox.Text = messageFailToLoadBitmap;
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
      openedFilenameTextBox.ForeColor = Color.Red;
      openedFilenameTextBox.Text = messageFailToLoadBitmap;
      return false;
    }




    public class BmpItem
    {
      public readonly Bitmap bitmap;
      public readonly string Filename;
      public readonly int Width;
      public readonly int Height;
      public readonly int BitWidthData;
      public readonly int BitWidthAddress;
      public readonly UInt32[] IndexedColors;
      public BmpItem(Bitmap openedBitmap, string filename, Color c,
                     int x0, int y0, int width, int height, UInt32[] indexColors)
      {
        if (openedBitmap == null || filename == null || indexColors == null)
          throw new ArgumentNullException("BmpItem constructor expect not null parameters");
        this.Filename = filename; this.Width = width; this.Height = height;
        IndexedColors = indexColors;
        BitWidthData = Color2Byte.GetBitWidth(indexColors.Length - 1);
        BitWidthAddress = Color2Byte.GetBitWidth(width * height - 1);
        int worg = openedBitmap.Width, horg = openedBitmap.Height;
        bitmap = new Bitmap(width, height, PixelFormat.Format24bppRgb);
        for (int y = 0; y < Height; y++)
        {
          int yorg = y - y0;
          bool yorgOk = yorg >= 0 && yorg < horg;
          for (int x = 0; x < Width; x++)
          {
            int xorg = x - x0;
            Color cnew = xorg >= 0 && xorg < worg && yorgOk ? openedBitmap.GetPixel(xorg, yorg) : c;
            bitmap.SetPixel(x, y, cnew);
          }
        }
      }

      public Color this[int x, int y]
      { get { return bitmap.GetPixel(x, y); } }

      public override string ToString()
      {
        int size = Width * Height;
        return String.Format("File={0}; W x H={1}x{2}=[0x{3:X}] bits; Data bus width={4} bits; Memory size={5} [0x{5:X}] bits",
        Filename, Width, Height, size, BitWidthData, size * BitWidthData);
      }
    }


    int lastBitmapW = 0, lastBitmapH = 0;

    void setWidthMessage(int w)
    {
      if (isPower2orSum(w))
      {
        displayMessage(messagesErrorRichTextBox, "OK: The width allows calculating memory addresses without a hardware multiplier.", MessageSeverity.Info);
        nudReloadWidth.ForeColor = SystemColors.ControlText;
      }
      else
      {
        nudReloadWidth.ForeColor = Color.Red;
        //      tsslMessage.ForeColor = Color.Red;
        int min = w - 1, max = w + 1;
        while (min > 0)
        {
          if (isPower2orSum(min)) break; min--;
        }
        while (max <= 65536)
        {
          if (isPower2orSum(max)) break; max++;
        }
        displayMessage(messagesErrorRichTextBox, String.Format("KO: ROM width is not 2**n or (2**n+2**m)! The nearest better widths are {0} or {1}", max, min),
                       MessageSeverity.Error);
      }
    }


    private void recreatePreviewBitmap()
    {
      if (!isFormShown || openedBitmap == null) return;
      try
      {
        int w = (int)(nudReloadWidth.Value);
        setWidthMessage(w);
        int h = (int)(nudReloadHeight.Value);
        lastBitmapH = h; lastBitmapW = w;
        int x0 = (int)(nudX0.Value);
        int y0 = (int)(nudY0.Value);


        previewBitmap = new BmpItem(openedBitmap, openedBitmapFile,
          panelExtendColor.BackColor, x0, y0, w, h, Color2Byte.ListColorsSortedHSV.ToArray());
        previewBitmapPanel.Invalidate();
        palettePanel.Invalidate();
        UpdateBitmapInMemoryParameters();
        assignFilenameOfOpenedBitmap(openedBitmapFile);
      }
      catch (Exception ex)
      {
        Trace.WriteLine(ex.ToString());
        displayStatusMessage(ex.Message, MessageSeverity.Error);
        setNoBitmapLoaded();
      }
    }
    private int scaleOfPreviewImage = 1;


    private void nudX0_ValueChanged(object sender, EventArgs e)
    {
      if (previewBitmap == null) return;
      UpdateBitmapInMemoryParameters();
      recreatePreviewBitmap();
    }

    private void nudY0_ValueChanged(object sender, EventArgs e)
    {
      if (previewBitmap == null) return;
      UpdateBitmapInMemoryParameters();
      recreatePreviewBitmap();
    }

    private void nudReloadHeight_ValueChanged(object sender, EventArgs e)
    {
      if (previewBitmap == null) return;
      UpdateBitmapInMemoryParameters();
      recreatePreviewBitmap();
    }

    private void nudReloadWidth_ValueChanged(object sender, EventArgs e)
    {
      if (previewBitmap == null) return;
      UpdateBitmapInMemoryParameters();
      recreatePreviewBitmap();
    }

    private struct M9Kconfig
    {
      public int b, w;
      public M9Kconfig(int bits, int width)
      {
        b = bits; w = width;
      }
    }
    private readonly M9Kconfig[] M9K = new M9Kconfig[]
    { new M9Kconfig(8192,1),new M9Kconfig(4096,2),new M9Kconfig(2048,4),
      new M9Kconfig(1024,8),new M9Kconfig(1024,9),new M9Kconfig(512,16),
      new M9Kconfig(512,18),new M9Kconfig(256,32),new M9Kconfig(256,36) };
    int getM9K(int outputBits, int memoryBits)
    {
      if (memoryBits <= 0 || outputBits <= 0) return 0;
      int required;
      int aw = GetMemoryAddressWidth(memoryBits, out required);
      for (int i = 0; i < M9K.Length; i++)
      {
        M9Kconfig m9k = M9K[i];
        if (m9k.w >= outputBits)
        {
          int count = (int)(Math.Ceiling((double)required / m9k.b));
          return count;
        }
      }
      return 0;

    }
    void UpdateBitmapInMemoryParameters()
    {
      infoMemory.Clear();
      if (previewBitmap == null) return;

      int bitWidth = previewBitmap.BitWidthData;
      int pixels = previewBitmap.Width * previewBitmap.Height;
      int requiredSize;
      int memoryAddressWidth = GetMemoryAddressWidth(pixels, out requiredSize);
      double procent = requiredSize > 0 ? (100 * (double)pixels / requiredSize) : 100.0;
      infoMemory.WrB(previewBitmap.Width);
      infoMemory.WrR(" x ");
      infoMemory.WrB(previewBitmap.Height);
      infoMemory.WrR(" bitmap uses ");
      if (procent < 75) { infoMemory.SetColor(Color.Red); infoMemory.WrB(" only "); }
      infoMemory.WrB(procent.ToString("0.##"));
      infoMemory.WrR(" % "); infoMemory.RegularColor();
      infoMemory.WrR("of ");
      infoMemory.WrB(getM9K(bitWidth, pixels));
      infoMemory.WrLineR(" necessary M9K memory blocks");
      infoMemory.WrR(" that contain ");
      infoMemory.WrB(requiredSize);
      infoMemory.WrR(" x ");
      infoMemory.WrB(bitWidth);
      infoMemory.WrR(" data bits = ");
      int size = bitWidth * requiredSize;
      infoMemory.WrB(size);
      infoMemory.WrR(" [");
      infoMemory.WrR("0x");
      infoMemory.WrB(size.ToString("X"));
      infoMemory.WrR("] ROM bits");
    }

    private void toolStripMenuItemCopyToClipboard_Click(object sender, EventArgs e)
    {
      if (previewBitmap == null)
      {
        displayStatusMessage("No memory in preview", MessageSeverity.Warning);
        return;
      }
      string s = previewBitmap.ToString(); // UpdateListOfMemories();
      Clipboard.SetData(System.Windows.Forms.DataFormats.Text, s);
    }

    List<BmpItem> listOfBitmaps = new List<BmpItem>();


    private void saveAsMemoryInitializationFileToolStripMenuItem_Click(object sender, EventArgs e)
    {
      if (previewBitmap == null)
      {
        displayStatusMessage("No available bitmap in the preview", MessageSeverity.Warning);
        return;
      }
      try
      {
        saveFileDialog1.Filter = "MIF(*.mif)|*.mif|All files(*.*)|*.*";
        saveFileDialog1.Title = "Save as Memory Initialization File";
        saveFileDialog1.DefaultExt = "mif";

        if (!IniSettings.FilesBitmap.ShowSaveDialog(saveFileDialog1, new string[] { "mif", "vhdl", "bmp" },
                                                  previewBitmap.Filename, "mif")) return;
      }
      catch (Exception ex)
      {
        Trace.WriteLine(ex.ToString());
        displayStatusMessage(ex.Message, MessageSeverity.Error);
      }
      string filename = saveFileDialog1.FileName;

      //            string pattern = @"
      //-- Generated Memory Initialization File (.mif)
      //
      //WIDTH=1;
      //DEPTH=256;
      //
      //ADDRESS_RADIX=UNS;
      //DATA_RADIX=HEX;
      //
      //CONTENT BEGIN
      //	0    :   0;
      //	1    :   1;
      //	2    :   0;
      //	3    :   1;
      //	4    :   0;
      //	[5..7]  :   1;
      //	[8..10]  :   0;
      //	[11..12]  :   1;
      //	[13..255]  :   0;
      //END;
      //";
      StringBuilder sb = new StringBuilder();
      BmpItem bi = previewBitmap;
      int requiredMemorySize;
      addMemoryHeader(sb, bi, out requiredMemorySize);
      int memoryCountOfItems = bi.Height * bi.Width;
      int memoryDataBitWidth = bi.BitWidthData;
      sb.AppendLine(String.Format("-- Quartus MegaWizard Plug-In parameters: 1) the output bus width={0} bits; 2) the words of memory={1}",
                                 memoryDataBitWidth, requiredMemorySize));
      sb.AppendLine();
      sb.AppendLine(String.Format("WIDTH={0};", memoryDataBitWidth));

      sb.AppendLine(String.Format("DEPTH={0};", requiredMemorySize));
      sb.AppendLine();
      sb.AppendLine("ADDRESS_RADIX=UNS;");
      sb.AppendLine("DATA_RADIX=HEX;");
      sb.AppendLine();
      sb.AppendLine("CONTENT BEGIN");
      memoryCountOfItems = 0;
      byte last = 0; // listOfBitmaps[0].bitmap[0];
      int addressLast = 0, address = 0;
      int sizeY = bi.Height;
      int sizeX = bi.Width;
      for (int irow = 0; irow < sizeY; irow++)
      {
        for (int icolumn = 0; icolumn < sizeX; icolumn++)
        {
          Color c = bi.bitmap.GetPixel(icolumn, irow);
          byte b = (byte)(Color2Byte.ToIndex(c));
          if (b != last && address > 0)
          {
            WriteMIFLine(sb, address, addressLast, last, memoryDataBitWidth);
            last = b; addressLast = address;
          }
          address++;
        }
      }

      if (last != 0 || address == requiredMemorySize)
      { WriteMIFLine(sb, address, addressLast, last, memoryDataBitWidth); addressLast = address; }

      if (address < requiredMemorySize)
      {
        WriteMIFLine(sb, requiredMemorySize, addressLast, last, memoryDataBitWidth);
      }

      sb.AppendLine("END;");
      string result = sb.ToString();
      try
      {
        File.WriteAllText(filename, result);
        displayStatusMessage("MIF saved in file: " + filename, MessageSeverity.Info);
      }
      catch (Exception ex)
      {
        Trace.WriteLine(ex.ToString());
        displayStatusMessage(ex.Message, MessageSeverity.Error);
      }
    }

    private void addMemoryHeader(StringBuilder sb, BmpItem bi, out int requiredMemorySize)
    {
      sb.AppendLine("-- CTU-FEE in Prague, Dept. of Control Eng. [Richard Susta]");
      sb.AppendLine(String.Format("-- FPGA-LCD Utils generated file on {0}", DateTime.Now));
      int size = bi.Height * bi.Width;
      sb.AppendLine(String.Format("-- from bitmap file {0}", bi.Filename));
      int addressWidth = GetMemoryAddressWidth(size, out requiredMemorySize);
      if (requiredMemorySize > 0)
      {
        sb.AppendLine(String.Format("-- adjusted to the sizes: Width x Height= {0}x{1}={2} [0x{2:X}] pixels.",
                                   bi.Width, bi.Height, size));
        sb.AppendLine(String.Format("-- {0} [0x{0:X}] bit memory is arranged for an {1}-bit address bus reading a {2}-bit data output." +
          "",
                                     requiredMemorySize * bi.BitWidthData, bi.BitWidthAddress, bi.BitWidthData));
      }
      uint[] carr = bi.IndexedColors;
      if (carr != null && carr.Length > 0)
      {
        sb.AppendLine("-- The color palette in the index order as std_logic_vector(23 downto 0) items:");
        int colcount = 0, istart = 0;
        for (int i = 0; i < carr.Length; i++)
        {
          uint uc = carr[i];
          if (colcount == 0) { sb.Append("-- "); istart = i; } else sb.Append(",  ");
          sb.AppendFormat("X\"{0:X6}\"", uc);
          if (++colcount >= 8)
          {
            sb.AppendLine(String.Format("  -- {0} to {1}", istart, i)); colcount = 0;
          }
        }
        if (colcount != 0) sb.AppendLine(String.Format("  -- {0} to {1}", istart, carr.Length - 1));
      }
    }

    void WriteMIFLine(StringBuilder sb, int address, int addressLast, byte last, int n)
    {
      if (address - addressLast == 1)
      {
        if (n <= 4)
          sb.AppendFormat("\t{0}    :   {1:X};", addressLast, last);
        else
          sb.AppendFormat("\t{0}    :   {1:X2};", addressLast, last);

      }
      else
      {
        if (n <= 4)
          sb.AppendFormat("\t[{0}..{1}]    :   {2:X};", addressLast, address - 1, last);
        else
          sb.AppendFormat("\t[{0}..{1}]    :   {2:X2};", addressLast, address - 1, last);
      }
      sb.AppendLine();
    }

    private void saveAsVHDL_toolStripMenuItem_Click(object sender, EventArgs e)
    {
      if (previewBitmap == null)
      {
        displayStatusMessage("No bitmap in the preview.", MessageSeverity.Warning);
        return;
      }
      try
      {
        saveFileDialog1.Filter = "VHDL (*.vhd)|*.vhd|All files(*.*)|*.*";
        saveFileDialog1.Title = "Save as VHDL Entity";
        saveFileDialog1.DefaultExt = "vhd";
        if (!IniSettings.FilesBitmap.ShowSaveDialog(saveFileDialog1, new string[] { "vhdl", "mif", "bmp" },
                                                    previewBitmap.Filename, "vhd"))
          return;

      }
      catch (Exception ex)
      {
        Trace.WriteLine(ex.ToString());
        displayStatusMessage(ex.Message, MessageSeverity.Error);
        return;
      }
      string filename = saveFileDialog1.FileName;

      //      library ieee, work; use ieee.std_logic_1164.all; use ieee.numeric_std.all;
      //      entity romLCD10g is
      //        port
      //        (address: in std_logic_vector(13 downto 0);
      //    clock: in STD_LOGIC:= '1';
      //    q: out std_logic_vector(1 downto 0));
      //      end entity;

      //      architecture rtl of romLCD10g is
      //      --signal ix: integer range 0 to 2 * *address'LENGTH-1:=0;
      //type arr_t is array(0 to 2 * *address'LENGTH-1) of std_logic_vector(q'RANGE);
      //      constant arr :arr_t:=


      StringBuilder sb = new StringBuilder();
      sb.AppendLine("-- !!! Do not edit this file!!! You could corrupt its exact structure");
      sb.AppendLine("-- that is compiled by Quartus into M9K memory ROM blocks.");
      BmpItem bi = previewBitmap;
      int requiredMemorySize;
      addMemoryHeader(sb, bi, out requiredMemorySize);
      int memoryDataBitWidth = bi.BitWidthData;
      int memoryCountOfItems = bi.Height * bi.Width;
      sb.AppendLine();
      int maxItem = memoryCountOfItems - 1;
      if (maxItem == 0) maxItem = 1;
      int addressWidth = Color2Byte.GetBitWidth(maxItem);

      string entityname = Path.GetFileNameWithoutExtension(filename);
      sb.AppendFormat(@"library ieee, work; use ieee.std_logic_1164.all; use ieee.numeric_std.all;
entity {0} is
port ( address: in std_logic_vector({1} downto 0):=(others=>'0');
       clock: in std_logic:= '1';
       q: out std_logic_vector({2} downto 0):=(others=>'0'));
end entity;

architecture rtl of {0} is
  type arr_t is array(0 to 2**address'LENGTH-1) of std_logic_vector(q'RANGE);
  constant arr :arr_t:=(
", entityname, addressWidth - 1, memoryDataBitWidth - 1);
      //     memory = 0;
      byte last;
      unchecked
      {

        last = (byte)Color2Byte.ToIndex(bi.bitmap.GetPixel(0, 0));
      }
      int addressLast = 0, address = 0;
      _writeVhdlLineLength = 0; _writeVhdlLineComma = false;
      int sizeY = bi.Height, sizeX = bi.Width;
      for (int irow = 0; irow < sizeY; irow++)
      {
        for (int icolumn = 0; icolumn < sizeX; icolumn++)
        {

          Color c = bi.bitmap.GetPixel(icolumn, irow);
          byte b;
          unchecked
          {
            b = (byte)(Color2Byte.ToIndex(c));
          }
          if (b != last && address > 0)
          {
            WriteVHDLline(sb, address, addressLast, last, memoryDataBitWidth);
            last = b; addressLast = address;
          }
          address++;
        }
      }

      //if (last != 0 || address == memoryCountOfItems)
      //{ WriteVHDLline(sb, address, addressLast, last, memoryDataBitWidth); addressLast = address; }

      if (addressLast < requiredMemorySize)
      {
        WriteVHDLothers(sb, last, memoryDataBitWidth);
      }

      sb.AppendFormat(
@");
-------------------------------------------------------
begin
  process(clock)
  variable ix:integer range 0 to 2**address'LENGTH-1:=0;
  begin
      if rising_edge(clock) then
          ix:= to_integer(unsigned(address));
          q <= arr(ix);
      end if;
  end process;
end architecture;"
    );
      string result = sb.ToString();
      try
      {
        File.WriteAllText(filename, result);
        displayStatusMessage("The bitmap stored as the file: " + filename, MessageSeverity.Info);
      }
      catch (Exception ex)
      {
        Trace.WriteLine(ex.ToString());
        displayStatusMessage(ex.Message, MessageSeverity.Error);
      }

    }

    /// <summary>
    /// WriteVHDLline function uses it as internal counter.
    /// </summary>
    private int _writeVhdlLineLength = 0;
    private int _writeVhdlElements = 0;
    private bool _writeVhdlLineComma = false;
    /// <summary>
    /// Write from addressLast to address-1, storing byte lastDataByte with dataBitWidth  
    /// </summary>
    /// <param name="sb"></param>
    /// <param name="address"></param>
    /// <param name="addressLast"></param>
    /// <param name="lastDataByte"></param>
    /// <param name="dataBitWidth"></param>
    void WriteVHDLline(StringBuilder sb, int address, int addressLast, byte lastDataByte, int dataBitWidth)
    {
      if (dataBitWidth < 1) return;
      string txt = String.Empty;
      string lastBits = Convert.ToString(lastDataByte, 2); lastBits = lastBits.PadLeft(dataBitWidth, '0');
      if (address - addressLast == 1)
      {
        txt = String.Format(" {0}=>\"{1}\"", addressLast, lastBits);
      }
      else
      {
        if (address > 0) txt = String.Format(" {0} to {1}=>\"{2}\"", addressLast, address - 1, lastBits);
      }
      if (_writeVhdlLineComma) { sb.Append(','); _writeVhdlLineLength++; }
      _writeVhdlLineComma = true;
      if (_writeVhdlElements >= 8 || _writeVhdlLineLength > 180)
      {
        sb.AppendLine(); _writeVhdlLineLength = 0; _writeVhdlElements = 0;
      }
      sb.Append(txt); _writeVhdlLineLength += txt.Length; _writeVhdlElements++;
    }

    void WriteVHDLothers(StringBuilder sb, byte lastDataByte, int dataBitWidth)
    {
      if (dataBitWidth < 1) return;
      string txt = String.Empty;
      string lastBits = Convert.ToString(lastDataByte, 2); lastBits = lastBits.PadLeft(dataBitWidth, '0');
      txt = String.Format(" others=>\"{0}\"", lastBits);
      if (_writeVhdlLineComma) { sb.Append(','); _writeVhdlLineLength++; }
      _writeVhdlLineComma = true;
      if (_writeVhdlLineLength + txt.Length > 128) { sb.AppendLine(); _writeVhdlLineLength = 0; }
      sb.Append(txt); _writeVhdlLineLength += txt.Length;
    }

    /// <summary>
    /// Round size to M9K memory blocks
    /// </summary>
    /// <param name="countOfItems"></param>
    /// <param name="requiredSize"></param>
    /// <returns></returns>
    int GetMemoryAddressWidth(int countOfItems, out int requiredSize)
    {
      if (countOfItems < 1)
      {
        requiredSize = 0; return 0;
      }
      int maxItem = countOfItems - 1;
      if (maxItem <= 0) maxItem = 1;
      int addressWidth = Color2Byte.GetBitWidth(maxItem);
      requiredSize = 1;
      // size of memory is power of 2
      for (int i = 1; i <= addressWidth; i++) requiredSize = requiredSize * 2;
      return addressWidth;
    }

    private void exitToolStripMenuItem_Click(object sender, EventArgs e)
    {
      System.Windows.Forms.Application.Exit();
    }


    private void numericUpDownScale_ValueChanged(object sender, EventArgs e)
    {
      previewBitmapPanel.Invalidate();
    }

    private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
    {
      BMFormAbout fa = new BMFormAbout();
      fa.ShowDialog(this);
      fa.Dispose();
    }

    private void helpToolStripMenuItem1_Click(object sender, EventArgs e)
    {
      BMFormHelp fh = new BMFormHelp();
      fh.ShowDialog(this);
      fh.Dispose();

    }

    private void previewBitmapPanel_Paint(object sender, PaintEventArgs e)
    {
      DrawBitmap(e.Graphics, this.previewBitmap);
    }
    void DrawBitmap(Graphics g, BmpItem? bi)
    {
      if (previewBitmapPanel == null || bi == null) return;
      HatchBrush hBrush = new HatchBrush(HatchStyle.DiagonalCross, SystemColors.ControlDark, SystemColors.ControlLight);
      Rectangle rpanel = previewBitmapPanel.ClientRectangle;
      g.FillRectangle(hBrush, rpanel);
      scaleOfPreviewImage = (int)(numericUpDownScale.Value);
      g.DrawImage(bi.bitmap, 0, 0, bi.Width * scaleOfPreviewImage, bi.Height * scaleOfPreviewImage);
      //     previewBitmapItem = bi;
    }

    private void previewBitmapPanel_MouseMove(object sender, MouseEventArgs e)
    {
      try
      {
        //        string sx, sy;
        int x = e.X / scaleOfPreviewImage, y = e.Y / scaleOfPreviewImage;
        string xyText;
        if (x >= 0 && previewBitmap != null && y >= 0 && x < previewBitmap.Width && y < previewBitmap.Height)
        {
          xyText = String.Format("Pixel xy=[{0,4} {1,4}]; ", x, y);
          Color2Mesage(previewBitmap[x, y], xyText);
        }
        else displayStatusMessage(String.Empty, MessageSeverity.Info);
      }
      catch (Exception ex)
      {
        Trace.WriteLine(ex.ToString());
        displayStatusMessage(ex.Message, MessageSeverity.Error);
      }
    }
    private void previewBitmapPanel_MouseDoubleClick(object sender, MouseEventArgs e)
    {
      try
      {
        int x = e.X / scaleOfPreviewImage, y = e.Y / scaleOfPreviewImage;
        if (x >= 0 && previewBitmap != null && y >= 0 && x < previewBitmap.Width && y < previewBitmap.Height)
        {
          Color extendColor = previewBitmap[x, y];
          panelExtendColor.BackColor = extendColor;
          txbExtendValue.Text = String.Format("0x{0:X} = {0}", Color2Byte.ToIndex(extendColor));
          recreatePreviewBitmap();
        }
      }
      catch (Exception ex)
      {
        displayStatusMessage(ex.Message, MessageSeverity.Error);
        Trace.WriteLine(ex.ToString());
      }

    }

    private void btnToWindow_Click(object sender, EventArgs e)
    {
      //if (previewBitmap == null) return;
      //try
      //{
      //  FormPreview fp = new FormPreview(previewBitmap, scaleOfPreviewImage, previewBitmap?.Filename??String.Empty);
      //  fp.Show();
      //}
      //catch (Exception ex)
      //{
      //  displayStatusMessage(ex.Message, MessageSeverity.Error);
      //}
    }
    string? _displayMessageLastText = String.Empty;
    private void displayMessage(string text)
    {
      displayMessage(text, MessageSeverity.Info);
    }
    private void displayMessage(string text, MessageSeverity severity)
    {
      if (text == _displayMessageLastText) return;
      else _displayMessageLastText = text;
      displayMessage(messageRichTextBox, text, severity);
    }
    private void displayMessage(RichTextBox rtb, string text, MessageSeverity severity)
    {
      if (text == null || text.Trim().Length == 0) return;
      Color foreColor = SystemColors.WindowText;
      switch (severity)
      {
        case MessageSeverity.Error: foreColor = Color.Red; break;
        case MessageSeverity.Warning: foreColor = Color.BlueViolet; break;
      }
      int start = rtb.TextLength;
      if (rtb != messagesErrorRichTextBox)
      {
        rtb.AppendText(text + Environment.NewLine);
      }
      else { start = 0; rtb.Text = text; }
      int end = rtb.TextLength;
      // Textbox may transform chars, so (end-start) != text.Length
      rtb.Select(start, end - start);
      {
        rtb.SelectionColor = foreColor;
      }
      rtb.SelectionLength = 0;
    }

    private readonly TimeSpan _messageDuration = new TimeSpan(0, 0, 30); // 30 seconds
    public enum MessageSeverity { Info, Warning, Error };
    private bool _messageHasText = false;
    private DateTime _messageStartTime = DateTime.MinValue; // 10 seconds
    private void displayStatusMessage(string text)
    {
      displayStatusMessage(text, MessageSeverity.Info);
    }
    private void displayStatusMessage(string text, MessageSeverity severity)
    {
      Color foreColor = SystemColors.WindowText;
      switch (severity)
      {
        case MessageSeverity.Error: foreColor = Color.Red; break;
        case MessageSeverity.Warning: foreColor = Color.Green; break;
      }
      tsslMessage.ForeColor = foreColor;
      if (text == null || text.Trim().Length == 0)
      {
        tsslMessage.Text = String.Empty; _messageHasText = false;
      }
      else
      {
        tsslMessage.Text = text; _messageHasText = true;
        _messageStartTime = DateTime.Now;
      }
    }

    private void timerUpdate_Tick(object sender, EventArgs e)
    {
      if (_messageHasText)
      {
        DateTime dt = DateTime.Now;
        if ((dt - _messageStartTime) >= _messageDuration)
          displayStatusMessage(String.Empty);
      }
    }

    byte extendByte = 0;
    Color extendColor = Color.Black;

    private void txbExtendValue_TextChanged(object sender, EventArgs e)
    {
      if (!isFormShown) return;
      string s = txbExtendValue.Text.Trim();
      extendByte = 0;
      if (s.Length == 0) return;
      int n;
      CultureInfo provider = CultureInfo.InvariantCulture;
      if (s.StartsWith("0x", StringComparison.OrdinalIgnoreCase))
      {
        s = s.Substring(2);
        if (s.Length == 0) return;
        if (!int.TryParse(s, NumberStyles.HexNumber, provider, out n))
        {
          displayStatusMessage("Entered extend value has unknown hexadecimal digits after 0x...", MessageSeverity.Error);
          return;
        }
      }
      else
      {
        if (!int.TryParse(s, NumberStyles.Integer, provider, out n))
        {
          displayStatusMessage("Entered extend value has unknown integer format", MessageSeverity.Error);
          return;
        }
      }
      unchecked { extendByte = (byte)(n); };
      extendColor = Color2Byte.ToColor(extendByte);
      panelExtendColor.BackColor = extendColor;
      txbExtendValue.Text = String.Format("0x{0:X} = {0}", extendByte);
      previewBitmapPanel.Invalidate();
    }


    private void savePreviewBitmapToolStripMenuItem6_Click(object sender, EventArgs e)
    {
      if (previewBitmap == null) { displayMessage("No image loaded"); return; }
      try
      {
        if (!IniSettings.FilesBitmap.ShowSaveDialog(savePreviewBitmapFileDialog, new string[] { "bmp", "vhdl", "mif" },
                                                    null, "bmp")) // do not suggest file name
          return;
        string filename = savePreviewBitmapFileDialog.FileName;
        previewBitmap.bitmap.Save(filename, System.Drawing.Imaging.ImageFormat.Bmp);
        displayMessage("Bitmap saved as " + filename);
      }
      catch (Exception ex)
      {
        displayStatusMessage(ex.Message, MessageSeverity.Error);
      }
    }


    static public Bitmap CreatePaletteBitmap(Graphics gpanel, Panel palettePanel, UInt32[] palette)
    {
      if (palettePanel == null || gpanel == null) return new Bitmap(1, 1);
      Rectangle rpanel = palettePanel.ClientRectangle;
      Bitmap bmp = new Bitmap(rpanel.Width, rpanel.Height, gpanel);
      using (Graphics g = Graphics.FromImage(bmp))
      {
        g.FillRectangle(SystemBrushes.Control, rpanel);
        if (palette == null || palette.Length == 0) { return bmp; }
        int count = palette.Length;
        int rowCount = 1;
        // to obtain optimal result without rounding
        while (count > rowCount * 16 && rowCount < 16) rowCount++;
        int columnCount = count / rowCount; columnCount++;
        float hrow = rpanel.Height / (float)rowCount;
        float wcolumn = rpanel.Width / (float)columnCount;
        int xipos = 0, yipos = 0; ;
        for (int i = 0; i < count; i++)
        {
          UInt32 xu = palette[i];
          Color c = Color2Byte.FromUColor(xu);
          float ypos = hrow * yipos;
          g.FillRectangle(new SolidBrush(c), wcolumn * xipos, ypos, wcolumn, hrow);
          xipos++;
          if (xipos >= columnCount) { xipos = 0; yipos++; if (yipos >= rowCount) break; }
        }
      }
      return bmp;
    }
    private Bitmap? _paletteBmp = null;

    private void palettePanel_Paint(object sender, PaintEventArgs e)
    {
      if (previewBitmap == null) return;
      if (_paletteBmp == null) _paletteBmp = CreatePaletteBitmap(e.Graphics, palettePanel, previewBitmap.IndexedColors);
      if (_paletteBmp != null) e.Graphics.DrawImageUnscaled(_paletteBmp, 0, 0);
    }
    private void palettePanel_MouseMove(object sender, MouseEventArgs e)
    {
      try
      {
        if (_paletteBmp == null) return;
        int x = e.X, y = e.Y;
        if (x >= 0 && x < _paletteBmp.Width && y >= 0 && y < _paletteBmp.Height)
        {
          Color c = _paletteBmp.GetPixel(x, y);
          Color2Mesage(c);
        }
      }
      catch (Exception ex)
      {
        displayStatusMessage(ex.Message, MessageSeverity.Error);
        Trace.WriteLine(ex.ToString());
      }
    }



    private void palettePanel_Resize(object sender, EventArgs e)
    {
      _paletteBmp = null; palettePanel.Invalidate();
    }
    private void Color2Mesage(Color c)
    {
      Color2Mesage(c, String.Empty);
    }

    private void recentBitmapsToolStripMenuItem_Click(object sender, EventArgs e)
    {
      ToolStripMenuItem? mi = sender as ToolStripMenuItem;
      if (mi != null)
      {
        try
        {
          string filename = mi.ToolTipText;
          if (openBitmap(filename))
          {
            IniSettings.MRUFilesBitmap.AddFile(filename);
            IniSettings.MRUFilesBitmap.AddToMenuItem(recentBitmapsToolStripMenuItem, recentBitmapsToolStripMenuItem_Click);
          }
          else
          {
            IniSettings.MRUFilesBitmap.RemoveFile(filename, recentBitmapsToolStripMenuItem);
          }
        }
        catch (Exception ex)
        {
          displayMessage(ex.Message, MessageSeverity.Error);
        }

      }
    }
    Control? _sourceControl = null;

    private void messagesContextMenuStrip_Opened(object sender, EventArgs e)
    {
      _sourceControl = messagesContextMenuStrip.SourceControl;
    }

    private void copySelected_messagesContextMenuStrip_Click(object sender, EventArgs e)
    {
      ToolStripMenuItem? menuItem = sender as ToolStripMenuItem;
      if (_sourceControl != null)
      {
        String? tag = _sourceControl.Tag as String;
        if (tag == "E") messagesErrorRichTextBox.Copy(); else messageRichTextBox.Copy();
      }
    }

    private void deleteAll_messagesContextMenuStrip_Click(object sender, EventArgs e)
    {
      ToolStripMenuItem? menuItem = sender as ToolStripMenuItem;
      if (_sourceControl != null)
      {
        String? tag = _sourceControl.Tag as String;
        if (tag == "E") messagesErrorRichTextBox.Text = " ";
        else
        {
          messageRichTextBox.Text = " "; // Clear() clears also font size; 
          _displayMessageLastText = " ";
        }

      }

    }

    private void Color2Mesage(Color c, string xyText)
    {
      int ic;
      string s = TBColorLookupTable.ToNearestNamedColor(c.R, c.G, c.B, out ic);
      s = s.ToUpper();
      UInt32 uc = Color2Byte.ToUColor(c);
      if (uc != ic) { s = "near " + s; }
      int b;
      if (Color2Byte.ColorList.TryGetValue(uc, out b))
      {
        Color2Byte.HSV hsv = new Color2Byte.HSV(uc);
        string text = String.Format("{0} Color {7} has index=0x{1:X4}={1,4}; RGB=0x{2:X6} | HSV=[{3}, {4}, {5}] | Luminocity={6}",
                                     xyText, b, uc, hsv.Hue, hsv.Saturation, hsv.Value, (int)(hsv.lum),
                                     s);
        displayStatusMessage(text, MessageSeverity.Info);
      }

    }
  } // class
}
