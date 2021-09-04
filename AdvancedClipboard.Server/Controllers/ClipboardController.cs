using AdvancedClipboard.Server.Data;
using AdvancedClipboard.Server.Database;
using AdvancedClipboard.Server.Repositories;
using AdvancedClipboard.Server.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace AdvancedClipboard.Server.Controllers
{
  [Authorize]
  [Route("[controller]")]
  [ApiController]
  public class ClipboardController : ControllerBase
  {
    #region Fields

    private readonly IAuthService authService;
    private readonly FileRepository fileRepository;

    #endregion Fields

    #region Constructors

    public ClipboardController(IAuthService authService, FileRepository fileRepository)
    {
      this.authService = authService;
      this.fileRepository = fileRepository;
    }

    #endregion Constructors

    #region Methods

    [HttpGet("Authorize")]
    public async Task<IActionResult> Authorize()
    {
      using var connection = authService.Connection;
      await connection.CloseAsync();
      return this.Ok();
    }

    [HttpDelete]
    public async Task<ActionResult> DeleteAsync(Guid Id)
    {
      using var connection = authService.Connection;

      var context = new DatabaseContext(connection);
      var cc = await context.ClipboardContent.FindAsync(Id);
      cc.IsArchived = true;

      if (cc.UserId != authService.UserId)
      {
        return this.BadRequest();
      }

      await context.SaveChangesAsync();
      await connection.CloseAsync();

      return this.Ok();
    }

    [HttpGet]
    public async Task<IEnumerable<ClipboardGetData>> Get()
    {
      using var connection = authService.Connection;

      var context = new DatabaseContext(connection);
      var result = await (from cc in context.ClipboardContent
                          where cc.UserId == authService.UserId
                          && cc.IsArchived == false
                          select ClipboardGetData.CreateFromEntity(cc, cc.FileToken)).ToListAsync();

      await connection.CloseAsync();

      return result;
    }

    [HttpPut]
    public async Task<IActionResult> Put(ClipboardPutData data)
    {
      using var connection = authService.Connection;

      var context = new DatabaseContext(connection);
      var cc = await context.ClipboardContent.FindAsync(data.Id);

      if (cc.UserId != authService.UserId)
      {
        return this.BadRequest();
      }

      cc.DisplayFileName = data.FileName;
      cc.TextContent = data.TextContent;
      cc.LaneId = data.LaneId;

      await context.SaveChangesAsync();
      await connection.CloseAsync();

      return this.Ok();
    }

    [HttpGet("GetLane")]
    public async Task<IEnumerable<ClipboardGetData>> GetLane(Guid lane)
    {
      using var connection = authService.Connection;

      var context = new DatabaseContext(connection);
      var result = await (from cc in context.ClipboardContent
                          where cc.UserId == authService.UserId
                          && cc.IsArchived == false
                          && cc.LaneId == lane
                          select ClipboardGetData.CreateFromEntity(cc, cc.FileToken)).ToListAsync();

      await connection.CloseAsync();

      return result;
    }

    [HttpPost("PostFile")]
    public async Task<ClipboardGetData> PostFile(IFormFile file, string fileExtension)
    {
      return await this.PostFileInternal(file, fileExtension, null);
    }

    [HttpPost("PostNamedFile")]
    public async Task<ClipboardGetData> PostNamedFile(IFormFile file, string fileName)
    {
      return await this.PostFileInternal(file, null, fileName);
    }

    [HttpPost("PostPlainText")]
    public async Task<ClipboardGetData> PostPlainText(ClipboardPostPlainTextData data)
    {
      using var connection = authService.Connection;

      DateTime now = DateTime.Now;
      var entry = new ClipboardContentEntity()
      {
        ContentTypeId = Constants.ContentTypes.PlainText,
        CreationDate = now,
        LastUsedDate = now,
        TextContent = data.Content,
        UserId = authService.UserId
      };

      var context = new DatabaseContext(connection);
      await context.AddAsync(entry);
      await context.SaveChangesAsync();
      await connection.CloseAsync();

      return ClipboardGetData.CreateWithPlainTextContent(entry.Id, entry.LaneId, entry.TextContent);
    }

    private async Task<ClipboardGetData> PostFileInternal(IFormFile file, string fileExtension, string fileName)
    {
      using SqlConnection connection = this.authService.Connection;

      DateTime now = DateTime.Now;
      string extension = (fileExtension ?? Path.GetExtension(fileName) ?? Path.GetExtension(file.FileName));
      string filename = $"clip_{now:yyyyMMdd'_'HHmmss}" + extension;
      FileAccessTokenEntity token = await this.fileRepository.UploadInternal(filename,
                                                                             file.OpenReadStream(),
                                                                             connection,
                                                                             false);

      var contentType = this.fileRepository.GetContentTypeForExtension(extension).StartsWith("image") ? Constants.ContentTypes.Image :
                                                                                                   Constants.ContentTypes.File;

      var entry = new ClipboardContentEntity()
      {
        ContentTypeId = contentType,
        CreationDate = now,
        LastUsedDate = now,
        FileTokenId = token.Id,
        UserId = authService.UserId,
        DisplayFileName = fileName
      };

      using var context = new DatabaseContext(connection);
      await context.AddAsync(entry);
      await context.SaveChangesAsync();
      await connection.CloseAsync();

      connection.Close();

      return contentType == Constants.ContentTypes.Image ? ClipboardGetData.CreateWithImageContent(entry.Id, entry.LaneId, token, fileName) :
                                                           ClipboardGetData.CreateWithFileContent(entry.Id, entry.LaneId, token, fileName);
    }

    #endregion Methods
  }
}