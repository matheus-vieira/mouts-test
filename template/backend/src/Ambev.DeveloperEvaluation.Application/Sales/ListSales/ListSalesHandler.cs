using Ambev.DeveloperEvaluation.Common.Pagination;
using Ambev.DeveloperEvaluation.Domain.Entities.Sales;
using Ambev.DeveloperEvaluation.Domain.Repositories.Sales;
using Ambev.DeveloperEvaluation.Domain.Specifications.Sales;
using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Sales.ListSales;

/// <summary>
/// Handles the <see cref="ListSalesQuery"/> by applying filters,
/// ordering, and pagination via the read repository.
/// </summary>
public class ListSalesHandler(ISaleReadRepository readRepository)
    : IRequestHandler<ListSalesQuery, PaginatedList<ListSalesResult>>
{
    /// <inheritdoc/>
    public async Task<PaginatedList<ListSalesResult>> Handle(
        ListSalesQuery request,
        CancellationToken cancellationToken)
    {
        var specification = new SaleFilterSpecification
        {
            CustomerName = request.CustomerName,
            BranchName = request.BranchName,
            MinDate = request.MinDate,
            MaxDate = request.MaxDate,
            MinAmount = request.MinAmount,
            MaxAmount = request.MaxAmount,
            IsCancelled = request.IsCancelled
        };

        var normalizedOrderBy = NormalizeOrderBy(request.OrderBy);

        var paged = await readRepository.GetPagedAsync(
            specification,
            request.Page,
            request.PageSize,
            normalizedOrderBy,
            cancellationToken);

        var results = paged.Select(sale => new ListSalesResult
        {
            Id = sale.Id,
            SaleNumber = sale.SaleNumber,
            SaleDate = sale.SaleDate,
            CustomerId = sale.CustomerId,
            CustomerName = sale.CustomerName,
            BranchId = sale.BranchId,
            BranchName = sale.BranchName,
            TotalAmount = sale.TotalAmount,
            IsCancelled = sale.IsCancelled
        }).ToList();

        return new PaginatedList<ListSalesResult>(
            results,
            paged.TotalCount,
            paged.CurrentPage,
            paged.PageSize);
    }

    /// <summary>
    /// Maps public API ordering aliases to domain entity property names.
    /// Prevents arbitrary Dynamic LINQ expressions from reaching the repository layer.
    /// </summary>
    private static string? NormalizeOrderBy(string? orderBy)
    {
        if (string.IsNullOrWhiteSpace(orderBy))
            return null;

        var parts = orderBy
            .Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

        var property = parts[0] switch
        {
            var f when f.Equals(nameof(ListSalesResult.SaleDate), StringComparison.OrdinalIgnoreCase)
                => nameof(Sale.SaleDate),

            var f when f.Equals(nameof(ListSalesResult.TotalAmount), StringComparison.OrdinalIgnoreCase)
                => nameof(Sale.TotalAmount),

            var f when f.Equals(nameof(ListSalesResult.CustomerName), StringComparison.OrdinalIgnoreCase)
                => nameof(Sale.CustomerName),

            var f when f.Equals(nameof(ListSalesResult.BranchName), StringComparison.OrdinalIgnoreCase)
                => nameof(Sale.BranchName),

            var f when f.Equals(nameof(ListSalesResult.SaleNumber), StringComparison.OrdinalIgnoreCase)
                => nameof(Sale.SaleNumber),

            _ => null
        };

        if (property is null)
            return null;

        var direction = parts.Length > 1
            ? parts[1].ToLowerInvariant()
            : "asc";

        return $"{property} {direction}";
    }
}