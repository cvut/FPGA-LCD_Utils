using System;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;

namespace LSPtools
{
  public static class Color2Byte
  {
   /// <summary>
   /// Sorted by RGB
   /// </summary>
    static public SortedList<UInt32, int> ColorList = new SortedList<UInt32, int>();
/// <summary>
/// RGB sorted by HSV
/// </summary>
    static public List<UInt32> ListColorsSortedHSV = new List<UInt32>();
 
    public static void AddColorsToList(Color rgb)
    {
      int average = (rgb.R + rgb.G + rgb.B) / 3;
 //     int x;
      UInt32 xi = ((UInt32)rgb.R) << 16 | ((UInt32)rgb.G) << 8 | (UInt32)rgb.B;
      int bv;
      if (!ColorList.TryGetValue(xi, out bv)) { ColorList.Add(xi, bv = ColorList.Count); }
    }

    public static UInt32 ToUColor(Color rgb)
    {
      return ((UInt32)rgb.R) << 16 | ((UInt32)rgb.G) << 8 | (UInt32)rgb.B;
    }
     public static Color FromUColor(UInt32 xu)
    {
       return Color.FromArgb((byte)((xu >> 16) & 0xFF), (byte)((xu >> 8) & 0xFF), (byte)(xu & 0xFF));
    }

    public static int ToIndex(Color rgb)
    {
      
          UInt32 xi = ToUColor(rgb);
          int bv;
          if (ColorList.TryGetValue(xi, out bv)) return bv;
          else return 0;

    }

    public static Color ToColor(int b)
    {
   
          int ix = ColorList.IndexOfValue(b);
          if (ix >= 0)
          {
            UInt32 xu = ColorList.Keys[ix];
            return FromUColor(xu);
          }
          else return Color.Black;
    }
    /// <summary>
    /// Return minimum bit length of integer number
    /// </summary>
    /// <param name="x"></param>
    /// <returns></returns>
    public static int GetBitWidth(int x)
    {
      return x<0 ? sizeof(int) : GetBitWidth((UInt32)x);
    }

      public static int GetBitWidth(UInt32 xu)
    {
      int bitCount = 0;
      do { xu >>= 1; bitCount++; }
      while (xu > 0);
      return bitCount;
    }

    static private int colorSorter(HSV x1, HSV x2)
    {
     // sort dark colors as hue =0;
      int h1 = x1.Saturation <= 16 || x1.Value <= 12 ? 0 : x1.Hue;
      int h2 = x2.Saturation <= 16 || x2.Value <= 12 ? 0 : x2.Hue;
      if (h1 < h2) return -1;
      if (h1 > h2) return 1;
      if (x1.lum < x2.lum) return -1;
      if (x1.lum > x2.lum) return 1;
      return 0;
    }
    static public string processBitmap(Bitmap bmp)
    {
      int w = bmp.Width, h = bmp.Height;
      Color2Byte.ColorList.Clear();
      try
      {
        for (int y = 0; y < h; y++)
        {
          for (int x = 0; x < w; x++)
          {
            Color c;
  //          byte b;
            if (x < bmp.Width && y < bmp.Height)
            {
              c = bmp.GetPixel(x, y);
              AddColorsToList(c);
            }
          }
        }
        UInt32[] arr = ColorList.Keys.ToArray();
        List<HSV> hsvList = new List<HSV>();
        for (int i = 0; i < arr.Length; i++)
        {
          hsvList.Add(new HSV(arr[i]));
        }
        hsvList.Sort(colorSorter);
        ColorList.Clear(); ListColorsSortedHSV.Clear();
        for (int i = 0; i < hsvList.Count; i++)
        {
          ListColorsSortedHSV.Add(hsvList[i].RGB); ColorList.Add(hsvList[i].RGB, i);
        }
        return String.Empty;
      }
      catch (Exception ex)
      {
        return ex.Message;
      }
    }
    private static List<int> clamps = new List<int>();

    public class HSV
    {
      private UInt32 rgb;
      private int hue;
      private int sat;
      private int val;
      public readonly double lum;
      private double qmean(double x1, double x2, double x3)
      {
        return Math.Sqrt(x1 * x1 + x2 * x2 + x3 * x3);
      }
      public HSV(UInt32 ucolor)
      {
        hue = 0; sat = 0; val = 0; rgb = ucolor; FromRGB(ucolor);
        lum = qmean(0.299 * ((ucolor >> 16) & 0xFF), 0.587 * ((ucolor >> 8) & 0xFF), 0.114 * (ucolor & 0xFF));
      }
      public int Hue
      {
        get { return hue; }
      }
      public int Saturation
      {
        get { return sat; }
      }
      public int Value
      {
        get { return val; }
      }

      public UInt32 RGB
      {
        get { return rgb; }
      }

 

      public void FromRGB(UInt32 uc)
      {
        double r = (double)((uc >> 16) & 0xFF) / 255.0;
        double g = (double)((uc >> 8) & 0xFF) / 255.0;
        double b = (double)(uc & 0xFF) / 255.0;

        double min; double max; double delta;
         double h; double s; double v;

        min = Math.Min(Math.Min(r, g), b);
        max = Math.Max(Math.Max(r, g), b);
        v = max;
        delta = max - min;
        if (max == 0 || delta == 0)
        {
          s = 0;
          h = 0;
        }
        else
        {
          s = delta / max;
          if (r == max)
          {
            h = (60D * ((g - b) / delta)) % 360D;
          }
          else if (g == max)
          {
            h = 60D * ((b - r) / delta) + 120D;
          }
          else
          {
            h = 60D * ((r - g) / delta) + 240D;
          }
        }
        if (h < 0)
        {
          h += 360D;
        }

        this.hue = (int)((h / 360D) * 255D);
        this.sat = (int)(s * 255D);
        this.val = (int)(v * 255D);
        if (this.sat < 2 || this.val<2) this.hue = 0; // hues of dark gray colors are not visialy distinguishable
      }
    }
  }
}



