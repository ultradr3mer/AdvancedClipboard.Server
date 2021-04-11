using System;
using System.ComponentModel.DataAnnotations;

namespace AdvancedClipboard.Server.Database
{
  /// <summary>
  /// A content type.
  /// </summary>
  public class ContentTypeEntity
  {
    #region Properties

    /// <summary>
    /// The id.
    /// </summary>
    [Key]
    public Guid Id { get; set; }

    /// <summary>
    /// The filename.
    /// </summary>
    public string Name { get; set; }

    #endregion Properties
  }
}