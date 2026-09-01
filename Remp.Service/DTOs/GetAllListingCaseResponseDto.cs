using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Remp.Service.DTOs;

public class GetAllListingCaseResponseDto
{
    public int Id { get; set; }
    public string Title { get; set; } = null!;
    public string Description { get; set; } = null!;
    public string Street { get; set; } = null!;
    public string City { get; set; } = null!;
    public string State { get; set; }= null!;
    public int PostCode { get; set; }
    public decimal Longitude { get; set; }
    public decimal Latitude { get; set; }
    public decimal Price { get; set; }
    public int Bedrooms { get; set; }
    public int Bathrooms { get; set; }
    public int Garages { get; set; }
    public double FloorArea { get; set; }
    public DateTime CreatedAt { get; set; }
    public Models.Enums.PropertyType PropertyType {get; set;}
    public Models.Enums.SaleCategory SaleCategory {get;set;}
    public Models.Enums.ListcaseStatus ListcaseStatus {get;set;}

}
