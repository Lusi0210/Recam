using System;
using Remp.Common.Exceptions;
using Remp.Models.Entities;
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
    public AgentService(IAgentRepository agentRepository,IPhotographyCompanyRepository photographyCompanyRepository,IUnitOfWork unitOfWork)
    {
        _agentRepository = agentRepository;
        _photographyCompanyRepository = photographyCompanyRepository;
        _unitOfWork = unitOfWork;
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
}
