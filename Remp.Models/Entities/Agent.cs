using System;

namespace Remp.Models.Entities;

public class Agent
{
    public string Id {get;set;} = null!;
    public string AgentFirstName {get;set;} = null!;
    public string AgentLastName {get;set;} = null!;
    public string? AvatarUrl {get;set;}
    public string? CompanyName {get;set;}
    public ICollection<ListingCase> ListingCases {get;set;} = new List<ListingCase>();
    public ICollection<PhotographyCompany> PhotographyCompanies {get;set;} = new List<PhotographyCompany>();
    public User User {get;set;} = null!;
}
