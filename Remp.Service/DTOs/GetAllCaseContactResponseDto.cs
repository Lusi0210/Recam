using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Remp.Models.Entities;

namespace Remp.Service.DTOs;

public class GetAllCaseContactResponseDto
{
    public int Id {get;set;}
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public string CompanyName { get; set; } = null!;
    public string ProfileUrl { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string PhoneNumber { get; set; } = null!;
    public int ListingCaseId { get; set; }
}

