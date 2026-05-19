namespace Ambev.DeveloperEvaluation.Application.Sales.UpdateSale;

/// <summary>
/// Represents the result returned by the <see cref="UpdateSaleHandler"/>
/// after successfully updating a sale.
/// </summary>
public class UpdateSaleResult
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
    /// The recalculated total amount of the sale after applying all discount rules
    /// across all updated items.
    /// </summary>
    public decimal TotalAmount { get; set; }

    /// <summary>
    /// The updated list of items with recalculated totals and applied discounts.
    /// </summary>
    public List<UpdateSaleItemResult> Items { get; set; } = [];
}