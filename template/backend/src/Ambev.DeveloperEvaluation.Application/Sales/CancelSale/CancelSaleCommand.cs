using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Sales.CancelSale;

/// <summary>
/// Command to cancel an existing sale.
/// </summary>
/// <param name="Id">The unique identifier of the sale to cancel.</param>
public record CancelSaleCommand(Guid Id) : IRequest<Unit>;