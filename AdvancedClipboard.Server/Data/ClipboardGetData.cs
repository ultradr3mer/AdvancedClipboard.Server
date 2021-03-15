using AdvancedClipboard.Server.Constants;
using AdvancedClipboard.Server.DataBase;
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
    public Guid Id { get; }
    public string ImageContentUrl { get; private set; }
    public string TextContent { get; private set; }

    #endregion Properties

    #region Methods

    public static ClipboardGetData CreateWithImageContent(Guid id, string imageUrl)
    {
      return new ClipboardGetData(id) { ImageContentUrl = imageUrl, ContentTypeId = Constants.ContentTypes.Image, };
    }

    public static ClipboardGetData CreateWithImageContent(Guid id, FileAccessTokenEntity imageToken)
    {
      return CreateWithImageContent(id, FileTokenData.CreateUrl(imageToken));
    }

    public static ClipboardGetData CreateWithPlainTextContent(Guid id, string text)
    {
      return new ClipboardGetData(id) { TextContent = text, ContentTypeId = Constants.ContentTypes.PlainText, };
    }

    internal static ClipboardGetData CreateFromEntity(ClipboardContentEntity cc, FileAccessTokenEntity imageToken)
    {
      var contentType = cc.ContentType?.Id ?? cc.ContentTypeId;

      if (contentType == ContentTypes.Image)
      {
        return CreateWithImageContent(cc.Id, imageToken);
      }
      else if (contentType == ContentTypes.PlainText)
      {
        return CreateWithPlainTextContent(cc.Id, cc.TextContent);
      }

      throw new Exception("Unexpected Content Type");
    }

    #endregion Methods
  }
}