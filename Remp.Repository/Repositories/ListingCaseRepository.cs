using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
}
