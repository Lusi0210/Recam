using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Remp.Models.Entities;
using Remp.Models.Enums;

namespace Remp.Service.DTOs;

public class UploadMediaAssetsResponseDto
{
    public int Id {get;set;}
    public string MediaUrl {get;set;} = null!;
    public MediaType MediaType {get;set;}
}
