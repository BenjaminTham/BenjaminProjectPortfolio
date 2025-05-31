using Ecommerce.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ecommerce.Infrastructure.Sql.Configurations;

public class CustomerConfiguration : IEntityTypeConfiguration<Customer>
{
    public void Configure(EntityTypeBuilder<Customer> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(prop => prop.ShippingAddress)
            .HasMaxLength(500);

        builder.Property(prop => prop.BillingAddress)
            .HasMaxLength(500);

        builder.Property(prop => prop.ApplicationUserId)
            .HasMaxLength(450)
            .IsRequired();

        builder.HasIndex(prop => prop.ApplicationUserId)
            .IsUnique();

        builder.HasOne(c => c.User)
            .WithOne(c=>c.Customer)
            .HasForeignKey<Customer>(c => c.ApplicationUserId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade); ;


        //modelBuilder.Entity<Customer>()
        //    .HasOne(c => c.User)
        //    .WithOne(u => u.Customer)
        //    .HasForeignKey<Customer>(c => c.ApplicationUserId)
        //    .IsRequired();


        //builder.HasOne(prop => prop.User)
        //    .WithOne(prop => prop.Customer)
        //    .HasForeignKey<Customer>(prop => prop.ApplicationUserId);

    }
}
