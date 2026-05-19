using Ambev.DeveloperEvaluation.Common.Pagination;
using Microsoft.EntityFrameworkCore;

namespace Ambev.DeveloperEvaluation.ORM.Extensions;

/// <summary>
/// EF Core extension methods for IQueryable pagination.
/// Keeps EF Core dependency isolated in the ORM layer.
/// </summary>
public static class QueryableExtensions
{
    public static async Task<PaginatedList<T>> ToPagedListAsync<T>(
        this IQueryable<T> source,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        var count = await source.CountAsync(cancellationToken);
        var items = await source
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return new PaginatedList<T>(items, count, pageNumber, pageSize);
    }
}