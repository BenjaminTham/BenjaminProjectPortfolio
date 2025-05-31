using Ecommerce.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ecommerce.Infrastructure.Sql.Configurations;

public class DeliveryMethodConfiguration : IEntityTypeConfiguration<DeliveryMethod>
{
    public void Configure(EntityTypeBuilder<DeliveryMethod> builder)
    {
        builder.HasKey(x => x.Id);
        //builder.Property(r => r.Id)
        //    .ValueGeneratedNever();

        builder.Property(prop => prop.Name)
            .HasMaxLength(256);

        builder.Property(prop => prop.Description)
            .HasMaxLength(256);

        builder.Property(prop => prop.IsActive)
            .HasDefaultValue(false);

    }
}
