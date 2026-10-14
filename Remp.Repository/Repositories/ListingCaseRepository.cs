using Microsoft.EntityFrameworkCore;
using Remp.Common.Responses;
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

    public async Task<PagedResult<ListingCase>> GetPagedByPhotographyCompanyAsync(int pageNumber, int pageSize,string userId)
    {
        IQueryable<ListingCase> query = _dbContext.ListingCases
                                                .AsNoTracking()
                                                .Where(l => !l.IsDeleted && l.UserId == userId);
        int totalCount = await query.CountAsync();

        List<ListingCase> items = await query
            .OrderByDescending(l => l.CreatedAt)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return new PagedResult<ListingCase>
        {
            Items = items,
            PageNumber = pageNumber,
            PageSize = pageSize,
            TotalCount = totalCount
        };

    }

    public async Task<PagedResult<ListingCase>> GetPagedByAgentAsync(int pageNumber, int pageSize,string userId)
    {
        IQueryable<ListingCase> query = _dbContext.ListingCases
                                                    .AsNoTracking()
                                                    .Where(l => !l.IsDeleted && l.Agents.Any(a=> a.Id == userId));

        int totalCount = await query.CountAsync();

        List<ListingCase> items = await query
            .OrderByDescending(l => l.CreatedAt)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return new PagedResult<ListingCase>
        {
            Items = items,
            PageNumber = pageNumber,
            PageSize = pageSize,
            TotalCount = totalCount
        };

    }

    
}
