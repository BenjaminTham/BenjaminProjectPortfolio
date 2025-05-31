using Ecommerce.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ecommerce.Infrastructure.Sql.Configurations;

public class VariantConfiguration : IEntityTypeConfiguration<Variant>
{
    public void Configure(EntityTypeBuilder<Variant> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(prop => prop.Title)
            .HasMaxLength(256)
            .IsRequired();

        builder.Property(prop => prop.Price)
            .IsRequired();

        builder.Property(prop => prop.Price)
            .IsRequired();

        builder.Property(prop => prop.ProductId)
            .IsRequired();

        builder.Property(prop => prop.Sku)
            .HasMaxLength(50);
    }
}
