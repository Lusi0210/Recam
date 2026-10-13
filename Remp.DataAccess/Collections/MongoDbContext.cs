using System;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using Remp.Common.Settings;
using Remp.Models.Documents;

namespace Remp.DataAccess.Collections;

public class MongoDbContext
{
    private readonly IMongoDatabase _database;

    public MongoDbContext(IOptions<MongoDbSettings> options)
    {
        MongoDbSettings settings = options.Value;
        MongoClient client = new MongoClient(settings.ConnectionString);
        _database = client.GetDatabase(settings.DatabaseName);
    }

    public IMongoCollection<UserActivityLog> UserActivityLogs =>
        _database.GetCollection<UserActivityLog>("UserActivityLogs");

    public IMongoCollection<ListingCaseHistory> ListingCaseHistories =>
        _database.GetCollection<ListingCaseHistory>("ListingCaseHistories");
}
