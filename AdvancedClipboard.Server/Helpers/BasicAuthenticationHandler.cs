using AdvancedClipboard.Server.Database;
using AdvancedClipboard.Server.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace AdvancedClipboard.Server.Helpers
{
  public class BasicAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
  {
    #region Fields

    private readonly IAuthService userService;

    #endregion Fields

    #region Constructors

    public BasicAuthenticationHandler(
        IOptionsMonitor<AuthenticationSchemeOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder,
        ISystemClock clock,
        IAuthService userService)
        : base(options, logger, encoder, clock)
    {
      this.userService = userService;
    }

    #endregion Constructors

    #region Methods

    protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
    {
      // skip authentication if endpoint has [AllowAnonymous] attribute
      var endpoint = Context.GetEndpoint();
      if (endpoint?.Metadata?.GetMetadata<IAllowAnonymous>() != null)
        return AuthenticateResult.NoResult();

      if (!Request.Headers.ContainsKey("Authorization"))
        return AuthenticateResult.Fail("Missing Authorization Header");

      UserEntity user;
      try
      {
        var authHeader = AuthenticationHeaderValue.Parse(Request.Headers["Authorization"]);
        var credentialBytes = Convert.FromBase64String(authHeader.Parameter);
        var credentials = Encoding.UTF8.GetString(credentialBytes).Split(new[] { ':' }, 2);
        var username = credentials[0];
        var password = credentials[1];
        user = await userService.Authenticate(username, password);
      }
      catch
      {
        return AuthenticateResult.Fail("Invalid Authorization Header");
      }

      if (user == null)
        return AuthenticateResult.Fail("Invalid Username or Password");

      var claims = new[] {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Login),
            };
      var identity = new ClaimsIdentity(claims, Scheme.Name);
      var principal = new ClaimsPrincipal(identity);
      var ticket = new AuthenticationTicket(principal, Scheme.Name);

      return AuthenticateResult.Success(ticket);
    }

    #endregion Methods
  }
}