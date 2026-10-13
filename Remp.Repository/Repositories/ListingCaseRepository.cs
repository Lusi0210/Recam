using System;
using Remp.DataAccess.Data;
using Remp.Models.Entities;
using Remp.Repository.Interfaces;

namespace Remp.Repository.Repositories;

public class ListingCaseRepository : IListingCaseRepository
{
    private readonly RempDbContext _dbContext;
    public ListingCaseRepository(RempDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public void Add(ListingCase listingCase)
    {
        _dbContext.ListingCases.Add(listingCase);
    }
}
