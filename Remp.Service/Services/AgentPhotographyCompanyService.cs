using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Remp.Common;
using Remp.Models.Entities;
using Remp.Repository.Interfaces;
using Remp.Service.DTOs;
using Remp.Service.Interfaces;

namespace Remp.Service.Services;

public class AgentPhotographyCompanyService : IAgentPhotographyCompanyService
{
    private readonly IAgentPhotographyCompanyRepository _repo;
    public AgentPhotographyCompanyService(IAgentPhotographyCompanyRepository repo)
    {
        _repo=repo;    
    }
    public async Task<ApiResponse<bool>> AddAgentToPhotographyCompany(AddAgentToPhotographyCompanyDto dto)
    {
        var existing = await _repo.CheckAgentinPhotographyCompany(dto.AgentId,dto.PhotographyCompanyId);
        if (existing)
        {
            return ApiResponse<bool>.FailureResponse("This agent already in this photographyCompany!");
        }

        AgentPhotographyCompany result = new AgentPhotographyCompany
        {
            AgentId=dto.AgentId,
            PhotographyCompanyId=dto.PhotographyCompanyId
        };

        await _repo.AddAgentToPhotographyCompanyAsync(result);
        
        return ApiResponse<bool>.SuccessResponse(true,"Add agent to photography company successfully!");
    }

    public async Task<ApiResponse<List<AgentListItemResponseDto>>> GetAgentByPhotographyCompanyAsync(string photographyCompanyId)
    {
        var agents = await _repo.GetAgentByPhotographyCompanyAsync(photographyCompanyId);

        var dtos = agents.Select(a => new AgentListItemResponseDto
        {
            Id = a.Id,
            AgentFirstName = a.AgentFirstName,
            AgentLastName = a.AgentLastName,
            AvatarUrl = a.AvatarUrl,
            CompanyName = a.CompanyName
        }).ToList();

        return ApiResponse<List<AgentListItemResponseDto>>.SuccessResponse(dtos, "Get agents under this company successfully!");
    }

}
