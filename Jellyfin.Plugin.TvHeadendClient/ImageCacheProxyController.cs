using System.ComponentModel.DataAnnotations;
using System.Net;
using Jellyfin.Plugin.TvHeadendClient.Helpers;
using Jellyfin.Plugin.TvHeadendClient.TVHeadendApiClient;
using MediaBrowser.Model.Drawing;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Jellyfin.Plugin.TvHeadendClient;

[ApiController]
[Route($"{PluginInfo.IdString}/imagecache")]
public class ImageCacheProxyController(
    ILogger<ImageCacheProxyController> logger,
    ITvHeadendApiClient tvHeadendApiClient) : ControllerBase
{
    [HttpGet("{id}")]
    public async Task<ActionResult> ProxyImageCacheRequest([FromRoute] [Required] int id)
    {
        logger.LogInformation("ProxyImageCacheRequest: Request received for id {Id}", id);

        try
        {
            var imageBytes = await tvHeadendApiClient
                .GetImageAsync(Plugin.ConnectionInfo, $"imagecache/{id}")
                .ConfigureAwait(false);

            if (imageBytes is null || imageBytes.Length == 0) return NotFound();

            return File(imageBytes,
                ImageUtilities.TryGetImageType(imageBytes, out var imageFormat)
                    ? imageFormat.GetMimeType()
                    : "application/octet-stream");
        }
        catch (HttpRequestException ex) when (ex.StatusCode is HttpStatusCode.NotFound)
        {
            return NotFound();
        }
        catch (HttpRequestException ex) when (ex.StatusCode.HasValue)
        {
            return Problem(statusCode: (int)ex.StatusCode.Value);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unexpected error while proxying image request for id {Id}", id);
            return Problem();
        }
    }
}