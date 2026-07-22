using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Remp.Models.Entities;

namespace Remp.DataAccess.Data;

public static class DataSeeder
{
    public static async Task SeedRolesAsync(RoleManager<IdentityRole> roleManager)
    {
        string[] roles = { "PhotographyCompany", "Agent" };

        foreach (var role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                await roleManager.CreateAsync(new IdentityRole(role));
            }
        }
    }

    public static async Task SeedPhotographyCompanyAsync (UserManager<ApplicationUser> userManager)
    {
        var email = "company@test.com";

        var existing = await userManager.FindByEmailAsync(email);

        if (existing != null)
        {
            return;
        }

        var user = new ApplicationUser
        {
            UserName=email,
            Email=email,
            IsDeleted=false,
            CreatedAt=DateTime.UtcNow
        };

        var result = await userManager.CreateAsync(user,"Company@1234");

        if (result.Succeeded)
        {
            await userManager.AddToRoleAsync(user,"PhotographyCompany");
        }
    }
}
