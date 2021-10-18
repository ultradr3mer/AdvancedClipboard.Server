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
    #region Fields

    private readonly IAuthService authService;

    #endregion Fields

    #region Constructors

    public LaneController(IAuthService authService)
    {
      this.authService = authService;
    }

    #endregion Constructors

    #region Methods

    public static async Task<List<LaneGetData>> GetLanesForUser(DatabaseContext context, Guid userId)
    {
      return await (from lane in context.Lane
                    where lane.UserId == userId
                    select CreateGetDataFromEntity(lane)).ToListAsync();
    }

    [HttpPut("AssignContent")]
    public async Task<ActionResult> AssignContent(AssignContentToLanePutData data)
    {
      using System.Data.SqlClient.SqlConnection connection = this.authService.Connection;

      DatabaseContext context = new DatabaseContext(connection);
      ClipboardContentEntity content = await context.ClipboardContent.FindAsync(data.ContentId);

      if (content.UserId != this.authService.UserId)
      {
        throw new Exception("Content belongs to another user");
      }

      content.LaneId = data.LaneId;

      await context.SaveChangesAsync();
      await connection.CloseAsync();

      return this.Ok();
    }

    [HttpDelete]
    public async Task<ActionResult> Delete(Guid laneId)
    {
      using System.Data.SqlClient.SqlConnection connection = this.authService.Connection;

      DatabaseContext context = new DatabaseContext(connection);
      LaneEntity lane = await context.Lane.FindAsync(laneId);

      if (lane.UserId != this.authService.UserId)
      {
        throw new Exception("Content belongs to another user");
      }

      context.Remove(lane);

      await context.SaveChangesAsync();
      await connection.CloseAsync();

      return this.Ok();
    }

    [HttpGet]
    public async Task<IEnumerable<LaneGetData>> Get()
    {
      using System.Data.SqlClient.SqlConnection connection = this.authService.Connection;

      DatabaseContext context = new DatabaseContext(connection);
      List<LaneGetData> result = await GetLanesForUser(context, this.authService.UserId);

      await connection.CloseAsync();

      return result;
    }

    [HttpPost("PostLane")]
    public async Task<LaneGetData> Post(LanePostData data)
    {
      using System.Data.SqlClient.SqlConnection connection = this.authService.Connection;

      LaneEntity entity = new LaneEntity()
      {
        Name = data.Name,
        Color = data.Color,
        UserId = this.authService.UserId
      };

      DatabaseContext context = new DatabaseContext(connection);
      await context.AddAsync(entity);
      await context.SaveChangesAsync();
      await connection.CloseAsync();

      return CreateGetDataFromEntity(entity);
    }

    [HttpPut]
    public async Task<ActionResult> Put(LanePutData data)
    {
      using System.Data.SqlClient.SqlConnection connection = this.authService.Connection;

      DatabaseContext context = new DatabaseContext(connection);
      LaneEntity lane = await context.Lane.FindAsync(data.Id);

      if (lane.UserId != this.authService.UserId)
      {
        throw new Exception("Content belongs to another user");
      }

      lane.Name = data.Name;
      lane.Color = data.Color;

      await context.SaveChangesAsync();
      await connection.CloseAsync();

      return this.Ok();
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

    #endregion Methods
  }
}