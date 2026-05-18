namespace Ambev.DeveloperEvaluation.Domain.Repositories.Sales;
using Ambev.DeveloperEvaluation.Domain.Entities.Sales;

/// <summary>
/// Handles delete operations for <see cref="Sale"/> aggregates.
/// Injected exclusively by delete-side handlers.
/// </summary>
public interface ISaleDeleteRepository
{
    /// <summary>
    /// Removes a sale by its unique identifier.
    /// Returns <c>true</c> if deleted, <c>false</c> if not found.
    /// </summary>
    Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}