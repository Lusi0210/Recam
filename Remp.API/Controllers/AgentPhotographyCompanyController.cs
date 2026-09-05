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
    public class AgentPhotographyCompanyController : ControllerBase
    {
        private readonly IAgentPhotographyCompanyService _service;
        public AgentPhotographyCompanyController(IAgentPhotographyCompanyService service)
        {
            _service=service;
        }

        [Authorize(Roles = "PhotographyCompany")]
        [HttpPost]
        public async Task<IActionResult> AddAgentToPhotographCompany([FromBody] AddAgentToPhotographyCompanyDto dto)
        {
            var result = await _service.AddAgentToPhotographyCompany(dto);
            if (!result.Success)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }

        [Authorize(Roles = "PhotographyCompany")]
        [HttpGet]
        public async Task<IActionResult> GetAgentsByPhotographyCompany()
        {
            var companyId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var result = await _service.GetAgentByPhotographyCompanyAsync(companyId!);
            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }


    }
}