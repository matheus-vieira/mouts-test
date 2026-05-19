namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales.CancelSale;

/// <summary>
/// Represents the HTTP request for cancelling a sale.
/// The sale ID is provided via route parameter.
/// </summary>
public class CancelSaleRequest
{
    /// <summary>
    /// The unique identifier of the sale to be cancelled.
    /// </summary>
    public Guid Id { get; set; }
}