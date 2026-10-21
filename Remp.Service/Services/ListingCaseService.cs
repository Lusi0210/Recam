using AutoMapper;
using FluentValidation;
using Microsoft.Extensions.Options;
using Remp.Common.Exceptions;
using Remp.Common.Responses;
using Remp.Common.Settings;
using Remp.Models.Constants;
using Remp.Models.Entities;
using Remp.Models.Enums;
using Remp.Models.Filters;
using Remp.Repository.Common;
using Remp.Repository.Interfaces;
using Remp.Service.Common;
using Remp.Service.DTOs;
using Remp.Service.Interfaces;

namespace Remp.Service.Services;

public class ListingCaseService : IListingCaseService
{
    private readonly IListingCaseRepository _listingCaseRepository;
    private readonly IValidator<CreateListingCaseDto> _createListingCaseValidator;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IValidator<ListingCaseQueryDto> _listingCaseQueryValidator;
    private readonly IValidator<UpdateListingCaseDto> _updateListingCaseValidator;
    private readonly IValidator<UpdateListingCaseStatusDto> _updateListingCaseStatusValidator;
    private readonly IMapper _mapper;
    private readonly IAgentRepository _agentRepository;
    private readonly IValidator<AddAgentToListingCaseDto> _addAgentToListingCaseValidator;
    private readonly IBlobStorageService _blobStorageService;
    private readonly FrontendSettings _frontendSettings;
    private readonly IActivityLogService _activityLogService;

    public ListingCaseService(
        IListingCaseRepository listingCaseRepository,
        IValidator<CreateListingCaseDto> createListingCaseValidator,
        IUnitOfWork unitOfWork,
        IValidator<ListingCaseQueryDto> listingCaseQueryValidator,
        IValidator<UpdateListingCaseDto> updateListingCaseValidator,
        IValidator<UpdateListingCaseStatusDto> updateListingCaseStatusValidator,
        IMapper mapper,
        IAgentRepository agentRepository,
        IValidator<AddAgentToListingCaseDto> addAgentToListingCaseValidator,
        IBlobStorageService blobStorageService,
        IOptions<FrontendSettings> frontendOptions,
        IActivityLogService activityLogService)
    {
        _listingCaseRepository = listingCaseRepository;
        _createListingCaseValidator = createListingCaseValidator;
        _unitOfWork = unitOfWork;
        _listingCaseQueryValidator = listingCaseQueryValidator;
        _updateListingCaseValidator = updateListingCaseValidator;
        _updateListingCaseStatusValidator = updateListingCaseStatusValidator;
        _mapper = mapper;
        _agentRepository = agentRepository;
        _addAgentToListingCaseValidator = addAgentToListingCaseValidator;
        _blobStorageService = blobStorageService;
        _frontendSettings = frontendOptions.Value;
        _activityLogService = activityLogService; 
    }

    private static bool CanTransition(ListcaseStatus from, ListcaseStatus to)
    {
        return (from, to) switch
        {
            (ListcaseStatus.Created, ListcaseStatus.Pending) => true,
            (ListcaseStatus.Pending, ListcaseStatus.InReview) => true,
            (ListcaseStatus.InReview, ListcaseStatus.Delivered) => true,
            (ListcaseStatus.Pending, ListcaseStatus.Created) => true,
            (ListcaseStatus.InReview, ListcaseStatus.Pending) => true,
            _ => false
        };
    }
 

    public async Task<int> CreateListingCaseAsync(CreateListingCaseDto dto,string userId,string? ipAddress)
    {
        await _createListingCaseValidator.ValidateOrThrowAsync(dto);

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

        
        await _activityLogService.LogListingCaseAsync(ListingCaseActivityType.Create,listingCase.Id,userId,true,"Create listingCase successful",ipAddress);
        
        return listingCase.Id;
    }

