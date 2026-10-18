using System;

namespace Remp.Service.DTOs;

public class ChangePasswordResponseDto
{
    public string UserId { get; set; } = null!;
    public string Email { get; set; } = null!;
}
