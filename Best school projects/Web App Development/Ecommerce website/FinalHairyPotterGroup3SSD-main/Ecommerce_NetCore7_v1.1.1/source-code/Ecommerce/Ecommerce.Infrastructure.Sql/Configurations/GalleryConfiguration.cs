using Ecommerce.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ecommerce.Infrastructure.Sql.Configurations;

public class GalleryConfiguration : IEntityTypeConfiguration<Gallery>
{
    public void Configure(EntityTypeBuilder<Gallery> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(prop => prop.Name)
            .HasMaxLength(500)
            .IsRequired();

        builder.Property(prop => prop.Title)
            .HasMaxLength(256);

    }
}
