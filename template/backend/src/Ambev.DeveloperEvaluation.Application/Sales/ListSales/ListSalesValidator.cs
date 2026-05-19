using FluentValidation;

namespace Ambev.DeveloperEvaluation.Application.Sales.ListSales;

/// <summary>
/// Validator for <see cref="ListSalesQuery"/>.
/// </summary>
public class ListSalesValidator : AbstractValidator<ListSalesQuery>
{
    private static readonly HashSet<string> AllowedOrderByFields = new(StringComparer.OrdinalIgnoreCase)
    {
        nameof(ListSalesResult.SaleDate),
        nameof(ListSalesResult.TotalAmount),
        nameof(ListSalesResult.CustomerName),
        nameof(ListSalesResult.BranchName),
        nameof(ListSalesResult.SaleNumber)
    };

    public ListSalesValidator()
    {
        RuleFor(x => x.Page)
            .GreaterThan(0)
            .WithMessage("Page must be greater than 0.");

        RuleFor(x => x.PageSize)
            .InclusiveBetween(1, 100)
            .WithMessage("PageSize must be between 1 and 100.");

        RuleFor(x => x.OrderBy)
            .Must(BeValidOrderBy)
            .When(x => !string.IsNullOrWhiteSpace(x.OrderBy))
            .WithMessage(
                $"OrderBy must be one of: {string.Join(", ", AllowedOrderByFields)} " +
                "(optionally followed by 'asc' or 'desc').");

        RuleFor(x => x.MinDate)
            .LessThanOrEqualTo(x => x.MaxDate)
            .When(x => x.MinDate.HasValue && x.MaxDate.HasValue)
            .WithMessage("MinDate must be less than or equal to MaxDate.");

        RuleFor(x => x.MinAmount)
            .LessThanOrEqualTo(x => x.MaxAmount)
            .When(x => x.MinAmount.HasValue && x.MaxAmount.HasValue)
            .WithMessage("MinAmount must be less than or equal to MaxAmount.");
    }

    private static bool BeValidOrderBy(string? orderBy)
    {
        if (string.IsNullOrWhiteSpace(orderBy))
            return true;

        var parts = orderBy
            .Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

        if (parts.Length is < 1 or > 2)
            return false;

        var field = parts[0];

        if (!AllowedOrderByFields.Contains(field))
            return false;

        if (parts.Length == 1)
            return true;

        return parts[1].Equals("asc", StringComparison.OrdinalIgnoreCase)
            || parts[1].Equals("desc", StringComparison.OrdinalIgnoreCase);
    }
}