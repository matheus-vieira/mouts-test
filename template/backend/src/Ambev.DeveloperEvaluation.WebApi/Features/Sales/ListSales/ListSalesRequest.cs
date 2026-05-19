namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales.ListSales;

/// <summary>
/// Represents the query parameters for filtering, ordering, and paginating sales.
/// All filter parameters are optional — only provided values are applied.
/// </summary>
public class ListSalesRequest
{
    /// <summary>Page number (default: 1).</summary>
    public int _page { get; set; } = 1;

    /// <summary>Number of items per page (default: 10, max: 100).</summary>
    public int _size { get; set; } = 10;

    /// <summary>
    /// Dynamic ordering expression (e.g. "totalAmount desc", "saleDate asc").
    /// Allowed fields: saleDate, totalAmount, customerName, branchName, saleNumber.
    /// </summary>
    public string? _order { get; set; }

    /// <summary>Partial match filter for customer name.</summary>
    public string? CustomerName { get; set; }

    /// <summary>Partial match filter for branch name.</summary>
    public string? BranchName { get; set; }

    /// <summary>Minimum sale date (inclusive).</summary>
    public DateTime? MinDate { get; set; }

    /// <summary>Maximum sale date (inclusive).</summary>
    public DateTime? MaxDate { get; set; }

    /// <summary>Minimum total amount (inclusive).</summary>
    public decimal? MinAmount { get; set; }

    /// <summary>Maximum total amount (inclusive).</summary>
    public decimal? MaxAmount { get; set; }

    /// <summary>Filter by cancellation status. Null returns all.</summary>
    public bool? IsCancelled { get; set; }
}