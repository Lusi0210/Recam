using AutoMapper;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.Extensions.Logging;
using Remp.Common.Exceptions;
using Remp.Common.Responses;
using Remp.Models.Constants;
using Remp.Models.Documents;
using Remp.Models.Entities;
using Remp.Models.Enums;
using Remp.Repository.Common;
using Remp.Repository.Interfaces;
using Remp.Service.DTOs;
using Remp.Service.Interfaces;

namespace Remp.Service.Services;

public class ListingCaseService : IListingCaseService
{
    private readonly IListingCaseRepository _listingCaseRepository;
    private readonly IValidator<CreateListingCaseDto> _createListingCaseValidator;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IListingCaseHistoryRepository _listingCaseHistoryRepository;
    private readonly ILogger<ListingCaseService> _logger;
    private readonly IValidator<PaginationQueryDto> _paginationValidator;
    private readonly IMapper _mapper;

    public ListingCaseService(
        IListingCaseRepository listingCaseRepository,
        IValidator<CreateListingCaseDto> createListingCaseValidator,
        IUnitOfWork unitOfWork,
        IListingCaseHistoryRepository listingCaseHistoryRepository,
        ILogger<ListingCaseService> logger,
        IValidator<PaginationQueryDto> paginationValidator,
        IMapper mapper)
    {
        _listingCaseRepository = listingCaseRepository;
        _createListingCaseValidator = createListingCaseValidator;
        _unitOfWork = unitOfWork;
        _listingCaseHistoryRepository = listingCaseHistoryRepository;
        _logger = logger;
        _paginationValidator = paginationValidator;
        _mapper = mapper;
    }

    private async Task LogListingCaseActivityAsync(ListingCaseActivityType action,int id, string? userId,bool success,string? message,string? ipAddress)
    {
        ListingCaseHistory listingCaseHistory = new ListingCaseHistory
        {
          Action=action,
          ListingCaseId=id,
          UserId=userId,
          Success=success,
          Message=message,
          IpAddress=ipAddress,
          Timestamp = DateTime.UtcNow  
        };

        try
        {
            await _listingCaseHistoryRepository.AddAsync(listingCaseHistory);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to write listing case history for case {ListingCaseId}", id);
        }
    }
 

    public async Task<int> CreateListingCaseAsync(CreateListingCaseDto dto,string userId,string? ipAddress)
    {
        ValidationResult validationResult = await _createListingCaseValidator.ValidateAsync(dto);
        if (!validationResult.IsValid)
        {
            List<string> errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
            throw new BadRequestException("Validation failed",errors);
        }

        ListingCase listingCase = new ListingCase
        {
            Title = dto.Title,
            Description = dto.Description,
            Street = dto.Street,
            City = dto.City,
            State = dto.State,
            Postcode = dto.Postcode,
            Longitude = dto.Longitude,
            Latitude = dto.Latitude,
            Price = dto.Price,
            Bedrooms = dto.Bedrooms,
            Bathrooms = dto.Bathrooms,
            Garages = dto.Garages,
            FloorArea = dto.FloorArea,
            CreatedAt = DateTime.UtcNow,
            IsDeleted = false,
            PropertyType = dto.PropertyType,
            SaleCategory = dto.SaleCategory,
            ListcaseStatus = ListcaseStatus.Created,
            UserId = userId
        };
        _listingCaseRepository.Add(listingCase);
        await _unitOfWork.SaveChangesAsync();

        
        await LogListingCaseActivityAsync(ListingCaseActivityType.Create,listingCase.Id,userId,true,"Create listingCase successful",ipAddress);
        
        return listingCase.Id;
    }

    public async Task<PagedResult<GetListingCaseResponseDto>> GetAllListingCaseAsync(string role,string userId,PaginationQueryDto query)
    {
        ValidationResult validationResult = await _paginationValidator.ValidateAsync(query);
        if (!validationResult.IsValid)
        {
            List<string> errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
            throw new BadRequestException("Validation failed", errors);
        }

        PagedResult<ListingCase> listingCases;

        if(role == RoleNames.PhotographyCompany)
        {
            listingCases = await _listingCaseRepository.GetPagedByPhotographyCompanyAsync(query.PageNumber,query.PageSize,userId);
        } else if (role == RoleNames.Agent)
        {
            listingCases = await _listingCaseRepository.GetPagedByAgentAsync(query.PageNumber,query.PageSize,userId);
        } else
        {
            throw new ForbiddenException("You do not have permission to view listing cases.");
        }


        List<GetListingCaseResponseDto> listingCaseDto= _mapper.Map<List<GetListingCaseResponseDto>>(listingCases.Items);

        return new PagedResult<GetListingCaseResponseDto>
        {
            Items = listingCaseDto,
            PageNumber = query.PageNumber,
            PageSize = query.PageSize,
            TotalCount = listingCases.TotalCount
        };

    }
}
