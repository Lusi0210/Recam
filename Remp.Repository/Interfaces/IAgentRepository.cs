using System;
using Remp.Models.Entities;

namespace Remp.Repository.Interfaces;

public interface IAgentRepository
{
    void Add(Agent agent);
}
