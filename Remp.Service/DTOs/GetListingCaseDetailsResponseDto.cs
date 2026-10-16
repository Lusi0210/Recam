using System;
using Remp.Models.Enums;

namespace Remp.Service.DTOs;

public class GetListingCaseDetailsResponseDto
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
    public PropertyType PropertyType {get;set;}
    public SaleCategory SaleCategory {get;set;}
    public ListcaseStatus ListcaseStatus {get;set;}
    public ICollection<MediaAssetDto> MediaAssets {get;set;} = new List<MediaAssetDto>();
    public ICollection<AgentDto> Agents {get;set;} = new List<AgentDto>();
}