    public async Task<PagedResult<GetListingCaseResponseDto>> GetAllListingCaseAsync(string role,string userId,ListingCaseQueryDto query)
    {
        await _listingCaseQueryValidator.ValidateOrThrowAsync(query);
        

        ListingCaseFilter filter = _mapper.Map<ListingCaseFilter>(query);
        PagedResult<ListingCase> listingCases;

        if (role == RoleNames.PhotographyCompany)
        {
            listingCases = await _listingCaseRepository.GetPagedByPhotographyCompanyAsync(query.PageNumber, query.PageSize, userId, filter);
        }
        else if (role == RoleNames.Agent)
        {
            listingCases = await _listingCaseRepository.GetPagedByAgentAsync(query.PageNumber, query.PageSize, userId, filter);
        }
        else
        {
            throw new ForbiddenException("You do not have permission to view listing cases.");
        }

        List<GetListingCaseResponseDto> listingCaseDto = _mapper.Map<List<GetListingCaseResponseDto>>(listingCases.Items);

        return new PagedResult<GetListingCaseResponseDto>
        {
            Items = listingCaseDto,
            PageNumber = query.PageNumber,
            PageSize = query.PageSize,
            TotalCount = listingCases.TotalCount
        };

    }

    public async Task<UpdateListingCaseResponseDto> UpdateListingCaseAsync(int id,UpdateListingCaseDto dto,string userId,string? ipAddress)
    {
        await _updateListingCaseValidator.ValidateOrThrowAsync(dto);

        ListingCase? exist = await _listingCaseRepository.GetListingCaseAsync(id);
        if (exist == null)
        {
            throw new NotFoundException("The listing case does not exist!");
        }

        exist.EnsureOwner(userId, "You can only update your own listing cases.");
        

        _mapper.Map(dto, exist);
        
        await _unitOfWork.SaveChangesAsync();

        await _activityLogService.LogListingCaseAsync(ListingCaseActivityType.Update,id,userId,true,"Update listing case successful!",ipAddress);
        
        return _mapper.Map<UpdateListingCaseResponseDto>(exist);
    }

    public async Task DeleteListingCaseAsync(int id,string userId,string? ipAddress)
    {
        ListingCase? listingCase = await _listingCaseRepository.GetListingCaseWithMediaAssetAsync(id);
        if(listingCase == null)
        {
            throw new NotFoundException("The listing case does not exist!");
        }

        listingCase.EnsureOwner(userId, "You can only delete your own listing cases.");
        

        listingCase.IsDeleted = true;
        foreach (MediaAsset mediaAsset in listingCase.MediaAssets)
        {
            mediaAsset.IsDeleted = true;
        }

        await _unitOfWork.SaveChangesAsync();

        await _activityLogService.LogListingCaseAsync(ListingCaseActivityType.Delete,id,userId,true,"Delete listing case successful!",ipAddress);
    }


    public async Task<GetListingCaseDetailsResponseDto> GetListingCaseDetailsByIdAsync(int id,string userId)
    {
        ListingCase? listingCase = await _listingCaseRepository.GetListingCaseDetailsByIdAsync(id);
        if(listingCase == null)
        {
            throw new NotFoundException("The listing case does not exist!");
        }

        listingCase.EnsureOwnerOrAssignedAgent(userId, "You can only get the listing case details assigned to you");
        

        GetListingCaseDetailsResponseDto result = _mapper.Map<GetListingCaseDetailsResponseDto>(listingCase);
        foreach (MediaAssetDto media in result.MediaAssets)
        {
            media.MediaUrl = _blobStorageService.GetReadUrl(media.MediaUrl);
        }
        return result;

    }

