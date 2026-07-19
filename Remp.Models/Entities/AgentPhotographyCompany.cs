namespace Remp.Models.Entities;

public class AgentPhotographyCompany
{
    public string AgentId { get; set; } = null!;
    public Agent Agent {get;set;} = null!;
    public string PhotographyCompanyId { get; set; } = null!;
    public PhotographyCompany PhotographyCompany {get;set;} = null!;
}