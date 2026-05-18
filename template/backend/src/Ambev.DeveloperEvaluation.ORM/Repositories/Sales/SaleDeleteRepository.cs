using Ambev.DeveloperEvaluation.Domain.Repositories.Sales;
using Microsoft.EntityFrameworkCore;

namespace Ambev.DeveloperEvaluation.ORM.Repositories.Sales;

/// <summary>
/// Handles delete operations for Sale aggregates.
/// Loads the aggregate before removal to ensure EF Core tracks the entity correctly.
/// </summary>
public class SaleDeleteRepository(
    DefaultContext context
) : SaleRepository(context),
    ISaleDeleteRepository
{
    /// <inheritdoc/>
    /// <returns><c>true</c> if deleted, <c>false</c> if not found.</returns>
    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var sale = await Table
            .FirstOrDefaultAsync(s => s.Id == id, cancellationToken);

        if (sale is null)
            return false;

        Table.Remove(sale);
        await SaveChangesAsync(cancellationToken);
        return true;
    }
}