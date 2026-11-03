using System;

namespace Remp.Service.DTOs;

public class DashboardResponseDto
{
    public DashboardStatisticsDto Statistics { get; set; } = new DashboardStatisticsDto();
    public List<DashboardActivityDto> LatestActivities { get; set; } = new List<DashboardActivityDto>();
}