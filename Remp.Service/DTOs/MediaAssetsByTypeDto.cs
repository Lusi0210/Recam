using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Remp.Models.Enums;

namespace Remp.Service.DTOs;

public class MediaAssetsByTypeDto
{
    public MediaType MediaType { get; set; }
    public List<MediaAssetItemDto> Items { get; set; } = new List<MediaAssetItemDto>();

}
