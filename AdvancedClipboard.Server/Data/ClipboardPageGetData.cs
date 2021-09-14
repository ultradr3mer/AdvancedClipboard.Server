using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AdvancedClipboard.Server.Data
{
  public class ClipboardPageGetData
  {
    public int PagesCount { get; set; }

    public int PageNumber { get; set; }

    public List<ClipboardGetData> PageContent { get; set; }
  }
}
