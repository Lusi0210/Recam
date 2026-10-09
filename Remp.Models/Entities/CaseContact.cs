using System;

namespace Remp.Models.Entities;

public class CaseContact
{
    public int Id {get;set;}
    public string FirstName {get;set;} = null!;
    public string LastName {get;set;} = null!;
    public string? CompanyName {get;set;}
    public string? ProfileUrl {get;set;}
    public string Email {get;set;} = null!;
    public string PhoneNumber {get;set;} = null!;
    public int ListingCaseId {get;set;}
    public ListingCase ListingCase {get;set;} = null!;
}
