using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Remp.Common.Responses;
using Remp.Models.Constants;
using Remp.Service.DTOs;
using Remp.Service.Interfaces;

namespace Remp.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DashboardController : ControllerBase
    {
        private readonly IDashboardService _dashboardService;

        public DashboardController(IDashboardService dashboardService)
        {
            _dashboardService = dashboardService;
        }

        /// <summary>
        /// Get listing statistics and latest activities for the current user
        /// </summary>
        /// <returns>Dashboard statistics and latest activities</returns>
        [HttpGet]
        [Authorize(Roles = $"{RoleNames.PhotographyCompany},{RoleNames.Agent}")]
        [ProducesResponseType(typeof(ApiResponse<DashboardResponseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<ApiResponse<DashboardResponseDto>>> GetDashboard()
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            string role = User.FindFirstValue(ClaimTypes.Role)!;
            DashboardResponseDto result = await _dashboardService.GetDashboardAsync(userId, role);
            return Ok(ApiResponse<DashboardResponseDto>.SuccessResponse(result));
        }
    }
}