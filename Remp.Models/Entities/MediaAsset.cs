using System;
using Remp.Models.Enums;

namespace Remp.Models.Entities;

public class MediaAsset
{
    public int Id {get;set;}
    public MediaType MediaType {get;set;}
    public string MediaUrl {get;set;} = null!;
    public DateTime UploadedAt {get;set;}
    public bool IsSelected {get;set;}
    public bool IsHero {get;set;}
    public int ListingCaseId {get;set;}
    public ListingCase ListingCase {get;set;} = null!;
    public string UserId {get;set;} = null!;
    public User User {get;set;} = null!;
    public bool IsDeleted {get;set;}
}
