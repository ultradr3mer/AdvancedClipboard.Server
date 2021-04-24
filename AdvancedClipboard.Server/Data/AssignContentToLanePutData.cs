using System;

namespace AdvancedClipboard.Server.Data
{
  public class AssignContentToLanePutData
  {
    #region Properties

    public Guid ContentId { get; set; }
    public Guid LaneId { get; set; }

    #endregion Properties
  }
}