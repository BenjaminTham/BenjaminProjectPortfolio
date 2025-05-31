using Ecommerce.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ecommerce.Infrastructure.Sql.Configurations;

public class OrderStatusValueConfiguration : IEntityTypeConfiguration<OrderStatusValue>
{
    public void Configure(EntityTypeBuilder<OrderStatusValue> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(prop => prop.StatusValue)
            .HasMaxLength(100);

        builder.Property(prop => prop.Description)
            .HasMaxLength(256);

    }
}
