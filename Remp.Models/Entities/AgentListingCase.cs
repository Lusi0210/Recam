namespace Remp.Models.Entities;
public class AgentListingCase
{
    public string AgentId {get;set;} = null!;
    public Agent Agent{get;set;} = null!;
    public int ListingCaseId{get;set;}
    public ListingCase ListingCase{get;set;} = null!;
}