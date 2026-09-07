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
    public class ListingCaseController : ControllerBase
    {
        private readonly IListingCaseService _service;
        private readonly IMediaAssetsService _mediaAssetsService;

        public ListingCaseController(IListingCaseService service,IMediaAssetsService mediaAssetsService)
        {
            _service = service;
            _mediaAssetsService=mediaAssetsService;
        }

        [Authorize(Roles = "PhotographyCompany")]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateListingCaseDto dto)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            var result = await _service.CreateAsync(dto, userId!);

            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var role = User.FindFirst(ClaimTypes.Role)?.Value;

            ApiResponse<List<GetAllListingCaseResponseDto>> result;
            if (role == "PhotographyCompany")
            {
                result =await _service.GetByPhotographyCompanyAsync(userId!);
            }
            else   // Agent
            {
                result = await _service.GetByAgentAsync(userId!);
            }

            if(!result.Success)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }

        [Authorize(Roles="PhotographyCompany")]
        [HttpPost("add-agent")]
        public async Task<IActionResult> AddAgentToListingCase([FromBody] AddAgentToListingCaseDto dto)
        {
            var result = await _service.AddAgentToListingCaseAsync(dto);
            if (!result.Success)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }

        [Authorize(Roles = "PhotographyCompany")]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateListingCase([FromRoute] int id, [FromBody] UpdateListingCaseDto dto)
        {
            var result = await _service.UpdateListingCaseAsync(id,dto);
            if (!result.Success)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }

        [Authorize(Roles = "PhotographyCompany")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteListingCase([FromRoute] int id)
        {
            var result = await _service.DeleteListingCaseAsync(id);
            if (!result.Success)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }

        [Authorize]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetListingCaseDetailsById([FromRoute] int id)
        {
            var result =await _service.GetListingCaseDetailsById(id);
            if (!result.Success)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }
        [Authorize(Roles = "PhotographyCompany")]
        [HttpPatch("{id}/status")]
        public async Task<IActionResult> ChangeListingCaseStatusAsync([FromRoute] int id)
        {
            var result = await _service.ChangeListingCaseStatusAsync(id);
            if (!result.Success)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }

        [Authorize(Roles = "PhotographyCompany")]
        [HttpPatch("{id}/publish")]
        public async Task<IActionResult> Publish([FromRoute] int id)
        {
            var result = await _service.GenerateShareableLinkAsync(id);
            if (!result.Success) return BadRequest(result);
            return Ok(result);
        }

        [Authorize]
        [HttpGet("{id}/download")]
        public async Task<IActionResult> DownloadListingCaseAsZip(int id)
        {
            var result = await _mediaAssetsService.DownloadListingCaseAsZipAsync(id);
            if (result == null)
            {
                return NotFound(ApiResponse<string>.FailureResponse("No media files found for this listing case."));
            }
            return File(result.Content, result.ContentType, result.FileName); 
        }

    }
}