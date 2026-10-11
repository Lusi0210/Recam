using System;
using Remp.Common.Responses;
using Remp.Service.DTOs;

namespace Remp.Service.Interfaces;

public interface IUserService
{
    Task<PagedResult<UserDto>> GetAllUsersAsync(PaginationQueryDto query);
}
