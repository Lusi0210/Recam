using Microsoft.EntityFrameworkCore;
using Remp.Common.Responses;
using Remp.DataAccess.Data;
using Remp.Models.Entities;
using Remp.Models.Filters;
using Remp.Repository.Interfaces;

namespace Remp.Repository.Repositories;

public class ListingCaseRepository : IListingCaseRepository
{
    private readonly RempDbContext _dbContext;
    public ListingCaseRepository(RempDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    private static IQueryable<ListingCase> ApplyFilters(IQueryable<ListingCase> query, ListingCaseFilter filter)
    {
        if (filter.Status.HasValue)
        {
            query = query.Where(l => l.ListcaseStatus == filter.Status.Value);
        }

        if (filter.PropertyType.HasValue)
        {
            query = query.Where(l => l.PropertyType == filter.PropertyType.Value);
        }

        if (filter.SaleCategory.HasValue)
        {
            query = query.Where(l => l.SaleCategory == filter.SaleCategory.Value);
        }

        if (!string.IsNullOrWhiteSpace(filter.City))
        {
            query = query.Where(l => l.City.Contains(filter.City));
        }

        if (filter.MinPrice.HasValue)
        {
            query = query.Where(l => l.Price >= filter.MinPrice.Value);
        }

        if (filter.MaxPrice.HasValue)
        {
            query = query.Where(l => l.Price <= filter.MaxPrice.Value);
        }

        if (!string.IsNullOrWhiteSpace(filter.Search))
        {
            query = query.Where(l => l.Title.Contains(filter.Search)
                                || l.Street.Contains(filter.Search)
                                || l.City.Contains(filter.Search));
        }

        return query;
    }

    private static IQueryable<ListingCase> ApplySorting(IQueryable<ListingCase> query, ListingCaseFilter filter)
    {
        bool descending = filter.SortDirection?.ToLower() == "desc";

        IOrderedQueryable<ListingCase> ordered = filter.SortBy?.ToLower() switch
        {
            "title"  => descending ? query.OrderByDescending(l => l.Title)          : query.OrderBy(l => l.Title),
            "city"   => descending ? query.OrderByDescending(l => l.City)           : query.OrderBy(l => l.City),
            "price"  => descending ? query.OrderByDescending(l => l.Price)          : query.OrderBy(l => l.Price),
            "status" => descending ? query.OrderByDescending(l => l.ListcaseStatus) : query.OrderBy(l => l.ListcaseStatus),
            _        => query.OrderByDescending(l => l.CreatedAt)
        };

        return ordered.ThenBy(l => l.Id);
    }

    public void Add(ListingCase listingCase)
    {
        _dbContext.ListingCases.Add(listingCase);
    }

    public async Task<PagedResult<ListingCase>> GetPagedByPhotographyCompanyAsync(int pageNumber, int pageSize,string userId,ListingCaseFilter filter)
    {
        IQueryable<ListingCase> query = _dbContext.ListingCases
                                                .AsNoTracking()
                                                .Where(l => !l.IsDeleted && l.UserId == userId);
        query = ApplyFilters(query, filter);
        
        int totalCount = await query.CountAsync();

        query = ApplySorting(query, filter);

        List<ListingCase> items = await query
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

    public async Task<PagedResult<ListingCase>> GetPagedByAgentAsync(int pageNumber, int pageSize,string userId,ListingCaseFilter filter)
    {
        IQueryable<ListingCase> query = _dbContext.ListingCases
                                                    .AsNoTracking()
                                                    .Where(l => !l.IsDeleted && l.Agents.Any(a=> a.Id == userId));

        query = ApplyFilters(query, filter);

        int totalCount = await query.CountAsync();

        query = ApplySorting(query, filter);

        List<ListingCase> items = await query
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

    public async Task<ListingCase?> GetListingCaseAsync(int id)
    {
        ListingCase? listingCase = await _dbContext.ListingCases.FirstOrDefaultAsync(l=>l.Id ==id && !l.IsDeleted);
        return listingCase;
    }

    public async Task<ListingCase?> GetListingCaseWithMediaAssetAsync(int id)
    {
        ListingCase? listingCase = await _dbContext.ListingCases
                                                    .Include(l => l.MediaAssets.Where(m => !m.IsDeleted))
                                                    .FirstOrDefaultAsync(l=>l.Id ==id && !l.IsDeleted);
        return listingCase;
    }

    public async Task<ListingCase?> GetListingCaseDetailsByIdAsync(int id)
    {
        ListingCase? listingCase = await _dbContext.ListingCases.AsNoTracking()
                                                                .Include(l => l.MediaAssets.Where(m => !m.IsDeleted))
                                                                .Include(l => l.Agents).ThenInclude(a => a.User)
                                                                .AsSplitQuery()
                                                                .FirstOrDefaultAsync(l => l.Id == id && !l.IsDeleted);
        return listingCase;
    }

    public async Task<ListingCase?> GetListingCaseWithContactsAsync(int id)
    {
        return await _dbContext.ListingCases
            .AsNoTracking()
            .Include(l => l.Agents)
            .Include(l => l.CaseContacts)
            .AsSplitQuery()
            .FirstOrDefaultAsync(l => l.Id == id && !l.IsDeleted);
    }
    
    public async Task<ListingCase?> GetListingCaseWithAgentAsync(int id)
    {
        return await _dbContext.ListingCases
            .Include(l => l.Agents)
            .FirstOrDefaultAsync(l => l.Id == id && !l.IsDeleted);
    }

    public async Task<ListingCase?> GetListingCaseWithMediaAndAgentsAsync(int id)
    {
        return await _dbContext.ListingCases
            .Include(l => l.MediaAssets.Where(m => !m.IsDeleted))
            .Include(l => l.Agents)
            .AsSplitQuery()
            .FirstOrDefaultAsync(l => l.Id == id && !l.IsDeleted);
    }
}