using System;
using Remp.Models.Entities;

namespace Remp.Models.Projections;

public class UserWithRole
{
    public User User { get; set; } = null!;
    public string? Role { get; set; }
}
