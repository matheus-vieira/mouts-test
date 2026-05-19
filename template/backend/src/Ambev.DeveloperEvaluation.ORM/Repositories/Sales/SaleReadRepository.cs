using Ambev.DeveloperEvaluation.Common.Pagination;
using Ambev.DeveloperEvaluation.Domain.Entities.Sales;
using Ambev.DeveloperEvaluation.Domain.Repositories.Sales;
using Ambev.DeveloperEvaluation.Domain.Specifications;
using Ambev.DeveloperEvaluation.ORM.Extensions;
using Microsoft.EntityFrameworkCore;
using System.Linq.Dynamic.Core;

namespace Ambev.DeveloperEvaluation.ORM.Repositories.Sales;

/// <summary>
/// Handles read operations for <see cref="Sale"/> aggregates.
/// Eagerly loads the Items collection to ensure aggregate consistency on every query.
/// </summary>
public class SaleReadRepository(DefaultContext context)
    : SaleRepository(context), ISaleReadRepository
{
    /// <inheritdoc/>
    public async Task<Sale?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await Table
            .Include(s => s.Items)
            .FirstOrDefaultAsync(s => s.Id == id, cancellationToken);
    }

    /// <inheritdoc/>
    public async Task<Sale?> GetBySaleNumberAsync(string saleNumber, CancellationToken cancellationToken = default)
    {
        return await Table
            .Include(s => s.Items)
            .FirstOrDefaultAsync(s => s.SaleNumber == saleNumber, cancellationToken);
    }

    /// <inheritdoc/>
    public async Task<PaginatedList<Sale>> GetPagedAsync(
        IQuerySpecification<Sale> specification,
        int page,
        int pageSize,
        string? orderBy,
        CancellationToken cancellationToken = default)
    {
        var query = Table
            .Include(s => s.Items)
            .Where(specification.ToExpression());

        query = string.IsNullOrWhiteSpace(orderBy)
            ? query.OrderByDescending(s => s.SaleDate)
            : query.OrderBy(orderBy);

        return await query.ToPagedListAsync(page, pageSize, cancellationToken);
    }
}