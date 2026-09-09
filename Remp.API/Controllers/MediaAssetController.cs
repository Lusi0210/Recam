using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Remp.Common;
using Remp.Service.DTOs;
using Remp.Service.Interfaces;

namespace Remp.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MediaAssetController : ControllerBase
    {
        private readonly IMediaAssetsService _service;
        public MediaAssetController(IMediaAssetsService service)
        {
            _service = service;
        }

        [Authorize(Roles = "PhotographyCompany")]
        [HttpPost("upload")]
        public async Task<IActionResult> UploadMediaAssets([FromForm] UploadMediaAssetsDto dto)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var result = await _service.UploadMediaAssetAsync(dto,userId!);
            if (!result.Success)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }

        [Authorize]
        [HttpGet("download/{id}")]
        public async Task<IActionResult> DownloadMediaAsset(int id)
        {
            var result = await _service.DownloadMediaAssetAsync(id);
            if (result == null)
            {
                return NotFound(ApiResponse<string>.FailureResponse("Media file not found."));
            }
            return File(result.Content, result.ContentType, result.FileName);
        }

        [Authorize(Roles = "PhotographyCompany")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMediaAsset([FromRoute]int id)
        {
            var result = await _service.DeleteMediaAssetByIdAsync(id);
            if (!result.Success)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }

    }
}