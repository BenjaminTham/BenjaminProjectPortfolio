using Ecommerce.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ecommerce.Infrastructure.Sql.Configurations;

public class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(prop => prop.Name)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(prop => prop.Slug)
            .HasMaxLength(150)
            .IsRequired();

        builder.Property(prop => prop.ShortDescription)
            .HasMaxLength(200);

        builder.Property(prop => prop.VariableTheme)
            .HasMaxLength(100);

        builder.Property(prop => prop.CategoryId)
            .IsRequired();

    }
}
