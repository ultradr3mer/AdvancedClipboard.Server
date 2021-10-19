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
      using SqlConnection connection = this.authService.Connection;
      await connection.CloseAsync();
      return this.Ok();
    }

    [HttpDelete]
    public async Task<ActionResult> DeleteAsync(Guid Id)
    {
      using SqlConnection connection = this.authService.Connection;

      DatabaseContext context = new DatabaseContext(connection);
      ClipboardContentEntity cc = await context.ClipboardContent.FindAsync(Id);
      cc.IsArchived = true;

      if (cc.UserId != this.authService.UserId)
      {
        return this.BadRequest();
      }

      await context.SaveChangesAsync();
      await connection.CloseAsync();

      return this.Ok();
    }

    [HttpGet]
    [Obsolete("Get is deprecated, please use GetWithContext instead.")]
    public async Task<IEnumerable<ClipboardGetData>> Get(Guid? id = null)
    {
      using var connection = authService.Connection;

      var context = new DatabaseContext(connection);
      var result = await (from cc in context.ClipboardContent
                          where cc.UserId == authService.UserId
                          && cc.IsArchived == false
                          && (cc.Id == id || id == null)
                          select ClipboardGetData.CreateFromEntity(cc, cc.FileToken)).ToListAsync();

      await connection.CloseAsync();

      return result;
    }

    [HttpGet(nameof(GetLane))]
    [Obsolete("GetLane is deprecated, please use GetLaneWithContext instead.")]
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

    [HttpGet(nameof(GetLaneWithContext))]
    public async Task<ClipboardContainerGetData> GetLaneWithContext(Guid lane)
    {
      if (lane == Guid.Empty)
      {
        throw new ArgumentNullException(nameof(lane));
      }
      using SqlConnection connection = this.authService.Connection;

      DatabaseContext context = new DatabaseContext(connection);
      List<ClipboardGetData> entries = await (from cc in context.ClipboardContent
                                              where cc.UserId == this.authService.UserId
                                              && cc.IsArchived == false
                                              && cc.LaneId == lane
                                              select ClipboardGetData.CreateFromEntity(cc, cc.FileToken)).ToListAsync();

      List<LaneGetData> lanes = await LaneController.GetLanesForUser(context, this.authService.UserId);

      ClipboardContainerGetData result = new ClipboardContainerGetData()
      {
        Entries = entries,
        Lanes = lanes
      };

      await connection.CloseAsync();

      return result;
    }

    [HttpGet(nameof(GetWithContext))]
    public async Task<ClipboardContainerGetData> GetWithContext(Guid? id = null)
    {
      using SqlConnection connection = this.authService.Connection;

      DatabaseContext context = new DatabaseContext(connection);
      List<ClipboardGetData> entries = await (from cc in context.ClipboardContent
                                              where cc.UserId == this.authService.UserId
                                              && cc.IsArchived == false
                                              && (cc.Id == id || id == null)
                                              select ClipboardGetData.CreateFromEntity(cc, cc.FileToken)).ToListAsync();

      List<LaneGetData> lanes = await LaneController.GetLanesForUser(context, this.authService.UserId);

      ClipboardContainerGetData result = new ClipboardContainerGetData()
      {
        Entries = entries,
        Lanes = lanes
      };

      await connection.CloseAsync();

      return result;
    }

    [HttpPost("PostFile")]
    public async Task<ClipboardGetData> PostFile(IFormFile file, string fileExtension, Guid? laneId = null)
    {
      return await this.PostFileInternal(file, fileExtension, null, laneId);
    }

    [HttpPost("PostNamedFile")]
    public async Task<ClipboardGetData> PostNamedFile(IFormFile file, string fileName, Guid? laneId = null)
    {
      return await this.PostFileInternal(file, null, fileName, laneId);
    }

    [HttpPost("PostPlainText")]
    public async Task<ClipboardGetData> PostPlainText(ClipboardPostPlainTextData data)
    {
      using SqlConnection connection = this.authService.Connection;

      DateTime now = DateTime.Now;
      ClipboardContentEntity entry = new ClipboardContentEntity()
      {
        ContentTypeId = Constants.ContentTypes.PlainText,
        CreationDate = now,
        LastUsedDate = now,
        TextContent = data.Content,
        UserId = this.authService.UserId,
        LaneId = data.LaneGuid
      };

      DatabaseContext context = new DatabaseContext(connection);
      await context.AddAsync(entry);
      await context.SaveChangesAsync();
      await connection.CloseAsync();

      return ClipboardGetData.CreateWithPlainTextContent(entry.Id, entry.LaneId, entry.TextContent);
    }

    [HttpPut]
    public async Task<IActionResult> Put(ClipboardPutData data)
    {
      using SqlConnection connection = this.authService.Connection;

      DatabaseContext context = new DatabaseContext(connection);
      ClipboardContentEntity cc = await context.ClipboardContent.FindAsync(data.Id);

      if (cc.UserId != this.authService.UserId)
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

    private async Task<ClipboardGetData> PostFileInternal(IFormFile file, string fileExtension, string fileName, Guid? laneId)
    {
      using SqlConnection connection = this.authService.Connection;

      DateTime now = DateTime.Now;
      string extension = (fileExtension ?? Path.GetExtension(fileName) ?? Path.GetExtension(file.FileName));
      string filename = $"clip_{now:yyyyMMdd'_'HHmmss}" + extension;
      FileAccessTokenEntity token = await this.fileRepository.UploadInternal(filename,
                                                                             file.OpenReadStream(),
                                                                             connection,
                                                                             false);

      Guid contentType = this.fileRepository.GetContentTypeForExtension(extension).StartsWith("image") ? Constants.ContentTypes.Image :
                                                                                                   Constants.ContentTypes.File;

      ClipboardContentEntity entry = new ClipboardContentEntity()
      {
        ContentTypeId = contentType,
        CreationDate = now,
        LastUsedDate = now,
        FileTokenId = token.Id,
        UserId = this.authService.UserId,
        DisplayFileName = fileName,
        LaneId = laneId
      };

      using DatabaseContext context = new DatabaseContext(connection);
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