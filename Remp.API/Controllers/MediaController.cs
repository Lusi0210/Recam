using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Remp.Common.Responses;
using Remp.Models.Constants;
using Remp.Service.Interfaces;

namespace Remp.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MediaController : ControllerBase
    {
        private readonly IMediaAssetService _mediaAssetService;

        public MediaController(IMediaAssetService mediaAssetService)
        {
            _mediaAssetService = mediaAssetService;
        }

        /// <summary>
        /// Download a single media file
        /// </summary>
        /// <param name="id">Media asset id</param>
        /// <returns>The file stream</returns>
        [HttpGet("{id:int}/download")]
        [Authorize(Roles = $"{RoleNames.PhotographyCompany},{RoleNames.Agent}")]
        [ProducesResponseType(typeof(FileStreamResult), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DownloadMediaAsset(int id)
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            (Stream content, string contentType, string fileName) = await _mediaAssetService.DownloadMediaAssetAsync(id, userId);
            return File(content, contentType, fileName);
        }
    }
}
