using System;

namespace Remp.Service.DTOs;

public class LoginResponseDto
{
    public string Token {get;set;} = null!;
    public string UserId {get;set;} = null!;
    public string Email {get;set;} = null!;
    public string Role {get;set;} = null!;
}
