using System;
using Remp.Common.Responses;
using Remp.Service.DTOs;

namespace Remp.Service.Interfaces;

public interface IListingCaseService
{
    Task<int> CreateListingCaseAsync(CreateListingCaseDto dto,string userId,string? ipAddress);
    Task<PagedResult<GetListingCaseResponseDto>> GetAllListingCaseAsync(string role,string userId,ListingCaseQueryDto query);
    Task<UpdateListingCaseResponseDto> UpdateListingCaseAsync(int id,UpdateListingCaseDto dto,string userId,string? ipAddress);
    Task DeleteListingCaseAsync(int id,string userId,string? ipAddress);
    Task<GetListingCaseDetailsResponseDto> GetListingCaseDetailsByIdAsync(int id,string userId);
    Task<UpdateListingCaseStatusResponseDto> UpdateListingCaseStatusByIdAsync(UpdateListingCaseStatusDto dto,int id,string userId,string? ipAddress);
    Task<List<GetListingCaseMediaAssetsResponseDto>> GetListingCaseMediaAssetsAsync(int id,string userId);
    Task<List<GetCaseContactsResponseDto>> GetCaseContactsByListingCaseIdAsync(int id,string userId);
    Task<AddAgentToListingCaseResponseDto> AddAgentToListingCaseAsync(AddAgentToListingCaseDto dto,string userId,int id,string? ipAddress,string email);
    Task<SetCoverImageResponseDto> SetCoverImageAsync(int listingCaseId, SetCoverImageDto dto, string userId, string? ipAddress); 
    Task<AgentSelectMediaResponseDto> AgentSelectMediaAsync(string userId,int id,AgentSelectMediaDto dto,string? ipAddress);
    Task<List<GetFInalSelectionResponseDto>> GetFinalSelectionAsync(int id,string userId);
    Task<PublishListingCaseResponseDto> PublishListingCaseAsync(int id, string userId, string? ipAddress);
}
