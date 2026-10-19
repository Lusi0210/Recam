using System;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Remp.Common.Exceptions;
using Remp.Models.Documents;
using Remp.Models.Entities;
using Remp.Models.Enums;
using Remp.Repository.Common;
using Remp.Repository.Interfaces;
using Remp.Service.Common;
using Remp.Service.DTOs;
using Remp.Service.Interfaces;

namespace Remp.Service.Services;

public class MediaAssetService : IMediaAssetService
{ 
    private readonly IListingCaseRepository _listingCaseRepository;
    private readonly IMediaAssetRepository _mediaAssetRepository;
    private readonly IBlobStorageService _blobStorageService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IListingCaseHistoryRepository _listingCaseHistoryRepository;
    private readonly ILogger<MediaAssetService> _logger;

    public MediaAssetService(
        IListingCaseRepository listingCaseRepository,
        IMediaAssetRepository mediaAssetRepository,
        IBlobStorageService blobStorageService,
        IUnitOfWork unitOfWork,
        IMapper mapper,
        IListingCaseHistoryRepository listingCaseHistoryRepository,
        ILogger<MediaAssetService> logger)
    {
        _listingCaseRepository = listingCaseRepository;
        _mediaAssetRepository = mediaAssetRepository;
        _blobStorageService = blobStorageService;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _listingCaseHistoryRepository = listingCaseHistoryRepository;
        _logger = logger;
    }

    private static void ValidateFiles(MediaType mediaType, List<IFormFile> files)
    {
        if (!Enum.IsDefined(mediaType))
        {
            throw new BadRequestException("Invalid media type.");
        }

        if (files == null || files.Count == 0)
        {
            throw new BadRequestException("At least one file is required.");
        }

        if (mediaType != MediaType.Photos && files.Count > 1)
        {
            throw new BadRequestException($"Only one file can be uploaded for {mediaType}.");
        }

        HashSet<string> allowedExtensions = MediaFileRules.AllowedExtensions[mediaType];
        long maxSize = MediaFileRules.MaxFileSizeBytes[mediaType];
        List<string> errors = new List<string>();

        foreach (IFormFile file in files)
        {
            string extension = Path.GetExtension(file.FileName).ToLowerInvariant();

            if (!allowedExtensions.Contains(extension))
            {
                errors.Add($"{file.FileName}: file type {extension} is not allowed for {mediaType}.");
            }

            if (file.Length == 0)
            {
                errors.Add($"{file.FileName}: file is empty.");
            }
            else if (file.Length > maxSize)
            {
                errors.Add($"{file.FileName}: file exceeds the {maxSize / (1024 * 1024)}MB limit.");
            }
        }

        if (errors.Count > 0)
        {
            throw new BadRequestException("Validation failed", errors);
        }
    }

    private async Task LogAsync(int listingCaseId, string userId, string message, string? ipAddress)
    {
        ListingCaseHistory history = new ListingCaseHistory
        {
            Action = ListingCaseActivityType.UploadMedia,
            ListingCaseId = listingCaseId,
            UserId = userId,
            Success = true,
            Message = message,
            IpAddress = ipAddress,
            Timestamp = DateTime.UtcNow
        };

        try
        {
            await _listingCaseHistoryRepository.AddAsync(history);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to write upload log for listing case {ListingCaseId}", listingCaseId);
        }
    }

    public async Task<List<MediaAssetDto>> UploadMediaAssetsAsync(int listingCaseId, MediaType mediaType, List<IFormFile> files, string userId, string? ipAddress)
    {
        ListingCase? listingCase = await _listingCaseRepository.GetListingCaseAsync(listingCaseId);
        if (listingCase == null)
        {
            throw new NotFoundException("The listing case does not exist!");
        }

        if (listingCase.UserId != userId)
        {
            throw new ForbiddenException("You can only upload media to your own listing cases.");
        }

        ValidateFiles(mediaType, files);

        List<string> uploadedBlobNames = new List<string>();
        List<MediaAsset> mediaAssets = new List<MediaAsset>();

        try
        {
            foreach (IFormFile file in files)
            {
                string extension = Path.GetExtension(file.FileName).ToLowerInvariant();
                string blobName = $"listing-cases/{listingCaseId}/{Guid.NewGuid()}{extension}";

                using Stream stream = file.OpenReadStream();
                await _blobStorageService.UploadAsync(stream, blobName, file.ContentType);
                uploadedBlobNames.Add(blobName);

                MediaAsset mediaAsset = new MediaAsset
                {
                    ListingCaseId = listingCaseId,
                    UserId = userId,
                    MediaType = mediaType,
                    MediaUrl = blobName,
                    UploadedAt = DateTime.UtcNow,
                    IsDeleted = false
                };
                _mediaAssetRepository.Add(mediaAsset);
                mediaAssets.Add(mediaAsset);
            }

            await _unitOfWork.SaveChangesAsync();
        }
        catch
        {
            foreach (string blobName in uploadedBlobNames)
            {
                await _blobStorageService.DeleteAsync(blobName);
            }
            throw;
        }

        await LogAsync(listingCaseId, userId, $"Uploaded {files.Count} {mediaType} file(s)", ipAddress);

        List<MediaAssetDto> result = new List<MediaAssetDto>();
        foreach (MediaAsset mediaAsset in mediaAssets)
        {
            MediaAssetDto dto = _mapper.Map<MediaAssetDto>(mediaAsset);
            dto.MediaUrl = _blobStorageService.GetReadUrl(mediaAsset.MediaUrl);
            result.Add(dto);
        }

        return result;
    }
}
