using System;
using System.Linq;
using System.Threading.Tasks;
using EVDMS.DataAccess.Constants;
using EVDMS.DataAccess.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace EVDMS.DataAccess.Database;

public static class IdentitySeed
{
    private static readonly string[] Roles =
    {
        RoleNames.Admin,
        RoleNames.EvmStaff,
        RoleNames.DealerManager,
        RoleNames.DealerStaff
    };

    public static async Task EnsureSeedAsync(
        IServiceProvider services,
        ILogger logger)
    {
        var roleManager = services.GetRequiredService<RoleManager<ApplicationRole>>();
        foreach (var role in Roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                await roleManager.CreateAsync(new ApplicationRole(role));
                logger.LogInformation("Created role {Role}", role);
            }
        }

        var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();

        await EnsureUserAsync(userManager, logger,
            email: "admin@evdms.vn",
            displayName: "System Admin",
            roles: new[] { RoleNames.Admin });

        await EnsureUserAsync(userManager, logger,
            email: "staff@evdms.vn",
            displayName: "EVM Staff",
            roles: new[] { RoleNames.EvmStaff });

        await EnsureUserAsync(userManager, logger,
            email: "dealermanager@evdms.vn",
            displayName: "Dealer Manager",
            roles: new[] { RoleNames.DealerManager },
            dealerId: 1);

        await EnsureUserAsync(userManager, logger,
            email: "dealerstaff@evdms.vn",
            displayName: "Dealer Staff",
            roles: new[] { RoleNames.DealerStaff },
            dealerId: 1);
    }

    private static async Task EnsureUserAsync(
        UserManager<ApplicationUser> userManager,
        ILogger logger,
        string email,
        string displayName,
        string[] roles,
        int? dealerId = null)
    {
        var user = await userManager.Users.FirstOrDefaultAsync(u => u.Email == email);
        if (user == null)
        {
            user = new ApplicationUser
            {
                UserName = email,
                Email = email,
                DisplayName = displayName,
                EmailConfirmed = true,
                DealerId = dealerId
            };

            var createResult = await userManager.CreateAsync(user, "Evdms@123");
            if (!createResult.Succeeded)
            {
                var errors = string.Join(", ", createResult.Errors.Select(e => e.Description));
                logger.LogError("Failed to create user {Email}: {Errors}", email, errors);
                return;
            }

            logger.LogInformation("Created user {Email}", email);
        }
        else if (user.DealerId != dealerId)
        {
            user.DealerId = dealerId;
            await userManager.UpdateAsync(user);
        }

        foreach (var role in roles)
        {
            if (!await userManager.IsInRoleAsync(user, role))
            {
                await userManager.AddToRoleAsync(user, role);
                logger.LogInformation("Assigned role {Role} to {Email}", role, email);
            }
        }
    }
}
