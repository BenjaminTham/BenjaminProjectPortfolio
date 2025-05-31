using Ecommerce.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ecommerce.Infrastructure.Sql.Configurations;

public class SizeConfiguration : IEntityTypeConfiguration<Size>
{
    public void Configure(EntityTypeBuilder<Size> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(prop => prop.Name)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(prop => prop.IsActive)
            .HasDefaultValue(false);

        builder.HasIndex(e => e.Name)
            .IsUnique()
            .IsClustered(false);
    }
}
