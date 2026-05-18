using Ambev.DeveloperEvaluation.Domain.Entities.Sales;
using Ambev.DeveloperEvaluation.Domain.Repositories.Sales;

namespace Ambev.DeveloperEvaluation.ORM.Repositories.Sales;

/// <summary>
/// Handles update operations for existing <see cref="Sale"/> aggregates.
/// Used by update and cancel handlers.
/// </summary>
public class SaleUpdateRepository(
    DefaultContext context
) : SaleRepository(context),
    ISaleUpdateRepository
{
    /// <inheritdoc/>
    public async Task<Sale> UpdateAsync(Sale sale, CancellationToken cancellationToken = default)
    {
        Table.Update(sale);
        await SaveChangesAsync(cancellationToken);
        return sale;
    }
}