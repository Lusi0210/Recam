using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Remp.Models.Enums;

namespace Remp.Service.DTOs;

public class UploadMediaAssetsDto
{
    public int ListingCaseId {get;set;}
    public MediaType MediaType {get;set;}
    public List<IFormFile> Files {get;set;} = new List<IFormFile>();
}
