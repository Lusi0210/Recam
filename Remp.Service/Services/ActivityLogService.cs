using System;
using Microsoft.Extensions.Logging;
using Remp.Models.Documents;
using Remp.Models.Enums;
using Remp.Repository.Interfaces;
using Remp.Service.Interfaces;

namespace Remp.Service.Services;

public class ActivityLogService : IActivityLogService
{
    private readonly IListingCaseHistoryRepository _listingCaseHistoryRepository;
    private readonly IPhotographyCompanyActivityLogRepository _photographyCompanyActivityLogRepository;
    private readonly IUserActivityLogRepository _userActivityLogRepository;
    private readonly ILogger<ActivityLogService> _logger;

    public ActivityLogService(
        IListingCaseHistoryRepository listingCaseHistoryRepository,
        IPhotographyCompanyActivityLogRepository photographyCompanyActivityLogRepository,
        IUserActivityLogRepository userActivityLogRepository,
        ILogger<ActivityLogService> logger)
    {
        _listingCaseHistoryRepository = listingCaseHistoryRepository;
        _photographyCompanyActivityLogRepository = photographyCompanyActivityLogRepository;
        _userActivityLogRepository = userActivityLogRepository;
        _logger = logger;
    }

    public async Task LogListingCaseAsync(ListingCaseActivityType action, int listingCaseId, string? userId, bool success, string? message, string? ipAddress)
    {
        ListingCaseHistory log = new ListingCaseHistory
        {
            Action = action,
            ListingCaseId = listingCaseId,
            UserId = userId,
            Success = success,
            Message = message,
            IpAddress = ipAddress,
            Timestamp = DateTime.UtcNow
        };

        try
        {
            await _listingCaseHistoryRepository.AddAsync(log);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to write listing case history for case {ListingCaseId}", listingCaseId);
        }
    }

    public async Task LogPhotographyCompanyAsync(PhotographyCompanyActivityType action, string email, string? userId, bool success, string message, string? ipAddress)
    {
        PhotographyCompanyActivityLog log = new PhotographyCompanyActivityLog
        {
            Action = action,
            Email = email,
            UserId = userId,
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

    public async Task LogUserAsync(UserActivityType action, string email, string? userId, bool success, string message, string? ipAddress)
    {
        UserActivityLog log = new UserActivityLog
        {
            Action = action,
            Email = email,
            UserId = userId,
            Success = success,
            Message = message,
            IpAddress = ipAddress,
            Timestamp = DateTime.UtcNow
        };

        try
        {
            await _userActivityLogRepository.AddAsync(log);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to write user activity log for {Email}", email);
        }
    }
}
