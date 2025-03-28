using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FpgaLcdUtils
{
  public class FrameItem
  {
    public long Position;
    public bool IsFull; // it is set after 1st reading of frame

    public FrameItem()
    {
      this.Position = 0; IsFull = false;
    }

    public FrameItem(long position)
    {
      this.Position = position; IsFull = false;
    }

    public override string ToString()
    {
      return String.Format("Pos={0} {1}",Position, IsFull?'F':'N');
    }

  }
  public class FrameInfos
  {
    public DateTime CreateTime = DateTime.MinValue;
    protected List<FrameItem> Items = new List<FrameItem>(128);

    private void initilize() { CreateTime = DateTime.MinValue; /*Items.AddNoRepeat(new FrameInfo(0, 0));*/ }
    public FrameInfos() { Items = new List<FrameItem>(); initilize(); }
    public void Clear() { Items.Clear(); initilize(); }

    public long GetLastStart(out int frameIndex)
    {
      if (Items.Count > 0)
      {
        frameIndex = Items.Count;
        return Items[frameIndex - 1].Position;
      }
      else { frameIndex = 0; return 0; }
    }
    public long GetLastFullStart(out int frameIndex)
    {
      if (Items.Count == 0) { frameIndex = 0; return 0; }
      if (Items.Count == 1) { frameIndex = 1; return 1; }  // we have no other to select
      frameIndex = Items.Count; ;
      FrameItem item = Items[frameIndex - 1];
      if (item.IsFull) return item.Position;
      else
      {
        frameIndex--; return Items[frameIndex - 1].Position; ; // this must be full
      }
    }

    public int Count { get { return Items.Count; } }

    public void Add(FrameItem x) { Items.Add(x); }

    public FrameItem this[int ix] { get { return ix > 0 && ix <= Items.Count ? Items[ix - 1] : Items[Items.Count - 1]; } }

  }

  public class FileDescriptor
  {
    public readonly string FileName;
    public readonly int FrameIndex;
    public readonly long StartPos;
    public readonly int DataLength;
    public readonly DateTime LastWrite;
    internal readonly int FrameCount;

    public FileDescriptor()
    {
      FileName = string.Empty; FrameIndex = 0; StartPos = 0; DataLength = 0; LastWrite = DateTime.MinValue;
      FrameCount = 0;
    }

    public FileDescriptor(string filename, int frameIndex, int frameCount, long startPos, int dataLength, DateTime lastWrite)
    {
      FileName = filename;
      FrameIndex = frameIndex;
      FrameCount = frameCount;
      StartPos = startPos;
      DataLength = dataLength;
      LastWrite = lastWrite;
    }

 
  }
  internal static class TBFileLoader
  {
    public static DateTime lastWriteTime = DateTime.MinValue;
    public static DateTime lastCreateTime = DateTime.MinValue;
    public static string lastFilename = String.Empty;
    public static long lastLength = 0;
    private static byte[] _data = new byte[1];
    private static FrameInfos _frames = new FrameInfos();
    private static int _dataLength = 0;
    private const string _findStart = "##=0,0\\r\\n";

    public const int FIRST_FRAME = 1;
    public const int LAST_FRAME = -1;
     internal static void SetFull(int frameIndex)
    {
      if(_frames.Count>0 && _frames.Count<=frameIndex && frameIndex>=FIRST_FRAME)
        _frames[frameIndex-1].IsFull = true;
    }

    public static readonly TimeSpan waitTimeOfFullFrame = new TimeSpan(0, 0, 0, 2);
    
    public static byte[] LoadFile(string filename, bool wait4CompleteFrame, ref int frameIndex, out FileDescriptor fileDescriptor)
    {
      try
      {
        bool isNew = false;
        if (!ThreadLoadFrame.PlayMode)
        {
          FileInfo fi = new FileInfo(filename);
          if (lastFilename != filename || lastWriteTime != fi.LastWriteTime || lastCreateTime != fi.CreationTime || lastLength != fi.Length)
          {
            lastFilename = filename; lastWriteTime = fi.LastWriteTime; lastCreateTime = fi.CreationTime; lastLength = fi.Length;
            isNew = true;
          }
        }
        if (isNew || frameIndex < FIRST_FRAME)
        {
          _data = new byte[lastLength + 10000]; //size of testbench file+possible change
                                                // Open file with share access
          using (FileStream rdtxt = new System.IO.FileStream(filename, FileMode.Open,
                                                           FileAccess.Read, FileShare.ReadWrite))
          {
            // We minimize its share access time
            _dataLength = rdtxt.Read(_data, 0, _data.Length);
          };

          _frames.Clear();
          // fast search of the beginnings of frames
          int i = 0; FrameItem? fix=null; byte c = 0;
          while (i < _data.Length - _findStart.Length)
          {
            // "##=0,0\r\n";
            if (_data[i++] != (byte)'#') continue;
            int start = i; // to speed up the loop, we assigned after the 1st # element
            if (_data[i++] != (byte)'#') continue;
            if (_data[i++] != (byte)'=') continue;
            if (_data[i++] != (byte)'0') continue;
            if (_data[i++] != (byte)',') continue;
            if ((c = _data[i++]) == (byte)'0')
            {
              if (_data[i++] != (byte)'\r') continue;
              if (_data[i++] != (byte)'\n') continue;
              _frames.Add(fix = new FrameItem(start - 1));
            }
            else // detect end line
            {
              if (c != (byte)'5') continue;
              if (_data[i++] != (byte)'2') continue;
              if (_data[i++] != (byte)'4') continue;
              if (_data[i++] != (byte)'\r') continue;
              if (_data[i++] != (byte)'\n') continue;
              if (fix != null) { fix.IsFull = true; fix = null; }
            }
          }
        }
        long startPos = 0;
        if (frameIndex < FIRST_FRAME)
        {
          if (!wait4CompleteFrame || lastWriteTime+waitTimeOfFullFrame < DateTime.Now)
               startPos = _frames.GetLastStart(out frameIndex);
          else { startPos = _frames.GetLastFullStart(out frameIndex); }
        }
        else
        {
          if (frameIndex == FIRST_FRAME || _frames.Count<1) startPos = 0;
          else startPos = _frames[frameIndex].Position;
        }
        fileDescriptor = new FileDescriptor(lastFilename, frameIndex, _frames.Count, startPos, _dataLength, lastWriteTime);

        return _data;
      }
      catch (Exception e)
      {
        Trace.WriteLine(e.Message); fileDescriptor = new FileDescriptor();
        return new byte[1];
      }
    }
  }
}
