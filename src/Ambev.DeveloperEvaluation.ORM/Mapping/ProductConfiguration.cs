using Ambev.DeveloperEvaluation.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Diagnostics.CodeAnalysis;

namespace Ambev.DeveloperEvaluation.ORM.Mapping;

[ExcludeFromCodeCoverage]

public class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.ToTable("Products");

        builder.HasKey(u => u.Id);
        builder.Property(u => u.Id).HasColumnType("uuid").HasDefaultValueSql("gen_random_uuid()");

        builder.Property(u => u.Name).IsRequired().HasMaxLength(50);
        builder.Property(u => u.Description).IsRequired().HasMaxLength(100);
        builder.Property(u => u.Price).IsRequired();
        builder.Property(u => u.Quantity).IsRequired();    
        builder.Property(u => u.Active).IsRequired().HasDefaultValue(true);    

        builder.Property(u => u.Category)
            .HasConversion<string>()
            .HasMaxLength(30);        
    }
}
