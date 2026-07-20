using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace Remp.Models.Entities;

public class ApplicationUser : IdentityUser
{
    public Boolean IsDeleted {get;set;}
    public DateTime CreatedAt {get;set;}
}
