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
    private readonly IValidator<UpdateListingCaseDto> _updateListingCaseValidator;
    private readonly IValidator<UpdateListingCaseStatusDto> _updateListingCaseStatusValidator;
    private readonly IMapper _mapper;
    private readonly IAgentRepository _agentRepository;
    private readonly IPhotographyCompanyActivityLogRepository _photographyCompanyActivityLogRepository;
    private readonly IValidator<AddAgentToListingCaseDto> _addAgentToListingCaseValidator;
    private readonly IBlobStorageService _blobStorageService;
    private readonly IValidator<SetCoverImageDto> _setCoverImageValidator;
    private readonly IValidator<AgentSelectMediaDto> _agentSelectMediaValidator;

    public ListingCaseService(
        IListingCaseRepository listingCaseRepository,
        IValidator<CreateListingCaseDto> createListingCaseValidator,
        IUnitOfWork unitOfWork,
        IListingCaseHistoryRepository listingCaseHistoryRepository,
        ILogger<ListingCaseService> logger,
        IValidator<PaginationQueryDto> paginationValidator,
        IValidator<UpdateListingCaseDto> updateListingCaseValidator,
        IValidator<UpdateListingCaseStatusDto> updateListingCaseStatusValidator,
        IMapper mapper,
        IAgentRepository agentRepository,
        IPhotographyCompanyActivityLogRepository photographyCompanyActivityLogRepository,
        IValidator<AddAgentToListingCaseDto> addAgentToListingCaseValidator,
        IBlobStorageService blobStorageService,
        IValidator<SetCoverImageDto> setCoverImageValidator,
        IValidator<AgentSelectMediaDto> agentSelectMediaValidator)
    {
        _listingCaseRepository = listingCaseRepository;
        _createListingCaseValidator = createListingCaseValidator;
        _unitOfWork = unitOfWork;
        _listingCaseHistoryRepository = listingCaseHistoryRepository;
        _logger = logger;
        _paginationValidator = paginationValidator;
        _updateListingCaseValidator = updateListingCaseValidator;
        _updateListingCaseStatusValidator = updateListingCaseStatusValidator;
        _mapper = mapper;
        _agentRepository = agentRepository;
        _photographyCompanyActivityLogRepository = photographyCompanyActivityLogRepository;
        _addAgentToListingCaseValidator = addAgentToListingCaseValidator;
        _blobStorageService = blobStorageService;
        _setCoverImageValidator = setCoverImageValidator;
        _agentSelectMediaValidator = agentSelectMediaValidator; 
    }

    private MediaAssetDto ToMediaAssetDto(MediaAsset mediaAsset)
    {
        MediaAssetDto dto = _mapper.Map<MediaAssetDto>(mediaAsset);
        dto.MediaUrl = _blobStorageService.GetReadUrl(mediaAsset.MediaUrl);
        return dto;
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

    private async Task LogActivityAsync(PhotographyCompanyActivityType action,string email, string? userId, bool success, string message, string? ipAddress)
    {
        PhotographyCompanyActivityLog log = new PhotographyCompanyActivityLog
        {
            UserId = userId,
            Email = email,
            Action = action,
            Success = success,
            Message = message,
            IpAddress = ipAddress,
            Timestamp = DateTime.UtcNow
        };

        try
        {
            await _photographyCompanyActivityLogRepository.AddAsync(log);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to write photography company activity log for {Email}", email);
        }
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

    public async Task<UpdateListingCaseResponseDto> UpdateListingCaseAsync(int id,UpdateListingCaseDto dto,string userId,string? ipAddress)
    {
        ValidationResult validationResult = await _updateListingCaseValidator.ValidateAsync(dto);
        if (!validationResult.IsValid)
        {
            List<string> errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
            throw new BadRequestException("Validation failed",errors);
        }

        ListingCase? exist = await _listingCaseRepository.GetListingCaseAsync(id);
        if (exist == null)
        {
            throw new NotFoundException("The listing case does not exist!");
        }

        if (exist.UserId != userId)
        {
            throw new ForbiddenException("You can only update your own listing cases.");
        }

        _mapper.Map(dto, exist);
        
        await _unitOfWork.SaveChangesAsync();

        await LogListingCaseActivityAsync(ListingCaseActivityType.Update,id,userId,true,"Update listing case successful!",ipAddress);
        
        return _mapper.Map<UpdateListingCaseResponseDto>(exist);
    }

    public async Task DeleteListingCaseAsync(int id,string userId,string? ipAddress)
    {
        ListingCase? listingCase = await _listingCaseRepository.GetListingCaseWithMediaAssetAsync(id);
        if(listingCase == null)
        {
            throw new NotFoundException("The listing case does not exist!");
        }

        if (listingCase.UserId != userId)
        {
            throw new ForbiddenException("You can only delete your own listing cases.");
        }

        listingCase.IsDeleted = true;
        foreach (MediaAsset mediaAsset in listingCase.MediaAssets)
        {
            mediaAsset.IsDeleted = true;
        }

        await _unitOfWork.SaveChangesAsync();

        await LogListingCaseActivityAsync(ListingCaseActivityType.Delete,id,userId,true,"Delete listing case successful!",ipAddress);
    }


    public async Task<GetListingCaseDetailsResponseDto> GetListingCaseDetailsByIdAsync(int id,string userId)
    {
        ListingCase? listingCase = await _listingCaseRepository.GetListingCaseDetailsByIdAsync(id);
        if(listingCase == null)
        {
            throw new NotFoundException("The listing case does not exist!");
        }

        bool isOwner = listingCase.UserId == userId;
        bool isAssignedAgent = listingCase.Agents.Any(a => a.Id == userId);

        if(!isOwner && !isAssignedAgent)
        {
            throw new ForbiddenException("You can only get the listing case details assigned to you");
        }

        GetListingCaseDetailsResponseDto result = _mapper.Map<GetListingCaseDetailsResponseDto>(listingCase);
        foreach (MediaAssetDto media in result.MediaAssets)
        {
            media.MediaUrl = _blobStorageService.GetReadUrl(media.MediaUrl);
        }
        return result;

    }

    public async Task<UpdateListingCaseStatusResponseDto> UpdateListingCaseStatusByIdAsync(UpdateListingCaseStatusDto dto,int id,string userId,string? ipAddress)
    {
        ValidationResult validationResult = await _updateListingCaseStatusValidator.ValidateAsync(dto);
        if (!validationResult.IsValid)
        {
            List<string> errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
            throw new BadRequestException("Validation failed",errors);
        }
        ListingCase? listingCase = await _listingCaseRepository.GetListingCaseAsync(id);
        if(listingCase == null)
        {
            throw new NotFoundException("The listing case does not exist!");
        }
        ListcaseStatus previousStatus = listingCase.ListcaseStatus;

        if(listingCase.UserId != userId)
        {
            throw new ForbiddenException("The listing case only can be updated by owner!");
        }

        if (!CanTransition(previousStatus, dto.Status))
        {
            throw new BadRequestException($"Cannot change status from {previousStatus} to {dto.Status}.");
        }
        listingCase.ListcaseStatus = dto.Status;
        await _unitOfWork.SaveChangesAsync();
        await LogListingCaseActivityAsync(ListingCaseActivityType.StatusChange,id,userId,true,$"Status changed from {previousStatus} to {dto.Status}",ipAddress);


        return new UpdateListingCaseStatusResponseDto{
            Id = id,
            PreviousStatus = previousStatus,
            CurrentStatus = listingCase.ListcaseStatus
        };
    }

    public async Task<List<GetListingCaseMediaAssetsResponseDto>> GetListingCaseMediaAssetsAsync(int id,string userId)
    {
        ListingCase? listingCase = await _listingCaseRepository.GetListingCaseDetailsByIdAsync(id);
        if(listingCase == null)
        {
            throw new NotFoundException("The listing case does not exist!");
        }

        bool isOwner = listingCase.UserId == userId;
        bool isAssignedAgent = listingCase.Agents.Any(a => a.Id == userId);

        if (!isOwner && !isAssignedAgent)
        {
            throw new ForbiddenException("You do not have permission to view this listing case's media.");
        }


        List<GetListingCaseMediaAssetsResponseDto> result = listingCase.MediaAssets
            .GroupBy(m => m.MediaType)
            .Select(group => new GetListingCaseMediaAssetsResponseDto
            {
                MediaType = group.Key,
                MediaAssets = group.Select(m => ToMediaAssetDto(m)).ToList()
            })
            .ToList();
        
        return result;
    }

    public async Task<List<GetCaseContactsResponseDto>> GetCaseContactsByListingCaseIdAsync(int id,string userId)
    {
        ListingCase? listingCase = await _listingCaseRepository.GetListingCaseWithContactsAsync(id);
        if(listingCase == null)
        {
            throw new NotFoundException("The listing case does not exist!");
        }

        bool isOwner = listingCase.UserId == userId;
        bool isAssignedAgent = listingCase.Agents.Any(a => a.Id == userId);

        if (!isOwner && !isAssignedAgent)
        {
            throw new ForbiddenException("You do not have permission to view this listing case's case contacts.");
        }

        List<GetCaseContactsResponseDto> responseDtos = _mapper.Map<List<GetCaseContactsResponseDto>>(listingCase.CaseContacts);

        return responseDtos;
    }

    public async Task<AddAgentToListingCaseResponseDto> AddAgentToListingCaseAsync(AddAgentToListingCaseDto dto,string userId,int id,string? ipAddress,string email)
    {
        ValidationResult validationResult = await _addAgentToListingCaseValidator.ValidateAsync(dto);
        if (!validationResult.IsValid)
        {
            List<string> errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
            throw new BadRequestException("Validation failed",errors);
        }
        ListingCase? listingCase = await _listingCaseRepository.GetListingCaseWithAgentAsync(id);
        if(listingCase == null)
        {
            throw new NotFoundException("The listing case does not exist!");
        }

        if(listingCase.UserId != userId)
        {
            throw new ForbiddenException("You can only add agent to your listing case!");
        }

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

        await LogActivityAsync(PhotographyCompanyActivityType.AddAgentToListingCase,email,userId,true,$"Assigned agent {agent.Id} to listing case {id}",ipAddress);
        
        return new AddAgentToListingCaseResponseDto
        {
            Id = agent.Id,
            ListingCaseId = id
        };
    }

    public async Task<SetCoverImageResponseDto> SetCoverImageAsync(int listingCaseId, SetCoverImageDto dto, string userId, string? ipAddress)
    {
        ValidationResult validationResult = await _setCoverImageValidator.ValidateAsync(dto);
        if (!validationResult.IsValid)
        {
            List<string> errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
            throw new BadRequestException("Validation failed", errors);
        }

        ListingCase? listingCase = await _listingCaseRepository.GetListingCaseWithMediaAndAgentsAsync(listingCaseId);
        if (listingCase == null)
        {
            throw new NotFoundException("The listing case does not exist!");
        }

        bool isAssignedAgent = listingCase.Agents.Any(a => a.Id == userId);
        if (!isAssignedAgent)
        {
            throw new ForbiddenException("Only the assigned agent can set the cover image.");
        }

        MediaAsset? coverImage = listingCase.MediaAssets.FirstOrDefault(m => m.Id == dto.MediaId);
        if (coverImage == null)
        {
            throw new BadRequestException("The media does not belong to this listing case.");
        }

        if (coverImage.MediaType != MediaType.Photos)
        {
            throw new BadRequestException("Only photos can be set as the cover image.");
        }

        foreach (MediaAsset mediaAsset in listingCase.MediaAssets)
        {
            mediaAsset.IsHero = mediaAsset.Id == coverImage.Id;
        }

        await _unitOfWork.SaveChangesAsync();

        await LogListingCaseActivityAsync(ListingCaseActivityType.SetCoverImage, listingCaseId, userId,true, $"Set media {coverImage.Id} as cover image", ipAddress);

        return new SetCoverImageResponseDto
        {
            ListingCaseId = listingCaseId,
            MediaId = coverImage.Id
        };
    }

    public async Task<AgentSelectMediaResponseDto> AgentSelectMediaAsync(string userId,int id,AgentSelectMediaDto dto,string? ipAddress)
    {
        ValidationResult validationResult = await _agentSelectMediaValidator.ValidateAsync(dto);
        if (!validationResult.IsValid)
        {
            List<string> errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
            throw new BadRequestException("Validation failed", errors);
        }

        ListingCase? listingCase = await _listingCaseRepository.GetListingCaseWithMediaAndAgentsAsync(id);
        if (listingCase == null)
        {
            throw new NotFoundException("The listing case does not exist!");
        }

        bool isAssignedAgent = listingCase.Agents.Any(a => a.Id == userId);
        if (!isAssignedAgent)
        {
            throw new ForbiddenException("Only the assigned agent can select the media.");
        }

        List<int> invalidIds = dto.MediaAssetsId
            .Where(mediaId => !listingCase.MediaAssets.Any(m => m.Id == mediaId))
            .ToList();
        if (invalidIds.Count > 0)
        {
            throw new BadRequestException($"Media {string.Join(", ", invalidIds)} do not belong to this listing case.");
        }

        bool hasNonPhoto = listingCase.MediaAssets.Any(m => dto.MediaAssetsId.Contains(m.Id) && m.MediaType != MediaType.Photos);
        if (hasNonPhoto)
        {
            throw new BadRequestException("Only photos can be selected.");
        }

        foreach (MediaAsset mediaAsset in listingCase.MediaAssets)
        {
            mediaAsset.IsSelected = dto.MediaAssetsId.Contains(mediaAsset.Id);
        }


        await _unitOfWork.SaveChangesAsync();

        string message = dto.MediaAssetsId.Count == 0
            ? "Cleared all selected media"
            : $"Selected media: {string.Join(", ", dto.MediaAssetsId)}";
        await LogListingCaseActivityAsync(ListingCaseActivityType.SelectMedia, id, userId,true, message, ipAddress);
        return new AgentSelectMediaResponseDto
        {
            Id = id,
            MediaAssetsId = dto.MediaAssetsId
        };
    }
}
