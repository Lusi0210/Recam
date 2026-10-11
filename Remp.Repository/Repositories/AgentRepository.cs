using System;
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
}