using System;
using AutoMapper;
using Remp.Models.Entities;
using Remp.Models.Projections;
using Remp.Service.DTOs;

namespace Remp.Service.Mappers;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<UserWithRole, UserDto>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.User.Id))
            .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.User.Email))
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.User.CreatedAt));

        CreateMap<ListingCase,GetListingCaseResponseDto>();
        CreateMap<UpdateListingCaseDto,ListingCase>();
        CreateMap<ListingCase,UpdateListingCaseResponseDto>();
        CreateMap<MediaAsset,MediaAssetDto>();
        CreateMap<Agent,AgentDto>()
            .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.User.Email));
        CreateMap<ListingCase,GetListingCaseDetailsResponseDto>();
        CreateMap<CaseContact,GetCaseContactsResponseDto>();
        CreateMap<Agent,GetAgentResponseDto>()
            .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.User.Email));
    }
}
