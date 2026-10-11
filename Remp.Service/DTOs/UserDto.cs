using System;

namespace Remp.Service.DTOs;

public class UserDto
{
    public string Id { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string? Role { get; set; }
    public DateTime CreatedAt { get; set; }
}
