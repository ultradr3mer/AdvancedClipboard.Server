using System;
using System.ComponentModel.DataAnnotations;

namespace AdvancedClipboard.Server.Database
{
  /// <summary>
  /// A clipboard content.
  /// </summary>
  public class ClipboardContentEntity
  {
    #region Properties

    /// <summary>
    /// The content type.
    /// </summary>
    public ContentTypeEntity ContentType { get; set; }

    /// <summary>
    /// The content type id.
    /// </summary>
    public Guid ContentTypeId { get; set; }

    /// <summary>
    /// The date this content was created.
    /// </summary>
    public DateTime CreationDate { get; set; }

    /// <summary>
    /// Filename to display.
    /// </summary>
    public string DisplayFileName { get; set; }

    /// <summary>
    /// The image token off the image content.
    /// </summary>
    public FileAccessTokenEntity FileToken { get; set; }

    /// <summary>
    /// The token id off the file content.
    /// </summary>
    public Guid? FileTokenId { get; set; }

    /// <summary>
    /// The id.
    /// </summary>
    [Key]
    public Guid Id { get; set; }

    /// <summary>
    /// If this content is archived.
    /// </summary>
    public bool IsArchived { get; set; }

    /// <summary>
    /// The Lane.
    /// </summary>
    public LaneEntity Lane { get; set; }

    /// <summary>
    /// The Lane Id.
    /// </summary>
    public Guid? LaneId { get; set; }

    /// <summary>
    /// The last used date.
    /// </summary>
    public DateTime LastUsedDate { get; set; }

    /// <summary>
    /// The text content.
    /// </summary>
    public string TextContent { get; set; }

    /// <summary>
    /// The id off the user.
    /// </summary>
    public Guid UserId { get; set; }

    #endregion Properties
  }
}