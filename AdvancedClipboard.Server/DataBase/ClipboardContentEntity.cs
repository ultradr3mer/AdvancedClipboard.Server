using System;
using System.ComponentModel.DataAnnotations;

namespace AdvancedClipboard.Server.DataBase
{
  /// <summary>
  /// A clipboard content.
  /// </summary>
  public class ClipboardContentEntity
  {
    #region Properties

    /// <summary>
    /// The id.
    /// </summary>
    [Key]
    public Guid Id { get; set; }

    /// <summary>
    /// The date this content was created.
    /// </summary>
    public DateTime CreationDate { get; set; }

    /// <summary>
    /// The last used date.
    /// </summary>
    public DateTime LastUsedDate { get; set; }

    /// <summary>
    /// The content type id.
    /// </summary>
    public short ContentTypeId { get; set; }

    /// <summary>
    /// The content type.
    /// </summary>
    public ContentTypeEntity ContentType { get; set; }

    /// <summary>
    /// The text content.
    /// </summary>
    public string TextContent { get; set; }

    /// <summary>
    /// The image token id off the image content.
    /// </summary>
    public Guid ImageTokenId { get; set; }

    /// <summary>
    /// The id off the user.
    /// </summary>
    public Guid UserId { get; set; }

    #endregion Properties
  }
}