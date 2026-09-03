using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using Remp.Common;
using Remp.Models.Entities;
using Remp.Repository.Interfaces;
using Remp.Repository.Repositories;
using Remp.Service.DTOs;
using Remp.Service.Interfaces;

namespace Remp.Service.Services;

public class CaseContactService:ICaseContactService
{
    private readonly ICaseContactRepository _caseContactRepo;
    private readonly IListingCaseRepository _listingCaseRepo;
    public CaseContactService(ICaseContactRepository caseContactRepo,IListingCaseRepository listingCaseRepo)
    {
        _caseContactRepo=caseContactRepo;
        _listingCaseRepo=listingCaseRepo;
    }

    public async Task<ApiResponse<int>> CreateCaseContactAsync(CreateCaseContactDto dto)
    {
        var existing = await _listingCaseRepo.GetByIdAsync(dto.ListingCaseId);
        if (existing == null)
        {
            return ApiResponse<int>.FailureResponse("The listing case does not exists!");
        }
        var caseContact = new CaseContact
        {
            FirstName=dto.FirstName,
            LastName=dto.LastName,
            CompanyName=dto.CompanyName,
            ProfileUrl=dto.ProfileUrl,
            Email=dto.Email,
            PhoneNumber=dto.PhoneNumber,
            ListingCaseId=dto.ListingCaseId
        };

        var saved = await _caseContactRepo.CreateCaseContactAsync(caseContact);
        return ApiResponse<int>.SuccessResponse(saved.Id,"Create Case Contact Successfully!");
    }

    public async Task<ApiResponse<List<GetAllCaseContactResponseDto>>> GetAllCaseContact(int listingCaseId)
    {
        var existing = await _listingCaseRepo.GetByIdAsync(listingCaseId);
        if (existing == null)
        {
            return ApiResponse<List<GetAllCaseContactResponseDto>>.FailureResponse("The listing case does not exists!");
        }

        List<CaseContact> result = await _caseContactRepo.GetAllCaseContactAsync(listingCaseId);
        List<GetAllCaseContactResponseDto> dtos = result.Select(x => new GetAllCaseContactResponseDto
        {
            Id = x.Id,
            FirstName = x.FirstName,
            LastName = x.LastName,
            CompanyName = x.CompanyName,
            ProfileUrl = x.ProfileUrl,
            Email = x.Email,
            PhoneNumber = x.PhoneNumber,
            ListingCaseId=x.ListingCaseId,
        }).ToList();
    
        return ApiResponse<List<GetAllCaseContactResponseDto>>.SuccessResponse(dtos,"Get case contact successfully");
    }
}
