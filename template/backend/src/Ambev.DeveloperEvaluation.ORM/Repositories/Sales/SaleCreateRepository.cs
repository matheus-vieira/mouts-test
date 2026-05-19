using Ambev.DeveloperEvaluation.Domain.Entities.Sales;
using Ambev.DeveloperEvaluation.Domain.Repositories.Sales;
using Microsoft.EntityFrameworkCore;

namespace Ambev.DeveloperEvaluation.ORM.Repositories.Sales;

/// <summary>
/// Handles persistence of new <see cref="Sale"/> aggregates.
/// Implements only the write side of the repository pattern.
/// </summary>
public class SaleCreateRepository(DefaultContext context)
    : SaleRepository(context), ISaleCreateRepository
{
    /// <inheritdoc/>
    public async Task<Sale> CreateAsync(Sale sale, CancellationToken cancellationToken = default)
    {
        await Table.AddAsync(sale, cancellationToken);
        await SaveChangesAsync(cancellationToken);
        return sale;
    }

    /// <inheritdoc/>
    public async Task<bool> ExistsBySaleNumberAsync(string saleNumber, CancellationToken cancellationToken)
        => await Table.AnyAsync(x => x.SaleNumber == saleNumber, cancellationToken);
}