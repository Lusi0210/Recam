using System;
using Remp.Service.DTOs;

namespace Remp.Service.Interfaces;

public interface IAuthService
{
    Task<LoginResponseDto> LoginAsync(LoginRequestDto dto,string? ipAddress);
    Task<RegisterResponseDto> RegisterAsync(RegisterRequestDto dto,string? ipAddress);
    Task<ChangePasswordResponseDto> ChangePasswordAsync(ChangePasswordDto dto, string userId, string? ipAddress);
}