using Ecommerce.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ecommerce.Infrastructure.Sql.Configurations;

public class ColorConfiguration : IEntityTypeConfiguration<Color>
{
    public void Configure(EntityTypeBuilder<Color> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(prop => prop.Name)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(prop => prop.HexCode)
            .HasMaxLength(8);

        builder.Property(prop => prop.IsActive)
            .HasDefaultValue(false);


        builder.HasIndex(e => e.Name)
            .IsUnique()
            .IsClustered(false);

        builder.HasIndex(e => e.HexCode)
            .IsUnique()
            .IsClustered(false);

    }
}
