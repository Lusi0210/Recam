using System;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Remp.Models.Constants;
using Remp.Models.Entities;

namespace Remp.DataAccess.Data;

public static class DbSeeder
{
    private const string TestCompanyEmail = "photo@recam.test";
    private const string TestCompanyPassword = "Test@12345";

    public static async Task SeedAsync(IServiceProvider serviceProvider, bool includeTestData)
    {
        // RoleManager / UserManager / DbContext 都是 Scoped 服务，
        // 程序启动时没有 HTTP 请求，所以要手动创建一个 scope 来获取它们
        using var scope = serviceProvider.CreateScope();
        var services = scope.ServiceProvider;

        var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
        var userManager = services.GetRequiredService<UserManager<User>>();
        var context = services.GetRequiredService<RempDbContext>();
        var logger = services.GetRequiredService<ILoggerFactory>().CreateLogger("DbSeeder");

        await SeedRolesAsync(roleManager, logger);

        if (includeTestData)
        {
            await SeedTestPhotographyCompanyAsync(userManager, context, logger);
        }
    }

    private static async Task SeedRolesAsync(RoleManager<IdentityRole> roleManager, ILogger logger)
    {
        string[] roles = { RoleNames.PhotographyCompany, RoleNames.Agent };

        foreach (var role in roles)
        {
            if (await roleManager.RoleExistsAsync(role)) continue;

            // RoleManager 会自动生成 Id 和 NormalizedName，不用手写 GUID
            var result = await roleManager.CreateAsync(new IdentityRole(role));
            EnsureSucceeded(result, $"create role '{role}'");
            logger.LogInformation("Seeded role {Role}", role);
        }
    }

    private static async Task SeedTestPhotographyCompanyAsync(
        UserManager<User> userManager, RempDbContext context, ILogger logger)
    {
        if (await userManager.FindByEmailAsync(TestCompanyEmail) != null) return;

        // 要写 AspNetUsers、AspNetUserRoles、PhotographyCompanies 三张表，
        // 按规范用事务：任何一步失败就全部回滚，不会留下"有账号没公司"的半成品数据
        await using var transaction = await context.Database.BeginTransactionAsync();

        var user = new User
        {
            UserName = TestCompanyEmail,
            Email = TestCompanyEmail,
            EmailConfirmed = true,
            CreatedAt = DateTime.UtcNow
        };

        // CreateAsync 会自动把密码哈希后再存
        EnsureSucceeded(await userManager.CreateAsync(user, TestCompanyPassword), "create test user");
        EnsureSucceeded(await userManager.AddToRoleAsync(user, RoleNames.PhotographyCompany), "assign role");

        // 一对一：PhotographyCompany 的 Id 就是 User 的 Id
        context.PhotographyCompanies.Add(new PhotographyCompany
        {
            Id = user.Id,
            PhotographyCompanyName = "Test Photography Co"
        });
        await context.SaveChangesAsync();

        await transaction.CommitAsync();
        logger.LogInformation("Seeded test photography company {Email}", TestCompanyEmail);
    }

    private static void EnsureSucceeded(IdentityResult result, string action)
    {
        if (result.Succeeded) return;

        var errors = string.Join("; ", result.Errors.Select(e => e.Description));
        throw new InvalidOperationException($"Failed to {action}: {errors}");
    }
}
