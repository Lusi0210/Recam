using System;

namespace Remp.Service.DTOs;

public class GetUserResponseDto
{
    public string UserId {get;set;} =null!;
    public string? Role {get;set;} 
    public ICollection<int> ListingCaseIds {get;set;} = new List<int>();
}
