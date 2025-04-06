using Ambev.DeveloperEvaluation.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ambev.DeveloperEvaluation.ORM.Mapping;

public class SalesConfiguration : IEntityTypeConfiguration<Sales>
{
    public void Configure(EntityTypeBuilder<Sales> builder)
    {
        builder.ToTable("Sales");

        builder.HasKey(s => s.Id);

        builder.Property(s => s.SaleNumber)
            .IsRequired();

        builder.Property(s => s.SoldAt)
            .IsRequired();

        builder.Property(s => s.TotalSaleAmount)
            .HasColumnType("decimal(18,2)")
            .IsRequired();       

        builder.Property(s => s.Status)
            .HasConversion<string>()
            .IsRequired();

        builder.Property(s => s.CreatedAt)
            .IsRequired();

        builder.Property(s => s.UpdatedAt);

        builder.Property(s => s.CancelledAt);

        builder.Property(s => s.DeletedAt);

        builder.Property(s => s.BoughtById)
            .IsRequired();
        
        
        builder.HasOne(s => s.CreatedBy)
            .WithMany()
            .HasForeignKey("CreatedById")
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(s => s.BoughtBy)
            .WithMany()
            .HasForeignKey(s => s.BoughtById)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(s => s.CancelledBy)
            .WithMany()
            .HasForeignKey("CancelledById")
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(s => s.DeletedBy)
            .WithMany()
            .HasForeignKey("DeletedById")
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(s => s.Items)
            .WithOne(i => i.Sale)
            .HasForeignKey(i => i.SaleId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
