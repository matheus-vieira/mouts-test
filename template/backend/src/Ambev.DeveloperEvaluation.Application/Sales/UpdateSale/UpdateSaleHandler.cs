using Ambev.DeveloperEvaluation.Domain.Entities.Sales;
using Ambev.DeveloperEvaluation.Domain.Repositories.Sales;
using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Ambev.DeveloperEvaluation.Application.Sales.UpdateSale;

public partial class UpdateSaleHandler(
    ISaleRepository repository,
    IMapper mapper,
    ILogger<UpdateSaleHandler> logger) : IRequestHandler<UpdateSaleCommand, UpdateSaleResult>
{
    public async Task<UpdateSaleResult> Handle(UpdateSaleCommand command, CancellationToken cancellationToken)
    {
        LogUpdateInitiated(logger, command.Id);

        var sale = await repository.GetByIdAsync(command.Id, cancellationToken)
            ?? throw new KeyNotFoundException($"Sale with ID {command.Id} was not found");

        if (sale.IsCancelled)
            throw new InvalidOperationException("Cannot update a cancelled sale.");

        // Re-creating items ensures business rules (discounts) are recalculated via SaleItem.Create
        var newItems = command.Items.Select(i =>
            SaleItem.Create(i.ProductId, i.ProductName, i.Quantity, i.UnitPrice));

        sale.UpdateItems(newItems);

        await repository.UpdateAsync(sale, cancellationToken);

        LogUpdateSuccess(logger, sale.Id, sale.Items.Count);

        return mapper.Map<UpdateSaleResult>(sale);
    }
}