using System;

namespace AdvancedClipboard.Server.Data
{
  public class ClipboardPutData
  {
    #region Properties

    public string FileName { get; set; }
    public Guid Id { get; set; }
    public Guid? LaneId { get; set; }
    public string TextContent { get; set; }

    #endregion Properties
  }
}