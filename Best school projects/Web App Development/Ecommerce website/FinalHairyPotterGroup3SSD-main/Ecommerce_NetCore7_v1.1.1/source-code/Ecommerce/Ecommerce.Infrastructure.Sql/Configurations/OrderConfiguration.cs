using Ecommerce.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ecommerce.Infrastructure.Sql.Configurations;

public class OrderConfiguration : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(e => e.InvoiceNo)
            .IsRequired()
            .HasComputedColumnSql($"'IN' + RIGHT('0000000000' + CAST(Id AS VARCHAR(10)), 10)");

        builder.HasIndex(e => e.InvoiceNo)
            .IsUnique()
            .IsClustered(false);

        builder.Property(prop => prop.PaymentMethod)
            .HasMaxLength(100);

        builder.Property(prop => prop.PaymentStatus)
            .HasMaxLength(100);

        builder.Property(prop => prop.ShippingAddress)
            .HasMaxLength(256);

        builder.Property(prop => prop.CustomerName)
            .HasMaxLength(256);

        builder.Property(prop => prop.CustomerMobile)
            .HasMaxLength(20);

        builder.Property(prop => prop.CustomerEmail)
            .HasMaxLength(100);

        builder.Property(prop => prop.CustomerComment)
            .HasMaxLength(500)
            .IsRequired(false);

        builder.HasMany(prop => prop.OrderDetails)
            .WithOne(prop => prop.Order)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(prop => prop.OrderStatus)
            .WithOne(prop => prop.Order)
            .OnDelete(DeleteBehavior.Cascade);

    }
}
