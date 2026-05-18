namespace Ambev.DeveloperEvaluation.Domain.Repositories.Sales;
using Ambev.DeveloperEvaluation.Domain.Entities.Sales;

/// Handles read operations for <see cref="Sale"/> aggregates.
/// Injected exclusively by query-side handlers.
/// </summary>
public interface ISaleReadRepository
{
    /// <summary>Returns a sale by its unique identifier, or null if not found.</summary>
    Task<Sale?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>Returns a sale by its human-readable sale number, or null if not found.</summary>
    Task<Sale?> GetBySaleNumberAsync(string saleNumber, CancellationToken cancellationToken = default);
}
