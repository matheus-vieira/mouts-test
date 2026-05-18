using Ambev.DeveloperEvaluation.Domain.Entities.Sales;
using Microsoft.EntityFrameworkCore;

namespace Ambev.DeveloperEvaluation.ORM.Repositories.Sales;

/// <summary>
/// Abstract base repository for all Sale-related repository implementations.
/// Provides shared access to the EF Core <see cref="DbSet{Sale}"/> and
/// a centralized <see cref="SaveChangesAsync"/> to avoid context duplication.
/// </summary>
/// <remarks>
/// All concrete sale repositories must inherit from this class.
/// This class is not registered in the IoC container directly —
/// only the concrete implementations are registered via their interfaces.
/// </remarks>
public abstract class SaleRepository(DefaultContext context)
{
    protected readonly DefaultContext _context = context;

    /// <summary>Direct access to the Sales table for all derived repositories.</summary>
    protected readonly DbSet<Sale> Table = context.Sales;

    /// <summary>Persists all pending changes in the current unit of work.</summary>
    protected Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        => _context.SaveChangesAsync(cancellationToken);
}