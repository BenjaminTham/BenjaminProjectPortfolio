using Ecommerce.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ecommerce.Infrastructure.Sql.Configurations;

public class CategoryConfiguration : IEntityTypeConfiguration<Category>
{
    public void Configure(EntityTypeBuilder<Category> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(prop => prop.Name)
            .HasMaxLength(256)
            .IsRequired();

        builder.HasMany(prop => prop.Products)
                    .WithOne(prop => prop.Category);

        builder.Property(prop => prop.Slug)
            .HasMaxLength(256);

        builder.HasMany(prop => prop.Children)
            .WithOne(prop => prop.ParentCategory)
            .HasForeignKey(prop => prop.ParentCategoryId)
            .IsRequired(false)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
