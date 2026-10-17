using System;
using Remp.Service.DTOs;

namespace Remp.Service.Interfaces;

public interface IAgentService
{
    Task<string> AddAgentToPhotographyCompanyAsync(string userId,AddAgentToPhotographyCompanyDto dto);
    Task<CreateAgentByPhotographyCompanyResponseDto> CreateAgentByPhotographyCompanyAsync(CreateAgentByPhotographyCompanyDto dto,string userId,string? ipAddress);
    Task<SearchAgentByEmailResponseDto> SearchAgentByEmailAsync(string email);
}
