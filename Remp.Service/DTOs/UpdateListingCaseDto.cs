using Remp.Models.Enums;

namespace Remp.Service.DTOs;

public class UpdateListingCaseDto
{
    public string Title {get;set;} = null!;
    public string? Description {get;set;}
    public decimal Price {get;set;}
    public int Bedrooms {get;set;}
    public int Bathrooms {get;set;}
    public int Garages {get;set;}
    public double FloorArea {get;set;}
    public SaleCategory SaleCategory {get;set;}
}
