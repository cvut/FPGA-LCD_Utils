using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.InteropServices.ComTypes;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FpgaLcdUtils
{
  internal class RulerData
  {
    public readonly NumericUpDown[] numericUpDowns;
    protected int[] lastNUDValues;
    public readonly RadioButton?[] radioButtons;
    protected int lastRadioButton = 0;
    public readonly Button colorButton;
    public readonly Button selectButton;
    public Pen lastPen = Pens.White;
    public readonly CheckBox visibleCheckBox;
    public readonly RichTBWriter rtb;
    internal static readonly float[] DottedPatern = { 4f, 1f };
    internal bool isSelected = false;

    protected TextBox? semiX = null;
    protected TextBox? semiY = null;
    protected TextBox? semiZ = null;
    protected TextBox? gcdXYZ = null;

    public const int ID_RECT = 0, ID_LINE = 1, ID_ELLIPSE = 2;
    public const int ID_SELECTIONS = 3; // Unknown ID tab, we need to select one

    public const int SELECT_PIXEL_SIZE = 8;

    internal const int SELECT_PIXEL_SQUARE_RADIUS = RulerData.SELECT_PIXEL_SIZE * RulerData.SELECT_PIXEL_SIZE / 4;


    public const int MRU_LENGTH = 3;
    /// <summary>
    /// Most recently used ruler tab
    /// </summary>
    internal static int[] MRURuler = new int[MRU_LENGTH] { ID_RECT, ID_LINE, ID_ELLIPSE };
    public enum DragEnum { NONE, TOPLEFT, TOP, TOPRIGHT, RIGHT, BOTTOMRIGHT, BOTTOM, BOTTOMLEFT, LEFT, HAND, SIZEALLSTART, SIZEALLEND, DRAWNEW };
    public enum DraggedRuler { RECTANGLE, LINE, ELLIPSE, NONE };

    #region Constants
    public const int RB11 = 0, RB12 = 1, RB13 = 2, RB21 = 3, RB22 = 4, RB23 = 5, RB31 = 6, RB32 = 7, RB33 = 8;
    public const int MRX = 0, MRY = 1, MRW = 2, MRH = 3, MRXEND = 2, MRYEND = 3; // MRW==MRXEND, MRH==MRYEND is the intention

    private const int WIDTH_LCD = 1024;
    private const int HEIGHT_LCD = 525;
    private const int VISIBLE_WIDTH_LCD = 800;
    private const int VISIBLE_HEIGHT_LCD = 480;
    // Frame
    public const int BORDER_AROUND = 5; // We add border around of image for its better visibility
                                        //Sizes of bitmap with image
    public const int WIDTH = WIDTH_LCD + 2 * BORDER_AROUND;
    public const int HEIGHT = HEIGHT_LCD + 2 * BORDER_AROUND;
    public const int WIDTH_VISIBLE = VISIBLE_WIDTH_LCD + 2 * BORDER_AROUND;
    public const int HEIGHT_VISIBLE = VISIBLE_HEIGHT_LCD + 2 * BORDER_AROUND;

    private const int CANVAS_XUL = -WIDTH_LCD * 2;
    private const int CANVAS_YUL = -HEIGHT_LCD * 2;
    private const int CANVAS_XDR = WIDTH_LCD * 3;
    private const int CANVAS_YDR = -HEIGHT_LCD * 3;
    public const int CANVAS_WIDTH = 5 * WIDTH_LCD;
    public const int CANVAS_HEIGHT = 5 * HEIGHT_LCD;
    public const int CANVAS_XIMGCENTER = 2 * WIDTH_LCD + WIDTH_VISIBLE / 2;
    public const int CANVAS_YIMGCENTER = 2 * HEIGHT_LCD + HEIGHT_VISIBLE / 2;

    #endregion

    public static Rectangle transformScrollsToCanvas(Rectangle scroll)
    {
      return transformScrollsToCanvas(scroll.X, scroll.Y, scroll.Width, scroll.Height);
    }
    public static Rectangle transformScrollsToCanvas(int x0, int y0, int width, int height)
    {
      int nX = x0 + CANVAS_XUL;
      int nY = y0 + CANVAS_YUL;
      int nW = width;
      int nH = height;
      return new Rectangle(nX, nY, nW, nH);
      /*    return new Rectangle(hScrollBarGraph.Value, vScrollBarGraph.Value,
                           hScrollBarGraph.LargeChange, vScrollBarGraph.LargeChange);
*/
    }

    public static Rectangle transformCanvasToScrolls(int xC, int yC, int width, int height)
    {
      int nX = xC - CANVAS_XUL; if (nX < 0) nX = 0;
      int nY = yC - CANVAS_YUL; if (nY < 0) nY = 0;
      int nW = width;
      int nH = height;
      return new Rectangle(nX, nY, nW, nH);
      /*    return new Rectangle(hScrollBarGraph.Value, vScrollBarGraph.Value,
                           hScrollBarGraph.LargeChange, vScrollBarGraph.LargeChange);
*/
    }
    internal enum SelRuler_enum { Toggle, True, False };

    private static NumericUpDownAcceleration[] accelerations = new NumericUpDownAcceleration[0];

    public NumericUpDownAcceleration[] GetNUD_acceleration()
    {
      if (accelerations.Length == 0)
      {
        NumericUpDownAccelerationCollection accs = new NumericUpDownAccelerationCollection();
        accs.Add(new NumericUpDownAcceleration(1, 2));
        accs.Add(new NumericUpDownAcceleration(3, 10));
        accs.Add(new NumericUpDownAcceleration(5, 100));
        accelerations = accs.ToArray();
      }
      return accelerations;
    }

    /// <summary>
    /// Reverse value shift by border size
    /// </summary>
    /// <param name="pos"></param>
    /// <returns>shifted </returns>
    public static int Index2LCDImage(int pos) { return pos - BORDER_AROUND; }

    /// <summary>
    /// Reverse value shift by border size
    /// </summary>
    /// <param name="pos"></param>
    /// <returns>shifted value</returns>
    public static float Index2LCDImage(float pos) { return pos - BORDER_AROUND; }

    public void setColorButtonPen(UInt32 uc)
    {
      setColorButtonPen(Color2Byte.FromUColor(uc));
    }

    /// <summary>
    /// Test if bitmapColumn,bitmapRow index is in VGA image 
    /// </summary>
    /// <param name="column"></param>
    /// <param name="row"></param>
    /// <returns></returns>
    public static bool IsLCDImageIndex(int column, int row)
    {
      return (column >= 0 && column < WIDTH_LCD
          && row >= 0 && row < HEIGHT_LCD);
    }
    /// <summary>
    /// Set color to pen and text in color button
    /// </summary>
    /// <param name="c"></param>
    public void setColorButtonPen(Color c)
    {
      this.colorButton.BackColor = c;
      lastPen = new Pen(c, 1.0f); lastPen.DashPattern = DottedPatern;
      // we create XOR fore-color that is visible on any background
      UInt32 uc = Color2Byte.ToUColor(c);
      uc = uc ^ 0xFFFFFF;
      c = Color2Byte.FromUColor(uc);
      this.colorButton.ForeColor = c;
      this.colorButton.Invalidate();
    }
    internal void selectButton_Clicked(SelRuler_enum setState)
    {
      switch (setState)
      {
        case SelRuler_enum.Toggle:
          selectButton_Clicked(!isSelected);
          break;
        case SelRuler_enum.True:
          selectButton_Clicked(true); break;
        case SelRuler_enum.False:
          selectButton_Clicked(false); break;
      }
    }

    public void selectButton_Clicked(bool setSelect)
    {
      if (selectButton != null)
      {
        if (setSelect)
        {
          isSelected = true;
          selectButton.BackColor = SystemColors.Info;
        }
        else
        {
          isSelected = false;
          selectButton.BackColor = SystemColors.Control;
        }
      }
    }
#pragma warning disable CS8618 // Non-nullable field must contain a value
    public RulerData(TableLayoutPanel tlbParams, TableLayoutPanel? tlbRef, char tagChar)
    {
      List<NumericUpDown> listNuD = new List<NumericUpDown>();
      List<RadioButton> listRB = new List<RadioButton>();
      lastPen = new Pen(Color.White, 1);
      lastPen.DashPattern = DottedPatern;
      NumericUpDownAcceleration[] ac = GetNUD_acceleration();
      foreach (Control control in tlbParams.Controls)
      {
        if (control == null) continue;
        string? tagString = control.Tag as string;
        if (tagString == null || tagString.Length == 0) continue;

        if (tagString[0] == tagChar)
        {
          char c2 = '\0';
          if (tagString.Length > 1) c2 = Char.ToUpper(tagString[1]);
          NumericUpDown? nud = control as NumericUpDown;
          if (nud != null)
          {
            nud.Accelerations.Clear(); nud.Accelerations.AddRange(ac);
            listNuD.Add(nud); continue;
          }
          Button? button = control as Button;
          if (button != null)
          {
            switch (c2)
            {
              case 'C':
                colorButton = button; setColorButtonPen(button.BackColor);
                break;
              case 'S': selectButton = button; break;
            }
            continue;
          }
          CheckBox? checkBox = control as CheckBox;
          if (checkBox != null)
          { visibleCheckBox = checkBox; continue; }
          RichTextBox? richTextBox = control as RichTextBox;
          if (richTextBox != null)
          {
            rtb = new RichTBWriter(richTextBox); continue;
          }

          if (c2 != '\0')
          {
            TextBox? textBox = control as TextBox;
            if (textBox == null) return;
            switch (c2)
            {
              case 'X': semiX = textBox; break;
              case 'Y': semiY = textBox; break;
              case 'Z': semiZ = textBox; break;
              case 'G': gcdXYZ = textBox; break;
            }
          }
          ExtendedIdentification(control, tagString);
        }

      }
      listNuD.Sort(_nudComparission);
      numericUpDowns = listNuD.ToArray();
      if (visibleCheckBox == null)
        throw new MissingMemberException("Missing visible check box control.");
      if (tlbRef != null)
      {
        radioButtons = new RadioButton[9];
        foreach (Control control in tlbRef.Controls)
        {
          if (control == null) continue;
          RadioButton? rb = control as RadioButton; if (rb == null) continue;
          string? s = rb.Tag as string;
          if (s == null || s.Length == 0) continue;
          if (s[0] == tagChar && s.Length > 2)
          {
            int iy = (s[1] - '0'), ix = (s[2] - '0');
            int rbIndex = 0;
            if (ix >= 1 && ix <= 3 && iy >= 1 && iy <= 3)
            {
              rbIndex = (iy - 1) * 3 + ix - 1;
              radioButtons[rbIndex] = rb;
              if (rb.Checked) lastRadioButton = rbIndex;
            }
          }
        }
        for (int i = 0; i < radioButtons.Length; i++)
        {
          if (radioButtons[i] == null)
            throw new MissingMemberException("Ruler accept only 0 or 9 RadionButtons");
        }
      }
      else radioButtons = new RadioButton[0];
      if (numericUpDowns == null || numericUpDowns.Length == 0)
        throw new MissingMemberException("Ruler suppose non zero count of NumericUpdown controls");
      if (colorButton == null)
        throw new MissingMemberException("Expected presence of Color Button.");
      if (selectButton == null)
        throw new MissingMemberException("Expected presence of Select/Unselect Button.");
      lastNUDValues = new int[numericUpDowns.Length];
      for (int i = 0; i < numericUpDowns.Length; i++)
        lastNUDValues[i] = (int)(numericUpDowns[i].Value);
    }
#pragma warning restore CS8618
    static public void SetNudVal(NumericUpDown nud, decimal v)
    {
      decimal v0 = nud.Value;
      if (v < nud.Minimum) v = nud.Minimum;
      else
      {
        if (v > nud.Maximum) v = nud.Maximum;
      }
      if (v0 != v) nud.Value = v;
    }
    public void writeNUD(string tag, int newValue)
    {
      for (int i = 0; i < numericUpDowns.Length; i++)
      {
        string? s = numericUpDowns[i].Tag as string;
        if (s != tag) continue;
        if (lastNUDValues[i] == newValue) return; // we do not generate event changed
        else
        {
          lastNUDValues[i] = newValue;
          SetNudVal(numericUpDowns[i], newValue);
        }
      }
    }

    private int _nudComparission(NumericUpDown x, NumericUpDown y)
    {
      string? s1 = x.Tag as string;
      string? s2 = y.Tag as string;
      return String.Compare(s1, s2, true);
    }

    /// <summary>
    /// Greatest Common Divisor of num1 and num2
    /// </summary>
    /// <param name="num1"></param>
    /// <param name="num2"></param>
    /// <returns></returns>
    public static long GCD(long num1, long num2)
    {
      long Remainder;

      while (num2 != 0)
      {
        Remainder = num1 % num2;
        num1 = num2;
        num2 = Remainder;
      }
      return num1;
    }

    protected virtual void ExtendedIdentification(Control control, string tagString)
    {

    }

  }

  internal struct Rect
  {
    private PointF _topLeft;
    private PointF _bottomRight;
    private RulerData.DraggedRuler _ruler = RulerData.DraggedRuler.RECTANGLE;
    //    private Rectangle rectDraw;

    public bool IsLine
    {
      get { return _ruler == RulerData.DraggedRuler.LINE; }
    }

    public Rect() { }
    public Rect(RulerData.DraggedRuler dr) { _ruler = dr; }

    public Rect(Rect rect) : this()
    {
      _topLeft = rect._topLeft; _bottomRight = rect._bottomRight;
    }

    public Rect(PointF pt0, PointF pt1) : this()
    {
      _topLeft = pt0; _bottomRight = pt1;
    }

    public Rect(PointF pt0, PointF pt1, RulerData.DraggedRuler dr) : this(dr)
    {
      _topLeft = pt0; _bottomRight = pt1;
    }

    public Rect(float x0, float y0, float x1, float y1) : this()
    {
      _topLeft.X = x0; _topLeft.Y = y0;
      _bottomRight.X = x1; _bottomRight.Y = y1;
    }
    public Rect(float x0, float y0, float x1, float y1, RulerData.DraggedRuler dr) : this(dr)
    {
      _topLeft.X = x0; _topLeft.Y = y0;
      _bottomRight.X = x1; _bottomRight.Y = y1;
    }

    public Rect(PointF pt, float xDifferenceP11, float xDifferenceP33, float yDifferenceP11, float yDifferenceP33)
    {
      _topLeft.X = pt.X + xDifferenceP11; _bottomRight.X = pt.X + xDifferenceP33;
      _topLeft.Y = pt.Y + yDifferenceP11; _bottomRight.Y = pt.Y + yDifferenceP33;
      Normalize();
    }

    public Rect(PointF[] pt, RulerData.DraggedRuler dr) : this(dr)
    {
      if (IsLine)
      {
        _topLeft.X = RulerData.Index2LCDImage(pt[0].X);
        _topLeft.Y = RulerData.Index2LCDImage(pt[0].Y);
      }
      else
      {  // rectangle is around, shift was lesser
        _topLeft.X = RulerData.Index2LCDImage(pt[0].X) + 1;
        _topLeft.Y = RulerData.Index2LCDImage(pt[0].Y) + 1;
      }
      _bottomRight.X = RulerData.Index2LCDImage(pt[1].X);
      _bottomRight.Y = RulerData.Index2LCDImage(pt[1].Y);
    }



    public Rect(int initAllToValue) : this()
    {
      _topLeft.X = initAllToValue; _topLeft.X = initAllToValue;
      _bottomRight.X = initAllToValue; _bottomRight.Y = initAllToValue;
    }

    public Rect(RectangleF rectDraw) : this()
    {
      this._topLeft = new PointF(rectDraw.Left, rectDraw.Top);
      this._bottomRight = new PointF(rectDraw.Right, rectDraw.Bottom);
    }

    public RectangleF ToRectangle()
    {
      return new RectangleF(this.P11F.X, this.P11F.Y, this.WidthF, this.HeightF);
    }

    private float _middleXF { get { return (_topLeft.X + _bottomRight.X) / 2; } }
    private float _middleYF { get { return (_topLeft.Y + _bottomRight.Y) / 2; } }
    public int Width { get { return (int)Math.Round(_bottomRight.X - _topLeft.X, 0); } }
    public int Height { get { return (int)Math.Round(_bottomRight.Y - _topLeft.Y, 0); } }
    public float WidthF { get { return _bottomRight.X - _topLeft.X; } }
    public float HeightF { get { return _bottomRight.Y - _topLeft.Y; } }
    public bool IsValid
    {
      get
      {
        return this.IsLine ? (_bottomRight.X != _topLeft.X) || (_bottomRight.Y != _topLeft.Y)
                                  : (_bottomRight.X > _topLeft.X) && (_bottomRight.Y > _topLeft.Y);
      }
    }

    public static int toInt(float x) { unchecked { return (int)Math.Round(x, MidpointRounding.AwayFromZero); }; }
    public RectangleF GetRectangleF()
    { return new RectangleF(_topLeft, new SizeF(WidthF, HeightF)); }
    /// <summary>
    /// Rectangle location is shifted byRulerData.BORDER_AROUND
    /// </summary>
    /// <returns>Rectangle adjusted to drawing</returns>
    public Rectangle GetDrawRectangle()
    {
      // we create the rectangle using as surroundings
      return new Rectangle(toInt(_topLeft.X + RulerData.BORDER_AROUND - 1),
                           toInt(_topLeft.Y + RulerData.BORDER_AROUND - 1),
                                 Width + 1, Height + 1);
    }
    /// <summary>
    /// points shifted byRulerData.BORDER_AROUND
    /// </summary>
    /// <returns></returns>
    public Point[] GetDrawPoints()
    {
      return new Point[2] { new Point(toInt(_topLeft.X + RulerData.BORDER_AROUND),
                                      toInt(_topLeft.Y + RulerData.BORDER_AROUND)),
                            new Point(toInt(_bottomRight.X + RulerData.BORDER_AROUND),
                                      toInt(_bottomRight.Y + RulerData.BORDER_AROUND)) };
    }

    public Rectangle GetRectangle()
    { return new Rectangle(P11, new Size(Width, Height)); }

    public SizeF GetSizeF() { return new SizeF(WidthF, HeightF); }
    public Size GetSize() { return new Size(Width, Height); }

    public Point F2P(PointF p) { return new Point(toInt(p.X), toInt(p.Y)); }
    public Point P11 { get { return F2P(P11F); } }
    public Point P12 { get { return F2P(P12F); } }
    public Point P13 { get { return F2P(P13F); } }
    public Point P21 { get { return F2P(P21F); } }
    public Point P22 { get { return F2P(P22F); } }
    public Point P23 { get { return F2P(P23F); } }
    public Point P31 { get { return F2P(P31F); } }
    public Point P32 { get { return F2P(P32F); } }
    public Point P33 { get { return F2P(P33F); } }

    public PointF P11F { get { return _topLeft; } }
    public PointF P12F { get { return new PointF(_middleXF, _topLeft.Y); } }
    public PointF P13F { get { return new PointF(_bottomRight.X, _topLeft.Y); } }
    public PointF P21F { get { return new PointF(_topLeft.X, _middleYF); } }
    public PointF P22F { get { return new PointF(_middleXF, _middleYF); } }
    public PointF P23F { get { return new PointF(_bottomRight.X, _middleYF); } }
    public PointF P31F { get { return new PointF(_topLeft.X, _bottomRight.Y); } }
    public PointF P32F { get { return new PointF(_middleXF, _bottomRight.Y); } }
    public PointF P33F { get { return _bottomRight; } }

    public int GetNudX(int ixRb)
    {
      switch (ixRb)
      {
        case RulerData.RB11:
        case RulerData.RB21:
        case RulerData.RB31:
          return (int)Math.Round(_topLeft.X, 0);
        case RulerData.RB12:
        case RulerData.RB22:
        case RulerData.RB32:
          return (int)Math.Round(_middleXF, 0);
        case RulerData.RB13:
        case RulerData.RB23:
        case RulerData.RB33:
          return (int)Math.Round(_bottomRight.X, 0);
      }
      return -1;
    }
    public int GetNudY(int ixRb)
    {
      switch (ixRb)
      {
        case RulerData.RB11:
        case RulerData.RB12:
        case RulerData.RB13:
          return (int)Math.Round(_topLeft.Y, 0);
        case RulerData.RB21:
        case RulerData.RB22:
        case RulerData.RB23:
          return (int)Math.Round(_middleYF, 0);
        case RulerData.RB31:
        case RulerData.RB32:
        case RulerData.RB33:
          return (int)Math.Round(_bottomRight.Y, 0);
      }
      return -1;
    }
    /// <summary>
    /// Test if left upper left coordinates are less than bottom right
    /// and swap them if it is required.
    /// </summary>
    public void Normalize()
    {
      if (IsLine) return;
      if (_topLeft.X < _bottomRight.X && _topLeft.Y < _bottomRight.Y) return;
      {
        float min = _topLeft.X, max = _bottomRight.X;
        if (min > max) { max = min; min = _bottomRight.X; }
        _topLeft.X = min; _bottomRight.X = max;
        min = _topLeft.Y; max = _bottomRight.Y;
        if (min > max) { max = min; min = _bottomRight.Y; }
        _topLeft.Y = min; _bottomRight.Y = max;
      }
    }
    public void SetWidth(int w, int ixRb)
    {
      if (w <= 0) return;
      switch (ixRb)
      {
        case RulerData.RB11:
        case RulerData.RB21:
        case RulerData.RB31:
          _bottomRight.X = _topLeft.X + w; break;
        case RulerData.RB12:
        case RulerData.RB22:
        case RulerData.RB32:
          float m = _middleXF;
          _bottomRight.X = m + w / 2;
          _topLeft.X = m - w / 2;
          break;
        case RulerData.RB13:
        case RulerData.RB23:
        case RulerData.RB33:
          _topLeft.X = _bottomRight.X - w; break;
      }
    }
    public void SetHeight(int h, int ixRb)
    {
      if (h <= 0) return;
      switch (ixRb)
      {
        case RulerData.RB11:
        case RulerData.RB12:
        case RulerData.RB13:
          _bottomRight.Y = _topLeft.Y + h; break;
        case RulerData.RB21:
        case RulerData.RB22:
        case RulerData.RB23:
          float m = _middleYF;
          _bottomRight.Y = m + h / 2;
          _topLeft.Y = m - h / 2;
          break;
        case RulerData.RB31:
        case RulerData.RB32:
        case RulerData.RB33:
          _topLeft.Y = _bottomRight.Y - h; break;
      }

    }

    internal void SetFromNuds(int[] lastNUDValues)
    {
      if (lastNUDValues == null || lastNUDValues.Length < 4) return;
      float x = lastNUDValues[RulerData.MRX], y = lastNUDValues[RulerData.MRY];
      float xend = lastNUDValues[RulerData.MRXEND], yend = lastNUDValues[RulerData.MRYEND];
      _topLeft.X = x; _topLeft.Y = y; _bottomRight.X = xend; _bottomRight.Y = yend;
      // no Normalize
    }

    internal void SetValues(float x0, float y0, float x1, float y1)
    {
      _topLeft.X = x0; _topLeft.Y = y0; _bottomRight.X = x1; _bottomRight.Y = y1;
      Normalize();
    }


    internal void SetFromNuds(int[] lastNUDValues, int ixRb)
    {
      if (lastNUDValues == null || lastNUDValues.Length < 4) return;
      float x = lastNUDValues[RulerData.MRX], y = lastNUDValues[RulerData.MRY];
      float w = lastNUDValues[RulerData.MRW], h = lastNUDValues[RulerData.MRH];

      switch (ixRb)
      {
        case RulerData.RB11:
          _topLeft.X = x; _topLeft.Y = y; _bottomRight.X = x + w; _bottomRight.Y = y + h;
          break;
        case RulerData.RB12:
          _topLeft.X = x - w / 2; _topLeft.Y = y; _bottomRight.X = x + w / 2; _bottomRight.Y = y + h;
          break;
        case RulerData.RB13:
          _topLeft.X = x - w; _topLeft.Y = y; _bottomRight.X = x; _bottomRight.Y = y + h;
          break;
        case RulerData.RB21:
          _topLeft.X = x; _topLeft.Y = y - h / 2; _bottomRight.X = x + w; _bottomRight.Y = y + h / 2;
          break;
        case RulerData.RB22:
          _topLeft.X = x - w / 2; _topLeft.Y = y - h / 2; _bottomRight.X = x + w / 2; _bottomRight.Y = y + h / 2;
          break;
        case RulerData.RB23:
          _topLeft.X = x - w; _topLeft.Y = y - h / 2; _bottomRight.X = x; _bottomRight.Y = y + h / 2;
          break;
        case RulerData.RB31:
          _bottomRight.X = x; _topLeft.Y = y - h; _bottomRight.X = x + w; _bottomRight.Y = y;
          break;
        case RulerData.RB32:
          _topLeft.X = x - w / 2; _topLeft.Y = y - h; _bottomRight.X = x + w / 2; _bottomRight.Y = y;
          break;
        case RulerData.RB33:
          _topLeft.X = x - w; _topLeft.Y = y - h; _bottomRight.X = x; _bottomRight.Y = y;
          break;
      }
      Normalize();
    }
    /// <summary>
    /// From rectangle update all NUDs and their last Values
    /// </summary>
    /// <param name="nuds"></param>
    /// <param name="lastNUDValues"></param>
    /// <param name="ixRb"></param>
    internal bool UpdateNuds(NumericUpDown[] nuds, int[] lastNUDValues, int ixRb)
    {
      bool wasUpdate = false;
      if (lastNUDValues == null || lastNUDValues.Length < RulerData.MRH + 1) return false;
      if (nuds == null || nuds.Length < RulerData.MRH + 1) return false;
      int[] newNUDValues = new int[lastNUDValues.Length];
      newNUDValues[RulerData.MRX] = this.GetNudX(ixRb);
      newNUDValues[RulerData.MRY] = this.GetNudY(ixRb);
      newNUDValues[RulerData.MRW] = this.Width;
      newNUDValues[RulerData.MRH] = this.Height;
      for (int i = 0; i < newNUDValues.Length; i++)
      {
        if (lastNUDValues[i] != newNUDValues[i]) // we try to reduce Windows messages
        {
          wasUpdate = true;
          lastNUDValues[i] = newNUDValues[i];
          RulerData.SetNudVal(nuds[i], newNUDValues[i]);
        }
      }
      return wasUpdate;
    }

    /// <summary>
    /// Update for line
    /// </summary>
    /// <param name="nuds"></param>
    /// <param name="lastNUDValues"></param>
    /// <returns></returns>
    internal bool UpdateNuds(NumericUpDown[] nuds, int[] lastNUDValues)
    {
      bool wasUpdate = false;
      if (lastNUDValues == null || lastNUDValues.Length < RulerData.MRYEND + 1) return false;
      if (nuds == null || nuds.Length < RulerData.MRYEND + 1) return false;
      int[] newNUDValues = new int[lastNUDValues.Length];
      newNUDValues[RulerData.MRX] = toInt(_topLeft.X);
      newNUDValues[RulerData.MRY] = toInt(_topLeft.Y);
      newNUDValues[RulerData.MRXEND] = toInt(_bottomRight.X);
      newNUDValues[RulerData.MRYEND] = toInt(_bottomRight.Y);
      for (int i = 0; i < newNUDValues.Length; i++)
      {
        if (lastNUDValues[i] != newNUDValues[i]) // we try to reduce Windows messages
        {
          wasUpdate = true;
          lastNUDValues[i] = newNUDValues[i];
          RulerData.SetNudVal(nuds[i], newNUDValues[i]);
        }
      }
      return wasUpdate;
    }


    internal void ApplyChange(RulerFormMain.DragData dd, int xDistance, int yDistance, Keys keys)
    {
      if (dd == null || (xDistance == 0 && yDistance == 0)) return;
      bool isControl = (keys & Keys.Control) != 0;
      bool isShift = (keys & Keys.Shift) != 0;
      int xDistanceM = xDistance, yDistanceM = yDistance;
      if (Math.Abs(xDistance) >= Math.Abs(yDistance))
      {
        yDistanceM = Math.Abs(xDistance);
        if (yDistance < 0) yDistanceM = -yDistanceM;
      }
      else
      {
        xDistanceM = Math.Abs(yDistance);
        if (xDistance < 0) xDistanceM = -xDistanceM;
      }
      switch (dd.dragEnumOnDown)
      {
        case RulerData.DragEnum.NONE:
        default:
          return;
        case RulerData.DragEnum.DRAWNEW:
          _topLeft.X = dd.mouseDownRectInital.P11.X;
          _topLeft.Y = dd.mouseDownRectInital.P11.Y;
          _bottomRight.X = dd.mouseDownRectInital.P11.X + xDistance;
          _bottomRight.Y = dd.mouseDownRectInital.P11.Y + yDistance;
          break;
        case RulerData.DragEnum.TOPLEFT:
          _topLeft.X = dd.mouseDownRectInital.P11.X + xDistance;
          _topLeft.Y = dd.mouseDownRectInital.P11.Y + yDistance;
          if (isControl)
          {
            _bottomRight.X = dd.mouseDownRectInital.P33.X - xDistance;
            _bottomRight.Y = dd.mouseDownRectInital.P33.Y - yDistance;
          }
          return;
        case RulerData.DragEnum.TOP:
          _topLeft.Y = dd.mouseDownRectInital.P11.Y + yDistance;
          if (isControl) _bottomRight.Y = dd.mouseDownRectInital.P33.Y - yDistance;
          break;
        case RulerData.DragEnum.TOPRIGHT:
          _bottomRight.X = dd.mouseDownRectInital.P33.X + xDistance;
          _topLeft.Y = dd.mouseDownRectInital.P11.Y + yDistance;
          if (isControl)
          {
            _bottomRight.Y = dd.mouseDownRectInital.P33.Y - yDistance;
            _topLeft.X = dd.mouseDownRectInital.P11.X - xDistance;
          }

          break;
        case RulerData.DragEnum.RIGHT:
          _bottomRight.X = dd.mouseDownRectInital.P33.X + xDistance;
          if (isControl) _topLeft.X = dd.mouseDownRectInital.P11.X - xDistance;
          break;

        case RulerData.DragEnum.BOTTOMRIGHT:
          _bottomRight.X = dd.mouseDownRectInital.P33.X + xDistance;
          _bottomRight.Y = dd.mouseDownRectInital.P33.Y + yDistance;
          if (isControl)
          {
            _topLeft.Y = dd.mouseDownRectInital.P11.Y - xDistance;
            _topLeft.X = dd.mouseDownRectInital.P11.Y - yDistance;
          }
          break;

        case RulerData.DragEnum.BOTTOM:
          _bottomRight.Y = dd.mouseDownRectInital.P33.Y + yDistance;
          if (isControl)
          {
            _topLeft.Y = dd.mouseDownRectInital.P11.Y - yDistance;
          }
          break;
        case RulerData.DragEnum.BOTTOMLEFT:
          _topLeft.X = dd.mouseDownRectInital.P11.X + xDistance;
          _bottomRight.Y = dd.mouseDownRectInital.P33.Y + yDistance;
          if (isControl)
          {
            _topLeft.Y = dd.mouseDownRectInital.P11.Y - xDistance;
            _bottomRight.X = dd.mouseDownRectInital.P33.X - yDistance;
          }
          break;

        case RulerData.DragEnum.LEFT:
          _topLeft.X = dd.mouseDownRectInital.P11.X + xDistance;
          if (isControl) _bottomRight.X = dd.mouseDownRectInital.P33.X - xDistance;
          break;

        case RulerData.DragEnum.HAND:
          _topLeft.X = dd.mouseDownRectInital.P11.X + xDistance;
          _topLeft.Y = dd.mouseDownRectInital.P11.Y + yDistance;
          _bottomRight.X = dd.mouseDownRectInital.P33.X + xDistance;
          _bottomRight.Y = dd.mouseDownRectInital.P33.Y + yDistance;
          // Normalization is not required here.
          return;
        case RulerData.DragEnum.SIZEALLSTART:
          _topLeft.X = dd.mouseDownRectInital.P11.X + xDistance;
          _topLeft.Y = dd.mouseDownRectInital.P11.Y + yDistance;
          if (isControl)
          {
            _bottomRight.X = dd.mouseDownRectInital.P33.X - xDistance;
            _bottomRight.Y = dd.mouseDownRectInital.P33.Y - yDistance;
          }
          break;
        case RulerData.DragEnum.SIZEALLEND:
          _bottomRight.X = dd.mouseDownRectInital.P33.X + xDistance;
          _bottomRight.Y = dd.mouseDownRectInital.P33.Y + yDistance;
          if (isControl)
          {
            _topLeft.X = dd.mouseDownRectInital.P11.X - xDistance;
            _topLeft.Y = dd.mouseDownRectInital.P11.Y - yDistance;
          }
          break;
      }
      Normalize();

    }
    /// <summary>
    /// On move, we keep width and height. The can be corrupted a little bit by rounding errors.
    /// </summary>
    /// <param name="rulerOrgRectangle"></param>
    internal void AdjustWidthHeight(Rect rulerOrgRectangle)
    {
      _bottomRight.X = _topLeft.X + rulerOrgRectangle.WidthF;
      _bottomRight.Y = _topLeft.Y + rulerOrgRectangle.HeightF;
    }

    internal bool HasInsidePoint(Point mousePoint)
    {
      return mousePoint.X >= _topLeft.X && mousePoint.X <= _bottomRight.X
           && mousePoint.Y >= _topLeft.Y && mousePoint.Y <= _bottomRight.Y;
    }
  }

  internal class RectRuler : RulerData
  {
    public Rect rect;
    protected Point[] ScreenPoints = new Point[2]; //debug
    internal const int RECT_AREAS_LENGTH = 8;
    internal Rect[] selectedScreenAreas = new Rect[RECT_AREAS_LENGTH];
    internal int selectedPointIndex = -1;

    public RectRuler(TableLayoutPanel tlbParams, TableLayoutPanel? tlbRef, List<float> floatList, char id)
     : base(tlbParams, tlbRef, id)
    {
      if (floatList == null || floatList.Count < 7)
      {
        rect.SetFromNuds(base.lastNUDValues, base.lastRadioButton);
        return;
      }
      rect = new Rect(floatList[MRX], floatList[MRY], floatList[MRW], floatList[MRH]);
      setColorButtonPen((UInt32)floatList[4]);
      int ix = (int)Math.Round(floatList[5], 0);

      if (radioButtons != null && radioButtons.Length > RB33 && ix >= 0 && ix <= RB33)
      {
        lastRadioButton = ix; RadioButton? rix = radioButtons[ix];
        if (rix != null) rix.Checked = true;
      }
      visibleCheckBox.Checked = floatList[6] > 0;
      rect.UpdateNuds(numericUpDowns, lastNUDValues, lastRadioButton);
      updateEquation();
    }

    public RectRuler(TableLayoutPanel tlbParams, TableLayoutPanel tlbRef, List<float> floatList)
        : this(tlbParams, tlbRef, floatList, 'S')
    {
    }

    internal void StoreData(List<float> floatList)
    {
      if (floatList == null) return;
      floatList.Clear();
      floatList.Add(rect.P11F.X); floatList.Add(rect.P11F.Y);
      floatList.Add(rect.P33F.X); floatList.Add(rect.P33F.Y);
      floatList.Add(Color2Byte.ToUColor(colorButton.BackColor)); //4
      floatList.Add(lastRadioButton); //5
      floatList.Add(visibleCheckBox.Checked ? 1 : -1); //6
    }
    /// <summary>
    /// It check Value Changed event and return true, if Invalidate call is necessary
    /// </summary>
    /// <param name="sender">NumericUpDown</param>
    /// <param name="e"></param>
    /// <returns>true in case of Invalidate</returns>
    internal bool NumericUpDown_ValueChanged(object sender, EventArgs e)
    {
      NumericUpDown? nud = sender as NumericUpDown;
      if (nud == null || base.numericUpDowns == null) return false;
      string? sid = nud.Tag as string;
      if (sid == null || sid.Length < 2) return false;
      int ix = sid[1] - '0' - 1;
      if (ix < 0 || ix >= base.numericUpDowns.Length) return false;
      int nudVal = (int)(base.numericUpDowns[ix].Value);
      if (nudVal == base.lastNUDValues[ix]) return false; // stop serving this message
      if (ix > RulerData.MRH) return false;
      lastNUDValues[RulerData.MRX] = (int)(base.numericUpDowns[RulerData.MRX].Value);
      lastNUDValues[RulerData.MRY] = (int)(base.numericUpDowns[RulerData.MRY].Value);
      lastNUDValues[RulerData.MRW] = (int)(base.numericUpDowns[RulerData.MRW].Value);
      lastNUDValues[RulerData.MRH] = (int)(base.numericUpDowns[RulerData.MRH].Value);
      rect.SetFromNuds(base.lastNUDValues, base.lastRadioButton);
      rect.UpdateNuds(base.numericUpDowns, base.lastNUDValues, base.lastRadioButton);
      updateEquation();
      return true;
    }
    /// <summary>
    /// It tests CheckedChanged event and return true, if Invalidate call is necessary
    /// </summary>
    /// <param name="sender">NumericUpDown</param>
    /// <param name="e"></param>
    /// <returns>true in case of Invalidate</returns>

    internal bool RB_CheckedChanged(object sender, EventArgs e)
    {
      RadioButton? rb = sender as RadioButton;
      if (rb == null || base.radioButtons == null) return false;
      string? sid = rb.Tag as string;
      if (sid == null || sid.Length < 3) return false;
      int ix = 3 * ((sid[1] - '0') - 1) + ((sid[2] - '0') - 1);
      if (ix < 0 || ix >= base.radioButtons.Length) return false;
      if (ix == lastRadioButton) return false;
      lastRadioButton = ix;
      rect.UpdateNuds(base.numericUpDowns, base.lastNUDValues, lastRadioButton);
      updateEquation();
      return true;
    }

    public bool IsValid { get { return rect.IsValid && base.visibleCheckBox.Checked; } }


    protected const int _minMouseRectangleDistance = 5;
    public virtual DragEnum testMousePosition(Point screen) // mouse is in screen coordinates
    {
      if (selectedScreenAreas != null)
      {
        for (int i = 0; i < selectedScreenAreas.Length; i++)
        {
          Rect r = selectedScreenAreas[i];
          if (r.HasInsidePoint(screen))
          {
            selectedPointIndex = i;
            switch (i)
            {
              case 0: return DragEnum.TOPLEFT;
              case 1: return DragEnum.TOP;
              case 2: return DragEnum.TOPRIGHT;
              case 3: return DragEnum.RIGHT;
              case 4: return DragEnum.BOTTOMRIGHT;
              case 5: return DragEnum.BOTTOM;
              case 6: return DragEnum.BOTTOMLEFT;
              case 7: return DragEnum.LEFT;
            }
          }
        }

      }
      int x0 = ScreenPoints[0].X, y0 = ScreenPoints[0].Y, x1 = ScreenPoints[1].X, y1 = ScreenPoints[1].Y;
      if (x0 <= screen.X && x1 >= screen.X && y0 <= screen.Y && y1 >= screen.Y)
        return DragEnum.HAND;
      else
        return DragEnum.NONE;
    }


    internal void Redefine(PointF[] pt)
    {
      if (pt == null || pt.Length < 2) return;
      rect = new Rect(pt[0], pt[1]);
      rect.UpdateNuds(numericUpDowns, lastNUDValues, lastRadioButton);
      updateEquation();
    }

    internal void RedefineDrawing(PointF[] pt, RulerData.DraggedRuler draggedRuler)
    {
      if (pt == null || pt.Length < 2) return;
      rect = new Rect(pt, draggedRuler);
      rect.UpdateNuds(numericUpDowns, lastNUDValues, lastRadioButton);
      updateEquation();
    }
    protected virtual void updateEquation()
    {
      return;
    }

    internal void SetScreenPoints(Point[] pt)
    {
      if (pt == null || pt.Length < 2) return;
      this.ScreenPoints = pt;
      Rect r = new Rect(pt[0], pt[1]);
      setRectangleArray(this.selectedScreenAreas, r, RulerData.SELECT_PIXEL_SIZE, RulerData.SELECT_PIXEL_SIZE);
    }

    internal Rect[] SetDrawPoints(Rect drawRactangle, float selsX, float selsY)
    {
      if (selsX <= 0) selsX = 1;
      if (selsY <= 0) selsY = 1;
      Rect[] selectedScreenRect = new Rect[RECT_AREAS_LENGTH];
      setRectangleArray(selectedScreenRect, drawRactangle, selsX, selsY);
      return selectedScreenRect;
    }
    private void setRectangleArray(Rect[] selectedAreas, Rect selectedRectangle, float selsX, float selsY)
    {
      if (selectedScreenAreas == null || selectedScreenAreas.Length < RECT_AREAS_LENGTH) return;
      selectedAreas[0] = new Rect(selectedRectangle.P11F, -selsX, 0, -selsY, 0);
      selectedAreas[1] = new Rect(selectedRectangle.P12F, -selsX / 2, selsX / 2, -selsY, 0);
      selectedAreas[2] = new Rect(selectedRectangle.P13F, 0, selsX, -selsY, 0);
      selectedAreas[3] = new Rect(selectedRectangle.P23F, 0, selsX, -selsY / 2, selsY / 2);
      selectedAreas[4] = new Rect(selectedRectangle.P33F, 0, selsX, 0, +selsY);
      selectedAreas[5] = new Rect(selectedRectangle.P32F, -selsX / 2, selsX / 2, 0, +selsY);
      selectedAreas[6] = new Rect(selectedRectangle.P31F, -selsX, 0, 0, +selsY);
      selectedAreas[7] = new Rect(selectedRectangle.P21F, -selsX, 0, -selsY / 2, +selsY / 2);
    }
    //internal int TestScreenPoints(Point mousePoint)
    //{
    //  for (int i = 0; i < selectedScreenAreas.Length; i++)
    //  {
    //    if (selectedScreenAreas[i].HasInsidePoint(mousePoint)) return i;
    //  }
    //  return -1;
    //}


  }


  internal class LinRuler : RulerData
  {
    public Rect rect;
    private Point[] ScreenPoints = new Point[2]; //Used in t4sting of mouse position
    internal int selectedPointIndex = -1;
    internal const int LINE_AREAS_LENGTH = 2;
    internal Rect[] selectedScreenAreas = new Rect[LINE_AREAS_LENGTH];


    public LinRuler(TableLayoutPanel tlbParams, TableLayoutPanel? tlbRef, List<float> floatList)
       : base(tlbParams, null, 'L') // no radio buttons
    {
      if (floatList == null || floatList.Count < 7)
      {
        rect.SetFromNuds(base.lastNUDValues);
        return;
      }
      rect = new Rect(floatList[MRX], floatList[MRY], floatList[MRXEND], floatList[MRYEND], RulerData.DraggedRuler.LINE);
      setColorButtonPen((UInt32)floatList[4]);
      int ix = (int)Math.Round(floatList[5], 0);

      // radio buttons are not here

      visibleCheckBox.Checked = floatList[6] > 0;
      rect.UpdateNuds(numericUpDowns, lastNUDValues);
      updateLineEquation();
    }


    internal void StoreData(List<float> floatList)
    {
      if (floatList == null) return;
      floatList.Clear();
      floatList.Add(rect.P11F.X); floatList.Add(rect.P11F.Y);
      floatList.Add(rect.P33F.X); floatList.Add(rect.P33F.Y);
      floatList.Add(Color2Byte.ToUColor(colorButton.BackColor)); //4
      floatList.Add(0); //5
      floatList.Add(visibleCheckBox.Checked ? 1 : -1); //6
    }

    internal bool NumericUpDown_ValueChanged(object sender, EventArgs e)
    {
      NumericUpDown? nud = sender as NumericUpDown;
      if (nud == null || base.numericUpDowns == null) return false;
      string? sid = nud.Tag as string;
      if (sid == null || sid.Length < 2) return false;
      int ix = sid[1] - '0' - 1;
      if (ix < 0 || ix >= base.numericUpDowns.Length) return false;
      int nudVal = (int)(base.numericUpDowns[ix].Value);
      if (nudVal == base.lastNUDValues[ix]) return false; // stop serving this message
      if (ix > RulerData.MRYEND) return false;
      for (int i = 0; i <= RulerData.MRYEND; i++)
      {
        lastNUDValues[i] = (int)(base.numericUpDowns[i].Value);
      }
      rect.SetFromNuds(base.lastNUDValues);
      rect.UpdateNuds(base.numericUpDowns, base.lastNUDValues);
      updateLineEquation();
      return true;
    }

    public bool IsValid { get { return rect.IsValid && visibleCheckBox.Checked; } }


    private float squareDistance(PointF p1, PointF p2)
    {
      return (p1.X - p2.X) * (p1.X - p2.X) + (p1.Y - p2.Y) * (p1.Y - p2.Y);
    }

    private int distanceFromLine(Point screen, PointF p1, PointF p2)
    {
      // line A*x+B*Y+C = (y1-y2)*x+(x2-x1)*y+(y2*x1-x2*y1)=0
      // 
      double A = (p1.Y - p2.Y), B = (p2.X - p1.X);
      double AB = Math.Sqrt(A * A + B * B);
      if (AB < 1) AB = 1;
      return (int)Math.Round(Math.Abs((p1.Y - p2.Y) * screen.X + (p2.X - p1.X) * screen.Y + (p2.Y * p1.X - p2.X * p1.Y)) / AB, 0);
    }

    private void updateLineEquation()
    {
      int x1 = rect.P11.X, y1 = rect.P11.Y;
      int x2 = rect.P33.X, y2 = rect.P33.Y;
      // (y1-y2) * x + (x2-x1) * y + (x1-x2)*y1 + (y2-y1)*x1 = 0
      if (x1 > x2 && y1 > y2) { x1 = x2; y1 = y2; x2 = rect.P11.X; y2 = rect.P11.Y; }
      long DY = (y2 - y1), DX = (x2 - x1), Q = DY * x1 - DX * y1; // Q =(y2-y1) * x1-(x2 - x1) * y1
      long gcd12 = RulerData.GCD(Math.Abs(DX), Math.Abs(DY));
      long gcd = RulerData.GCD(gcd12, Math.Abs(Q)); // For safety, GCD should be the same
      if (semiX != null) semiX.Text = DX.ToString();
      if (semiY != null) semiY.Text = DY.ToString();
      if (semiZ != null) semiZ.Text = Q.ToString();
      if (gcdXYZ != null)
      {
        gcdXYZ.ForeColor = SystemColors.ControlText; gcdXYZ.BackColor = SystemColors.ControlLight;
        if (DX == 0 || DY == 0)
        {
          gcd = 1; gcdXYZ.Text = String.Format("1", gcd);
          if (semiZ != null) semiZ.Text = "?";
        }
        else
        {
          if (gcd <= 1) gcdXYZ.BackColor = Color.LightPink;
          else
          {
            if (gcd <= 2) gcdXYZ.BackColor = Color.Yellow;
          }
          gcdXYZ.Text = String.Format("{0}=gcd(ΔX,ΔY)", gcd);
        }
      }
      if (gcd > 1)
      {
        long DX0 = DX, DY0 = DY, Q0 = Q;
        DX /= gcd; DY /= gcd; Q /= gcd;
        if (semiX != null) semiX.Text = String.Format("{0}/{1}={2}", DX0, gcd, DX);
        if (semiY != null) semiY.Text = String.Format("{0}/{1}={2}", DY0, gcd, DY);
        if (semiZ != null) semiZ.Text = String.Format("{0}/{1}={2}", Q0, gcd, Q);
      }
      //  bool isText = false; 
      rtb.Clear();
      if (gcdXYZ != null) rtb.SetBackColor(gcdXYZ.BackColor);
      // DX is in the equation with minus sign
      if (Q < 0 && DX >= 0 && DY < 0) { Q = -Q; DX = -DX; DY = -DY; }
      if (DY == 0)
      {
        if (DX == 0) rtb.WrR("?");
        else
        {
          rtb.WrR("y = "); rtb.WrB(y1.ToString());
        }
        return;
      }
      if (DX == 0)
      {
        rtb.WrR("x = "); rtb.WrB(x1.ToString());
        return;
      }
      //     if (DX > 0 && DY < 0) { DX = -DX; DY = -DY; Q = -Q; } // omitted for agreement with help
      if (DY == -1) rtb.WrR("-x ");
      else
      {
        if (DY != 1) { rtb.WrB(" "); rtb.WrB(DY.ToString()); rtb.WrR(" * "); }
        rtb.WrR("x ");
      }
      // DX is in the equation with minus sign
      if (DX < 0) rtb.WrR("+ "); else rtb.WrR("- "); // it is -DX
      long DXA = Math.Abs(DX);
      if (DXA != 1) { rtb.WrB(Math.Abs(DXA).ToString()); rtb.WrR(" * "); }
      rtb.WrR("y");
      rtb.WrR(" = ");
      rtb.WrB(Q.ToString());
    }
    /// <summary>
    /// Called from search optimal line
    /// </summary>
    /// <param name="x1"></param>
    /// <param name="y1"></param>
    /// <param name="x2"></param>
    /// <param name="y2"></param>
    /// <returns></returns>
    public static string ToLineEquation(int x1, int y1, int x2, int y2)
    {
      // (y1-y2) * x + (x2-x1) * y + (x1-x2)*y1 + (y2-y1)*x1 = 0
      if (x1 > x2 && y1 > y2)
      {
        int x10 = x1, y10 = y1;
        x1 = x2; y1 = y2; x2 = x10; y2 = y10;
      }
      long DY = (y2 - y1), DX = (x2 - x1), Q = DY * x1 - DX * y1; // Q =(y2-y1) * x1-(x2 - x1) * y1
      long gcd12 = RulerData.GCD(Math.Abs(DX), Math.Abs(DY));
      long gcd = RulerData.GCD(gcd12, Math.Abs(Q)); // For safety, GCD should be the same
      if (gcd > 1) { DX /= gcd; DY /= gcd; Q /= gcd; }
      // DX is in the equation with minus sign
      if (Q < 0 && DX >= 0 && DY < 0) { Q = -Q; DX = -DX; DY = -DY; }
      //  bool isText = false; 
      if (DY == 0)
      {
        if (DX == 0) return "?";
        else return String.Format("y = {0}", y1);
      }
      if (DX == 0) return String.Format("y = {0}", y1);
      //     if (DX > 0 && DY < 0) { DX = -DX; DY = -DY; Q = -Q; } // omitted for agreement with help
      StringBuilder sb = new StringBuilder();
      if (DY == -1) sb.Append("-x ");
      else
      {
        if (DY != 1) { sb.Append(" "); sb.Append(DY.ToString()); sb.Append(" * "); }
        sb.Append("x ");
      }
      if (DX < 0) sb.Append("+ "); else sb.Append("- "); // it is -DX
      long DXA = Math.Abs(DX);
      if (DXA != 1) { sb.Append(Math.Abs(DXA).ToString()); sb.Append(" * "); }
      sb.Append("y");
      sb.Append(" = ");
      sb.Append(Q);
      return sb.ToString();
    }
    public DragEnum testMousePosition(Point screen) // mouse is in screen coordinates
    {
      float startD = squareDistance(screen, ScreenPoints[0]);
      float endD = squareDistance(screen, ScreenPoints[1]);
      if (startD < endD)
      {
        if (startD < SELECT_PIXEL_SQUARE_RADIUS) return DragEnum.SIZEALLSTART;
      }
      else
      {
        if (endD < SELECT_PIXEL_SQUARE_RADIUS) return DragEnum.SIZEALLEND;
      }
      int x0 = ScreenPoints[0].X; int y0 = ScreenPoints[0].Y;
      int x1 = ScreenPoints[1].X; int y1 = ScreenPoints[1].Y;
      bool inRange = (x0 < x1) ? screen.X <= x1 && screen.X >= x0 : screen.X <= x0 && screen.X >= x1;
      if (!inRange) return DragEnum.NONE;
      inRange = (y0 < y1) ? screen.Y <= y1 && screen.Y >= y0 : screen.Y <= y0 && screen.Y >= y1;
      if (!inRange) return DragEnum.NONE;
      int dist = distanceFromLine(screen, ScreenPoints[0], ScreenPoints[1]);
      if (dist < SELECT_PIXEL_SIZE)
        return DragEnum.HAND;
      else
        return DragEnum.NONE;

    }

    internal void Redefine(PointF[] pt)
    {
      if (pt == null || pt.Length < 2) return;
      rect = new Rect(pt[0], pt[1]);
      rect.UpdateNuds(numericUpDowns, lastNUDValues);
      updateLineEquation();
    }

    internal void RedefineDrawing(PointF[] pt, RulerData.DraggedRuler dr)
    {
      if (pt == null || pt.Length < 2) return;
      rect = new Rect(pt, dr);  // we reset adding frame
      rect.UpdateNuds(numericUpDowns, lastNUDValues);
      updateLineEquation();
    }

    internal void SetScreenPoints(Point[] pt)
    {
      if (pt == null || pt.Length < 2) return;
      this.ScreenPoints = pt;
      Rect r = new Rect(pt[0], pt[1]);
      setLineArray(this.selectedScreenAreas, r, RulerData.SELECT_PIXEL_SIZE, RulerData.SELECT_PIXEL_SIZE);
    }
    private void setLineArray(Rect[] selectedAreas, Rect selectedRectangle, float selsX, float selsY)
    {
      if (selectedScreenAreas == null || selectedScreenAreas.Length < LINE_AREAS_LENGTH) return;
      selectedRectangle.Normalize();
      selectedAreas[0] = new Rect(selectedRectangle.P11F, -selsX, 0, -selsY, 0);
      selectedAreas[1] = new Rect(selectedRectangle.P33F, 0, selsX, 0, +selsY);
    }
    internal Rect[] SetDrawPoints(Rect drawRactangle, float selsX, float selsY)
    {
      if (selsX <= 0) selsX = 1;
      if (selsY <= 0) selsY = 1;
      Rect[] selectedScreenRect = new Rect[LINE_AREAS_LENGTH];
      setLineArray(selectedScreenRect, drawRactangle, selsX, selsY);
      return selectedScreenRect;
    }
  }
  internal class EllipticRuler : RectRuler
  {
    public EllipticRuler(TableLayoutPanel tlbParams, TableLayoutPanel? tlbRef, List<float> floatList)
      : base(tlbParams, tlbRef, floatList, 'E')
    {

    }

    //public override DragEnum testMousePosition(Point screen) // mouse is in screen coordinates
    //{
    //  Rect r = new Rect(ScreenPoints[0].X, ScreenPoints[0].Y, ScreenPoints[1].X, ScreenPoints[1].Y);
    //  // we test first rectangle
    //  int distance = isInsideEllipse(screen, rect);
    //  if (distance < 0) return DragEnum.HAND;
    //  else return base.testMousePosition(screen);
    //}

    //private int isInsideEllipse(Point p, Rect r)
    //{
    //  // ellipse  B*(x-xs)**2+A*(y-ys)=A*B = H/2*(x-selectedRectangle.P22.X)
    //  // 
    //  double xd = (p.X - r.P22F.X), yd = (p.Y - r.P22F.Y);
    //  double A = r.WidthF / 2, B = r.HeightF / 2;
    //  A = A * A; B = B * B;
    //  double AB = A * B;
    //  return (int)Math.Round((B * xd * xd + A * yd * yd - AB));
    //}

    protected override void updateEquation()
    {
      rtb.Clear();
      int xcenter = rect.P22.X, ycenter = rect.P22.Y;
      long xaxis = Math.Abs(rect.Width / 2), yaxis = Math.Abs(rect.Height / 2);
      if (semiX != null) semiX.Text = xaxis.ToString();
      if (semiY != null) semiY.Text = yaxis.ToString();
      if (xaxis == 0 || yaxis == 0) { rtb.WrR(" --- "); return; }
      long gcd = RulerData.GCD(Math.Abs(xaxis), Math.Abs(yaxis));
      if (gcdXYZ != null)
      {
        gcdXYZ.ForeColor = SystemColors.ControlText;
        gcdXYZ.Text = gcd.ToString();
        if (gcd <= 1) gcdXYZ.BackColor = Color.LightPink;
        else
        {
          if (gcd <= 4) gcdXYZ.BackColor = Color.Yellow;
          else gcdXYZ.BackColor = SystemColors.ControlLight;
        }
        rtb.SetBackColor(gcdXYZ.BackColor);
      }

      long xaxisorg = xaxis;
      if (gcd > 1)
      {
        long xaxis0 = xaxis / gcd, yaxis0 = yaxis / gcd;
        if (semiX != null) semiX.Text = String.Format("{0}; A/{1}={2}", xaxis, gcd, xaxis0); ;
        if (semiY != null) semiY.Text = String.Format("{0}; B/{1}={2}", yaxis, gcd, yaxis0); ;
        xaxis = xaxis0; yaxis = yaxis0;
      }
      rtb.WrR(" ");
      if (yaxis != 1)
      {
        rtb.WrB(yaxis.ToString());
        rtb.WrR("**2 * ");
      }
      rtb.WrR("(x-");
      rtb.WrB(xcenter.ToString());
      rtb.WrR(")**2 + ");
      if (xaxis != 1)
      {
        rtb.WrB(xaxis.ToString()); rtb.WrR("**2 *");
      }
      rtb.WrR("(y-"); rtb.WrB(ycenter.ToString()); rtb.WrR(")**2 = ");
      if (yaxis != 1) rtb.WrR("(");
      rtb.WrB(xaxisorg.ToString());
      if (yaxis != 1)
      {
        rtb.WrR("*"); rtb.WrB(yaxis.ToString()); rtb.WrR(")");
      }
      rtb.WrR("**2");

      base.updateEquation();
    }


  }
}
