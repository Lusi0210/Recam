using System;
using Remp.Models.Enums;

namespace Remp.Service.DTOs;

public class MediaAssetDto
{
    public int Id {get;set;}
    public MediaType MediaType {get;set;}
    public string MediaUrl {get;set;} = null!;
    public DateTime UploadedAt {get;set;}
    public string UserId {get;set;} = null!;
    
}
