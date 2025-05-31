using Ecommerce.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ecommerce.Infrastructure.Sql.Configurations;

public class CustomerReviewConfiguration : IEntityTypeConfiguration<CustomerReview>
{
    public void Configure(EntityTypeBuilder<CustomerReview> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(prop => prop.Rating)
            .HasColumnType("tinyint")
            .IsRequired();

        builder.Property(prop => prop.Comment)
            .HasMaxLength(500)
            .IsRequired();

        builder.Property(prop => prop.Reply)
            .HasMaxLength(500);

        builder.Property(prop => prop.RepliedBy)
            .HasMaxLength(100);

        builder.HasOne(prop => prop.Product)
            .WithMany(prop => prop.CustomerReviews)
            .HasForeignKey(o => o.ProductId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(prop => prop.Customer)
            .WithMany(prop => prop.CustomerReviews)
            .HasForeignKey(o => o.CustomerId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
