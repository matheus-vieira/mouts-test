using Ambev.DeveloperEvaluation.Domain.Entities.Sales;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ambev.DeveloperEvaluation.ORM.Mapping.Sales;

/// <summary>
/// Entity Framework configuration for the <see cref="Sale"/> aggregate root.
/// Defines table schema, indexes, and aggregate collection relationship.
/// </summary>
public class SaleConfiguration : IEntityTypeConfiguration<Sale>
{
    public void Configure(EntityTypeBuilder<Sale> builder)
    {
        // Table mapping
        builder.ToTable("Sales");

        // Primary Key inherited from BaseEntity
        builder.HasKey(s => s.Id);
        builder.Property(s => s.Id).HasDefaultValueSql("gen_random_uuid()");

        // Business identification with unique index
        builder.Property(s => s.SaleNumber).IsRequired().HasMaxLength(50);
        builder.HasIndex(s => s.SaleNumber).IsUnique();

        // Customer and Branch identification
        builder.Property(s => s.CustomerId).IsRequired();
        builder.Property(s => s.CustomerName).IsRequired().HasMaxLength(200);
        builder.Property(s => s.BranchId).IsRequired();
        builder.Property(s => s.BranchName).IsRequired().HasMaxLength(200);

        // Core properties
        builder.Property(s => s.SaleDate).IsRequired();
        builder.Property(s => s.TotalAmount).IsRequired().HasPrecision(18, 2);
        builder.Property(s => s.IsCancelled).IsRequired().HasDefaultValue(false);

        // Aggregate relationship mapping (One-to-Many)
        // Using PropertyAccessMode.Field to populate the private List<SaleItem> _items
        builder.HasMany(s => s.Items)
            .WithOne()
            .HasForeignKey("SaleId")
            .OnDelete(DeleteBehavior.Cascade)
            .Metadata.PrincipalToDependent?.SetPropertyAccessMode(PropertyAccessMode.Field);
    }
}