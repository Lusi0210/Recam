using System;
using Remp.Models.Enums;

namespace Remp.Models.Entities;

public class ListingCase
{
    public int Id {get;set;}
    public string Title {get;set;} = null!;
    public string? Description {get;set;}
    public string Street {get;set;} = null!;
    public string City {get;set;} = null!;
    public string State {get;set;} = null!;
    public string Postcode {get;set;} = null!;
    public decimal Longitude {get;set;}
    public decimal Latitude {get;set;}
    public decimal Price {get;set;}
    public int Bedrooms {get;set;}
    public int Bathrooms {get;set;}
    public int Garages {get;set;}
    public double FloorArea {get;set;}
    public DateTime CreatedAt {get;set;}
    public bool IsDeleted {get;set;}
    public PropertyType PropertyType {get;set;}
    public SaleCategory SaleCategory {get;set;}
    public ListcaseStatus ListcaseStatus {get;set;}
    public string UserId {get;set;} = null!;
    public User User {get;set;} = null!;
    public ICollection<CaseContact> CaseContacts {get;set;} = new List<CaseContact>();
    public ICollection<MediaAsset> MediaAssets {get;set;} = new List<MediaAsset>();
    public ICollection<Agent> Agents {get;set;} = new List<Agent>();
}
