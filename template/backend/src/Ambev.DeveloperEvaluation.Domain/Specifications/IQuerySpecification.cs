using System.Linq.Expressions;

namespace Ambev.DeveloperEvaluation.Domain.Specifications;

/// <summary>
/// Specification contract for IQueryable filtering.
/// Exposes an Expression that can be translated to SQL by EF Core.
/// </summary>
public interface IQuerySpecification<T>
{
    /// <summary>
    /// The filter expression to be applied to the query.
    /// </summary>
    Expression<Func<T, bool>> ToExpression();
}