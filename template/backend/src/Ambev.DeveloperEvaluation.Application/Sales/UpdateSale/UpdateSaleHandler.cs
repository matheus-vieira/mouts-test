using Ambev.DeveloperEvaluation.Domain.Entities.Sales;
using Ambev.DeveloperEvaluation.Domain.Events.Sales;
using Ambev.DeveloperEvaluation.Domain.Repositories.Sales;
using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Ambev.DeveloperEvaluation.Application.Sales.UpdateSale;

/// <summary>
/// Handler for processing the <see cref="UpdateSaleCommand"/>.
/// Implements domain-driven logic to update an existing sale,
/// ensuring recalculation of business rules like discounts.
/// </summary>
public partial class UpdateSaleHandler(
    ISaleReadRepository readRepository,
    ISaleUpdateRepository updateRepository,
    IMapper mapper,
    IMediator mediator,
    ILogger<UpdateSaleHandler> logger) : IRequestHandler<UpdateSaleCommand, UpdateSaleResult>
{
    /// <summary>
    /// Handles the update sale request.
    /// </summary>
    /// <param name="command">The update command containing new sale data.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The result of the update operation.</returns>
    /// <exception cref="KeyNotFoundException">Thrown when the sale ID does not exist.</exception>
    /// <exception cref="InvalidOperationException">Thrown when attempting to update a cancelled sale.</exception>
    public async Task<UpdateSaleResult> Handle(UpdateSaleCommand command, CancellationToken cancellationToken)
    {
        LogUpdateInitiated(logger, command.Id);

        // 1. Fetch existing sale — throws if not found
        var sale = await readRepository.GetByIdAsync(command.Id, cancellationToken)
            ?? throw new KeyNotFoundException($"Sale with ID {command.Id} was not found");

        // 2. Update customer and branch via domain method
        // Domain enforces immutability rules (e.g., cancelled sales cannot be updated)
        sale.Update(
            command.CustomerId,
            command.CustomerName,
            command.BranchId,
            command.BranchName);

        // 3. Re-create domain items to trigger discount and total amount recalculation
        // SaleItem.Create enforces business rules (max 20 items, discount tiers, etc.)
        var newItems = command.Items.Select(i =>
            SaleItem.Create(i.ProductId, i.ProductName, i.Quantity, i.UnitPrice));

        // 4. Replace items and recalculate total via domain method
        sale.UpdateItems(newItems);

        // 5. Persist changes
        await updateRepository.UpdateAsync(sale, cancellationToken);

        LogUpdateSuccess(logger, sale.Id, sale.Items.Count);

        // 6. Publish SaleModifiedEvent
        await mediator.Publish(new SaleModifiedEvent(sale), cancellationToken);

        // 7. Map domain entity to result DTO
        return mapper.Map<UpdateSaleResult>(sale);
    }
}