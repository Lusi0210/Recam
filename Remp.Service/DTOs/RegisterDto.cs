using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Remp.Service.DTOs;

public class RegisterDto
{
    [Required]
    public string Name {get;set;} = null!;
    [Required]
    [EmailAddress]
    public string Email {get;set;} = null!;
    [Required]
    [MinLength(6)]
    public string Password {get;set;} = null!;
    
}
