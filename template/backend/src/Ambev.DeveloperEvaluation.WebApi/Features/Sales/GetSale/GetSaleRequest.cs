namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales.GetSale;

/// <summary>
/// API request model for retrieving a sale by ID.
/// </summary>
public class GetSaleRequest
{
    /// <summary>
    /// The unique identifier of the sale to retrieve.
    /// </summary>
    public Guid Id { get; set; }
}