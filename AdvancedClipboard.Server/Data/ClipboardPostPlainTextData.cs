using System;

namespace AdvancedClipboard.Server.Data
{
  public class ClipboardPostPlainTextData
  {
    public string Content { get; set; }
    public Guid? LaneGuid { get; set; }
  }
}