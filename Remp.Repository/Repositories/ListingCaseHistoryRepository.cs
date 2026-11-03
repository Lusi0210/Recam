using System;
using MongoDB.Driver;
using Remp.DataAccess.Collections;
using Remp.Models.Documents;
using Remp.Repository.Interfaces;

namespace Remp.Repository.Repositories;

public class ListingCaseHistoryRepository : IListingCaseHistoryRepository
{
    private readonly MongoDbContext _dbContext;
    public ListingCaseHistoryRepository(MongoDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task AddAsync(ListingCaseHistory log)
    {
        await _dbContext.ListingCaseHistories.InsertOneAsync(log);
    }

    public async Task<List<ListingCaseHistory>> GetLatestByListingCaseIdsAsync(List<int> listingCaseIds, int limit)
    {
        return await _dbContext.ListingCaseHistories
            .Find(h => listingCaseIds.Contains(h.ListingCaseId) && h.Success)
            .SortByDescending(h => h.Timestamp)
            .Limit(limit)
            .ToListAsync();
    }
}
