using System;

namespace Remp.Service.DTOs;

public class DashboardStatisticsDto
{
    public int Total { get; set; }
    public int Created { get; set; }
    public int Pending { get; set; }
    public int InReview { get; set; }
    public int Delivered { get; set; }
}