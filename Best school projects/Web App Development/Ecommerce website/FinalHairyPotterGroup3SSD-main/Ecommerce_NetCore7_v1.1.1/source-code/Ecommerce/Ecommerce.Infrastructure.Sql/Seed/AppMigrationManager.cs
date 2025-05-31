using Ecommerce.Domain.Constants;
using Ecommerce.Infrastructure.Sql.DataAccess;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Ecommerce.Infrastructure.Sql.Seed;

public static class AppMigrationManager
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
            var context = scope.ServiceProvider.GetRequiredService<DataContext>();
            await SeedDefaultOrderStatusAsync(context);
            await SeedDefaultDefaultDeliveryMethodAsync(context);
            await SeedDefaultAppConfigurationAsync(context);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message + ". " + ex.Source);
            throw;
        }
    }

    private static async Task SeedDefaultOrderStatusAsync(DataContext context)
    {
        var defaultOrderStatus = DefaultOrderStatusValue.GetDefaultStatus();
        if (!await context.OrderStatusValues.AnyAsync())
        {
            foreach (var defaultStatus in defaultOrderStatus)
            {
                await context.OrderStatusValues.AddAsync(defaultStatus);
            }
            await context.SaveChangesAsync();
        }
    }

    private static async Task SeedDefaultDefaultDeliveryMethodAsync(DataContext context)
    {
        var defaultDeliveryMethod = DefaultDeliveryMethod.GetDefaultDeliveryMethod();
        if (!await context.DeliveryMethods.AnyAsync())
        {
            foreach (var deliveryMethod in defaultDeliveryMethod)
            {
                await context.DeliveryMethods.AddAsync(deliveryMethod);
            }
            await context.SaveChangesAsync();
        }
    }

    private static async Task SeedDefaultAppConfigurationAsync(DataContext context)
    {
        var defaultAppConfiguration = DefaultAppConfiguration.GetDefaultAppConfiguration();
        if (!await context.AppConfigurations.AnyAsync())
        {
            foreach (var appConfiguration in defaultAppConfiguration)
            {
                await context.AppConfigurations.AddAsync(appConfiguration);
            }
            await context.SaveChangesAsync();
        }
    }


}
