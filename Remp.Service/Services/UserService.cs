using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Remp.Common;
using Remp.Models.Entities;
using Remp.Service.DTOs;
using Remp.Service.Interfaces;

namespace Remp.Service.Services;

public class UserService : IUserService
{
    private readonly UserManager<ApplicationUser> _userManager;

    public UserService(UserManager<ApplicationUser> userManager)
    {
        _userManager=userManager;
    }

    public async Task<ApiResponse<List<UserResponseDto>>> GetAllUsersAsync(int pageNumber, int pageSize)
    {
        var users = await _userManager.Users
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        var result = new List<UserResponseDto>();

        foreach (var user in users)
        {
            var roles = await _userManager.GetRolesAsync(user);   // 查这个用户的角色

            result.Add(new UserResponseDto
            {
                Id = user.Id,
                Email = user.Email!,
                UserName = user.UserName!,
                CreatedAt = user.CreatedAt,
                Roles = roles
            });
        }

        return ApiResponse<List<UserResponseDto>>.SuccessResponse(result, "Users retrieved successfully");

    }

}
