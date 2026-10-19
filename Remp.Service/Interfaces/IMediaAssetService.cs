using System;
using Microsoft.AspNetCore.Http;
using Remp.Models.Enums;
using Remp.Service.DTOs;

namespace Remp.Service.Interfaces;

public interface IMediaAssetService
{
    Task<List<MediaAssetDto>> UploadMediaAssetsAsync(int listingCaseId, MediaType mediaType, List<IFormFile> files, string userId, string? ipAddress);
    Task<(Stream Content, string ContentType, string FileName)> DownloadMediaAssetAsync(int mediaAssetId, string userId);
    Task<(Stream Content, string FileName)> DownloadListingCaseMediaZipAsync(int listingCaseId, string userId);
}
