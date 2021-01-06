using System;

namespace AdvancedClipboard.Server.Data
{
  public class ClipboardGetData
  {
    public Guid Id { get; internal set; }
    public object PlainTextContent { get; internal set; }
  }
}