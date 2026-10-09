using System;

namespace Remp.Models.Entities;

public class PhotographyCompany
{
    public string Id {get;set;} = null!;
    public string PhotographyCompanyName {get;set;} = null!;
    public ICollection<Agent> Agents {get;set;} = new List<Agent>();
    public User User {get;set;} = null!;
}
