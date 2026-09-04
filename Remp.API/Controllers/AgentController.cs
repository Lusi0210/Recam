using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Remp.Models.Entities;
using Remp.Service.DTOs;
using Remp.Service.Interfaces;

namespace Remp.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AgentController : ControllerBase
    {
        private readonly IAgentService _agentService;

        public AgentController(IAgentService agentService)
        {
            _agentService=agentService;
        }

        [Authorize(Roles = "PhotographyCompany")]
        [HttpGet]
        public async Task<IActionResult> SeachAgentByEmail([FromQuery] string email)
        {
            var result = await _agentService.SearchAgentByEmail(email);
            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

    }
}