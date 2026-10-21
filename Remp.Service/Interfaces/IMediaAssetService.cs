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
    Task DeleteMediaAssetAsync(int mediaAssetId, string userId, string? ipAddress);
    Task<List<GetListingCaseMediaAssetsResponseDto>> GetListingCaseMediaAssetsAsync(int id,string userId);
    Task<SetCoverImageResponseDto> SetCoverImageAsync(int listingCaseId, SetCoverImageDto dto, string userId, string? ipAddress);
    Task<AgentSelectMediaResponseDto> AgentSelectMediaAsync(string userId,int id,AgentSelectMediaDto dto,string? ipAddress);
    Task<List<GetFInalSelectionResponseDto>> GetFinalSelectionAsync(int id,string userId);
}
