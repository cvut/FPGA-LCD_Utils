using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Net.Mime.MediaTypeNames;

namespace LSPtools
{

  public static class ThreadLoadFrame
  {
    public class InputQueueItem
    {
      public enum CmdEnum { ChangeFilename, ReLoad, AutoReload, LoadFrame }
      public CmdEnum Command = CmdEnum.ChangeFilename;
      public readonly string Filename;
      public int FrameIndex = 0;
      public InputQueueItem(string filename, CmdEnum command)
      {
        this.Filename = filename; this.Command = command;
      }
      public InputQueueItem(string filename, CmdEnum command, int frameIndex) : this(filename, command)
      { FrameIndex = frameIndex; }
    }
    public class OutputQueueItem
    {
      public readonly string Message;
      public RGBInfoArray RGBarray;
      public Bitmap Image;
      public OutputQueueItem(string message, RGBInfoArray array, Bitmap image)
      { this.Message = message; this.RGBarray = array; this.Image = image; }
    }

    /// <summary>
    /// Thread runs until FlagStopThread is set true
    /// </summary>
    public static bool FlagStopThread = false;
    /// <summary>
    /// Thread automatically reloads testbench after its change
    /// </summary>
    public static bool AutoReloadTestbench = true;
    /// <summary>
    /// When multi-frame testbench is loaded, the thread waits for complete frame
    /// </summary>
    public static bool Wait4CompleteFrame = false;

    /// <summary>
    /// In play mode, the thread ignores AutoReload option
    /// </summary>
    private static bool _playMode = false;
    public static bool PlayMode { get { return _playMode; } }
    public static void SetPlayMode(bool state) {  _playMode = state; }
    /// <summary>
    /// Memory of the last exception
    /// </summary>    
    public static string DebugException = String.Empty;

   
    public static ThreadSafeCircularQueue<InputQueueItem> InputQueue = new ThreadSafeCircularQueue<InputQueueItem>(4);
    public static ThreadSafeCircularQueue<OutputQueueItem> OutputQueue = new ThreadSafeCircularQueue<OutputQueueItem>(4);


    public struct DebugActivityCounters
    {
      public UInt64 MainLoop;
      public UInt64 ReadInQueue;
      public UInt64 WriteOutQueue;
      public UInt64 ImagesDone;
      public void Clear()
      {
        MainLoop = ReadInQueue = WriteOutQueue = ImagesDone = 0;
      }
      public override string ToString()
      {
        return String.Format("Activity counters> MainLoop={0} ReadInQueue={1} WriteOutQueue={2} ImagesDone={3}",
                             MainLoop, ReadInQueue, WriteOutQueue, ImagesDone);
      }
    }

    public static DebugActivityCounters activityCounters;

    /// <summary>
    /// ThreadCompare result data
    /// </summary>
    /// 
    public static void InitilizeAll()
    {
      FlagStopThread = _playMode =false; 
      activityCounters.Clear();
      InputQueue = new ThreadSafeCircularQueue<InputQueueItem>(4);
      OutputQueue = new ThreadSafeCircularQueue<OutputQueueItem>(4);
    }

    private static void envokeSleep()
    {
      if (!InputQueue.IsNextToProcessing) Thread.Sleep(10);
    }
    public static void Start(Object threadContext)
    {
      string filename = String.Empty;
      DateTime lastFileThreadAccess = DateTime.MinValue;
      InitilizeAll();
      TBFormMain.DBG("Thread started");
      while (!FlagStopThread)
      {
        try
        {
          unchecked { activityCounters.MainLoop++; }
          InputQueueItem? item;
          item = InputQueue.NextToProcessing();
          InputQueueItem.CmdEnum cmd;
          if (item == null)
          {
            if (_playMode || !AutoReloadTestbench) { envokeSleep(); continue; }
            else 
              cmd = InputQueueItem.CmdEnum.AutoReload;
          }
          else
          {
            cmd = item.Command; unchecked { activityCounters.ReadInQueue++; }
          }
          bool isValidFile = false;
          int frameIndex = -1;
          bool wait4CompleteFrame = !_playMode && Wait4CompleteFrame;
          switch (cmd)
          {
            case InputQueueItem.CmdEnum.ChangeFilename:
              filename = item?.Filename ?? String.Empty; wait4CompleteFrame = false;
              break;
            case InputQueueItem.CmdEnum.LoadFrame:
              frameIndex = item?.FrameIndex ?? -1; wait4CompleteFrame = false;
              break;
            case InputQueueItem.CmdEnum.ReLoad:
              wait4CompleteFrame = false;
              break;
            case InputQueueItem.CmdEnum.AutoReload:
              isValidFile = (!String.IsNullOrEmpty(filename) && File.Exists(filename));
              if (isValidFile)
              {
                FileInfo fix = new FileInfo(filename);
                if (lastFileThreadAccess < fix.LastWriteTime) break;
              }
              envokeSleep();
              continue;
          }
          FileInfo fi = new FileInfo(filename);
          lastFileThreadAccess = (fi != null) ? fi.LastWriteTime : DateTime.Now; // do not repeat error reading

          if (!isValidFile) isValidFile = !String.IsNullOrEmpty(filename) && File.Exists(filename);
          if (!isValidFile)
          {
            envokeSleep(); continue;
          }

          RGBInfoArray rgbArray; Bitmap image;
          string message = LoadTestbenchFile(filename, frameIndex, wait4CompleteFrame, out rgbArray, out image);

          OutputQueue.EnqueueRequest(new OutputQueueItem(message, rgbArray, image));
          unchecked { activityCounters.WriteOutQueue++; }
          /**********************************************************/
          //FlagStopThread = true; // DEBUG simulation of a thread error
          /**********************************************************/
        }
        catch (Exception ex)
        {
          Trace.WriteLine(DebugException = ex.ToString()); TBFormMain.DBG(DebugException);
        }
      } // while
      TBFormMain.DBG("Thread loop terminated");
    }

    static public string LoadTestbenchFile(string filename, int frameIndex, bool wait4CompleteFrame, out RGBInfoArray rgbArray, out Bitmap image)
    {
      rgbArray = new RGBInfoArray();
      StringBuilder sbx = new StringBuilder();
      sbx.AppendFormat("{0:HH:mm:ss} ", DateTime.Now);
      sbx.Append(rgbArray.LoadFromFile(filename, frameIndex, wait4CompleteFrame));
      FileInfo fi = new FileInfo(filename); // update file info
      if (fi != null) sbx.AppendFormat(" created at {0:HH:mm:ss [dd.MM.yyyy] }", fi.LastWriteTime);
      int nonBlack;
      image = rgbArray.CreateBitmap(true, out nonBlack);
      if (nonBlack == 0) sbx.Append(" Warning: All pixels have BLACK color!");
      else unchecked { activityCounters.ImagesDone++; }
      return sbx.ToString();
    }

  }
}
