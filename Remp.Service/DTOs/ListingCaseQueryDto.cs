using System;
using Remp.Models.Enums;

namespace Remp.Service.DTOs;

public class ListingCaseQueryDto : PaginationQueryDto
{
    public ListcaseStatus? Status { get; set; }
    public PropertyType? PropertyType { get; set; }
    public SaleCategory? SaleCategory { get; set; }
    public string? City { get; set; }
    public decimal? MinPrice { get; set; }
    public decimal? MaxPrice { get; set; }
    public string? Search { get; set; }
}
