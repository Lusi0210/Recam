using System;

namespace Remp.Service.DTOs;

public class AddAgentToListingCaseResponseDto
{
    public string Id {get;set;} = null!;
    public int ListingCaseId {get;set;}
}
