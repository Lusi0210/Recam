using System;
using System.Security.Cryptography;
using AutoMapper;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Remp.Common.Exceptions;
using Remp.Models.Constants;
using Remp.Models.Documents;
using Remp.Models.Entities;
using Remp.Models.Enums;
using Remp.Repository.Common;
using Remp.Repository.Interfaces;
using Remp.Service.DTOs;
using Remp.Service.Interfaces;

namespace Remp.Service.Services;

public class AgentService : IAgentService
{
    private readonly IAgentRepository _agentRepository;
    private readonly IPhotographyCompanyRepository _photographyCompanyRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IValidator<CreateAgentByPhotographyCompanyDto> _createAgentDtoValidator;
    private readonly UserManager<User> _userManager;
    private readonly ILogger<AgentService> _logger;
    private readonly IPhotographyCompanyActivityLogRepository _photographyCompanyActivityLogRepository;
    private readonly IEmailService _emailService;
    private readonly IMapper _mapper;
    public AgentService(
        IAgentRepository agentRepository,
        IPhotographyCompanyRepository photographyCompanyRepository,
        IUnitOfWork unitOfWork,
        IValidator<CreateAgentByPhotographyCompanyDto> createAgentDtoValidator,
        UserManager<User> userManager,
        ILogger<AgentService> logger,
        IPhotographyCompanyActivityLogRepository photographyCompanyActivityLogRepository,
        IEmailService emailService,
        IMapper mapper)
    {
        _agentRepository = agentRepository;
        _photographyCompanyRepository = photographyCompanyRepository;
        _unitOfWork = unitOfWork;
        _createAgentDtoValidator = createAgentDtoValidator;
        _userManager = userManager;
        _logger = logger;
        _photographyCompanyActivityLogRepository = photographyCompanyActivityLogRepository;
        _emailService = emailService;
        _mapper = mapper;
    }

    private static string GenerateRandomPassword(int length = 12)
    {
        const string upper = "ABCDEFGHJKLMNPQRSTUVWXYZ";
        const string lower = "abcdefghijkmnpqrstuvwxyz";
        const string digits = "23456789";
        const string special = "!@#$%^&*";
        string all = upper + lower + digits + special;

        List<char> chars = new List<char>
        {
            upper[RandomNumberGenerator.GetInt32(upper.Length)],
            lower[RandomNumberGenerator.GetInt32(lower.Length)],
            digits[RandomNumberGenerator.GetInt32(digits.Length)],
            special[RandomNumberGenerator.GetInt32(special.Length)]
        };

        while (chars.Count < length)
        {
            chars.Add(all[RandomNumberGenerator.GetInt32(all.Length)]);
        }

        for (int i = chars.Count - 1; i > 0; i--)
        {
            int j = RandomNumberGenerator.GetInt32(i + 1);
            (chars[i], chars[j]) = (chars[j], chars[i]);
        }

        return new string(chars.ToArray());
    }

    private async Task LogActivityAsync(PhotographyCompanyActivityType action,string email, string? userId, bool success, string message, string? ipAddress)
    {
        PhotographyCompanyActivityLog log = new PhotographyCompanyActivityLog
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
            await _photographyCompanyActivityLogRepository.AddAsync(log);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to write photography company activity log for {Email}", email);
        }
    } 
 
    public async Task<string> AddAgentToPhotographyCompanyAsync(string userId,AddAgentToPhotographyCompanyDto dto)
    {
        PhotographyCompany? photographyCompany = await _photographyCompanyRepository.GetPhotographyCompanyByIdAsync(userId);
        if(photographyCompany == null)
        {
            throw new NotFoundException("The photography company does not exist!");
        }

        Agent? agent = await _agentRepository.GetAgentByIdAsync(dto.Id);
        if(agent == null)
        {
            throw new NotFoundException("The agent does not exist!");
        }

        if (agent.PhotographyCompanies.Any(p => p.Id == userId))
        {
            throw new BadRequestException("This agent has already been added to your company.");
        }

        agent.PhotographyCompanies.Add(photographyCompany);
        await _unitOfWork.SaveChangesAsync();

        return dto.Id;
    }

    public async Task<CreateAgentByPhotographyCompanyResponseDto> CreateAgentByPhotographyCompanyAsync(CreateAgentByPhotographyCompanyDto dto,string userId,string? ipAddress)
    {
        ValidationResult validationResult = await _createAgentDtoValidator.ValidateAsync(dto);
        if (!validationResult.IsValid)
        {
            List<string> errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
            throw new BadRequestException("Validation failed", errors);
        }

        User? existingUser = await _userManager.FindByEmailAsync(dto.Email);
        if (existingUser!=null)
        {
            await LogActivityAsync(PhotographyCompanyActivityType.CreateAgent,dto.Email,userId,false,"Email already registered",ipAddress);
            throw new BadRequestException("Email is already registered");
        }

        User user = new User
        {
            UserName = dto.Email,
            Email = dto.Email,
            CreatedAt = DateTime.UtcNow,
            IsDeleted = false
        };

        string password = GenerateRandomPassword();
        

        await _unitOfWork.BeginTransactionAsync();
        try
        {
            IdentityResult createResult =  await _userManager.CreateAsync(user,password);
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

            PhotographyCompany? company = await _photographyCompanyRepository.GetPhotographyCompanyByIdAsync(userId);
            if(company == null)
            {
                throw new NotFoundException("The photography company does not exist!");
            }

            agent.PhotographyCompanies.Add(company);

            await _unitOfWork.SaveChangesAsync();
            await _unitOfWork.CommitAsync();

        }
        catch
        {
            await _unitOfWork.RollbackAsync();
            throw;
        }

        await _emailService.SendAsync(dto.Email, "Your RECAM account",$"Your account has been created.\nEmail: {dto.Email}\nPassword: {password}");

        await LogActivityAsync(PhotographyCompanyActivityType.CreateAgent,dto.Email,userId,true,"Agent created successfully",ipAddress);

        
        

        return new CreateAgentByPhotographyCompanyResponseDto
        {
            UserId = user.Id,
            Email = user.Email!,
            FirstName = dto.AgentFirstName,
            LastName = dto.AgentLastName
        };
    }

    public async Task<GetAgentResponseDto> SearchAgentByEmailAsync(string email)
    {
        Agent? agent = await _agentRepository.GetAgentByEmailAsync(email);
        if(agent == null)
        {
            throw new NotFoundException("The agent does not exist!");
        }


        return _mapper.Map<GetAgentResponseDto>(agent);
    }

    public async Task<List<GetAgentResponseDto>> GetAllAgentsUnderThisAccountAsync(string userId)
    {
        List<Agent> listOfAgent = await _agentRepository.GetAllAgentsUnderThisAccountAsync(userId);

        List<GetAgentResponseDto> responseDtos = _mapper.Map<List<GetAgentResponseDto>>(listOfAgent);
        return responseDtos;    
    }
}
