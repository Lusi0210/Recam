using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Remp.DataAccess.Data;
using Remp.Models.Entities;
using Remp.Repository.Interfaces;

namespace Remp.Repository.Repositories;

public class AgentPhotographyCompanyRepository : IAgentPhotographyCompanyRepository
{
    private readonly RempDbContext _rempDbContext;
    public AgentPhotographyCompanyRepository(RempDbContext dbContext)
    {
        _rempDbContext=dbContext;
    }

    public async Task<AgentPhotographyCompany> AddAgentToPhotographyCompanyAsync(AgentPhotographyCompany agentPhotographyCompany)
    {
        _rempDbContext.AgentPhotographyCompanies.Add(agentPhotographyCompany);
        await _rempDbContext.SaveChangesAsync();
        return agentPhotographyCompany;
    }

    public async Task<bool> CheckAgentinPhotographyCompany(string agentId,string photographyCompanyId)
    {
        return await _rempDbContext.AgentPhotographyCompanies
        .AnyAsync(x => x.AgentId == agentId && x.PhotographyCompanyId == photographyCompanyId);
    }
}
