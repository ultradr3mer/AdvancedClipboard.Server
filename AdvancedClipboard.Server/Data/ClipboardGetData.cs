using System;

namespace AdvancedClipboard.Server.Data
{
  public class ClipboardGetData
  {
    public Guid Id { get; internal set; }
    public string PlainTextContent { get; internal set; }
  }
}