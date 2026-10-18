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
using Remp.Repository.Common;
using Remp.Models.Constants;


namespace Remp.Service.Services;

public class AuthService : IAuthService
{
    private readonly UserManager<User> _userManager;
    private readonly ITokenService _tokenService;
    private readonly IValidator<LoginRequestDto> _loginRequestValidator;
    private readonly IValidator<RegisterRequestDto> _registerRequestValidator;
    private readonly IUserActivityLogRepository _activityLogRepository;
    private readonly ILogger<AuthService> _logger;
    private readonly IAgentRepository _agentRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IValidator<ChangePasswordDto> _changePasswordValidator;
    public AuthService(
        UserManager<User> userManager,
        ITokenService tokenService,
        IValidator<LoginRequestDto> loginRequestValidator,
        IValidator<RegisterRequestDto> registerRequestValidator,
        IUserActivityLogRepository activityLogRepository,
        ILogger<AuthService> logger,
        IAgentRepository agentRepository,
        IUnitOfWork unitOfWork,
        IValidator<ChangePasswordDto> changePasswordValidator)
    {
        _userManager = userManager;
        _tokenService = tokenService;
        _loginRequestValidator = loginRequestValidator;
        _registerRequestValidator = registerRequestValidator;
        _activityLogRepository = activityLogRepository;
        _logger = logger;
        _agentRepository = agentRepository;
        _unitOfWork = unitOfWork;
        _changePasswordValidator = changePasswordValidator;
    }

    private async Task LogActivityAsync(UserActivityType action,string email, string? userId, bool success, string message, string? ipAddress)
    {
        UserActivityLog log = new UserActivityLog
        {
            UserId = userId,
            Email = email,
            Action = action,
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
        ValidationResult validationResult = await _loginRequestValidator.ValidateAsync(dto);
        if (!validationResult.IsValid)
        {
            List<string> errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
            throw new BadRequestException("Validation failed", errors);
        }

        User? user = await _userManager.FindByEmailAsync(dto.Email);
        if (user == null)
        {
            await LogActivityAsync(UserActivityType.Login,dto.Email, null, false, "User not found", ipAddress);
            throw new UnauthorizedException("Invalid email or password");
        }

        if (user.IsDeleted)
        {
            await LogActivityAsync(UserActivityType.Login,dto.Email, user.Id, false, "User is deleted", ipAddress);
            throw new UnauthorizedException("Invalid email or password");
        }

        bool passwordIsCorrect = await _userManager.CheckPasswordAsync(user,dto.Password);
        if (!passwordIsCorrect)
        {
            await LogActivityAsync(UserActivityType.Login,dto.Email, user.Id, false, "Wrong password", ipAddress);
            throw new UnauthorizedException("Invalid email or password");
        }

        IList<string> roles =  await _userManager.GetRolesAsync(user);
        string? role = roles.FirstOrDefault();

        if(role == null)
        {
            await LogActivityAsync(UserActivityType.Login,dto.Email, user.Id, false, "User has no role", ipAddress);
            throw new UnauthorizedException("Invalid email or password");
        }

        string token = _tokenService.GenerateToken(user,role);

        await LogActivityAsync(UserActivityType.Login,dto.Email, user.Id, true, "Login successful", ipAddress);

        LoginResponseDto loginResponseDto = new LoginResponseDto
        {
            Token = token,
            UserId = user.Id,
            Email = user.Email!,
            Role = role
        };


        return loginResponseDto;
    }

    public async Task<RegisterResponseDto> RegisterAsync(RegisterRequestDto dto,string? ipAddress)
    {
        ValidationResult validationResult = await _registerRequestValidator.ValidateAsync(dto);
        if (!validationResult.IsValid)
        {
            List<string> errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
            throw new BadRequestException("Validation failed", errors);
        }

        User? existingUser = await _userManager.FindByEmailAsync(dto.Email);
        if (existingUser!=null)
        {
            await LogActivityAsync(UserActivityType.Register,dto.Email,null,false,"Email already registered",ipAddress);
            throw new BadRequestException("Email is already registered");
        }

        User user = new User
        {
            UserName = dto.Email,
            Email = dto.Email,
            CreatedAt = DateTime.UtcNow,
            IsDeleted = false
        };

        await _unitOfWork.BeginTransactionAsync();
        try
        {
            IdentityResult createResult =  await _userManager.CreateAsync(user,dto.Password);
            if (!createResult.Succeeded)
            {
                List<string> errors = createResult.Errors.Select(e => e.Description).ToList();
                throw new BadRequestException("Registration failed",errors);
            }

            IdentityResult roleResult = await _userManager.AddToRoleAsync(user,RoleNames.Agent);
            if (!roleResult.Succeeded)
            {
                throw new InvalidOperationException("Failed to assign Agent role to the new user.");
            }

            Agent agent = new Agent
            {
                Id = user.Id,
                AgentFirstName = dto.AgentFirstName,
                AgentLastName = dto.AgentLastName, 
                CompanyName = dto.CompanyName
            };
            _agentRepository.Add(agent);

            await _unitOfWork.SaveChangesAsync();
            await _unitOfWork.CommitAsync();

        }
        catch
        {
            await _unitOfWork.RollbackAsync();
            throw;
        }

        await LogActivityAsync(UserActivityType.Register,dto.Email,user.Id,true,"Registration successful",ipAddress);

        return new RegisterResponseDto
        {
            UserId = user.Id,
            Email = user.Email!,
            FirstName = dto.AgentFirstName,
            LastName = dto.AgentLastName
        };
    }

    public async Task<ChangePasswordResponseDto> ChangePasswordAsync(ChangePasswordDto dto, string userId, string? ipAddress)
    {
        ValidationResult validationResult = await _changePasswordValidator.ValidateAsync(dto);
        if (!validationResult.IsValid)
        {
            List<string> errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
            throw new BadRequestException("Validation failed", errors);
        }

        User? user = await _userManager.FindByIdAsync(userId);

        if (user == null || user.IsDeleted)
        {
            throw new NotFoundException("The user does not exist!");
        }

        IdentityResult result = await _userManager.ChangePasswordAsync(user,dto.CurrentPassword,dto.NewPassword);
        if (!result.Succeeded)
        {
            List<string> errors = result.Errors.Select(e => e.Description).ToList();
            await LogActivityAsync(UserActivityType.UpdatePassword, user.Email!, user.Id, false, "Update password failed", ipAddress);
            throw new BadRequestException("Update password failed", errors);
        }

        await LogActivityAsync(UserActivityType.UpdatePassword,user.Email!,user.Id,true,"Update password successful!",ipAddress);

        return new ChangePasswordResponseDto
        {
            UserId = user.Id,
            Email = user.Email!,
        };
    }
}
