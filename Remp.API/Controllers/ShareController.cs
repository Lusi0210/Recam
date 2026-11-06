using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Remp.Common.Responses;
using Remp.Service.DTOs;
using Remp.Service.Interfaces;

namespace Remp.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ShareController : ControllerBase
    {

        private readonly IListingCaseService _listingcaseService;
        public ShareController(IListingCaseService listingCaseService)
        {
            _listingcaseService = listingCaseService;
        }
        [HttpGet("{token}")]
        [AllowAnonymous]
        public async Task<ActionResult<ApiResponse<GetListingCaseByURLResponseDto>>> GetSharedListingCase(string token)
        {
            GetListingCaseByURLResponseDto result = await _listingcaseService.GetListingCaseByTokenAsync(token);
            return Ok(ApiResponse<GetListingCaseByURLResponseDto>.SuccessResponse(result));
        }
    }
}
