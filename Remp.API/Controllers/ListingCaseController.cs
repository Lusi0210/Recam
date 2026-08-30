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
            // 从 token 读当前用户 id
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            var result = await _service.CreateAsync(dto, userId!);

            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }
    }
}