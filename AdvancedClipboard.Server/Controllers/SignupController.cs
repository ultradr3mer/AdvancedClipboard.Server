using AdvancedClipboard.Server.Data;
using AdvancedClipboard.Server.DataBase;
using AdvancedClipboard.Server.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;

namespace AdvancedClipboard.Server.Controllers
{
  [AllowAnonymous]
  [Route("[controller]")]
  [ApiController]
  public class SignupController : ControllerBase
  {
    private readonly IAuthService authService;
    private readonly UserSevice userSevice;

    public SignupController(IAuthService authService, UserSevice userSevice)
    {
      this.authService = authService;
      this.userSevice = userSevice;
    }

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

    [HttpGet("Authorize")]
    public async Task<IActionResult> Authorize(string username, string password)
    {
      await this.authService.Authenticate(username, password);
      this.authService.Connection.Close();
      this.authService.Connection.Dispose();
      return this.Ok();
    }

    #endregion Methods
  }
}