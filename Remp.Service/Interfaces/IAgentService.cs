using System;
using Remp.Service.DTOs;

namespace Remp.Service.Interfaces;

public interface IAgentService
{
    Task<string> AddAgentToPhotographyCompanyAsync(string userId,AddAgentToPhotographyCompanyDto dto);
    Task<CreateAgentByPhotographyCompanyResponseDto> CreateAgentByPhotographyCompanyAsync(CreateAgentByPhotographyCompanyDto dto,string userId,string? ipAddress);
    Task<GetAgentResponseDto> SearchAgentByEmailAsync(string email);
    Task<List<GetAgentResponseDto>> GetAllAgentsUnderThisAccountAsync(string userId);
}
