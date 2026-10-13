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
    public class ListingCaseController : ControllerBase
    {
        private readonly IListingCaseService _listingCaseService;

        public ListingCaseController(IListingCaseService listingCaseService)
        {
            _listingCaseService = listingCaseService;
        }


        /// <summary>
        /// Create a new listingcase
        /// </summary>
        /// <param name="dto"></param>
        /// <returns>New listingcase id</returns>
        [Authorize(Roles = RoleNames.PhotographyCompany)]
        [HttpPost]
        [ProducesResponseType(typeof(ApiResponse<int>),StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ApiResponse<object>),StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<object>),StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ApiResponse<object>),StatusCodes.Status403Forbidden)]
        
        public async Task<ActionResult<ApiResponse<int>>> CreateListingCase(CreateListingCaseDto dto)
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            string? ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();
            int listingCaseId = await _listingCaseService.CreateListingCaseAsync(dto,userId,ipAddress);
            return StatusCode(StatusCodes.Status201Created,ApiResponse<int>.SuccessResponse(listingCaseId,"Create listing case successful!"));
        }
    }
}
