using System;
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
}
