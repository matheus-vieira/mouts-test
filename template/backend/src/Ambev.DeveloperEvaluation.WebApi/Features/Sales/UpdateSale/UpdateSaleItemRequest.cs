namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales.UpdateSale;

/// <summary>
/// Represents a single item in the update sale request.
/// </summary>
public class UpdateSaleItemRequest
{
    /// <summary>
    /// The product identifier.
    /// </summary>
    public Guid ProductId { get; set; }

    /// <summary>
    /// The product name captured at update time.
    /// </summary>
    public string ProductName { get; set; } = string.Empty;

    /// <summary>
    /// The quantity of the product (max 20 allowed by business rule).
    /// </summary>
    public int Quantity { get; set; }

    /// <summary>
    /// The unit price of the product.
    /// </summary>
    public decimal UnitPrice { get; set; }
}