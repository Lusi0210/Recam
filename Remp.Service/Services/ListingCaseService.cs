using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using System.Xml;
using Microsoft.EntityFrameworkCore.Migrations.Operations;
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

    public async Task<ApiResponse<int>> UpdateListingCaseAsync (int listingCaseId,UpdateListingCaseDto dto)
    {
        var exists = await _repo.GetByIdAsync(listingCaseId);
        if (exists!=null)
        {
            exists.Title = dto.Title;
            exists.Description = dto.Description;
            exists.Street=dto.Street;
            exists.City=dto.City;
            exists.State=dto.State;
            exists.PostCode=dto.PostCode;
            exists.Longitude=dto.Longitude;;
            exists.Latitude=dto.Latitude;
            exists.Price=dto.Price;
            exists.Bedrooms=dto.Bedrooms;
            exists.Bathrooms=dto.Bathrooms;
            exists.Garages=dto.Garages;
            exists.FloorArea=dto.FloorArea;
            exists.PropertyType=dto.PropertyType;
            exists.SaleCategory=dto.SaleCategory;
            await _repo.UpdateListingCaseAsync(exists);
            return ApiResponse<int>.SuccessResponse(listingCaseId, "Listing case updated successfully");
        }
        else
        {
            return ApiResponse<int>.FailureResponse("Listing case does not exist!");
        }
    }

    public async Task<ApiResponse<int>> DeleteListingCaseAsync (int listingCaseId)
    {
        var existing = await _repo.GetByIdAsync(listingCaseId);
        if(existing != null)
        {
            existing.IsDeleted = true;
            await _repo.UpdateListingCaseAsync(existing);
            return ApiResponse<int>.SuccessResponse(listingCaseId, "Listing case deleted successfully");
        }
        else
        {
           return ApiResponse<int>.FailureResponse("Listing case does not exist!");
        }
    }
}
