using System;

namespace Remp.Service.DTOs;

public class ChangePasswordDto
{
    public string CurrentPassword {get;set;} = null!;
    public string NewPassword {get;set;} = null!;
    public string ConfirmPassword {get;set;} = null!;
}
