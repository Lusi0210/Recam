using System;
using System.Collections.Generic;
using System.Data;
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
    private readonly ITokenService _tokenService;

    public AuthService(UserManager<ApplicationUser> userManager,ITokenService tokenService)
    {
        _userManager = userManager;
        _tokenService = tokenService;
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

    public async Task<ApiResponse<string>> LoginAsync (LoginDto dto)
    {
        var existingUser = await _userManager.FindByEmailAsync(dto.UserName);
        
        if (existingUser == null )
        {
            return ApiResponse<string>.FailureResponse("Invalid email or password");
        }

        var result = await _userManager.CheckPasswordAsync(existingUser, dto.Password);

        if (!result)
        {
            return ApiResponse<string>.FailureResponse("Invalid email or Password!");
        }

        var roles = await _userManager.GetRolesAsync(existingUser);
        var token = _tokenService.GenerateToken(existingUser,roles);

        return ApiResponse<string>.SuccessResponse(token,"Login successful!");
    }
}
