using AdvancedClipboard.Server.Repositories;
using AdvancedClipboard.Server.Services;
using Azure.Storage.Blobs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Globalization;
using System.IO;
using System.Threading.Tasks;

namespace AdvancedClipboard.Server.Controllers
{
  /// <summary>
  /// The <see cref="FileController"/> provides functionality to upload and download files.
  /// </summary>
  [Route("[controller]")]
  [Authorize]
  [ApiController]
  public class FileController : ControllerBase
  {
    #region Fields

    private readonly FileRepository fileRepository;

    #endregion Fields

    #region Constructors

    /// <summary>
    /// Creates a new instance of the <see cref="FileController"/> class.
    /// </summary>
    /// <param name="fileRepository">The file repository.</param>
    public FileController(FileRepository fileRepository)
    {
      this.fileRepository = fileRepository;
    }

    #endregion Constructors

    #region Methods

    /// <summary>
    /// Gets an image.
    /// </summary>
    /// <param name="token">The access token of the image to get.</param>
    /// <param name="filename">The filename of the image to get.</param>
    /// <returns>The detailed post information.</returns>
    [AllowAnonymous]
    [HttpGet]
    [Route("{token}/{filename}")]
    public async Task<IActionResult> Get(string token, string filename)
    {
      if (!long.TryParse(token, NumberStyles.HexNumber, null, out long actualToken))
      {
        throw new ArgumentException("Invalid Token.", nameof(token));
      }

      string contentType = this.fileRepository.GetContentTypeForExtension(Path.GetExtension(filename));

      BlobContainerClient azureContainer = await this.fileRepository.GetFile(actualToken, filename);
      BlobClient blob = azureContainer.GetBlobClient(filename);
      Azure.Response<Azure.Storage.Blobs.Models.BlobDownloadInfo> info = await blob.DownloadAsync();
      return this.File(info.Value.Content, contentType);
    }

    #endregion Methods
  }
}