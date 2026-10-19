using System;
using Microsoft.EntityFrameworkCore;
using Remp.DataAccess.Data;
using Remp.Models.Entities;
using Remp.Repository.Interfaces;

namespace Remp.Repository.Repositories;

public class MediaAssetRepository : IMediaAssetRepository
{
    private readonly RempDbContext _dbContext;

    public MediaAssetRepository(RempDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public void Add(MediaAsset mediaAsset)
    {
        _dbContext.MediaAssets.Add(mediaAsset);
    }

    public async Task<MediaAsset?> GetMediaAssetWithListingCaseAsync(int id)
    {
        return await _dbContext.MediaAssets
            .AsNoTracking()
            .Include(m => m.ListingCase)
                .ThenInclude(l => l.Agents)
            .FirstOrDefaultAsync(m => m.Id == id && !m.IsDeleted && !m.ListingCase.IsDeleted);
    }
}
