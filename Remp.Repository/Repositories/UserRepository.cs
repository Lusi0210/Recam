using System;
using Remp.Common.Responses;
using Remp.DataAccess.Data;
using Remp.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Remp.Repository.Interfaces;
using Remp.Models.Projections;
using Microsoft.AspNetCore.Identity;

namespace Remp.Repository.Repositories;

public class UserRepository : IUserRepository
{
    private readonly RempDbContext _context;

    public UserRepository(RempDbContext context)
    {
        _context = context;
    }

    public async Task<PagedResult<UserWithRole>> GetPagedAsync(int pageNumber, int pageSize)
    {
        IQueryable<User> query = _context.Users.Where(u => !u.IsDeleted);

        int totalCount = await query.CountAsync();

        List<UserWithRole> items = await query
            .OrderByDescending(u => u.CreatedAt)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .Select(u => new UserWithRole
            {
                User = u,
                Role = _context.UserRoles
                    .Where(ur => ur.UserId == u.Id)
                    .Join(_context.Roles, ur => ur.RoleId, r => r.Id, (ur, r) => r.Name)
                    .FirstOrDefault()
            })
            .ToListAsync();

        return new PagedResult<UserWithRole>
        {
            Items = items,
            PageNumber = pageNumber,
            PageSize = pageSize,
            TotalCount = totalCount
        };
    }

    public async Task<User?> GetCurrnetUserAsync(string userId)
    {
        User? user = await _context.Users.AsNoTracking()
                                        .Include(u => u.ListingCases.Where(l => !l.IsDeleted))
                                        .Include(u => u.Agent).ThenInclude(a => a!.ListingCases.Where(l => !l.IsDeleted))
                                        .AsSplitQuery()
                                        .FirstOrDefaultAsync(u => u.Id ==userId && !u.IsDeleted);                 
        return user;
        
    }
}
