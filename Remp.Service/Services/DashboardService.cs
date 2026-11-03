using System;
using Remp.Models.Documents;
using Remp.Models.Enums;
using Remp.Repository.Interfaces;
using Remp.Service.DTOs;
using Remp.Service.Interfaces;

namespace Remp.Service.Services;

public class DashboardService : IDashboardService
{
    private const int LatestActivityLimit = 10;

    private readonly IListingCaseRepository _listingCaseRepository;
    private readonly IListingCaseHistoryRepository _listingCaseHistoryRepository;

    public DashboardService(
        IListingCaseRepository listingCaseRepository,
        IListingCaseHistoryRepository listingCaseHistoryRepository)
    {
        _listingCaseRepository = listingCaseRepository;
        _listingCaseHistoryRepository = listingCaseHistoryRepository;
    }

    public async Task<DashboardResponseDto> GetDashboardAsync(string userId, string role)
    {
        Dictionary<ListcaseStatus, int> statusCounts = await _listingCaseRepository.GetStatusCountsAsync(userId, role);
        Dictionary<int, string> titles = await _listingCaseRepository.GetTitlesAsync(userId, role);

        List<ListingCaseHistory> histories = titles.Count == 0
            ? new List<ListingCaseHistory>()
            : await _listingCaseHistoryRepository.GetLatestByListingCaseIdsAsync(titles.Keys.ToList(), LatestActivityLimit);

        return new DashboardResponseDto
        {
            Statistics = new DashboardStatisticsDto
            {
                Total = statusCounts.Values.Sum(),
                Created = statusCounts.GetValueOrDefault(ListcaseStatus.Created),
                Pending = statusCounts.GetValueOrDefault(ListcaseStatus.Pending),
                InReview = statusCounts.GetValueOrDefault(ListcaseStatus.InReview),
                Delivered = statusCounts.GetValueOrDefault(ListcaseStatus.Delivered),
            },
            LatestActivities = histories.Select(h => new DashboardActivityDto
            {
                ListingCaseId = h.ListingCaseId,
                ListingCaseTitle = titles[h.ListingCaseId],
                Action = h.Action.ToString(),
                Message = h.Message,
                Timestamp = h.Timestamp,
            }).ToList(),
        };
    }
}