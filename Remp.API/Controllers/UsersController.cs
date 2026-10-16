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
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;
        public UsersController(IUserService userService)
        {
            _userService = userService;
        }


        [HttpGet]
        [Authorize(Roles = RoleNames.PhotographyCompany)]
        [ProducesResponseType(typeof(ApiResponse<PagedResult<UserDto>>),StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>),StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<ApiResponse<PagedResult<UserDto>>>> GetAllUser([FromQuery]PaginationQueryDto query)
        {
            PagedResult<UserDto> result = await _userService.GetAllUsersAsync(query);
            return Ok(ApiResponse<PagedResult<UserDto>>.SuccessResponse(result));
        }

        [HttpGet("me")]
        [Authorize(Roles = $"{RoleNames.PhotographyCompany},{RoleNames.Agent}")]
        [ProducesResponseType(typeof(ApiResponse<GetUserResponseDto>),StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>),StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<ApiResponse<GetUserResponseDto>>> GetCurrnetUserInformation()
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            string role = User.FindFirstValue(ClaimTypes.Role)!;
            GetUserResponseDto result = await _userService.GetCurrnetUserInformationAsync(userId,role);
            return Ok(ApiResponse<GetUserResponseDto>.SuccessResponse(result));
        }
    }
}
