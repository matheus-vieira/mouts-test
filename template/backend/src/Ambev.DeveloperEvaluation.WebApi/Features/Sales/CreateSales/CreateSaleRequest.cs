namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales.CreateSale;

/// <summary>
/// API request model for creating a new sale.
/// Maps client-side JSON input to the internal application command.
/// </summary>
public class CreateSaleRequest
{
    /// <summary>Human-readable unique sale identification.</summary>
    public string SaleNumber { get; set; } = string.Empty;

    /// <summary>External ID of the customer.</summary>
    public Guid CustomerId { get; set; }

    /// <summary>Full name of the customer at time of sale.</summary>
    public string CustomerName { get; set; } = string.Empty;

    /// <summary>External ID of the branch where the sale occurred.</summary>
    public Guid BranchId { get; set; }

    /// <summary>Name of the branch at time of sale.</summary>
    public string BranchName { get; set; } = string.Empty;

    /// <summary>List of products being sold.</summary>
    public List<CreateSaleItemRequest> Items { get; set; } = [];
}
