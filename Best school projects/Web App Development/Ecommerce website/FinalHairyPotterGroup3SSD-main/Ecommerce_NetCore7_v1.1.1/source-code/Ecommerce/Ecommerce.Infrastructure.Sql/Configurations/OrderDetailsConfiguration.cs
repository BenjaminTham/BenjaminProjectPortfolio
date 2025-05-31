using Ecommerce.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ecommerce.Infrastructure.Sql.Configurations;

public class OrderDetailsConfiguration : IEntityTypeConfiguration<OrderDetails>
{
    public void Configure(EntityTypeBuilder<OrderDetails> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(prop => prop.OrderId)
            .HasMaxLength(100);

        builder.Property(prop => prop.ProductName)
            .HasMaxLength(256)
            .IsRequired();

    }
}
