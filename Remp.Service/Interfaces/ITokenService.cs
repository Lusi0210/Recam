using System;
using Remp.Models.Entities;

namespace Remp.Service.Interfaces;

public interface ITokenService
{
    string GenerateToken(User user,string role);
}
