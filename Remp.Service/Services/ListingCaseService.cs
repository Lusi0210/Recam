using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Remp.Common;
using Remp.Models.Entities;
using Remp.Models.Enums;
using Remp.Repository.Interfaces;
using Remp.Service.DTOs;
using Remp.Service.Interfaces;

namespace Remp.Service.Services;

public class ListingCaseService : IListingCaseService
{
    private readonly IListingCaseRepository _repo;
    public ListingCaseService(IListingCaseRepository repo)
    {
        _repo=repo;
    }
    public async Task<ApiResponse<int>> CreateAsync(CreateListingCaseDto dto, string userId)
    {
        var listingCase = new ListingCase
        {
            Title = dto.Title,
            Description = dto.Description,
            Street = dto.Street,
            City = dto.City,
            State = dto.State,
            PostCode = dto.PostCode,
            Longitude = dto.Longitude,
            Latitude = dto.Latitude,
            Price = dto.Price,
            Bedrooms = dto.Bedrooms,
            Bathrooms = dto.Bathrooms,
            Garages = dto.Garages,
            FloorArea = dto.FloorArea,
            PropertyType = dto.PropertyType,
            SaleCategory = dto.SaleCategory,
            
            ListcaseStatus = ListcaseStatus.Created,
            CreatedAt = DateTime.UtcNow,
            UserId = userId,
            IsDeleted = false
        };

        var saved = await _repo.CreateAsync(listingCase);
        return ApiResponse<int>.SuccessResponse(saved.Id, "Listing case created successfully");
    }

    public async Task<ApiResponse<List<GetAllListingCaseResponseDto>>> GetByPhotographyCompanyAsync(string userId)
    {
        List<ListingCase> listingCases=  await _repo.GetByPhotographyCompanyAsync(userId);

        List<GetAllListingCaseResponseDto> dtos = listingCases.Select(x => new GetAllListingCaseResponseDto
        {
            Id = x.Id,
            Title = x.Title,
            Description = x.Description,
            Street = x.Street,
            City = x.City,
            State = x.State,
            PostCode = x.PostCode,
            Longitude = x.Longitude,
            Latitude = x.Latitude,
            Price = x.Price,
            Bedrooms = x.Bedrooms,
            Bathrooms = x.Bathrooms,
            Garages = x.Garages,
            FloorArea = x.FloorArea,
            CreatedAt = x.CreatedAt,
            PropertyType = x.PropertyType,
            SaleCategory = x.SaleCategory,
            ListcaseStatus = x.ListcaseStatus
        }).ToList();
        return ApiResponse<List<GetAllListingCaseResponseDto>>.SuccessResponse(dtos,"Get Listing cases successfully");
    }

    public async Task<ApiResponse<List<GetAllListingCaseResponseDto>>> GetByAgentAsync(string agentId)
    {
        List<ListingCase> listingCases = await _repo.GetByAgentAsync(agentId);
        List<GetAllListingCaseResponseDto> dtos = listingCases.Select(x => new GetAllListingCaseResponseDto
        {
            Id = x.Id,
            Title = x.Title,
            Description = x.Description,
            Street = x.Street,
            City = x.City,
            State = x.State,
            PostCode = x.PostCode,
            Longitude = x.Longitude,
            Latitude = x.Latitude,
            Price = x.Price,
            Bedrooms = x.Bedrooms,
            Bathrooms = x.Bathrooms,
            Garages = x.Garages,
            FloorArea = x.FloorArea,
            CreatedAt = x.CreatedAt,
            PropertyType = x.PropertyType,
            SaleCategory = x.SaleCategory,
            ListcaseStatus = x.ListcaseStatus
        }).ToList();
        return ApiResponse<List<GetAllListingCaseResponseDto>>.SuccessResponse(dtos,"Get Listing cases successfully");
    }

    public async Task<ApiResponse<bool>> AddAgentToListingCaseAsync(AddAgentToListingCaseDto dto)
    {
        var exists = await _repo.IsAgentAssignedAsync(dto.AgentId,dto.ListingCaseId);
        if (exists)
        {
            return ApiResponse<bool>.FailureResponse("This agent is already assigned to this listing case");
        }
        var agentListingCase = new AgentListingCase
        {
            AgentId=dto.AgentId,
            ListingCaseId=dto.ListingCaseId,
        };

        var saved = await _repo.AddAgentToListingCaseAsync(agentListingCase);
        return ApiResponse<bool>.SuccessResponse(true,"Add agent to Listing Case successfully");
    }
}
