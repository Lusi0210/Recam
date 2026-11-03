using System;
using Remp.Service.DTOs;

namespace Remp.Service.Interfaces;

public interface IDashboardService
{
    Task<DashboardResponseDto> GetDashboardAsync(string userId, string role);
}