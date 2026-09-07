using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Remp.Common;
using Remp.Models.Entities;
using Remp.Repository.Interfaces;
using Remp.Service.DTOs;
using Remp.Service.Interfaces;

namespace Remp.Service.Services;

public class MediaAssetsService : IMediaAssetsService
{
    private readonly IBlobStorageService _blobStorage;
    private readonly IMediaAssetRepository _mediaAssetRepo;
    private readonly IListingCaseRepository _listingCaseRepo;
    public MediaAssetsService(IBlobStorageService blobStorage,IMediaAssetRepository mediaAssetRepo,IListingCaseRepository listingCaseRepo)
    {
        _blobStorage = blobStorage;
        _mediaAssetRepo = mediaAssetRepo;
        _listingCaseRepo = listingCaseRepo;
    }

    public async Task<ApiResponse<List<UploadMediaAssetsResponseDto>>> UploadMediaAssetAsync(UploadMediaAssetsDto dto,string userId)
    {
        if(dto.Files == null || dto.Files.Count == 0)
        {
            return ApiResponse<List<UploadMediaAssetsResponseDto>>.FailureResponse("At least one file is requied!");
        }

        if(dto.MediaType != Models.Enums.MediaType.Photos && dto.Files.Count > 1)
        {
            return ApiResponse<List<UploadMediaAssetsResponseDto>>.FailureResponse("Only Photos allow multiple files. This media type accepts one file at a time.");
        }

        var listingCase = await _listingCaseRepo.GetByIdAsync(dto.ListingCaseId);
        if(listingCase == null)
        {
            return ApiResponse<List<UploadMediaAssetsResponseDto>>.FailureResponse("The listing case does not exist!");
        }

        var mediaAssets = new List<MediaAsset>();

        foreach (var file in dto.Files)
        {
            var blobUrl =await _blobStorage.UploadFileAsync(file);
            mediaAssets.Add(new MediaAsset
            {
                MediaType = dto.MediaType,
                MediaUrl = blobUrl,
                UploadedAt = DateTime.UtcNow,
                IsSelect = false,
                IsHero = false,
                ListingCaseId = dto.ListingCaseId,
                UserId = userId,
                IsDeleted = false
            });
        }

        var saved = await _mediaAssetRepo.AddRangeAsync(mediaAssets);

        var response = saved.Select(m => new UploadMediaAssetsResponseDto
        {
            Id=m.Id,
            MediaUrl = m.MediaUrl,
            MediaType = m.MediaType
        }).ToList();

        return ApiResponse<List<UploadMediaAssetsResponseDto>>.SuccessResponse(response,"Media uploaded successfully.");

    }

    public async Task<DownloadFileResponseDto?> DownloadMediaAssetAsync(int id)
    {
        var media = await _mediaAssetRepo.GetByIdAsync(id);
        if(media == null)
        {
            return null;
        }
        
        var (content,contentType,blobName) = await _blobStorage.DownloadFileAsync(media.MediaUrl);
        
        return new DownloadFileResponseDto
        {
            Content = content,
            ContentType=contentType,
            FileName = blobName
        };
    }

}
