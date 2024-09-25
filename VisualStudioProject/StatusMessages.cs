using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LSPtools
{
  internal class StatusMessages
  {
    private readonly TimeSpan _messageDuration; // 10 seconds
    public enum MessageSeverity { Info, Warning, Error };
    private bool _messageHasText = false;
    private DateTime _messageStartTime = DateTime.MinValue; // 10 seconds
    private readonly ToolStripStatusLabel tsslMessage;

    public StatusMessages(int durationSeconds, ToolStripStatusLabel tssl)
    {
      _messageDuration = new TimeSpan(0, 0, 10); this.tsslMessage=tssl;
    }

    public void Info(string text)
    {
      Message(text, MessageSeverity.Info);
    }
    public void Warning(string text)
    {
      Message(text, MessageSeverity.Warning);
    }
    public void Error(string text)
    {
      Message(text, MessageSeverity.Error);
    }
    public void Message(string text, MessageSeverity severity)
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

    public void timerUpdate_Tick(object sender, EventArgs e)
    {
      if (_messageHasText)
      {
        DateTime dt = DateTime.Now;
        if ((dt - _messageStartTime) >= _messageDuration)  Info(String.Empty);
      }
    }
  }
}
