using AdvancedClipboard.Server.Data;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;

namespace AdvancedClipboard.Server.Controllers
{
  [Route("[controller]")]
  [ApiController]
  public class ClipboardController : ControllerBase
  {
    #region Methods

    [HttpGet]
    public IEnumerable<ClipboardGetData> Get()
    {
      throw new Exception();
    }

    #endregion Methods
  }
}