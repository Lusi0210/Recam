using System;
using Remp.DataAccess.Collections;
using Remp.Models.Documents;
using Remp.Repository.Interfaces;

namespace Remp.Repository.Repositories;

public class UserActivityLogRepository : IUserActivityLogRepository
{
    private readonly MongoDbContext _context;

    public UserActivityLogRepository(MongoDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(UserActivityLog log)
    {
        await _context.UserActivityLogs.InsertOneAsync(log);
    }
}
