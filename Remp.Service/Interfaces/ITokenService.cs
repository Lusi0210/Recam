using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Remp.Models.Entities;

namespace Remp.Service.Interfaces;

public interface ITokenService
{
    string GenerateToken(ApplicationUser user,IList<string> roles);
}
