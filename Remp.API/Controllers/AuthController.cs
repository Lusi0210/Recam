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
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }


        /// <summary>
        /// Log in with email and password, returns a JWT token.
        /// </summary>
        /// <param name="dto">The user's email and password.</param>
        /// <returns>A JWT token and basic user info.</returns>
        [HttpPost("login")]
        [ProducesResponseType(typeof(ApiResponse<LoginResponseDto>),StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<ApiResponse<LoginResponseDto>>> Login([FromBody] LoginRequestDto dto)
        {
            string? ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();
            LoginResponseDto result = await _authService.LoginAsync(dto,ipAddress);
            return Ok(ApiResponse<LoginResponseDto>.SuccessResponse(result, "Login successful"));
        }

        /// <summary>
        /// Register a new agent account
        /// </summary>
        /// <param name="dto"></param>
        /// <returns>new agent info</returns>
        [HttpPost("register")]
        [ProducesResponseType(typeof(ApiResponse<RegisterResponseDto>),StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ApiResponse<object>),StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ApiResponse<RegisterResponseDto>>> Register([FromBody] RegisterRequestDto dto)
        {
            string? ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();
            RegisterResponseDto result = await _authService.RegisterAsync(dto,ipAddress);
            return StatusCode(StatusCodes.Status201Created,ApiResponse<RegisterResponseDto>.SuccessResponse(result, "Registration successful"));
        }

        /// <summary>
        /// Update user password
        /// </summary>
        /// <param name="dto"></param>
        /// <returns>User Id and Email</returns>
        [HttpPost("update-password")]
        [Authorize(Roles = $"{RoleNames.PhotographyCompany},{RoleNames.Agent}")]
        [ProducesResponseType(typeof(ApiResponse<ChangePasswordResponseDto>),StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>),StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ApiResponse<object>),StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<object>),StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApiResponse<ChangePasswordResponseDto>>> UpdatePassword(ChangePasswordDto dto)
        {
            string? ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            ChangePasswordResponseDto result = await _authService.ChangePasswordAsync(dto,userId,ipAddress);
            return Ok(ApiResponse<ChangePasswordResponseDto>.SuccessResponse(result,"Update password successful!"));
        }
    }
}
