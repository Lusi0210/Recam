using System;
using AutoMapper;
using FluentValidation;
using Remp.Common.Exceptions;
using Remp.Models.Entities;
using Remp.Repository.Common;
using Remp.Repository.Interfaces;
using Remp.Service.Common;
using Remp.Service.DTOs;
using Remp.Service.Interfaces;

namespace Remp.Service.Services;

public class CaseContactService : ICaseContactService
{
    private readonly ICaseContactRepository _caseContactRepository;
    private readonly IMapper _mapper;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IListingCaseRepository _listingCaseRepository;
    private readonly IValidator<AddCaseContactDto> _addCaseContactValidator;
    public CaseContactService(
        ICaseContactRepository caseContactRepository,
        IMapper mapper,
        IUnitOfWork unitOfWork,
        IListingCaseRepository listingCaseRepository,
        IValidator<AddCaseContactDto> addCaseContactValidator)
    {
        _caseContactRepository = caseContactRepository;
        _mapper = mapper;
        _unitOfWork = unitOfWork;
        _listingCaseRepository = listingCaseRepository;
        _addCaseContactValidator = addCaseContactValidator;
    }

    public async Task<int> AddCaseContactToListingCaseAsync(int id,AddCaseContactDto dto,string userId)
    {
        await _addCaseContactValidator.ValidateOrThrowAsync(dto);

        ListingCase? listingCase = await _listingCaseRepository.GetListingCaseWithAgentAsync(id);

        if(listingCase == null)
        {
            throw new NotFoundException("The listing case does not exist!");
        }

        listingCase.EnsureOwnerOrAssignedAgent(userId, "You do not have permission to add contacts to this listing case.");
        

        
        CaseContact caseContact = _mapper.Map<CaseContact>(dto);
        caseContact.ListingCaseId = id;
        _caseContactRepository.Add(caseContact);

        await _unitOfWork.SaveChangesAsync();
        
        return caseContact.Id;
    }

    public async Task<List<GetCaseContactsResponseDto>> GetCaseContactsByListingCaseIdAsync(int id,string userId)
    {
        ListingCase? listingCase = await _listingCaseRepository.GetListingCaseWithContactsAsync(id);
        if(listingCase == null)
        {
            throw new NotFoundException("The listing case does not exist!");
        }

        listingCase.EnsureOwnerOrAssignedAgent(userId, "You do not have permission to view this listing case's case contacts.");
        

        List<GetCaseContactsResponseDto> responseDtos = _mapper.Map<List<GetCaseContactsResponseDto>>(listingCase.CaseContacts);

        return responseDtos;
    }
}
