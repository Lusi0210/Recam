using System;
using Remp.DataAccess.Collections;
using Remp.Models.Documents;
using Remp.Repository.Interfaces;

namespace Remp.Repository.Repositories;

public class PhotographyCompanyActivityLogRepository : IPhotographyCompanyActivityLogRepository
{
    private readonly MongoDbContext _context;
    public PhotographyCompanyActivityLogRepository(MongoDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(PhotographyCompanyActivityLog log)
    {
        await _context.PhotographyCompanyActivityLogs.InsertOneAsync(log);
    }
}
