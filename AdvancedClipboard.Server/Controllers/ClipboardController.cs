using AdvancedClipboard.Server.Data;
using AdvancedClipboard.Server.Database;
using AdvancedClipboard.Server.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using System.Data.SqlClient;
using AdvancedClipboard.Server.Repositories;
using System.IO;
using handshake.Data;

namespace AdvancedClipboard.Server.Controllers
{
  [Authorize]
  [Route("[controller]")]
  [ApiController]
  public class ClipboardController : ControllerBase
  {
    private readonly IAuthService authService;
    private readonly FileRepository fileRepository;

    public ClipboardController(IAuthService authService, FileRepository fileRepository)
    {
      this.authService = authService;
      this.fileRepository = fileRepository;
    }

    #region Methods

    [HttpGet]
    public async Task<IEnumerable<ClipboardGetData>> Get()
    {
      using var connection = authService.Connection;

      var context = new DatabaseContext(connection);
      var result = await (from cc in context.ClipboardContent
                          where cc.UserId == authService.UserId
                          && cc.IsArchived == false
                          select ClipboardGetData.CreateFromEntity(cc, cc.ImageToken)).ToListAsync();

      await connection.CloseAsync();

      return result;
    }

    [HttpDelete]
    public async Task<ActionResult> DeleteAsync(Guid Id)
    {
      using var connection = authService.Connection;

      var context = new DatabaseContext(connection);
      var cc = await context.ClipboardContent.FindAsync(Id);
      cc.IsArchived = true;
      await context.SaveChangesAsync();

      await connection.CloseAsync();

      return this.Ok();
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

      return ClipboardGetData.CreateWithPlainTextContent(entry.Id, entry.TextContent);
    }

    [HttpGet("Authorize")]
    public async Task<IActionResult> Authorize()
    {
      using var connection = authService.Connection;
      await connection.CloseAsync();
      return this.Ok();
    }

    [HttpPost("PostFile")]
    public async Task<ClipboardGetData> PostFile(IFormFile file, string fileExtension)
    {
      return await this.PostImage(file, fileExtension);
    }

    [HttpPost("PostImage")]
    public async Task<ClipboardGetData> PostImage(IFormFile file, string fileExtension)
    {
      using SqlConnection connection = this.authService.Connection;

      DateTime now = DateTime.Now;
      string filename = $"clip_{now:yyyyMMdd'_'HHmmss}" + (fileExtension ?? Path.GetExtension(file.FileName));
      FileAccessTokenEntity token = await this.fileRepository.UploadInternal(filename,
                                                                             file.OpenReadStream(),
                                                                             connection,
                                                                             false);

      var entry = new ClipboardContentEntity()
      {
        ContentTypeId = Constants.ContentTypes.Image,
        CreationDate = now,
        LastUsedDate = now,
        ImageTokenId = token.Id,
        UserId = authService.UserId
      };

      using var context = new DatabaseContext(connection);
      await context.AddAsync(entry);
      await context.SaveChangesAsync();
      await connection.CloseAsync();

      connection.Close();

      return ClipboardGetData.CreateWithImageContent(entry.Id, token);
    }


    #endregion Methods
  }
}