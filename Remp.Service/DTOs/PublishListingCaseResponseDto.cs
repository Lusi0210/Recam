using System;

namespace Remp.Service.DTOs;

public class PublishListingCaseResponseDto
{
    public int ListingCaseId { get; set; }
    public string ShareableUrl { get; set; } = null!;
}
