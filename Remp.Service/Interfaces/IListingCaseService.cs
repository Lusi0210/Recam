using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Remp.Common;
using Remp.Models.Entities;
using Remp.Service.DTOs;

namespace Remp.Service.Interfaces;

public interface IListingCaseService
{
    Task<ApiResponse<int>> CreateAsync(CreateListingCaseDto dto, string userId);
    Task<ApiResponse<List<GetAllListingCaseResponseDto>>> GetByPhotographyCompanyAsync(string userId);
    Task<ApiResponse<List<GetAllListingCaseResponseDto>>> GetByAgentAsync(string agentId);
    Task<ApiResponse<bool>> AddAgentToListingCaseAsync(AddAgentToListingCaseDto dto);
    Task<ApiResponse<int>> UpdateListingCaseAsync (int listingCaseId,UpdateListingCaseDto dto);
    Task<ApiResponse<int>> DeleteListingCaseAsync (int listingCaseId);
    Task<ApiResponse<GetListingCaseDetailsResponseDto>> GetListingCaseDetailsById(int listingCaseId);
    Task<ApiResponse<int>> ChangeListingCaseStatusAsync (int listingCaseId);
}
