using System;
using System.Collections.Generic;
using System.Linq;
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

        public UserController(IUserService userService)
        {
            _userSerive=userService;
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
        
    }
}