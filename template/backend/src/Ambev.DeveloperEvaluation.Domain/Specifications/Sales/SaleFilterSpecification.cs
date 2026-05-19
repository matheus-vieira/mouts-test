using Ambev.DeveloperEvaluation.Domain.Entities.Sales;
using System.Linq.Expressions;

namespace Ambev.DeveloperEvaluation.Domain.Specifications.Sales;

/// <summary>
/// Specification for filtering Sale aggregates via IQueryable.
/// All criteria are optional — only non-null values are applied.
/// </summary>
public class SaleFilterSpecification : IQuerySpecification<Sale>
{
    public string? CustomerName { get; init; }
    public string? BranchName { get; init; }
    public DateTime? MinDate { get; init; }
    public DateTime? MaxDate { get; init; }
    public decimal? MinAmount { get; init; }
    public decimal? MaxAmount { get; init; }
    public bool? IsCancelled { get; init; }

    /// <inheritdoc/>
    public Expression<Func<Sale, bool>> ToExpression()
    {
        return sale =>
            (CustomerName == null || sale.CustomerName.Contains(CustomerName)) &&
            (BranchName == null || sale.BranchName.Contains(BranchName)) &&
            (MinDate == null || sale.SaleDate >= MinDate) &&
            (MaxDate == null || sale.SaleDate <= MaxDate) &&
            (MinAmount == null || sale.TotalAmount >= MinAmount) &&
            (MaxAmount == null || sale.TotalAmount <= MaxAmount) &&
            (IsCancelled == null || sale.IsCancelled == IsCancelled);
    }
}