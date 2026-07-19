namespace Remp.Models.Entities;
public class Agent
{
    public string Id {get; set;} = null!;
    public string AgentFirstName {get; set;} = null!;
    public string AgentLastName {get; set;} = null!;
    public string AvatarUrl {get; set;} = null!;
    public string CompanyName {get; set;} = null!;
    public ICollection<AgentListingCase> AgentListingCases { get; set; } = new List<AgentListingCase>();
    public ICollection<AgentPhotographyCompany> AgentPhotographyCompanies { get; set; } = new List<AgentPhotographyCompany>();

}