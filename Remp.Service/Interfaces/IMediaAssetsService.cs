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
}
