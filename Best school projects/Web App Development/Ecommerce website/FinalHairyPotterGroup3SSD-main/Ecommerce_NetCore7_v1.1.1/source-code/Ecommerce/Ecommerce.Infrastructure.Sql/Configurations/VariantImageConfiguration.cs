using Ecommerce.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ecommerce.Infrastructure.Sql.Configurations;

public class VariantImageConfiguration : IEntityTypeConfiguration<VariantImage>
{
    public void Configure(EntityTypeBuilder<VariantImage> builder)
    {
        builder.HasKey(o => new { o.ImageId, o.VariantId });

        builder.Property(prop => prop.ImageId)
            .HasMaxLength(100);

    }
}
