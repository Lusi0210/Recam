using System;
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
}
