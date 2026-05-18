using Ambev.DeveloperEvaluation.Domain.Common;

namespace Ambev.DeveloperEvaluation.Domain.Entities.Sales;

/// <summary>
/// Represents a single product line within a sale.
/// </summary>
/// <remarks>
/// This is a partial class. Responsibilities are split across:
/// - <c>SaleItem.Factory.cs</c>   — creation and guard clauses
/// - <c>SaleItem.Discount.cs</c>  — discount calculation logic
/// - <c>SaleItem.Validation.cs</c> — FluentValidation integration
/// </remarks>
public partial class SaleItem : BaseEntity
{
    /// <summary>External identity reference to the product catalog.</summary>
    public Guid ProductId { get; private set; }

    /// <summary>Denormalized product name captured at the time of sale.</summary>
    public string ProductName { get; private set; } = string.Empty;

    /// <summary>Number of units sold. Must be between 1 and 20.</summary>
    public int Quantity { get; private set; }

    /// <summary>Unit price captured at the time of sale.</summary>
    public decimal UnitPrice { get; private set; }

    /// <summary>Calculated discount amount based on quantity tier rules.</summary>
    public decimal Discount { get; private set; }

    /// <summary>Final amount after discount: (UnitPrice * Quantity) - Discount.</summary>
    public decimal TotalAmount { get; private set; }

    /// <summary>Required by EF Core. Use <see cref="Create"/> for domain creation.</summary>
    protected SaleItem() { }
}