namespace Ambev.DeveloperEvaluation.Application.Sales.CreateSale;

/// <summary>
/// Response returned by <see cref="CreateSaleHandler"/> after a successful sale creation.
/// </summary>
/// <remarks>
/// This DTO is intentionally minimal — it returns only the data the API layer
/// needs to build the HTTP 201 Created response.
/// For full sale details, use the GetSale query.
/// </remarks>
public class CreateSaleResult
{
    /// <summary>Unique identifier of the newly created sale.</summary>
    public Guid Id { get; set; }

    /// <summary>Human-readable sale number assigned to the created sale.</summary>
    public string SaleNumber { get; set; } = string.Empty;

    /// <summary>
    /// Final total amount after all item discounts have been applied.
    /// Calculated by the domain aggregate during creation.
    /// </summary>
    public decimal TotalAmount { get; set; }
}