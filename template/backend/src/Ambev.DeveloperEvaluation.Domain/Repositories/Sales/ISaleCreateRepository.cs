namespace Ambev.DeveloperEvaluation.Domain.Repositories.Sales;
using Ambev.DeveloperEvaluation.Domain.Entities.Sales;

/// <summary>
/// Handles persistence of new <see cref="Sale"/> aggregates.
/// Injected exclusively by create-side handlers.
/// </summary>
public interface ISaleCreateRepository
{
    /// <summary>Persists a new sale to the data store.</summary>
    Task<Sale> CreateAsync(Sale sale, CancellationToken cancellationToken = default);

    /// <summary>Checks if a sale with the given sale number already exists in the data store.</summary>
    Task<bool> ExistsBySaleNumberAsync(string saleNumber, CancellationToken cancellationToken);
}
