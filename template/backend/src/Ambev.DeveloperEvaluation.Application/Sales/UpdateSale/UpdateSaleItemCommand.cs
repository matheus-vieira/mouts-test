namespace Ambev.DeveloperEvaluation.Application.Sales.UpdateSale;

/// <summary>
/// Represents a single item within the <see cref="UpdateSaleCommand"/>.
/// </summary>
public class UpdateSaleItemCommand
{
    /// <summary>
    /// The unique identifier of the product being sold.
    /// </summary>
    public Guid ProductId { get; set; }

    /// <summary>
    /// The name of the product at the time of the sale.
    /// Stored denormalized to avoid joins on read operations.
    /// </summary>
    public string ProductName { get; set; } = string.Empty;

    /// <summary>
    /// The quantity of the product.
    /// Discount rules are applied based on this value:
    /// 4–9 items = 10%, 10–20 items = 20%, above 20 = not allowed.
    /// </summary>
    public int Quantity { get; set; }

    /// <summary>
    /// The unit price of the product at the time of the sale.
    /// </summary>
    public decimal UnitPrice { get; set; }
}