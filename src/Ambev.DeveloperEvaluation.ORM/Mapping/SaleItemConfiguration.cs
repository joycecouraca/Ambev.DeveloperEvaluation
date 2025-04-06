using Ambev.DeveloperEvaluation.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ambev.DeveloperEvaluation.ORM.Mapping;

public class SaleItemConfiguration : IEntityTypeConfiguration<SaleItem>
{
    public void Configure(EntityTypeBuilder<SaleItem> builder)
    {
        builder.ToTable("SaleItems");

        builder.HasKey(si => si.Id);

        builder.Property(si => si.Quantity)
            .IsRequired();

        builder.Property(si => si.UnitPrice)
            .HasPrecision(18, 2)
            .IsRequired();

        builder.Property(si => si.Status)
            .HasConversion<string>() 
            .IsRequired();

        builder.Property(si => si.CreatedAt)
            .IsRequired();

        builder.Property(si => si.UpdatedAt)
            .IsRequired(false);

        builder.Property(si => si.DeletedAt)
            .IsRequired(false);

        builder.HasOne(si => si.Sale)
            .WithMany(s => s.Items)
            .HasForeignKey(si => si.SaleId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(si => si.Product)
            .WithMany()
            .HasForeignKey(si => si.ProductId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(si => si.DeletedBy)
            .WithMany()
            .OnDelete(DeleteBehavior.NoAction); 

        builder.HasOne(si => si.CancelBy)
            .WithMany()
            .OnDelete(DeleteBehavior.NoAction);
    }
}
