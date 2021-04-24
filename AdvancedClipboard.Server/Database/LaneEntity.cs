using System;
using System.ComponentModel.DataAnnotations;

namespace AdvancedClipboard.Server.Database
{
  /// <summary>
  /// A clipboard lane.
  /// </summary>
  public class LaneEntity
  {
    #region Properties

    /// <summary>
    /// The color.
    /// </summary>
    public string Color { get; set; }

    /// <summary>
    /// The id.
    /// </summary>
    [Key]
    public Guid Id { get; set; }

    /// <summary>
    /// The name.
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// The owner.
    /// </summary>
    public UserEntity User { get; set; }

    /// <summary>
    /// The owners id.
    /// </summary>
    public Guid UserId { get; set; }

    #endregion Properties
  }
}