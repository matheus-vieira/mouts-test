namespace Ambev.DeveloperEvaluation.Domain.Repositories.Sales;
using Ambev.DeveloperEvaluation.Domain.Entities.Sales;

/// <summary>
/// Handles update operations for existing <see cref="Sale"/> aggregates.
/// Injected exclusively by update-side handlers (e.g. cancel, modify).
/// </summary>
public interface ISaleUpdateRepository
{
    /// <summary>Persists changes to an existing sale.</summary>
    Task<Sale> UpdateAsync(Sale sale, CancellationToken cancellationToken = default);
}
