using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Remp.DataAccess.Data;
using Remp.Models.Entities;
using Remp.Repository.Interfaces;

namespace Remp.Repository.Repositories;

public class AgentRepository : IAgentRepository
{
    private readonly RempDbContext _context;

    public AgentRepository(RempDbContext context)
    {
        _context = context;
    }

    public async Task<Agent> CreateAsync(Agent agent)
    {
        _context.Agents.Add(agent);
        await _context.SaveChangesAsync();
        return agent;
    }
}
