using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Remp.Common.Responses;
using Remp.Models.Constants;
using Remp.Models.Enums;
using Remp.Service.DTOs;
using Remp.Service.Interfaces;

namespace Remp.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ListingCaseController : ControllerBase
    {
        private readonly IListingCaseService _listingCaseService;
        private readonly ICaseContactService _caseContactService;
        private readonly IMediaAssetService _mediaAssetService;

        public ListingCaseController(
            IListingCaseService listingCaseService,
            ICaseContactService caseContactService,
            IMediaAssetService mediaAssetService)
        {
            _listingCaseService = listingCaseService;
            _caseContactService = caseContactService;
            _mediaAssetService = mediaAssetService;
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

        /// <summary>
        /// Delete the listing case by Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize(Roles = RoleNames.PhotographyCompany)]
        [HttpDelete("{id:int}")]
        [ProducesResponseType(typeof(ApiResponse<object>),StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>),StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ApiResponse<object>),StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ApiResponse<object>),StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApiResponse<object>>> DeleteListingCase(int id)
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            string? ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();
            await _listingCaseService.DeleteListingCaseAsync(id,userId,ipAddress);
            return Ok(ApiResponse<object>.SuccessResponse(null,"Delete listing case successful!"));
        }


        /// <summary>
        /// Get listing case details by Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Listing case details</returns>
        [ProducesResponseType(typeof(ApiResponse<GetListingCaseDetailsResponseDto>),StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>),StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ApiResponse<object>),StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ApiResponse<object>),StatusCodes.Status404NotFound)]
        [HttpGet("{id:int}")]
        [Authorize(Roles = $"{RoleNames.PhotographyCompany},{RoleNames.Agent}")]
        public async Task<ActionResult<ApiResponse<GetListingCaseDetailsResponseDto>>> GetListingCaseDetails(int id)
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            GetListingCaseDetailsResponseDto result = await _listingCaseService.GetListingCaseDetailsByIdAsync(id,userId);
            return Ok(ApiResponse<GetListingCaseDetailsResponseDto>.SuccessResponse(result));
        }

        /// <summary>
        /// Update listing case status
        /// </summary>
        /// <param name="id"></param>
        /// <param name="dto"></param>
        /// <returns>previous status and current status</returns>
        [HttpPatch("{id:int}/status")]
        [Authorize(Roles = RoleNames.PhotographyCompany)]
        [ProducesResponseType(typeof(ApiResponse<UpdateListingCaseStatusResponseDto>),StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<object>),StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ApiResponse<object>),StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ApiResponse<object>),StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApiResponse<UpdateListingCaseStatusResponseDto>>> UpdateListingCaseStatus(int id,UpdateListingCaseStatusDto dto)
        {
            string? ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            UpdateListingCaseStatusResponseDto result = await _listingCaseService.UpdateListingCaseStatusByIdAsync(dto,id,userId,ipAddress);
            return Ok(ApiResponse<UpdateListingCaseStatusResponseDto>.SuccessResponse(result,"Update listing case status successful!"));
        }


        /// <summary>
        /// Get listing case media assets
        /// </summary>
        /// <param name="id"></param>
        /// <returns>List of mediaassets</returns>
        [HttpGet("{id:int}/media")]
        [Authorize(Roles = $"{RoleNames.PhotographyCompany},{RoleNames.Agent}")]
        [ProducesResponseType(typeof(ApiResponse<List<GetListingCaseMediaAssetsResponseDto>>),StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>),StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ApiResponse<object>),StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ApiResponse<object>),StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApiResponse<List<GetListingCaseMediaAssetsResponseDto>>>> GetListingCaseMediaAssets(int id)
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            List<GetListingCaseMediaAssetsResponseDto> result = await _listingCaseService.GetListingCaseMediaAssetsAsync(id,userId);
            return Ok(ApiResponse<List<GetListingCaseMediaAssetsResponseDto>>.SuccessResponse(result));
        }

        /// <summary>
        /// Get listing case case contacts
        /// </summary>
        /// <param name="id"></param>
        /// <returns>List of case contact</returns>
        [HttpGet("{id:int}/casecontacts")]
        [Authorize(Roles = $"{RoleNames.PhotographyCompany},{RoleNames.Agent}")]
        [ProducesResponseType(typeof(ApiResponse<List<GetCaseContactsResponseDto>>),StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>),StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ApiResponse<object>),StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ApiResponse<object>),StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApiResponse<List<GetCaseContactsResponseDto>>>> GetCaseContactsByListingCaseId(int id)
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            List<GetCaseContactsResponseDto> result = await _listingCaseService.GetCaseContactsByListingCaseIdAsync(id,userId);
            return Ok(ApiResponse<List<GetCaseContactsResponseDto>>.SuccessResponse(result));
        }


        [HttpPost("{id:int}/agents")]
        [Authorize(Roles = RoleNames.PhotographyCompany)]
        [ProducesResponseType(typeof(ApiResponse<AddAgentToListingCaseResponseDto>),StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ApiResponse<object>),StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<object>),StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ApiResponse<object>),StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ApiResponse<object>),StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApiResponse<AddAgentToListingCaseResponseDto>>> AddAgentToListingCase(AddAgentToListingCaseDto dto,int id)
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            string? ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();
            string email = User.FindFirstValue(ClaimTypes.Email)!;
            AddAgentToListingCaseResponseDto result = await _listingCaseService.AddAgentToListingCaseAsync(dto,userId,id,ipAddress,email);
            return StatusCode(StatusCodes.Status201Created,ApiResponse<AddAgentToListingCaseResponseDto>.SuccessResponse(result,"Agent assigned successfully!"));
        }


        [HttpPost("{id:int}/casecontacts")]
        [Authorize(Roles = $"{RoleNames.PhotographyCompany},{RoleNames.Agent}")]
        [ProducesResponseType(typeof(ApiResponse<int>),StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ApiResponse<object>),StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ApiResponse<object>),StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<object>),StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ApiResponse<object>),StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApiResponse<int>>> AddCaseContactToListingCase(int id,AddCaseContactDto dto)
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            // string? ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();
            int result = await _caseContactService.AddCaseContactToListingCaseAsync(id,dto,userId);
            return StatusCode(StatusCodes.Status201Created,ApiResponse<int>.SuccessResponse(result,"Add case contact successful!"));
        }

        [HttpPost("{id:int}/media")]
        [Authorize(Roles = RoleNames.PhotographyCompany)]
        [Consumes("multipart/form-data")]
        [RequestSizeLimit(524_288_000)]
        [RequestFormLimits(MultipartBodyLengthLimit = 524_288_000)]
        [ProducesResponseType(typeof(ApiResponse<List<MediaAssetDto>>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApiResponse<List<MediaAssetDto>>>> UploadMediaAssets(int id, [FromForm] List<IFormFile> files, [FromForm] MediaType mediaType)
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            string? ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();
            List<MediaAssetDto> result = await _mediaAssetService.UploadMediaAssetsAsync(id, mediaType, files, userId, ipAddress);
            return StatusCode(StatusCodes.Status201Created, ApiResponse<List<MediaAssetDto>>.SuccessResponse(result, "Upload media successful!"));
        }

        /// <summary>
        /// Download all media of a listing case as a zip file
        /// </summary>
        /// <param name="id">Listing case id</param>
        /// <returns>A zip file</returns>
        [HttpGet("{id:int}/download")]
        [Authorize(Roles = $"{RoleNames.PhotographyCompany},{RoleNames.Agent}")]
        [ProducesResponseType(typeof(FileStreamResult), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DownloadListingCaseMedia(int id)
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            (Stream content, string fileName) = await _mediaAssetService.DownloadListingCaseMediaZipAsync(id, userId);
            return File(content, "application/zip", fileName);
        }


        /// <summary>
        /// Set the cover image of a listing case
        /// </summary>
        /// <param name="id">Listing case id</param>
        /// <param name="dto">The selected media id</param>
        [HttpPut("{id:int}/cover-image")]
        [Authorize(Roles = RoleNames.Agent)]
        [ProducesResponseType(typeof(ApiResponse<SetCoverImageResponseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApiResponse<SetCoverImageResponseDto>>> SetCoverImage(int id, SetCoverImageDto dto)
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            string? ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();
            SetCoverImageResponseDto result = await _listingCaseService.SetCoverImageAsync(id, dto, userId, ipAddress);
            return Ok(ApiResponse<SetCoverImageResponseDto>.SuccessResponse(result, "Set cover image successful!"));
        }

        /// <summary>
        /// Agent select the media assets
        /// </summary>
        /// <param name="id"></param>
        /// <param name="dto"></param>
        /// <returns>The selected media ids and listing case id</returns>
        [HttpPut("{id:int}/selected-media")]
        [Authorize(Roles = RoleNames.Agent)]
        [ProducesResponseType(typeof(ApiResponse<AgentSelectMediaResponseDto>),StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApiResponse<AgentSelectMediaResponseDto>>> AgentSelectMedia(int id,AgentSelectMediaDto dto)
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            string? ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();
            AgentSelectMediaResponseDto result = await _listingCaseService.AgentSelectMediaAsync(userId,id,dto,ipAddress);
            return Ok(ApiResponse<AgentSelectMediaResponseDto>.SuccessResponse(result,"Select media successful!"));
        }
    }

}
