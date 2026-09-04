using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Remp.Common;
using Remp.Models.Entities;
using Remp.Repository.Interfaces;
using Remp.Service.DTOs;
using Remp.Service.Interfaces;

namespace Remp.Service.Services
{
    public class AgentService : IAgentService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IAgentRepository _agentRepo;

        public AgentService(UserManager<ApplicationUser> userManager,IAgentRepository agentRepo)
        {
            _userManager=userManager;
            _agentRepo=agentRepo;
        }

        public async Task<ApiResponse<SearchAgentByEmailResponseDto>> SearchAgentByEmail(string email)
        {
            var existing = await _userManager.FindByEmailAsync(email);
            if (existing == null)
            {
                return ApiResponse<SearchAgentByEmailResponseDto>.FailureResponse("The agent not found!");
            }

            string id = existing.Id;
            Agent? agent = await _agentRepo.SearchAgentByIdAsync(id);

            if (agent == null)
            {
                return ApiResponse<SearchAgentByEmailResponseDto>.FailureResponse("The agent not found!");
            }

            SearchAgentByEmailResponseDto responseDto = new SearchAgentByEmailResponseDto
            {
                Id = agent.Id,
                AgentFirstName = agent.AgentFirstName,
                AgentLastName = agent.AgentLastName,
                AvatarUrl = agent.AvatarUrl,
                CompanyName = agent.CompanyName
            };



            return ApiResponse<SearchAgentByEmailResponseDto>.SuccessResponse(responseDto,"Search agent by email successfully!");
        }

    }
}