using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FpgaLcdUtils
{
  public partial class BMInfoForm : Form
  {
    Bitmap bitmap;
    UInt32[] indexedColors;
    private int scaleOfPreviewImage = 1;

    public BMInfoForm(Bitmap bmp, UInt32[] indexedColors)
    {
      InitializeComponent();
      this.bitmap = bmp;
      this.indexedColors = indexedColors;
    }

    private void BMInfoForm_Load(object sender, EventArgs e)
    {
      infoTextBox.Text =
@"In FPGAs, indexing of colors generates optimal results only for a minor palette.
Its maximum acceptable size is 256 different colors, but 16 colors or less allows better encoding. 
For example, the popular freeware Fast Stone Image Viewer has “Color->Reduce Colors...” an option in the context menu of an image.

";

      colorCountTextBox.Text = indexedColors == null ? "unknown" : indexedColors.Length.ToString();
    }

    private void panel1_Paint(object sender, PaintEventArgs e)
    {
      if (bitmap != null)
      {
        e.Graphics.FillRectangle(SystemBrushes.ControlLight, bitmapPanel.ClientRectangle);
        scaleOfPreviewImage = (int)(numericUpDownScale.Value);
        e.Graphics.DrawImage(bitmap, 0, 0, bitmap.Width * scaleOfPreviewImage, bitmap.Height * scaleOfPreviewImage);
      }
    }
    Bitmap? _paletteBmp = null;
    private void palettePanel_Paint(object sender, PaintEventArgs e)
    {
      if (_paletteBmp == null) _paletteBmp = BMForm.CreatePaletteBitmap(e.Graphics, palettePanel, indexedColors);
      if (_paletteBmp != null) { e.Graphics.DrawImageUnscaled(_paletteBmp, 0, 0); }
    }

    private void color2Message(Color c)
    {
      UInt32 uc = Color2Byte.ToUColor(c);
      int b;
      if (Color2Byte.ColorList.TryGetValue(uc, out b))
      {
        Color2Byte.HSV hsv = new Color2Byte.HSV(uc);
        string text = String.Format("Color index=0x{0:X4}={0,4}; RGB=0x{1:X6} | HSV=[{2}, {3}, {4}] | Luminocity={5:0.###}",
                                     b, uc, hsv.Hue, hsv.Saturation, hsv.Value, hsv.lum);
        messageLabel.Text = text;
      }

    }

    private void bitmapPanel_MouseMove(object sender, MouseEventArgs e)
    {
      try
      {
        int x = e.X / scaleOfPreviewImage, y = e.Y / scaleOfPreviewImage;
 //       bool ok = true;
        if (x >= 0 && bitmap != null && x < bitmap.Width && y > 0 && y < bitmap.Height)
        {
          color2Message(bitmap.GetPixel(x, y));
        }
        else messageLabel.Text = String.Empty;
      }
      catch (Exception)
      {

      }
    }

    private void palettePanel_MouseMove(object sender, MouseEventArgs e)
    {
      try
      {
        if (_paletteBmp == null) return;
        int x = e.X, y = e.Y;
         if (x >= 0 && x < _paletteBmp.Width && y >= 0 && y < _paletteBmp.Height)
        {
          color2Message(_paletteBmp.GetPixel(x, y));
        }
      }
      catch (Exception ex)
      {
        Trace.WriteLine(ex.ToString());
      }
    }

    private void palettePanel_Resize(object sender, EventArgs e)
    {
      _paletteBmp = null;
    }

    private void button1_Click(object sender, EventArgs e)
    {
      Close();
    }
  }
}
