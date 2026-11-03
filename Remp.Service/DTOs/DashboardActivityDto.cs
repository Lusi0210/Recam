using System;

namespace Remp.Service.DTOs;

public class DashboardActivityDto
{
    public int ListingCaseId { get; set; }
    public string ListingCaseTitle { get; set; } = null!;
    public string Action { get; set; } = null!;
    public string? Message { get; set; }
    public DateTime Timestamp { get; set; }
}