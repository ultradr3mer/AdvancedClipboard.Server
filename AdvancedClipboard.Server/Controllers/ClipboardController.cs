using AdvancedClipboard.Server.Data;
using AdvancedClipboard.Server.DataBase;
using AdvancedClipboard.Server.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AdvancedClipboard.Server.Controllers
{
  [Authorize]
  [Route("[controller]")]
  [ApiController]
  public class ClipboardController : ControllerBase
  {
    private readonly IAuthService authService;

    public ClipboardController(IAuthService authService)
    {
      this.authService = authService;
    }

    #region Methods

    [HttpGet]
    public IEnumerable<ClipboardGetData> Get()
    {
      using var connection = authService.Connection;

      var context = new DatabaseContext(connection);
      var result = (from cc in context.ClipboardContent
                    select new ClipboardGetData()
                    {
                      Id = cc.Id,
                      PlainTextContent = cc.TextContent
                    }).ToList();

      return result;
    }

    [HttpPost("PostPlainText")]
    public async Task<ClipboardPostResultData> PostPlainText(ClipboardPostPlainTextData data)
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

      return new ClipboardPostResultData()
      {
        Id = entry.Id
      };
    }

    #endregion Methods
  }
}