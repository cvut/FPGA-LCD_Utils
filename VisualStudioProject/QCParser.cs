using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FpgaLcdUtils
{
  internal class QCParser
  {
    private readonly string text;
    private int pos;
    private int posMemory;

    public QCParser(string text)
    {
      this.text = text; pos = posMemory = 0;
    }


    public int Pos
    {
      get { return pos; }
      set
      {
        if (value < 0) pos = 0;
        else
        {
          if (value >= LengthTotal) pos = LengthTotal;
          else pos = value;
        }
      }

    }
    public int LengthTotal { get { return text.Length; } }
    public int LengthToEnd { get { return text.Length - pos; } }
    public bool EndOfText { get { return pos >= LengthTotal; } }

    public bool MoveBy(int addToPos)
    {
      pos = pos + addToPos;
      if (pos >= LengthTotal) { pos = LengthTotal; return false; }
      return true;

    }
    public bool MoveToEnd(string openSeq)
    {
      if (pos >= LengthTotal) return false;
      int pos1 = 0;
      pos1 = text.IndexOf(openSeq, pos);
      if (pos1 < 0) return false;
      pos = pos1 + openSeq.Length;
      return true;
    }
    public int PosOfFirst(string openSeq)
    {
      if (pos >= LengthTotal) return -1;
      int pos1 = 0;
      pos1 = text.IndexOf(openSeq, pos);
      if (pos1 < 0) return -1;
      else return pos1;
    }
    public bool MoveToEnd(string openSeq1, string openSeq2)
    {
      return MoveToEnd(openSeq1) && MoveToEnd(openSeq2);
    }
    public bool MoveToEnd(string openSeq1, string openSeq2, string openSeq3)
    {
      return MoveToEnd(openSeq1) && MoveToEnd(openSeq2) && MoveToEnd(openSeq3);
    }
    public bool MoveToEnd(string openSeq1, string openSeq2, string openSeq3, string openSeq4)
    {
      return MoveToEnd(openSeq1) && MoveToEnd(openSeq2) && MoveToEnd(openSeq3) && MoveToEnd(openSeq4);
    }
    public string GetTextTo(string endSeq)
    {
      if (pos >= LengthTotal) return String.Empty;
      int pos1 = pos, pos2;
      pos2 = text.IndexOf(endSeq, pos);
      if (pos2 < 0) return String.Empty;
      pos = pos2 + endSeq.Length;
      return text.Substring(pos1, pos2 - pos1);
    }

    private void SaveLinePositions()
    {
      this.posMemory = this.pos;
    }
    public void RestorePreviousLine()
    {
      this.pos = this.posMemory;
    }

    public string ReadToEndLine()
    {
      SaveLinePositions();
      // skip white spaces  
      while (pos < text.Length && Char.IsWhiteSpace(text[pos])) pos++;
      if (pos >= text.Length) return String.Empty;
      return text.Substring(pos).Trim();

    }

    public string ReadToWhiteSpace()
    { return ReadToWhiteSpace('"'); }

    public string ReadToWhiteSpace(char surround)
    {
      SaveLinePositions();

      if (pos < text.Length)
      {
        // skip white spaces  
        while (pos < text.Length && Char.IsWhiteSpace(text[pos])) pos++;
        int pos0 = pos; char c=' ';
        if (pos < text.Length && text[pos] == surround)
        {
          pos++;
          while (pos < text.Length) { c=text[pos++]; if (c == surround) break; }
        }
        else
        {
          while (pos < text.Length && !Char.IsWhiteSpace(c = text[pos])) pos++;
        }
        int pos1 = pos;
        int len = pos1 - pos0;
        pos = pos1;
        return text.Substring(pos0, len);
      }
      else return String.Empty;
    }

    public static string RemoveApostrophe(string s)
    {
      return RemoveSurround(s, '"');
    }
    public static string RemoveSurround(string s, char surround)
    { if (s == null) return String.Empty;
      if(s.Length < 3) return s;
      if (s[0] == surround && s[s.Length - 1] == surround)
        return s.Substring(1, s.Length - 2);
      else return s;
    }



    public override string ToString()
    {
      const int FORWARD = 128;
      const int BACKWARD = 32;
      int len = LengthTotal - pos;
      if (len > FORWARD) len = FORWARD;
      int lenm = pos - 1 > BACKWARD ? BACKWARD : pos - 1;
      return String.Format("{0}â–ˆ{1}", lenm <= 0 ? "" : text.Substring(pos - 1 - lenm, lenm + 1),
                                                     len <= 0 ? "" : text.Substring(pos, len));
    }

  }
}
