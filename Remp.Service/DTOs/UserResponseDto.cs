using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Remp.Service.DTOs;

public class UserResponseDto
{
    public string Id { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string UserName { get; set; } = null!;
    public DateTime CreatedAt { get; set; }
    public IList<string> Roles { get; set; } = new List<string>();
}