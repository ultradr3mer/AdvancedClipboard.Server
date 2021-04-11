using AdvancedClipboard.Server.Constants;
using AdvancedClipboard.Server.Database;
using handshake.Data;
using System;

namespace AdvancedClipboard.Server.Data
{
  public class ClipboardGetData
  {
    #region Constructors

    private ClipboardGetData(Guid id)
    {
      this.Id = id;
    }

    #endregion Constructors

    #region Properties

    public Guid ContentTypeId { get; private set; }
    public string FileContentUrl { get; private set; }
    public Guid Id { get; }
    public string TextContent { get; private set; }

    #endregion Properties

    #region Methods

    public static ClipboardGetData CreateWithFileContent(Guid id, FileAccessTokenEntity fileToken)
    {
      return new ClipboardGetData(id) { FileContentUrl = FileTokenData.CreateUrl(fileToken), ContentTypeId = Constants.ContentTypes.File, };
    }

    public static ClipboardGetData CreateWithImageContent(Guid id, FileAccessTokenEntity fileToken)
    {
      return new ClipboardGetData(id) { FileContentUrl = FileTokenData.CreateUrl(fileToken), ContentTypeId = Constants.ContentTypes.Image, };
    }

    public static ClipboardGetData CreateWithPlainTextContent(Guid id, string text)
    {
      return new ClipboardGetData(id) { TextContent = text, ContentTypeId = Constants.ContentTypes.PlainText, };
    }

    internal static ClipboardGetData CreateFromEntity(ClipboardContentEntity cc, FileAccessTokenEntity fileToken)
    {
      var contentType = cc.ContentType?.Id ?? cc.ContentTypeId;

      if (contentType == ContentTypes.Image)
      {
        return CreateWithImageContent(cc.Id, fileToken);
      }
      else if (contentType == ContentTypes.PlainText)
      {
        return CreateWithPlainTextContent(cc.Id, cc.TextContent);
      }
      else if (contentType == ContentTypes.File)
      {
        return CreateWithImageContent(cc.Id, fileToken);
      }

      throw new Exception("Unexpected Content Type");
    }

    #endregion Methods
  }
}