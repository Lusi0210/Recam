using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Remp.Models.Entities;

namespace Remp.Repository.Interfaces;

public interface IMediaAssetRepository
{
    Task<List<MediaAsset>> AddRangeAsync(List<MediaAsset> mediaAssets);
    Task<MediaAsset?> GetByIdAsync(int id);
    Task<List<MediaAsset>> GetByListingCaseIdAsync(int listingCaseId);
    Task<MediaAsset> UpdateByIdAsync(MediaAsset mediaAsset);
    Task SaveChangesAsync();
}
