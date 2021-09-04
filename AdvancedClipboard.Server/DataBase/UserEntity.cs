using System;
using System.ComponentModel.DataAnnotations;

namespace AdvancedClipboard.Server.Database
{
  /// <summary>
  /// A user.
  /// </summary>
  public class UserEntity
  {
    #region Properties

    /// <summary>
    /// The id.
    /// </summary>
    [Key]
    public Guid Id { get; set; }

    /// <summary>
    /// The users login name.
    /// </summary>
    [MaxLength(50)]
    public string Login { get; set; }

    /// <summary>
    /// The password of the user.
    /// </summary>
    [MaxLength(32)]
    public string Password { get; set; }

    /// <summary>
    /// The salt of the user.
    /// </summary>
    [MaxLength(32)]
    public string Salt { get; set; }

    #endregion Properties
  }
}