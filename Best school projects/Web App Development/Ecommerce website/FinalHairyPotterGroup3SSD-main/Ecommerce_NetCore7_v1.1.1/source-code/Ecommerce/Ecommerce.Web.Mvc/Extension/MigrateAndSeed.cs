using Ecommerce.Infrastructure.Sql.Identity.Seeds;
using Ecommerce.Infrastructure.Sql.Seed;

namespace Ecommerce.Web.Mvc.Extension;

public static class GlobalMigrationManager
{
    public static IHost MigrateAndSeed(this IHost host)
    {
        AppMigrationManager.MigrateDatabaseAsync(host).GetAwaiter().GetResult();
        AppMigrationManager.SeedDatabaseAsync(host).GetAwaiter().GetResult();

        IdentityMigrationManager.MigrateDatabaseAsync(host).GetAwaiter().GetResult();
        IdentityMigrationManager.SeedDatabaseAsync(host).GetAwaiter().GetResult();
        return host;
    }
}
