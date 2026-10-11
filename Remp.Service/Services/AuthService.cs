using System;
using Remp.Models.Entities;
using Microsoft.AspNetCore.Identity;
using Remp.Service.Interfaces;
using Remp.Service.DTOs;
using Remp.Common.Exceptions;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.Extensions.Logging;
using Remp.Repository.Interfaces;
using Remp.Models.Documents;
using Remp.Models.Enums;


namespace Remp.Service.Services;

public class AuthService : IAuthService
{
    private readonly UserManager<User> _userManager;
    private readonly ITokenService _tokenService;
    private readonly IValidator<LoginRequestDto> _validator;
    private readonly IUserActivityLogRepository _activityLogRepository;
    private readonly ILogger<AuthService> _logger;
    public AuthService(UserManager<User> userManager,ITokenService tokenService,IValidator<LoginRequestDto> validator,IUserActivityLogRepository activityLogRepository,ILogger<AuthService> logger)
    {
        _userManager = userManager;
        _tokenService = tokenService;
        _validator = validator;
        _activityLogRepository = activityLogRepository;
        _logger = logger;
    }

    private async Task LogLoginAsync(string email, string? userId, bool success, string message, string? ipAddress)
    {
        UserActivityLog log = new UserActivityLog
        {
            UserId = userId,
            Email = email,
            Action = UserActivityType.Login,
            Success = success,
            Message = message,
            IpAddress = ipAddress,
            Timestamp = DateTime.UtcNow
        };

        try
        {
            await _activityLogRepository.AddAsync(log);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to write user activity log for {Email}", email);
        }
    }

    public async Task<LoginResponseDto> LoginAsync(LoginRequestDto dto,string? ipAddress)
    {
        ValidationResult validationResult = await _validator.ValidateAsync(dto);
        if (!validationResult.IsValid)
        {
            List<string> errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
            throw new BadRequestException("Validation failed", errors);
        }

        User? user = await _userManager.FindByEmailAsync(dto.Email);
        if (user == null)
        {
            await LogLoginAsync(dto.Email, null, false, "User not found", ipAddress);
            throw new UnauthorizedException("Invalid email or password");
        }

        if (user.IsDeleted)
        {
            await LogLoginAsync(dto.Email, user.Id, false, "User is deleted", ipAddress);
            throw new UnauthorizedException("Invalid email or password");
        }

        bool passwordIsCorrect = await _userManager.CheckPasswordAsync(user,dto.Password);
        if (!passwordIsCorrect)
        {
            await LogLoginAsync(dto.Email, user.Id, false, "Wrong password", ipAddress);
            throw new UnauthorizedException("Invalid email or password");
        }

        IList<string> roles =  await _userManager.GetRolesAsync(user);
        string? role = roles.FirstOrDefault();

        if(role == null)
        {
            await LogLoginAsync(dto.Email, user.Id, false, "User has no role", ipAddress);
            throw new UnauthorizedException("Invalid email or password");
        }

        string token = _tokenService.GenerateToken(user,role);

        await LogLoginAsync(dto.Email, user.Id, true, "Login successful", ipAddress);

        LoginResponseDto loginResponseDto = new LoginResponseDto
        {
            Token = token,
            UserId = user.Id,
            Email = user.Email!,
            Role = role
        };


        return loginResponseDto;
    }
}
