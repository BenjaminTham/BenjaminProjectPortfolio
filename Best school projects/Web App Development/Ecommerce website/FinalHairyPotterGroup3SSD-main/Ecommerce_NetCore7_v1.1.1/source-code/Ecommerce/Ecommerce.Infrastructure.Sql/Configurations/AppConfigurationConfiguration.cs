using Ecommerce.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ecommerce.Infrastructure.Sql.Configurations;

public class AppConfigurationConfiguration : IEntityTypeConfiguration<AppConfiguration>
{
    public void Configure(EntityTypeBuilder<AppConfiguration> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(prop => prop.Key)
            .HasMaxLength(256)
            .IsRequired();

        builder.Property(prop => prop.Value)
            .IsRequired();
    }
}