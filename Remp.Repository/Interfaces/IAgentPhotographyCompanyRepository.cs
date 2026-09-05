using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Remp.Models.Entities;

namespace Remp.Repository.Interfaces;

public interface IAgentPhotographyCompanyRepository
{
    Task<AgentPhotographyCompany> AddAgentToPhotographyCompanyAsync(AgentPhotographyCompany agentPhotographyCompany);
    Task<bool> CheckAgentinPhotographyCompany(string agentId,string photographyCompanyId);
    Task<List<Agent>> GetAgentByPhotographyCompanyAsync(string photographyCompanyId); 
}
