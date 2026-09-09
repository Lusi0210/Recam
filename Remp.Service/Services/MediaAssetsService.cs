using System;
using System.Collections.Generic;
using System.IO.Compression;
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

    public async Task<DownloadFileResponseDto?> DownloadListingCaseAsZipAsync(int listingCaseId)
    {
        var listingCase = await _listingCaseRepo.GetByIdAsync(listingCaseId);
        if (listingCase == null)
        {
            return null;
        }

        var mediaAssets = await _mediaAssetRepo.GetByListingCaseIdAsync(listingCaseId);
        if(mediaAssets.Count == 0)
        {
            return null;
        }

        var zipStream = new MemoryStream();
        using (var archive = new ZipArchive(zipStream, ZipArchiveMode.Create, leaveOpen: true))
        {
            foreach (var media in mediaAssets)
            {
                var (content,_,blobName) = await _blobStorage.DownloadFileAsync(media.MediaUrl);

                var entry = archive.CreateEntry(blobName,CompressionLevel.Optimal);
                await using var entryStream = entry.Open();
                await content.CopyToAsync(entryStream);
                await content.DisposeAsync();
            }
        }

        zipStream.Position=0;

        var zipFileName=$"{listingCase.Title}.zip";
        return new DownloadFileResponseDto
        {
            Content = zipStream,
            ContentType = "application/zip",
            FileName=zipFileName
        };
    }

    public async Task<ApiResponse<List<MediaAssetsByTypeDto>>> GetMediaAssetsByListingCaseAsync(int listingCaseId)
    {
        var existing  = await _listingCaseRepo.GetByIdAsync(listingCaseId);
        if (existing == null)
        {
            return ApiResponse<List<MediaAssetsByTypeDto>>.FailureResponse("The listing case does not exits!");
        }

        var mediaAssets = await _mediaAssetRepo.GetByListingCaseIdAsync(listingCaseId);

        var grouped = mediaAssets
            .GroupBy(m => m.MediaType)
            .Select(g => new MediaAssetsByTypeDto
            {
                MediaType = g.Key,
                Items = g.Select(m => new MediaAssetItemDto
                {
                    Id = m.Id,
                    MediaUrl = m.MediaUrl,
                    IsSelect = m.IsSelect,
                    IsHero = m.IsHero,
                    UploadedAt = m.UploadedAt
                }).ToList()
            })
            .ToList();

        return ApiResponse<List<MediaAssetsByTypeDto>>.SuccessResponse(grouped,"Media assets retrieved successfully.");
    }

    public async Task<ApiResponse<int>> DeleteMediaAssetByIdAsync(int id)
    {
        var media = await _mediaAssetRepo.GetByIdAsync(id);
        if (media == null)
        {
            return ApiResponse<int>.FailureResponse("The media asset does not exits!");
        }

        media.IsDeleted = true;
        await _mediaAssetRepo.UpdateByIdAsync(media);
        return ApiResponse<int>.SuccessResponse(id,"Delete the media asset successfully!");
    }
}
