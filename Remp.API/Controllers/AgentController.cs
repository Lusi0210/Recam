using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Remp.Common.Responses;
using Remp.Models.Constants;
using Remp.Service.DTOs;
using Remp.Service.Interfaces;

namespace Remp.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AgentController : ControllerBase
    {
        private readonly IAgentService _agentService;
        public AgentController(IAgentService agentService)
        {
            _agentService = agentService;
        }


        /// <summary>
        /// Add agent to photography company
        /// </summary>
        /// <param name="dto"></param>
        /// <returns>agent's id</returns>
        [HttpPost]
        [Authorize(Roles = RoleNames.PhotographyCompany)]
        [ProducesResponseType(typeof(ApiResponse<string>),StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ApiResponse<object>),StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<object>),StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ApiResponse<object>),StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ApiResponse<object>),StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApiResponse<string>>> AddAgentToPhotographyCompany(AddAgentToPhotographyCompanyDto dto)
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            string result = await _agentService.AddAgentToPhotographyCompanyAsync(userId,dto);
            return StatusCode(StatusCodes.Status201Created,ApiResponse<string>.SuccessResponse(result, "Agent added to your company successfully!"));
        }
    }
}
