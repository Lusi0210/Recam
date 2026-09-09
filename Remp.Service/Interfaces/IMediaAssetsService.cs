using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Remp.Common;
using Remp.Service.DTOs;

namespace Remp.Service.Interfaces;

public interface IMediaAssetsService
{
    Task<ApiResponse<List<UploadMediaAssetsResponseDto>>> UploadMediaAssetAsync (UploadMediaAssetsDto dto, string userId);
    Task<DownloadFileResponseDto?> DownloadMediaAssetAsync(int id);
    Task<DownloadFileResponseDto?> DownloadListingCaseAsZipAsync(int listingCaseId);
    Task<ApiResponse<List<MediaAssetsByTypeDto>>> GetMediaAssetsByListingCaseAsync(int listingCaseId);
    Task<ApiResponse<int>> DeleteMediaAssetByIdAsync(int id);
}
