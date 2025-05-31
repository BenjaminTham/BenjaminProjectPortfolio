using Ecommerce.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ecommerce.Infrastructure.Sql.Configurations;

public class OrderPaymentConfiguration : IEntityTypeConfiguration<OrderPayment>
{
    public void Configure(EntityTypeBuilder<OrderPayment> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(prop => prop.PaymentType)
            .HasMaxLength(100);

        builder.Property(prop => prop.Reference)
            .HasMaxLength(500);

        builder.HasOne(prop => prop.Order)
            .WithOne(prop => prop.OrderPayments)
            .HasForeignKey<OrderPayment>(b => b.OrderId);

    }
}
