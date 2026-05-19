using FluentValidation;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales.ListSales;

/// <summary>
/// Validator for <see cref="ListSalesRequest"/>.
/// </summary>
public class ListSalesRequestValidator : AbstractValidator<ListSalesRequest>
{
    public ListSalesRequestValidator()
    {
        RuleFor(x => x._page).GreaterThan(0);
        RuleFor(x => x._size).InclusiveBetween(1, 100);
    }
}