using Identity;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace Services;

public class UserService
{
    readonly ILogger logger;
    bool initialized = false;

    public UserService(ILogger<UserService> logger)
    {
        this.logger = logger;
    }

    public async Task InitRoleAndClaim(UserManager<AppUser> userManager, RoleManager<AppRole> roleManager)
    {
        if (initialized)
            return;
        initialized = true;

        logger.LogInformation("InitRoleAndClaim");
        const string roleName = "Admin";
        var role = await roleManager.FindByNameAsync(roleName);
        if (role == null)
            await roleManager.CreateAsync(new AppRole(roleName));

        role = await roleManager.FindByNameAsync(roleName);

        await roleManager.AddClaimAsync(role, new Claim(nameof(AppPermission), AppPermission.Manage));
        foreach (var user in userManager.Users)
            await userManager.AddToRoleAsync(user, role.Name);
    }
}

public static class AppPermission
{
    public const string Manage = nameof(Manage);
}
