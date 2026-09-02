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

        public ListingCaseController(IListingCaseService service)
        {
            _service = service;
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

    }
}