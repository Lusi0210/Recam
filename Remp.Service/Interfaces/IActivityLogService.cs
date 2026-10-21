using System;
using Remp.Models.Enums;

namespace Remp.Service.Interfaces;

public interface IActivityLogService
{
    Task LogListingCaseAsync(ListingCaseActivityType action, int listingCaseId, string? userId, bool success, string? message, string? ipAddress);
    Task LogPhotographyCompanyAsync(PhotographyCompanyActivityType action, string email, string? userId, bool success, string message, string? ipAddress);
    Task LogUserAsync(UserActivityType action, string email, string? userId, bool success, string message, string? ipAddress);
}
