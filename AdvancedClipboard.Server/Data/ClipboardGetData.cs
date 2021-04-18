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
    public string FileName { get; private set; }

    #endregion Properties

    #region Methods

    public static ClipboardGetData CreateWithFileContent(Guid id, FileAccessTokenEntity fileToken, string fileName)
    {
      return new ClipboardGetData(id) { FileContentUrl = FileTokenData.CreateUrl(fileToken), ContentTypeId = Constants.ContentTypes.File, FileName = fileName };
    }

    public static ClipboardGetData CreateWithImageContent(Guid id, FileAccessTokenEntity fileToken, string fileName)
    {
      return new ClipboardGetData(id) { FileContentUrl = FileTokenData.CreateUrl(fileToken), ContentTypeId = Constants.ContentTypes.Image, FileName = fileName  };
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
        return CreateWithImageContent(cc.Id, fileToken, cc.DisplayFileName);
      }
      else if (contentType == ContentTypes.PlainText)
      {
        return CreateWithPlainTextContent(cc.Id, cc.TextContent);
      }
      else if (contentType == ContentTypes.File)
      {
        return CreateWithFileContent(cc.Id, fileToken, cc.DisplayFileName);
      }

      throw new Exception("Unexpected Content Type");
    }

    #endregion Methods
  }
}