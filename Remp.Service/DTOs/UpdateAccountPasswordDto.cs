using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Remp.Service.DTOs;

public class UpdateAccountPasswordDto
{
    [Required]
    [MinLength(6)]
    public string CurrentPassword {get;set;}=null!;
    [Required]
    [MinLength(6)]
    public string NewPassword {get;set;}=null!;
}
