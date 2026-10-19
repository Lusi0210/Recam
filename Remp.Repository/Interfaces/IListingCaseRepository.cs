using System;
using Remp.Common.Responses;
using Remp.Models.Entities;

namespace Remp.Repository.Interfaces;

public interface IListingCaseRepository
{
    void Add(ListingCase listingCase);
    Task<PagedResult<ListingCase>> GetPagedByPhotographyCompanyAsync(int pageNumber, int pageSize,string userId);
    Task<PagedResult<ListingCase>> GetPagedByAgentAsync(int pageNumber, int pageSize,string userId);
    Task<ListingCase?> GetListingCaseAsync(int id);
    Task<ListingCase?> GetListingCaseWithMediaAssetAsync(int id);
    Task<ListingCase?> GetListingCaseDetailsByIdAsync(int id);
    Task<ListingCase?> GetListingCaseWithContactsAsync(int id);
    Task<ListingCase?> GetListingCaseWithAgentAsync(int id);
}
