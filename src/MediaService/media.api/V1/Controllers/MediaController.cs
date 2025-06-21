using media.services.V1.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace media.api.V1.Controllers
{
    [ApiController]
    [Route("api/v{version:apiVersion}/media")]
    [ApiVersion("1.0")]
    [Authorize]
    public class MediaController(IMediaService _mediaService) : ControllerBase
    {
        [HttpPost]
        [RequestFormLimits(MultipartBodyLengthLimit = 104857600)] // 100 MB limit
        public async Task<IActionResult> Upload(MultipartFormDataContent content, CancellationToken cancellationToken = default)
        {
            var userId = int.Parse(User.FindFirst("sub")?.Value ?? "0");
            var result = await _mediaService.UploadAsync(content, userId, cancellationToken);
            return result.Success ? Ok(result.Data) : StatusCode(result.StatusCode, new { error = result.ErrorMessage });
        }

        [HttpGet]
        public async Task<IActionResult> GetByIds([FromQuery] int[] mediaIds, CancellationToken cancellationToken = default)
        {
            var userId = int.Parse(User.FindFirst("sub")?.Value ?? "0");
            var result = await _mediaService.GetByIdsAsync(mediaIds, userId, cancellationToken);
            return result.Success ? Ok(result.Data) : StatusCode(result.StatusCode, new { error = result.ErrorMessage });
        }
    }
}
