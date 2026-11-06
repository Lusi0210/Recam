using System;
using Remp.Models.Enums;

namespace Remp.Service.DTOs;

public class GetListingCaseByURLResponseDto
{
    public string Title { get; set; } = null!;
    public string? Description { get; set; }
    public string Street { get; set; } = null!;
    public string City { get; set; } = null!;
    public string State { get; set; } = null!;
    public string Postcode { get; set; } = null!;
    public decimal Price { get; set; }
    public int Bedrooms { get; set; }
    public int Bathrooms { get; set; }
    public int Garages { get; set; }
    public double FloorArea { get; set; }
    public PropertyType PropertyType { get; set; }
    public SaleCategory SaleCategory { get; set; }
    public List<SharedMediaAssetDto> MediaAssets { get; set; } = new List<SharedMediaAssetDto>();
}
