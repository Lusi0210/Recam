using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Remp.Models.Entities;

namespace Remp.Service.DTOs;

public class AddAgentToListingCaseDto
{
    public string AgentId {get;set;} = null!;
    public int ListingCaseId{get;set;}
}
