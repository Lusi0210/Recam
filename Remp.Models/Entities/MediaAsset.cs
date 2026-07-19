namespace Remp.Models.Entities;
public class MediaAsset
{
    public int Id {get;set;}
    public Enums.MediaType MediaType {get;set;}
    public string MediaUrl {get;set;} = null!;
    public DateTime UploadedAt { get; set; }
    public Boolean IsSelect { get; set; }
    public Boolean IsHero { get; set; }
    public int ListingCaseId { get; set; }
    public string UserId { get; set; } = null!;
    public Boolean IsDeleted { get; set; }
    public ListingCase ListingCase {get;set;} = null!;
}