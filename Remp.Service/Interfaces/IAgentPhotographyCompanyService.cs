using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Remp.Common;
using Remp.Models.Entities;
using Remp.Service.DTOs;

namespace Remp.Service.Interfaces;

public interface IAgentPhotographyCompanyService
{
    Task<ApiResponse<bool>> AddAgentToPhotographyCompany(AddAgentToPhotographyCompanyDto dto);
    Task<ApiResponse<List<AgentListItemResponseDto>>> GetAgentByPhotographyCompanyAsync(string photographyCompanyId);
}
