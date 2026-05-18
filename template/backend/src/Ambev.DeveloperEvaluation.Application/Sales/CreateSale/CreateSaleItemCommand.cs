namespace Ambev.DeveloperEvaluation.Application.Sales.CreateSale;

/// <summary>
/// Represents a single product line within a <see cref="CreateSaleCommand"/>.
/// </summary>
/// <remarks>
/// This class is intentionally kept separate from <see cref="CreateSaleCommand"/>
/// to allow independent validation and future reuse across other commands
/// (e.g. UpdateSaleCommand).
/// Values captured here are denormalized at command time and passed directly
/// to <see cref="Ambev.DeveloperEvaluation.Domain.Entities.Sales.SaleItem.Create"/>.
/// </remarks>
public class CreateSaleItemCommand
{
    /// <summary>External identity reference to the product catalog.</summary>
    public Guid ProductId { get; set; }

    /// <summary>
    /// Denormalized product name captured at request time.
    /// Stored as-is in the domain entity to preserve historical accuracy.
    /// </summary>
    public string ProductName { get; set; } = string.Empty;

    /// <summary>
    /// Number of units to sell.
    /// Must be between 1 and 20 — enforced by both validator and domain entity.
    /// </summary>
    public int Quantity { get; set; }

    /// <summary>
    /// Price per unit at the time of sale.
    /// Discount is calculated automatically by the domain entity based on quantity.
    /// </summary>
    public decimal UnitPrice { get; set; }
}