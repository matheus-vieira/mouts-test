using Ambev.DeveloperEvaluation.Domain.Entities.Sales;
using Ambev.DeveloperEvaluation.Domain.Repositories.Sales;
using Microsoft.EntityFrameworkCore;

namespace Ambev.DeveloperEvaluation.ORM.Repositories.Sales;

/// <summary>
/// Handles read operations for <see cref="Sale"/> aggregates.
/// Eagerly loads the Items collection to ensure aggregate consistency.
/// </summary>
public abstract class SaleRepository(DefaultContext context)
{
    protected readonly DefaultContext _context = context;
    protected readonly DbSet<Sale> Table = context.Sales;

    protected Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return _context.SaveChangesAsync(cancellationToken);
    }
}