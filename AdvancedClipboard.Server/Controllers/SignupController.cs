﻿using AdvancedClipboard.Server.Data;
using AdvancedClipboard.Server.Database;
using AdvancedClipboard.Server.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace AdvancedClipboard.Server.Controllers
{
  [AllowAnonymous]
  [Route("[controller]")]
  [ApiController]
  public class SignupController : ControllerBase
  {
    #region Fields

    private readonly IAuthService authService;
    private readonly UserSevice userSevice;

    #endregion Fields

    #region Constructors

    public SignupController(IAuthService authService, UserSevice userSevice)
    {
      this.authService = authService;
      this.userSevice = userSevice;
    }

    #endregion Constructors

    #region Methods

    [HttpPost]
    public async Task<IActionResult> PostAsync(SignupPostData signupPostData)
    {
      signupPostData = new SignupPostData()
      {
        Password = signupPostData.Password.Trim(),
        Username = signupPostData.Username.Trim()
      };

      using var connection = await this.authService.Connect();
      var context = new DatabaseContext(connection);
      bool loginExists = await context.ClipboardUser.AnyAsync(o => o.Login == signupPostData.Username);
      if (loginExists)
      {
        return this.BadRequest("Username already existent.");
      }

      await this.userSevice.CreateUserAsync(signupPostData, context);
      await connection.CloseAsync();

      return this.Ok();
    }

    [HttpGet("TestAuthorize")]
    public async Task<IActionResult> TestAuthorize(string username, string password)
    {
      await this.authService.Authenticate(username, password);
      this.authService.Connection.Close();
      this.authService.Connection.Dispose();
      return this.Ok();
    }

    #endregion Methods
  }
}