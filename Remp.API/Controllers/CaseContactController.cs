using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Remp.Service.DTOs;
using Remp.Service.Interfaces;
using Remp.Service.Services;

namespace Remp.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CaseContactController : ControllerBase
    {
        private readonly ICaseContactService _service;
        public CaseContactController(ICaseContactService service)
        {
            _service=service;
        }

        [Authorize(Roles = "PhotographyCompany")]
        [HttpPost]
        public async Task<IActionResult> CreateCaseContact([FromBody] CreateCaseContactDto dto)
        {
            var result = await _service.CreateCaseContactAsync(dto);
            if (!result.Success)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }

        [Authorize]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetCaseContact([FromRoute] int id)
        {
            var result = await _service.GetAllCaseContact(id);
            if (!result.Success)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }

    }
}