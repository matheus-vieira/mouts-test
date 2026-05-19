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
    ILogger<CancelSaleHandler> logger) : IRequestHandler<CancelSaleCommand, Unit>
{
    public async Task<Unit> Handle(CancelSaleCommand command, CancellationToken cancellationToken)
    {
        var sale = await readRepository.GetByIdAsync(command.Id, cancellationToken)
            ?? throw new KeyNotFoundException($"Sale with ID {command.Id} was not found");

        sale.Cancel();
        
        await updateRepository.UpdateAsync(sale, cancellationToken);
        
        LogSaleCancelled(logger, sale.Id, sale.SaleNumber);

        return Unit.Value;
    }
}