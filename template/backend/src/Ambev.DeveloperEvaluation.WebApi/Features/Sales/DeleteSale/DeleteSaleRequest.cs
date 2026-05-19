namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales.DeleteSale;

/// <summary>
/// Represents the HTTP request for deleting a sale.
/// The sale ID is provided via route parameter.
/// </summary>
public class DeleteSaleRequest
{
    /// <summary>
    /// The unique identifier of the sale to be deleted.
    /// </summary>
    public Guid Id { get; set; }
}