using Ecommerce.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ecommerce.Infrastructure.Sql.Configurations;

public class ContactQueryConfiguration : IEntityTypeConfiguration<ContactQuery>
{
    public void Configure(EntityTypeBuilder<ContactQuery> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(prop => prop.FullName)
            .HasMaxLength(256);

        builder.Property(prop => prop.Email)
            .HasMaxLength(256);

        builder.Property(prop => prop.Subject)
            .HasMaxLength(50);

        builder.Property(prop => prop.MessageBody)
            .HasMaxLength(200);

    }
}
