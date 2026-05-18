using Ambev.DeveloperEvaluation.Domain.Repositories.Sales;
using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Sales.CancelSale;

public class CancelSaleHandler(
    ISaleRepository repository) : IRequestHandler<CancelSaleCommand, Unit>
{
    public async Task<Unit> Handle(CancelSaleCommand command, CancellationToken cancellationToken)
    {
        var sale = await repository.GetByIdAsync(command.Id, cancellationToken)
            ?? throw new KeyNotFoundException($"Sale with ID {command.Id} was not found");

        sale.Cancel();
        await repository.UpdateAsync(sale, cancellationToken);

        return Unit.Value;
    }
}