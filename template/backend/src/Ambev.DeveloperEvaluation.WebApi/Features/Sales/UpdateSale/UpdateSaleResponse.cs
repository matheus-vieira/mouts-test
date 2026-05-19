namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales.UpdateSale;

/// <summary>
/// Represents the HTTP response returned after successfully updating a sale.
/// </summary>
public class UpdateSaleResponse
{
    /// <summary>
    /// The unique identifier of the updated sale.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// The business identifier of the sale.
    /// Example: SALE-20260519-162726766
    /// </summary>
    public string SaleNumber { get; set; } = string.Empty;

    /// <summary>
    /// The recalculated total amount of the sale after applying all discount rules.
    /// </summary>
    public decimal TotalAmount { get; set; }

    /// <summary>
    /// Indicates whether the sale has been cancelled.
    /// </summary>
    public bool IsCancelled { get; set; }
}