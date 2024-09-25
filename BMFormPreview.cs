using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace LSPtools
{
  public partial class BMFormPreview : Form
  {
    Bitmap previewBitmap;
    string titleOfWindow = String.Empty;
    private int scaleOfPreviewImage = 1;
    private Bitmap? resizedBitmap = null;
    public BMFormPreview(Bitmap bitmap, int scale, string filename)
    {
      InitializeComponent();
      previewBitmap = bitmap; 
      this.Text = titleOfWindow = filename;
      this.scaleOfPreviewImage = scale; resizedBitmap = null;
      //           this.ResizeRedraw = true;
      numericUpDownScale.Value = scale;
      SetScrollbars();
    }

      int xorg = 0, yorg = 0;
    const int FRAME = 10;
    private void panelBitmap_Paint(object sender, PaintEventArgs e)
    {
      Rectangle rpanel = panelBitmap.ClientRectangle;
      Graphics g = e.Graphics;
      g.FillRectangle(SystemBrushes.Control, rpanel);
      if (previewBitmap == null) return;
      int xw0 = previewBitmap.Width;
      int yh0 = previewBitmap.Height;
      int xw = xw0 * scaleOfPreviewImage;
      int yh = yh0 * scaleOfPreviewImage;
      int x1, y1;
      if (resizedBitmap == null)
      {

        try
        {
          panelBitmap.Cursor = Cursors.WaitCursor;
          this.Text = "-- Resizing --";
          resizedBitmap = new Bitmap(previewBitmap, xw, yh);
          xw = resizedBitmap.Width;
          yh = resizedBitmap.Height;
          for (int x = 0; x < xw0; x++)
          {
            for (int y = 0; y < yh0; y++)
            {
                Color c = previewBitmap.GetPixel(x, y);
                for (int k = 0; k < scaleOfPreviewImage; k++)
                {
                  x1 = x * scaleOfPreviewImage + k;
                  for (int l = 0; l < scaleOfPreviewImage; l++)
                  {
                    y1 = y * scaleOfPreviewImage + l;
                    if (x1 >= 0 && x1 < xw && y1 >= 0 && y1 < yh)
                      resizedBitmap.SetPixel(x1, y1, c);
                  }
                }
             }
          }
        }
        catch (Exception ex)
        { 
           Trace.WriteLine(ex.ToString());
        }

        finally
        {
          panelBitmap.Cursor = Cursors.Cross;
          this.Text = titleOfWindow;

        }
      }
      g.DrawImage(resizedBitmap, FRAME - xorg, FRAME - yorg, xw, yh);
    }

    private void panelBitmap_MouseMove(object sender, MouseEventArgs e)
    {
      int x = (e.X + xorg - FRAME) / scaleOfPreviewImage, y = (e.Y + yorg - FRAME) / scaleOfPreviewImage;
      if (x >= 0 && previewBitmap != null && x < previewBitmap.Width)
        txbMouseX.Text = x.ToString();
      else { txbMouseX.Text = "---"; }
      if (y >= 0 && previewBitmap != null && y < previewBitmap.Height)
        txbMouseY.Text = y.ToString();
      else { txbMouseY.Text = "---"; }
   
    }

    private void btnClose_Click(object sender, EventArgs e)
    {
      this.Close();
    }

    private void SetScrollbars()
    {

      if (previewBitmap == null) return;
      Rectangle rpanel = panelBitmap.ClientRectangle;
      int rw = rpanel.Width;
      int rh = rpanel.Height;
      int xw = previewBitmap.Width * scaleOfPreviewImage + 2 * FRAME;
      int yh = previewBitmap.Height * scaleOfPreviewImage + 2 * FRAME;
      if (xw > rpanel.Width)
      {
        hScrollBar1.Minimum = 0;
        hScrollBar1.Maximum = xw - 1;
        if (xorg < 0) xorg = 0;
        if (xorg + rw > xw)
        {
          xorg = xw - rw;
          if (xorg < 0) xorg = 0;
        }
      }
      else
      {
        hScrollBar1.Minimum = 0;
        hScrollBar1.Maximum = rw - 1;
        xorg = 0;
      }
      hScrollBar1.LargeChange = rw - 1;
      hScrollBar1.Value = xorg;
      if (yh > rh)
      {
        vScrollBar1.Minimum = 0;
        vScrollBar1.Maximum = yh - 1;
        if (yorg < 0) yorg = 0;
        if (yorg + rh > yh)
        {
          yorg = yh - rpanel.Height;
          if (yorg < 0) yorg = 0;
        }
      }
      else
      {
        vScrollBar1.Minimum = 0;
        vScrollBar1.Maximum = rh - 1;
        yorg = 0;
      }
      vScrollBar1.LargeChange = rh - 1;
      vScrollBar1.Value = yorg;

      panelBitmap.Invalidate();

    }
    private void numericUpDownScale_ValueChanged(object sender, EventArgs e)
    {
      scaleOfPreviewImage = (int)numericUpDownScale.Value; resizedBitmap = null;
      SetScrollbars();
    }

    private void vScrollBar1_Scroll(object sender, ScrollEventArgs e)
    {
      yorg = vScrollBar1.Value;
      panelBitmap.Invalidate();
    }

    private void hScrollBar1_Scroll(object sender, ScrollEventArgs e)
    {
      xorg = hScrollBar1.Value;
      panelBitmap.Invalidate();
    }

    private void panelBitmap_Resize(object sender, EventArgs e)
    {
      SetScrollbars(); panelBitmap.Invalidate();
    }


  }
}
