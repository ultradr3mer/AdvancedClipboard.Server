using AdvancedClipboard.Server.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;

namespace AdvancedClipboard.Server.Controllers
{
  [Authorize]
  [Route("[controller]")]
  [ApiController]
  public class ClipboardController : ControllerBase
  {
    #region Methods

    [HttpGet]
    public IEnumerable<ClipboardGetData> Get()
    {
      return new ClipboardGetData[] { };
    }

    #endregion Methods
  }
}