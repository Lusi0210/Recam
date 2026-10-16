using AutoMapper;
using FluentValidation;
using FluentValidation.Results;
using Remp.Common.Exceptions;
using Remp.Common.Responses;
using Remp.Models.Constants;
using Remp.Models.Entities;
using Remp.Models.Projections;
using Remp.Repository.Interfaces;
using Remp.Service.DTOs;
using Remp.Service.Interfaces;

namespace Remp.Service.Services;

public class UserService : IUserService
{   
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;
    private readonly IValidator<PaginationQueryDto> _paginationValidator;

    public UserService(IUserRepository userRepository,IMapper mapper,IValidator<PaginationQueryDto> paginationValidator)
    {
        _userRepository = userRepository;
        _mapper = mapper;
        _paginationValidator = paginationValidator;
    }

    public async Task<PagedResult<UserDto>> GetAllUsersAsync(PaginationQueryDto query)
    {
        ValidationResult validationResult = await _paginationValidator.ValidateAsync(query);
        if (!validationResult.IsValid)
        {
            List<string> errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
            throw new BadRequestException("Validation failed", errors);
        }


        PagedResult<UserWithRole> pagedUsers =
            await _userRepository.GetPagedAsync(query.PageNumber, query.PageSize);

        List<UserDto> userDtos = _mapper.Map<List<UserDto>>(pagedUsers.Items);

        return new PagedResult<UserDto>
        {
            Items = userDtos,
            PageNumber = pagedUsers.PageNumber,
            PageSize = pagedUsers.PageSize,
            TotalCount = pagedUsers.TotalCount
        };
    }

    public async Task<GetUserResponseDto> GetCurrnetUserInformationAsync(string userId,string role)
    {
        User? user = await _userRepository.GetCurrnetUserAsync(userId);
        if (user == null)
        {
            throw new NotFoundException("The user does not exist!");
        }

        List<int> listingCaseIds;

        if(role == RoleNames.PhotographyCompany)
        {
            listingCaseIds = user.ListingCases.Select(l => l.Id).ToList();
        } else if(role == RoleNames.Agent)
        {
            listingCaseIds = user.Agent?.ListingCases.Select(l => l.Id).ToList() ?? new List<int>();
        } else
        {
            listingCaseIds = new List<int>();
        }
        

        
        return new GetUserResponseDto
        {
            UserId = userId,
            Role = role,
            ListingCaseIds = listingCaseIds
        };
        
            
    }


}
