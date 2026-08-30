using System;
using System.Collections.Generic;
using System.Linq;
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
}
