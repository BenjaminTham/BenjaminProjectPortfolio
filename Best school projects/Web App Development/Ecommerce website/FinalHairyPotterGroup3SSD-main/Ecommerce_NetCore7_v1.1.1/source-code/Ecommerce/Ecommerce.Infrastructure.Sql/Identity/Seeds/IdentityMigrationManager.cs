using Ecommerce.Domain.Identity.Constants;
using Ecommerce.Domain.Identity.Entities;
using Ecommerce.Domain.Identity.Permissions;
using Ecommerce.Infrastructure.Sql.DataAccess;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Ecommerce.Infrastructure.Sql.Identity.Seeds;

public static class IdentityMigrationManager
{
    public static async Task MigrateDatabaseAsync(IHost host)
    {
        using var scope = host.Services.CreateScope();
        await using var context = scope.ServiceProvider.GetRequiredService<DataContext>();
        try
        {
            await context.Database.MigrateAsync();
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message + ". " + ex.Source);
            throw;
        }
    }
    public static async Task SeedDatabaseAsync(IHost host)
    {
        using var scope = host.Services.CreateScope();
        try
        {
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            var permissionHelper = scope.ServiceProvider.GetRequiredService<IPermissionHelper>();
            await SeedDefaultUserRolesAsync(userManager, roleManager, permissionHelper);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message + ". " + ex.Source);
            throw;
        }
    }

    private static async Task SeedDefaultUserRolesAsync(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, IPermissionHelper permissionHelper)
    {
        var defaultRoles = DefaultApplicationRoles.GetDefaultRoles();
        if (!await roleManager.Roles.AnyAsync())
        {
            foreach (var defaultRole in defaultRoles)
            {
                await roleManager.CreateAsync(defaultRole);
            }
        }
        if (!await roleManager.RoleExistsAsync(DefaultApplicationRoles.SuperAdmin))
        {
            await roleManager.CreateAsync(new IdentityRole(DefaultApplicationRoles.SuperAdmin));
        }
        var defaultUser = DefaultApplicationUsers.GetSuperUser();
        var userByName = await userManager.FindByNameAsync(defaultUser.UserName);
        var userByEmail = await userManager.FindByEmailAsync(defaultUser.Email);
        if (userByName == null && userByEmail == null)
        {
            var userCreated = await userManager.CreateAsync(defaultUser, "superadmin");
            if (userCreated.Succeeded)
            {
                foreach (var defaultRole in defaultRoles)
                {
                    await userManager.AddToRoleAsync(defaultUser, defaultRole.Name);
                }
            }
            else
            {
                Console.WriteLine(userCreated.Errors);
            }

        }

        var role = await roleManager.FindByNameAsync(DefaultApplicationRoles.SuperAdmin);
        var rolePermissions = await roleManager.GetClaimsAsync(role);
        var allPermissions = permissionHelper.GetAllPermissions();
        foreach (var permission in allPermissions)
        {
            if (rolePermissions.Any(x => x.Value == permission.Value && x.Type == permission.Type) == false)
            {
                await roleManager.AddClaimAsync(role, permission);
            }
        }
    }
}
