using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Sales.UpdateSale;

/// <summary>
/// Command for updating an existing sale.
/// Implements <see cref="IRequest{TResponse}"/> to integrate with the MediatR pipeline.
/// </summary>
public class UpdateSaleCommand : IRequest<UpdateSaleResult>
{
    /// <summary>
    /// The unique identifier of the sale to be updated.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// The unique identifier of the customer associated with the sale.
    /// </summary>
    public Guid CustomerId { get; set; }

    /// <summary>
    /// The denormalized customer name captured at the time of the update.
    /// Stored to avoid joins on read operations.
    /// </summary>
    public string CustomerName { get; set; } = string.Empty;

    /// <summary>
    /// The unique identifier of the branch where the sale is being updated.
    /// </summary>
    public Guid BranchId { get; set; }

    /// <summary>
    /// The denormalized branch name captured at the time of the update.
    /// Stored to avoid joins on read operations.
    /// </summary>
    public string BranchName { get; set; } = string.Empty;

    /// <summary>
    /// The updated list of items for the sale.
    /// Replaces the existing items entirely — partial updates are not supported.
    /// </summary>
    public List<UpdateSaleItemCommand> Items { get; set; } = [];
}