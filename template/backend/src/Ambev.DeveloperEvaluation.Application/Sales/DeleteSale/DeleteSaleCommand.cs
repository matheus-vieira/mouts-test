using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Sales.DeleteSale;

/// <summary>
/// Command to delete an existing sale.
/// </summary>
/// <remarks>
/// This operation removes the sale aggregate from the system.
/// Depending on business rules, this could represent a hard delete or soft delete.
/// </remarks>
public class DeleteSaleCommand : IRequest<bool>
{
    /// <summary>
    /// The unique identifier of the sale to be deleted.
    /// </summary>
    public Guid Id { get; set; }
}