using System;
using Microsoft.AspNetCore.Http;
using Remp.Models.Enums;
using Remp.Service.DTOs;

namespace Remp.Service.Interfaces;

public interface IMediaAssetService
{
    Task<List<MediaAssetDto>> UploadMediaAssetsAsync(int listingCaseId, MediaType mediaType, List<IFormFile> files, string userId, string? ipAddress);
}
