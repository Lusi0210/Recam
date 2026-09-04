using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Remp.Service.DTOs;

public class CreateAgentDto
{
    [Required]
    public string Name {get;set;} = null!;
    [Required]
    [EmailAddress]
    public string Email {get;set;} = null!;
}
