using Ambev.DeveloperEvaluation.Domain.Events.Sales;
using Ambev.DeveloperEvaluation.Domain.Repositories.Sales;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Ambev.DeveloperEvaluation.Application.Sales.CancelSale;

/// <summary>
/// Handler for the CancelSaleCommand.
/// Ensures the domain invariant for cancellation is enforced.
/// </summary>
public partial class CancelSaleHandler(
    ISaleReadRepository readRepository,
    ISaleUpdateRepository updateRepository,
    IMediator mediator,
    ILogger<CancelSaleHandler> logger) : IRequestHandler<CancelSaleCommand, Unit>
{
    public async Task<Unit> Handle(CancelSaleCommand command, CancellationToken cancellationToken)
    {
        var sale = await readRepository.GetByIdAsync(command.Id, cancellationToken)
            ?? throw new KeyNotFoundException($"Sale with ID {command.Id} was not found");

        sale.Cancel();
      
        await updateRepository.UpdateAsync(sale, cancellationToken);
      
        LogSaleCancelled(logger, sale.Id, sale.SaleNumber);

        // Publish SaleCancelledEvent
        await mediator.Publish(new SaleCancelledEvent(sale.Id, sale.SaleNumber), cancellationToken);

        return Unit.Value;
    }
}