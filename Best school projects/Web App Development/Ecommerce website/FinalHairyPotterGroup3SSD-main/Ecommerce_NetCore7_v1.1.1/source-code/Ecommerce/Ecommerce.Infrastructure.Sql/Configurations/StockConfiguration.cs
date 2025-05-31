using Ecommerce.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ecommerce.Infrastructure.Sql.Configurations;

public class StockConfiguration : IEntityTypeConfiguration<Stock>
{
    public void Configure(EntityTypeBuilder<Stock> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(prop => prop.Description)
            .HasMaxLength(500);

        builder.Property(prop => prop.StockInputType)
            .HasMaxLength(100)
            .IsRequired();

    }
}
