namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales.CreateSale;

/// <summary>
/// API response model for a successfully created sale.
/// </summary>
public class CreateSaleResponse
{
    /// <summary>System-generated unique identifier (UUID) for the sale.</summary>
    public Guid Id { get; set; }

    /// <summary>The sale number provided during creation.</summary>
    public string SaleNumber { get; set; } = string.Empty;

    /// <summary>Total calculated amount after all item discounts.</summary>
    public decimal TotalAmount { get; set; }
}