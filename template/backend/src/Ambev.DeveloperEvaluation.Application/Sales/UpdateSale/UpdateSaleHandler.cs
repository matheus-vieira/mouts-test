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
        var sale = await repository.GetByIdAsync(command.Id, cancellationToken)
            ?? throw new KeyNotFoundException($"Sale with ID {command.Id} was not found");

        if (sale.IsCancelled)
            throw new InvalidOperationException("Cannot update a cancelled sale.");

        // Re-creating items to ensure business rules (discounts) are recalculated
        var newItems = command.Items.Select(i => 
            SaleItem.Create(i.ProductId, i.ProductName, i.Quantity, i.UnitPrice));

        // Aggregate method for update (presumed added to Sale entity for internal consistency)
        sale.UpdateItems(newItems);
        
        await repository.UpdateAsync(sale, cancellationToken);
        
        LogSaleUpdated(logger, sale.Id, sale.TotalAmount);

        return mapper.Map<UpdateSaleResult>(sale);
    }
}