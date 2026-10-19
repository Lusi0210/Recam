using System;

namespace Remp.Service.DTOs;

public class AddCaseContactDto
{
    public string FirstName {get;set;} = null!;
    public string LastName {get;set;} = null!;
    public string? CompanyName {get;set;}
    public string? ProfileUrl {get;set;}
    public string Email {get;set;} = null!;
    public string PhoneNumber {get;set;} = null!;
}
