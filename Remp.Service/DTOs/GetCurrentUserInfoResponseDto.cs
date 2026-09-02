using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Remp.Service.DTOs;

public class GetCurrentUserInfoResponseDto
{
    public string UserId {get;set;} = null!;
    public string Role {get;set;} = null!;
    public List<int> AssignedListingIds {get;set;} = new List<int>();
}
