using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Remp.Models.Entities;

namespace Remp.Repository.Interfaces;

public interface IAgentRepository
{
    Task<Agent> CreateAsync(Agent agent);
    Task<Agent?> SearchAgentByIdAsync(string id);
}