    public async Task<UpdateListingCaseStatusResponseDto> UpdateListingCaseStatusByIdAsync(UpdateListingCaseStatusDto dto,int id,string userId,string? ipAddress)
    {
        await _updateListingCaseStatusValidator.ValidateOrThrowAsync(dto);

        ListingCase? listingCase = await _listingCaseRepository.GetListingCaseAsync(id);
        if(listingCase == null)
        {
            throw new NotFoundException("The listing case does not exist!");
        }
        ListcaseStatus previousStatus = listingCase.ListcaseStatus;

        listingCase.EnsureOwner(userId, "The listing case only can be updated by owner!");
        

        if (!CanTransition(previousStatus, dto.Status))
        {
            throw new BadRequestException($"Cannot change status from {previousStatus} to {dto.Status}.");
        }
        listingCase.ListcaseStatus = dto.Status;
        await _unitOfWork.SaveChangesAsync();
        await _activityLogService.LogListingCaseAsync(ListingCaseActivityType.StatusChange,id,userId,true,$"Status changed from {previousStatus} to {dto.Status}",ipAddress);


        return new UpdateListingCaseStatusResponseDto{
            Id = id,
            PreviousStatus = previousStatus,
            CurrentStatus = listingCase.ListcaseStatus
        };
    }


    public async Task<AddAgentToListingCaseResponseDto> AddAgentToListingCaseAsync(AddAgentToListingCaseDto dto,string userId,int id,string? ipAddress,string email)
    {
        await _addAgentToListingCaseValidator.ValidateOrThrowAsync(dto);

        ListingCase? listingCase = await _listingCaseRepository.GetListingCaseWithAgentAsync(id);
        if(listingCase == null)
        {
            throw new NotFoundException("The listing case does not exist!");
        }

        listingCase.EnsureOwner(userId, "You can only add agent to your listing case!");
        

        Agent? agent = await _agentRepository.GetAgentByIdAsync(dto.Id);
        if (agent == null)
        {
            throw new NotFoundException("The agent does not exist!");
        }

        if (listingCase.Agents.Any(a => a.Id == agent.Id))
        {
            throw new BadRequestException("This agent has already been assigned to this listing case.");
        }

        if (!agent.PhotographyCompanies.Any(p => p.Id == userId))
        {
            throw new BadRequestException("This agent is not in your company.");
        }

        listingCase.Agents.Add(agent);
        await _unitOfWork.SaveChangesAsync();

        await _activityLogService.LogPhotographyCompanyAsync(PhotographyCompanyActivityType.AddAgentToListingCase,email,userId,true,$"Assigned agent {agent.Id} to listing case {id}",ipAddress);
        
        return new AddAgentToListingCaseResponseDto
        {
            Id = agent.Id,
            ListingCaseId = id
        };
    }

    public async Task<PublishListingCaseResponseDto> PublishListingCaseAsync(int id, string userId, string? ipAddress)
    {
        ListingCase? listingCase = await _listingCaseRepository.GetListingCaseAsync(id);
        if (listingCase == null)
        {
            throw new NotFoundException("The listing case does not exist!");
        }

        listingCase.EnsureOwner(userId, "You can only publish your own listing cases.");
        

        if (listingCase.ListcaseStatus != ListcaseStatus.Delivered)
        {
            throw new BadRequestException("Only delivered listing cases can be published.");
        }

        if (!string.IsNullOrEmpty(listingCase.ShareableUrl))
        {
            return new PublishListingCaseResponseDto
            {
                ListingCaseId = id,
                ShareableUrl = listingCase.ShareableUrl
            };
        }

        string token = Guid.NewGuid().ToString("N");
        string baseUrl = _frontendSettings.BaseUrl.TrimEnd('/');
        listingCase.ShareableUrl = $"{baseUrl}/share/{token}";

        await _unitOfWork.SaveChangesAsync();

        await _activityLogService.LogListingCaseAsync(ListingCaseActivityType.Publish,id,userId,true,$"Published listing case with URL {listingCase.ShareableUrl}",ipAddress);
    
        return new PublishListingCaseResponseDto
        {
            ListingCaseId = id,
            ShareableUrl = listingCase.ShareableUrl
        };
    }



}
