using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace LSPtools
{
  internal class RichTBWriter
  {
    public readonly RichTextBox rtb;
    private Font italicRTB = SystemFonts.DefaultFont;
    private Font boldRTB = SystemFonts.DefaultFont;
    private Font regularRTB = SystemFonts.DefaultFont;
    private Font underlineRTB = SystemFonts.DefaultFont;

    public enum FT { noChange, italic, bold, regular, underline };

    public RichTBWriter(RichTextBox rtb)
    {
      this.rtb = rtb;
      italicRTB = new Font(rtb.Font, FontStyle.Italic);
      boldRTB = new Font(rtb.Font, FontStyle.Bold);
      regularRTB = new Font(rtb.Font, FontStyle.Regular);
      underlineRTB = new Font(rtb.Font, FontStyle.Underline);
    }
    /// <summary>
    /// Clear RichTextBox
    /// </summary>
    public void Clear()
    {
      rtb.Text = " "; // Clear() of assigning String.Empty clears also ZoomFactor
    }
    /// <summary>
    /// Write text and append line by the previous font
    /// </summary>
    /// <param name="text"></param>
    public void WrLine(string text)
    {
      WrLine(text, FT.noChange);
    }
    /// <summary>
    /// Write text and append line by the bold font
    /// </summary>
    /// <param name="text"></param>
    public void WrLineB(string text)
    {
      WrLine(text, FT.bold);
    }
    /// <summary>
    /// Write text and append line by the italic font
    /// </summary>
    /// <param name="text"></param>
    public void WrLineI(string text)
    {
      WrLine(text, FT.italic);
    }
    /// <summary>
    /// Write text and append line by the regular font
    /// </summary>
    /// <param name="text"></param>
    public void WrLineR(string text)
    {
      WrLine(text, FT.regular);
    }
    /// <summary>
    /// Write text and append line by the underlined font
    /// </summary>
    /// <param name="text"></param>
    public void WrLineU(string text)
    {
      WrLine(text, FT.underline);
    }
    /// <summary>
    /// Write text and append line by the font
    /// </summary>
    /// <param name="text"></param>
    /// <param name="fontType"></param>
    public void WrLine(string text, FT fontType)
    {
      Wr(text, fontType); rtb.AppendText(Environment.NewLine);
    }
    /// <summary>
    /// Write text by the previous font
    /// </summary>
    /// <param name="text"></param>
    public void Wr(string text)
    {
      Wr(text, FT.noChange);
    }
    /// <summary>
    /// Write text by the bold font
    /// </summary>
    /// <param name="text"></param>
    public void WrB(string text)
    {
      Wr(text, FT.bold);
    }
    /// <summary>
    /// Write number by the bold font
    /// </summary>
    /// <param name="number"></param>
    public void WrB(int number)
    {
      Wr(number.ToString(), FT.bold);
    }
    /// <summary>
    /// Write text by the italic font
    /// </summary>
    /// <param name="text"></param>
    public void WrI(string text)
    {
      Wr(text, FT.italic);
    }
    /// <summary>
    /// Write number by the italic font
    /// </summary>
    /// <param name="number"></param>
    public void WrI(int number)
    {
      Wr(number.ToString(), FT.italic);
    }
    /// <summary>
    /// Write text by the regular font
    /// </summary>
    /// <param name="text"></param>
    public void WrR(string text)
    {
      Wr(text, FT.regular);
    }
    /// <summary>
    /// Write number by the regular font
    /// </summary>
    /// <param name="number"></param>
    public void WrR(int number)
    {
      Wr(number.ToString(), FT.regular);
    }
    /// <summary>
    /// Write text by the underlined font
    /// </summary>
    /// <param name="text"></param>
    public void WrU(string text)
    {
      Wr(text, FT.underline);
    }
    /// <summary>
    /// Write number by the underlined font
    /// </summary>
    /// <param name="number"></param>
    public void WrU(int number)
    {
      Wr(number.ToString(), FT.underline);
    }
    /// <summary>
    /// Write text by the specified font
    /// </summary>
    /// <param name="text"></param>
    /// <param name="fontType"></param>
    public void Wr(string text, FT fontType)
    {
      if (text == null || text.Length == 0) return;
      switch (fontType)
      {
        case FT.regular: rtb.SelectionFont = regularRTB; break;
        case FT.bold: rtb.SelectionFont = boldRTB; break;
        case FT.italic: rtb.SelectionFont = italicRTB; break;
        case FT.underline: rtb.SelectionFont = underlineRTB; break;
      }
      rtb.AppendText(text);
      rtb.ScrollToCaret();
    }

    public bool IsCaretVisible()
    {
      return rtb.ClientRectangle.Contains(rtb.GetPositionFromCharIndex(rtb.SelectionStart));
    }
    internal void SetColor(Color color)
    {
      rtb.SelectionColor = color;
    }
    internal void RegularColor()
    {
      rtb.SelectionColor = SystemColors.WindowText;
    }

    internal void ShowFirstLine()
    {
      rtb.SelectionStart = 0; rtb.SelectionLength = 0; rtb.ScrollToCaret();
    }
  }
}
