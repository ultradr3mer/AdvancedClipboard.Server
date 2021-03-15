using AdvancedClipboard.Server.DataBase;
using handshake.Data;

namespace AdvancedClipboard.Server.Data
{
  /// <summary>
  /// The result data of uploading a file.
  /// </summary>
  public class FileUploadResultData
  {
    #region Constructors

    /// <summary>
    /// Creates a new instance of the <see cref="FileUploadResultData"/> class based on the given <see cref="FileAccessTokenEntity"/>.
    /// </summary>
    /// <param name="entity">The <see cref="FileAccessTokenEntity"/> to copy properties from.</param>
    public FileUploadResultData(FileAccessTokenEntity entity)
    {
      this.LocalUrl = FileTokenData.CreateUrl(entity);
    }

    #endregion Constructors

    #region Properties

    /// <summary>
    /// The local url to retrive the file.
    /// </summary>
    public string LocalUrl { get; set; }

    #endregion Properties
  }
}