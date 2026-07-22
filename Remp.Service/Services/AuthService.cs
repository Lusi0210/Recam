using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Remp.Common;
using Remp.Models.Entities;
using Remp.Service.DTOs;
using Remp.Service.Interfaces;

namespace Remp.Service.Services;

public class AuthService : IAuthService
{
    private readonly UserManager<ApplicationUser> _userManager;

    public AuthService(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }
    public async Task<ApiResponse<string>> RegisterAsync (RegisterDto dto)
    {
        var existingUser = await _userManager.FindByEmailAsync(dto.Email);
        if(existingUser != null)
        {
            return ApiResponse<string>.FailureResponse("Email is already registered");
        }

        var user = new ApplicationUser
        {
            UserName=dto.Email,
            Email=dto.Email,
            IsDeleted=false,
            CreatedAt=DateTime.UtcNow
        };

        var result= await _userManager.CreateAsync(user,dto.Password);
        
        if (!result.Succeeded)
        {
            var errors = result.Errors.Select(e => e.Description).ToList();
            return ApiResponse<string>.FailureResponse("Registration failed",errors);
        } 

        await _userManager.AddToRoleAsync(user,"Agent");



        return ApiResponse<string>.SuccessResponse(user.Id, "Registration successful");
    }
}
