using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Remp.Models.Entities;

namespace Remp.Repository.Interfaces;

public interface IListingCaseRepository
{
    Task<ListingCase> CreateAsync(ListingCase listingCase);
    Task<List<ListingCase>> GetByPhotographyCompanyAsync(string userId);
    Task<List<ListingCase>> GetByAgentAsync(string agentId);
    Task<AgentListingCase> AddAgentToListingCaseAsync (AgentListingCase agentListingCase);
    Task<bool> IsAgentAssignedAsync (string agentId,int listingCaseId);
    Task<ListingCase?> GetByIdAsync (int listingCaseId);
    Task<ListingCase> UpdateListingCaseAsync (ListingCase listingCase);
    Task<ListingCase?> GetListingCaseDetailsByIdAsync (int listingCaseId);
    Task<List<int>> GetAssignedListingIdsAsync(string userId);
}
