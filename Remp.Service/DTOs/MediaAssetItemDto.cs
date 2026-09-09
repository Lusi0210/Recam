using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Remp.Service.DTOs;

public class MediaAssetItemDto
{
    public int Id {get;set;}
    public string MediaUrl {get;set;} = null!;
    public DateTime UploadedAt { get; set; }
    public Boolean IsSelect { get; set; }
    public Boolean IsHero { get; set; }
}
