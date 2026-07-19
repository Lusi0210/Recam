namespace Remp.Models.Entities;

public class PhotographyCompany
{
    public string Id { get; set; } = null!;
    public string PhotographyCompanyName { get; set; } = null!;
    public ICollection<AgentPhotographyCompany> AgentPhotographyCompanies { get; set; } = new List<AgentPhotographyCompany>();
}
