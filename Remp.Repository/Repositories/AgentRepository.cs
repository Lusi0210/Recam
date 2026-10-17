using System;
using Microsoft.EntityFrameworkCore;
using Remp.DataAccess.Data;
using Remp.Models.Entities;
using Remp.Repository.Interfaces;

namespace Remp.Repository.Repositories;

public class AgentRepository : IAgentRepository
{
    private readonly RempDbContext _dbContext;
    public AgentRepository(RempDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public void Add(Agent agent)
    {
        _dbContext.Agents.Add(agent);
    }

    public async Task<Agent?> GetAgentByIdAsync(string id)
    {
        Agent? agent = await _dbContext.Agents.Include(a => a.PhotographyCompanies)
                                                .FirstOrDefaultAsync(a => a.Id ==id);
        return agent;
    }
}