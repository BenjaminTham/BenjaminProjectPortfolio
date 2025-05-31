using Ecommerce.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ecommerce.Infrastructure.Sql.Configurations;

public class OrderStatusConfiguration : IEntityTypeConfiguration<OrderStatus>
{
    public void Configure(EntityTypeBuilder<OrderStatus> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(prop => prop.OrderId)
            .HasMaxLength(100);

        builder.Property(prop => prop.Description)
            .HasMaxLength(256)
            .IsRequired(false);

    }
}
