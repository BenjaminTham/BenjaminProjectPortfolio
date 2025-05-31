using Ecommerce.Infrastructure.Sql.DataAccess;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.Infrastructure.Sql.Helpers;

public static class IdentityInsertHelpers
{
    public static Task EnableIdentityInsert<T>(this DataContext context) => SetIdentityInsert<T>(context, enable: true);
    public static Task DisableIdentityInsert<T>(this DataContext context) => SetIdentityInsert<T>(context, enable: false);

    private static Task SetIdentityInsert<T>(DataContext context, bool enable)
    {
        var entityType = context.Model.FindEntityType(typeof(T));
        var value = enable ? "ON" : "OFF";
        return context.Database.ExecuteSqlRawAsync(
            $"SET IDENTITY_INSERT {entityType.GetSchema()}.{entityType.GetTableName()} {value}");
    }

    public static void SaveChangesWithIdentityInsert<T>(this DataContext context)
    {
        using var transaction = context.Database.BeginTransaction();


        try
        {
            context.EnableIdentityInsert<T>();
            context.SaveChanges();
            context.DisableIdentityInsert<T>();
            transaction.Commit();
        }
        catch (Exception ex)
        {
            transaction.Rollback();
        }
        finally { transaction.Dispose(); }
    }

}
