using System;
using Remp.Models.Entities;
using Remp.Models.Enums;

namespace Remp.Service.DTOs;

public class GetListingCaseMediaAssetsResponseDto
{
    public MediaType MediaType {get;set;}
    public ICollection<MediaAssetDto> MediaAssets {get;set;} = new List<MediaAssetDto>();
}
