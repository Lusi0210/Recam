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


        /// <summary>
        /// Get all listing cases under the account
        /// </summary>
        /// <param name="query"></param>
        /// <returns>list of listing cases</returns>
        [Authorize(Roles = $"{RoleNames.PhotographyCompany},{RoleNames.Agent}")]
        [HttpGet]
        [ProducesResponseType(typeof(ApiResponse<PagedResult<GetListingCaseResponseDto>>),StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>),StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<object>),StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ApiResponse<object>),StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<ApiResponse<PagedResult<GetListingCaseResponseDto>>>> GetAllListingCase([FromQuery] PaginationQueryDto query)
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            string role = User.FindFirstValue(ClaimTypes.Role)!;
            PagedResult<GetListingCaseResponseDto> result = await _listingCaseService.GetAllListingCaseAsync(role,userId,query);
            return Ok(ApiResponse<PagedResult<GetListingCaseResponseDto>>.SuccessResponse(result));
        }



        /// <summary>
        /// Update exist listing case details by Id
        /// </summary>
        /// <param name="dto"></param>
        /// <param name="id"></param>
        /// <returns>Updated listing case</returns>
        [Authorize(Roles = RoleNames.PhotographyCompany)]
        [HttpPut("{id:int}")]
        [ProducesResponseType(typeof(ApiResponse<UpdateListingCaseResponseDto>),StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>),StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<object>),StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ApiResponse<object>),StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ApiResponse<object>),StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApiResponse<UpdateListingCaseResponseDto>>> UpdateListingCase(UpdateListingCaseDto dto,int id)
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            string? ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();
            UpdateListingCaseResponseDto result =  await _listingCaseService.UpdateListingCaseAsync(id,dto,userId,ipAddress);
            return Ok(ApiResponse<UpdateListingCaseResponseDto>.SuccessResponse(result,"Update listing case successful!"));
        }
    }

}
