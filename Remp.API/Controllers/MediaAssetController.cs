using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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
    }
}