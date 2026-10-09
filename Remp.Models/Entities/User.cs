using System;
using Microsoft.AspNetCore.Identity;

namespace Remp.Models.Entities;

public class User :IdentityUser
{
    public bool IsDeleted {get;set;}
    public DateTime CreatedAt {get;set;}
    public Agent? Agent {get;set;}
    public PhotographyCompany? PhotographyCompany {get;set;}
    public ICollection<ListingCase> ListingCases {get;set;} = new List<ListingCase>();
    public ICollection<MediaAsset> MediaAssets {get;set;} = new List<MediaAsset>();
}
