using AdvancedClipboard.Server.Data;
using AdvancedClipboard.Server.Database;
using AdvancedClipboard.Server.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AdvancedClipboard.Server.Controllers
{
  [Authorize]
  [Route("[controller]")]
  [ApiController]
  public class LaneController : Controller
  {
    private readonly IAuthService authService;

    public LaneController(IAuthService authService)
    {
      this.authService = authService;
    }

    [HttpGet]
    public async Task<IEnumerable<LaneGetData>> Get()
    {
      using var connection = authService.Connection;

      var context = new DatabaseContext(connection);
      var result = await (from lane in context.Lane
                          where lane.UserId == this.authService.UserId
                          select CreateGetDataFromEntity(lane)).ToListAsync();

      await connection.CloseAsync();

      return result;
    }

    private static LaneGetData CreateGetDataFromEntity(LaneEntity lane)
    {
      return new LaneGetData()
      {
        Id = lane.Id,
        Name = lane.Name,
        Color = lane.Color
      };
    }

    [HttpPost("PostLane")]
    public async Task<LaneGetData> Post(LanePostData data)
    {
      using var connection = authService.Connection;

      var entity = new LaneEntity()
      {
        Name = data.Name,
        Color = data.Color,
        UserId = authService.UserId
      };

      var context = new DatabaseContext(connection);
      await context.AddAsync(entity);
      await context.SaveChangesAsync();
      await connection.CloseAsync();

      return CreateGetDataFromEntity(entity);
    }

    [HttpPut("AssignContent")]
    public async Task<ActionResult> AssignContent(AssignContentToLanePutData data)
    {
      using var connection = authService.Connection;

      var context = new DatabaseContext(connection);
      var content = await context.ClipboardContent.FindAsync(data.ContentId);

      if(content.UserId != authService.UserId)
      {
        throw new Exception("Content belongs to another user");
      }

      content.LaneId = data.LaneId;

      await context.SaveChangesAsync();
      await connection.CloseAsync();

      return Ok();
    }
  }
}
