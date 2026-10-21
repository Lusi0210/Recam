using System.IO.Compression;
using AutoMapper;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Remp.Common.Exceptions;
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
    private readonly ILogger<MediaAssetService> _logger;
    private readonly IActivityLogService _activityLogService;
    private readonly IValidator<SetCoverImageDto> _setCoverImageValidator;
    private readonly IValidator<AgentSelectMediaDto> _agentSelectMediaValidator;

    public MediaAssetService(
        IListingCaseRepository listingCaseRepository,
        IMediaAssetRepository mediaAssetRepository,
        IBlobStorageService blobStorageService,
        IUnitOfWork unitOfWork,
        IMapper mapper,
        ILogger<MediaAssetService> logger,
        IActivityLogService activityLogService,
        IValidator<SetCoverImageDto> setCoverImageValidator,
        IValidator<AgentSelectMediaDto> agentSelectMediaValidator)
    {
        _listingCaseRepository = listingCaseRepository;
        _mediaAssetRepository = mediaAssetRepository;
        _blobStorageService = blobStorageService;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _logger = logger;
        _activityLogService = activityLogService;
        _setCoverImageValidator = setCoverImageValidator;
        _agentSelectMediaValidator = agentSelectMediaValidator;
    }

    private MediaAssetDto ToMediaAssetDto(MediaAsset mediaAsset)
    {
        MediaAssetDto dto = _mapper.Map<MediaAssetDto>(mediaAsset);
        dto.MediaUrl = _blobStorageService.GetReadUrl(mediaAsset.MediaUrl);
        return dto;
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

    public async Task<List<MediaAssetDto>> UploadMediaAssetsAsync(int listingCaseId, MediaType mediaType, List<IFormFile> files, string userId, string? ipAddress)
    {
        ListingCase? listingCase = await _listingCaseRepository.GetListingCaseAsync(listingCaseId);
        if (listingCase == null)
        {
            throw new NotFoundException("The listing case does not exist!");
        }

        listingCase.EnsureOwner(userId, "You can only upload media to your own listing cases.");
        

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

        await _activityLogService.LogListingCaseAsync(ListingCaseActivityType.UploadMedia,listingCaseId, userId,true, $"Uploaded {files.Count} {mediaType} file(s)", ipAddress);

        List<MediaAssetDto> result = mediaAssets.Select(m => ToMediaAssetDto(m)).ToList();

        return result;
    }

    public async Task<(Stream Content, string ContentType, string FileName)> DownloadMediaAssetAsync(int mediaAssetId, string userId)
    {
        MediaAsset? mediaAsset = await _mediaAssetRepository.GetMediaAssetWithListingCaseAsync(mediaAssetId);
        if (mediaAsset == null)
        {
            throw new NotFoundException("The media asset does not exist!");
        }

        mediaAsset.ListingCase.EnsureOwnerOrAssignedAgent(userId, "You do not have permission to download this media.");


        return await _blobStorageService.DownloadFileAsync(mediaAsset.MediaUrl);
    }

    public async Task<(Stream Content, string FileName)> DownloadListingCaseMediaZipAsync(int listingCaseId, string userId)
    {
        ListingCase? listingCase = await _listingCaseRepository.GetListingCaseDetailsByIdAsync(listingCaseId);
        if (listingCase == null)
        {
            throw new NotFoundException("The listing case does not exist!");
        }

        listingCase.EnsureOwnerOrAssignedAgent(userId, "You do not have permission to download this listing case's media.");
        

        if (listingCase.MediaAssets.Count == 0)
        {
            throw new NotFoundException("This listing case has no media to download.");
        }

        string tempPath = Path.GetTempFileName();
        FileStream zipStream = new FileStream(tempPath, FileMode.Create, FileAccess.ReadWrite, FileShare.None, 81920, FileOptions.DeleteOnClose);

        try
        {
            using (ZipArchive archive = new ZipArchive(zipStream, ZipArchiveMode.Create, leaveOpen: true))
            {
                foreach (MediaAsset mediaAsset in listingCase.MediaAssets)
                {
                    (Stream Content, string ContentType, string FileName) file;
                    try
                    {
                        file = await _blobStorageService.DownloadFileAsync(mediaAsset.MediaUrl);
                    }
                    catch (NotFoundException)
                    {
                        _logger.LogWarning("Blob {BlobName} for media {MediaId} is missing, skipped in zip", mediaAsset.MediaUrl, mediaAsset.Id);
                        continue;
                    }

                    string entryName = $"{mediaAsset.MediaType}/{file.FileName}";
                    ZipArchiveEntry entry = archive.CreateEntry(entryName, CompressionLevel.Fastest);

                    using Stream entryStream = entry.Open();
                    using Stream blobStream = file.Content;
                    await blobStream.CopyToAsync(entryStream);
                }
            }

            zipStream.Position = 0;
            return (zipStream, $"listing-case-{listingCaseId}-media.zip");
        }
        catch
        {
            await zipStream.DisposeAsync();
            throw;
        }
    }

    public async Task DeleteMediaAssetAsync(int mediaAssetId, string userId, string? ipAddress)
    {
        MediaAsset? mediaAsset = await _mediaAssetRepository.GetMediaAssetByIdAsync(mediaAssetId);
        if (mediaAsset == null)
        {
            throw new NotFoundException("The media asset does not exist!");
        }

        mediaAsset.ListingCase.EnsureOwner(userId, "You can only delete media from your own listing cases.");
        

        mediaAsset.IsDeleted = true;
        mediaAsset.IsHero = false;
        mediaAsset.IsSelected = false;
        await _unitOfWork.SaveChangesAsync();

        await _activityLogService.LogListingCaseAsync(ListingCaseActivityType.DeleteMedia, mediaAsset.ListingCaseId, userId,true, $"Deleted media {mediaAssetId}", ipAddress);
    }

    public async Task<List<GetListingCaseMediaAssetsResponseDto>> GetListingCaseMediaAssetsAsync(int id,string userId)
    {
        ListingCase? listingCase = await _listingCaseRepository.GetListingCaseDetailsByIdAsync(id);
        if(listingCase == null)
        {
            throw new NotFoundException("The listing case does not exist!");
        }

        listingCase.EnsureOwnerOrAssignedAgent(userId, "You do not have permission to view this listing case's media.");
        


        List<GetListingCaseMediaAssetsResponseDto> result = listingCase.MediaAssets
            .GroupBy(m => m.MediaType)
            .Select(group => new GetListingCaseMediaAssetsResponseDto
            {
                MediaType = group.Key,
                MediaAssets = group.Select(m => ToMediaAssetDto(m)).ToList()
            })
            .ToList();
        
        return result;
    }

    public async Task<SetCoverImageResponseDto> SetCoverImageAsync(int listingCaseId, SetCoverImageDto dto, string userId, string? ipAddress)
    {
        await _setCoverImageValidator.ValidateOrThrowAsync(dto);

        ListingCase? listingCase = await _listingCaseRepository.GetListingCaseWithMediaAndAgentsAsync(listingCaseId);
        if (listingCase == null)
        {
            throw new NotFoundException("The listing case does not exist!");
        }

        listingCase.EnsureAssignedAgent(userId, "Only the assigned agent can set the cover image.");
        

        MediaAsset? coverImage = listingCase.MediaAssets.FirstOrDefault(m => m.Id == dto.MediaId);
        if (coverImage == null)
        {
            throw new BadRequestException("The media does not belong to this listing case.");
        }

        if (coverImage.MediaType != MediaType.Photos)
        {
            throw new BadRequestException("Only photos can be set as the cover image.");
        }

        foreach (MediaAsset mediaAsset in listingCase.MediaAssets)
        {
            mediaAsset.IsHero = mediaAsset.Id == coverImage.Id;
        }

        await _unitOfWork.SaveChangesAsync();

        await _activityLogService.LogListingCaseAsync(ListingCaseActivityType.SetCoverImage, listingCaseId, userId,true, $"Set media {coverImage.Id} as cover image", ipAddress);

        return new SetCoverImageResponseDto
        {
            ListingCaseId = listingCaseId,
            MediaId = coverImage.Id
        };
    }

    public async Task<AgentSelectMediaResponseDto> AgentSelectMediaAsync(string userId,int id,AgentSelectMediaDto dto,string? ipAddress)
    {
        await _agentSelectMediaValidator.ValidateOrThrowAsync(dto);

        ListingCase? listingCase = await _listingCaseRepository.GetListingCaseWithMediaAndAgentsAsync(id);
        if (listingCase == null)
        {
            throw new NotFoundException("The listing case does not exist!");
        }

        listingCase.EnsureAssignedAgent(userId, "Only the assigned agent can select the media.");
        

        List<int> invalidIds = dto.MediaAssetsId
            .Where(mediaId => !listingCase.MediaAssets.Any(m => m.Id == mediaId))
            .ToList();
        if (invalidIds.Count > 0)
        {
            throw new BadRequestException($"Media {string.Join(", ", invalidIds)} do not belong to this listing case.");
        }

        bool hasNonPhoto = listingCase.MediaAssets.Any(m => dto.MediaAssetsId.Contains(m.Id) && m.MediaType != MediaType.Photos);
        if (hasNonPhoto)
        {
            throw new BadRequestException("Only photos can be selected.");
        }

        foreach (MediaAsset mediaAsset in listingCase.MediaAssets)
        {
            mediaAsset.IsSelected = dto.MediaAssetsId.Contains(mediaAsset.Id);
        }


        await _unitOfWork.SaveChangesAsync();

        string message = dto.MediaAssetsId.Count == 0
            ? "Cleared all selected media"
            : $"Selected media: {string.Join(", ", dto.MediaAssetsId)}";
        await _activityLogService.LogListingCaseAsync(ListingCaseActivityType.SelectMedia, id, userId,true, message, ipAddress);
        return new AgentSelectMediaResponseDto
        {
            Id = id,
            MediaAssetsId = dto.MediaAssetsId
        };
    }

    public async Task<List<GetFInalSelectionResponseDto>> GetFinalSelectionAsync(int id,string userId)
    {
        ListingCase? listingCase = await _listingCaseRepository.GetListingCaseWithMediaAndAgentsAsync(id);
        if (listingCase == null)
        {
            throw new NotFoundException("The listing case does not exist!");
        }

        listingCase.EnsureOwnerOrAssignedAgent(userId, "You do not have permission to view this listing case's media.");
        

        List<MediaAsset> result = listingCase.MediaAssets.Where(m => m.IsSelected && !m.IsDeleted).ToList();
        return _mapper.Map<List<GetFInalSelectionResponseDto>>(result);

    }
}
