using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Remp.DataAccess.Data;
using Remp.Models.Entities;
using Remp.Repository.Interfaces;

namespace Remp.Repository.Repositories
{
    public class MediaAssetRepository : IMediaAssetRepository
    {
        private readonly RempDbContext _context;
        public MediaAssetRepository(RempDbContext context)
        {
            _context=context;
        }

        public async Task<List<MediaAsset>> AddRangeAsync(List<MediaAsset> mediaAssets)
        {
            await _context.MediaAssets.AddRangeAsync(mediaAssets);
            await _context.SaveChangesAsync();
            return mediaAssets;
        }

        public async Task<MediaAsset?> GetByIdAsync(int id)
        {
            return await _context.MediaAssets.FirstOrDefaultAsync(m => m.Id == id && !m.IsDeleted);
        }

        public async Task<List<MediaAsset>> GetByListingCaseIdAsync(int listingCaseId)
        {
            return await _context.MediaAssets.Where(m => m.ListingCaseId == listingCaseId && !m.IsDeleted).ToListAsync();
        }
    }
}