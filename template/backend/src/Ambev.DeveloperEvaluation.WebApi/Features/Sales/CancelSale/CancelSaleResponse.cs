namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales.CancelSale;

/// <summary>
/// Represents the HTTP response for a successful sale cancellation.
/// </summary>
public class CancelSaleResponse
{
    /// <summary>
    /// The unique identifier of the cancelled sale.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// The human-readable sale number (e.g., "SALE-20240101-123456789").
    /// </summary>
    public string SaleNumber { get; set; } = string.Empty;

    /// <summary>
    /// Confirmation message.
    /// </summary>
    public string Message { get; set; } = "Sale cancelled successfully.";
}