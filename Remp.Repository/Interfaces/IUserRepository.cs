using System;
using Remp.Common.Responses;
using Remp.Models.Entities;
using Remp.Models.Projections;

namespace Remp.Repository.Interfaces;

public interface IUserRepository
{
    Task<PagedResult<UserWithRole>> GetPagedAsync(int pageNumber, int pageSize);
    Task<User?> GetCurrnetUserAsync(string userId);
}
