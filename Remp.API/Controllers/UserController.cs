using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Remp.Service.Interfaces;
using Remp.Service.Services;

namespace Remp.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userSerive;
        private readonly IListingCaseService _listingCaseService;

        public UserController(IUserService userService,IListingCaseService listingCaseService)
        {
            _userSerive=userService;
            _listingCaseService=listingCaseService;
        }

        [HttpGet("GetAllUser")]
        [Authorize(Roles = "PhotographyCompany")]
        public async Task<IActionResult> GetAllUsers([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            var result = await _userSerive.GetAllUsersAsync(pageNumber,pageSize);

            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

        [Authorize]
        [HttpGet("me")]
        public async Task<IActionResult> GetCurrentUserInfo()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var role = User.FindFirst(ClaimTypes.Role)?.Value;
        
            var result = await _listingCaseService.GetCurrentUserInfoAsync(userId!, role!);
            if (!result.Success)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }
        
    }
}