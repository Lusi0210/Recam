using System;
using Remp.Models.Documents;

namespace Remp.Repository.Interfaces;

public interface IListingCaseHistoryRepository
{
    Task AddAsync(ListingCaseHistory log);
}
