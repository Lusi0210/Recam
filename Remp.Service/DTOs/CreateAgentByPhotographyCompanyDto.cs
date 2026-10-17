using System;

namespace Remp.Service.DTOs;

public class CreateAgentByPhotographyCompanyDto
{
    public string AgentFirstName {get;set;} = null!;
    public string AgentLastName {get;set;} = null!;
    public string? AvatarUrl {get;set;}
    public string? CompanyName {get;set;}
    public string Email { get; set; } = null!;
}
