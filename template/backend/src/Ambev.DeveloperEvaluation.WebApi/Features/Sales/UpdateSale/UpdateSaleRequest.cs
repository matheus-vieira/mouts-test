namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales.UpdateSale;

/// <summary>
/// Represents the HTTP request body for updating an existing sale.
/// The sale ID is typically provided via route parameter.
/// </summary>
public class UpdateSaleRequest
{
    /// <summary>
    /// The unique identifier of the sale to be updated.
    /// Ignored in the body if bound via route.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// The unique identifier of the customer.
    /// </summary>
    public Guid CustomerId { get; set; }

    /// <summary>
    /// The customer name captured at update time.
    /// </summary>
    public string CustomerName { get; set; } = string.Empty;

    /// <summary>
    /// The unique identifier of the branch.
    /// </summary>
    public Guid BranchId { get; set; }

    /// <summary>
    /// The branch name captured at update time.
    /// </summary>
    public string BranchName { get; set; } = string.Empty;

    /// <summary>
    /// The updated list of items in the sale.
    /// </summary>
    public List<UpdateSaleItemRequest> Items { get; set; } = [];
}
