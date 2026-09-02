using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.VisualBasic;
using Remp.DataAccess.Data;
using Remp.Models.Entities;
using Remp.Repository.Interfaces;

namespace Remp.Repository.Repositories;

public class ListingCaseRepository : IListingCaseRepository
{
    private readonly RempDbContext _context;
    public ListingCaseRepository(RempDbContext context)
    {
        _context = context;
    }
    public async Task<ListingCase> CreateAsync(ListingCase listingCase)
    {
        _context.ListingCases.Add(listingCase);   
        await _context.SaveChangesAsync();          
        return listingCase;
    }

    public async Task<List<ListingCase>> GetByPhotographyCompanyAsync(string userId)
    {
        return await _context.ListingCases.Where(x => x.UserId ==userId && !x.IsDeleted).ToListAsync();
    }

    public async Task<List<ListingCase>> GetByAgentAsync(string agentId)
    {
        List<int> listingCaseIds=  await _context.AgentListingCases.Where(x => x.AgentId == agentId).Select(x=> x.ListingCaseId).ToListAsync();
        List<ListingCase> listingCases = await _context.ListingCases.Where(x => listingCaseIds.Contains(x.Id) && !x.IsDeleted).ToListAsync();
        return listingCases;
    }

    public async Task<AgentListingCase> AddAgentToListingCaseAsync (AgentListingCase agentListingCase)
    {
        _context.AgentListingCases.Add(agentListingCase);
        await _context.SaveChangesAsync();
        return agentListingCase;
    }
    public async Task<bool> IsAgentAssignedAsync (string agentId,int listingCaseId)
    {
        return await _context.AgentListingCases.AnyAsync(x => x.AgentId ==agentId && x.ListingCaseId == listingCaseId);
    }

    public async Task<ListingCase?> GetByIdAsync (int listingCaseId)
    {
        return  await _context.ListingCases.FirstOrDefaultAsync(x => x.Id ==listingCaseId && !x.IsDeleted);
    }
    public async Task<ListingCase> UpdateListingCaseAsync (ListingCase listingCase)
    {
        await _context.SaveChangesAsync();
        return listingCase;     
    }

    public async Task<ListingCase?> GetListingCaseDetailsByIdAsync (int listingCaseId)
    {
        return await _context.ListingCases.Include(x => x.MediaAssets).Include(x => x.AgentListingCases).ThenInclude(alc => alc.Agent).FirstOrDefaultAsync(x=> x.Id == listingCaseId && !x.IsDeleted);
    }  
}

