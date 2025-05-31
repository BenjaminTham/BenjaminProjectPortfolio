using Ecommerce.Application.Common;
using Ecommerce.Domain.Entities;
using Ecommerce.Domain.Identity.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.Infrastructure.Sql.DataAccess;

public partial class DataContext : IdentityDbContext<ApplicationUser>, IDataContext
{
    private readonly IHttpContextAccessor _httpContext;

    public DataContext(DbContextOptions<DataContext> options, IHttpContextAccessor httpContext) : base(options)
    {
        _httpContext = httpContext;
        this.Database.Migrate();
    }

    public DbSet<Category> Categories { get; set; }
    public DbSet<Product> Products { get; set; }
    public DbSet<Gallery> Galleries { get; set; }
    //code continue below

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


}
