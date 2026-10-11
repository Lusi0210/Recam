using System;

namespace Remp.Service.DTOs;

public class RegisterRequestDto
{
    public string Email{get;set;} = null!;
    public string Password {get;set;} = null!;
    public string ConfirmPassword {get;set;} = null!;
    public string AgentFirstName {get;set;} = null!;
    public string AgentLastName {get;set;} = null!;
    public string? CompanyName { get; set; }
}
