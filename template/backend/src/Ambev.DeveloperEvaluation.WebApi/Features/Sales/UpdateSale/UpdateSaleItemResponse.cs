namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales.UpdateSale;

/// <summary>
/// Represents a single item in the HTTP response after updating a sale.
/// </summary>
public class UpdateSaleItemResponse
{
    /// <summary>
    /// The unique identifier of the product.
    /// </summary>
    public Guid ProductId { get; set; }

    /// <summary>
    /// The updated quantity of the product in the sale.
    /// </summary>
    public int Quantity { get; set; }

    /// <summary>
    /// The calculated total amount for this item after applying discount rules.
    /// </summary>
    public decimal TotalAmount { get; set; }
}