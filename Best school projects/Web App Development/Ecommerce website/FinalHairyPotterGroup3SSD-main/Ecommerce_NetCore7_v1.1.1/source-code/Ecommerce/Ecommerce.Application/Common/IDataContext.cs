using Ecommerce.Domain.Entities;
using Ecommerce.Domain.Identity.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Storage;

namespace Ecommerce.Application.Common;
public interface IDataContext
{
    public DbSet<Category> Categories { get; set; }
    public DbSet<Product> Products { get; set; }
    public DbSet<Gallery> Galleries { get; set; }
    public DbSet<ProductImage> ProductImages { get; set; }
    public DbSet<Size> Sizes { get; set; }
    public DbSet<Color> Colors { get; set; }
    public DbSet<Variant> Variants { get; set; }
    public DbSet<VariantImage> VariantImages { get; set; }
    public DbSet<Order> Orders { get; set; }
    public DbSet<OrderPayment> OrderPayments { get; set; }
    public DbSet<OrderDetails> OrderDetails { get; set; }
    public DbSet<OrderStatus> OrderStatus { get; set; }
    public DbSet<OrderStatusValue> OrderStatusValues { get; set; }
    public DbSet<DeliveryMethod> DeliveryMethods { get; set; }
    public DbSet<AppConfiguration> AppConfigurations { get; set; }
    public DbSet<Stock> Stocks { get; set; }
    public DbSet<CustomerReview> CustomerReviews { get; set; }
    public DbSet<Customer> Customers { get; set; }
    public DbSet<ContactQuery> ContactQueries { get; set; }
    public DbSet<User> Users { get; set; }


    Task<int> SaveChangesAsync();
    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
    IDbContextTransaction BeginTransaction();
    EntityEntry<TEntity> Entry<TEntity>(TEntity entity) where TEntity : class;
    void Reload();
}