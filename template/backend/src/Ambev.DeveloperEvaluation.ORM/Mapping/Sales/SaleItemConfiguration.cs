using Ambev.DeveloperEvaluation.Domain.Entities.Sales;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ambev.DeveloperEvaluation.ORM.Mapping.Sales;

/// <summary>
/// Entity Framework configuration for the <see cref="SaleItem"/> entity.
/// Defines table schema, constraints, and column types for PostgreSQL.
/// </summary>
public class SaleItemConfiguration : IEntityTypeConfiguration<SaleItem>
{
    public void Configure(EntityTypeBuilder<SaleItem> builder)
    {
        // Table mapping
        builder.ToTable("SaleItems");

        // Primary Key inherited from BaseEntity
        builder.HasKey(s => s.Id);
        builder.Property(s => s.Id).HasDefaultValueSql("gen_random_uuid()");

        // Product Identification
        builder.Property(s => s.ProductId).IsRequired();
        builder.Property(s => s.ProductName).IsRequired().HasMaxLength(200);

        // Numeric values with explicit precision
        builder.Property(s => s.Quantity).IsRequired();
        builder.Property(s => s.UnitPrice).IsRequired().HasPrecision(18, 2);
        builder.Property(s => s.Discount).IsRequired().HasPrecision(18, 2);
        builder.Property(s => s.TotalAmount).IsRequired().HasPrecision(18, 2);

        // Relationship to Sale
        // The foreign key is usually managed by the Sale aggregate collection
    }
}