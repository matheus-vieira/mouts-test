using FluentValidation;

namespace Ambev.DeveloperEvaluation.Application.Sales.CreateSale;

/// <summary>
/// Validates input consistency for <see cref="CreateSaleCommand"/>.
/// </summary>
/// <remarks>
/// This validator is responsible for input-level consistency only.
/// Business invariants (e.g. discount tier rules, cancellation guards)
/// are enforced exclusively by the domain entities.
///
/// Validation is also triggered automatically by the MediatR
/// ValidationBehavior pipeline before the handler is invoked.
/// </remarks>
public class CreateSaleCommandValidator : AbstractValidator<CreateSaleCommand>
{
    public CreateSaleCommandValidator()
    {
        RuleFor(x => x.SaleNumber)
            .NotEmpty()
            .WithMessage("Sale number is required.");

        RuleFor(x => x.SaleDate)
            .NotEqual(default(DateTime))
            .WithMessage("Sale date is required.");

        RuleFor(x => x.CustomerId)
            .NotEqual(Guid.Empty)
            .WithMessage("Customer id is required.");

        RuleFor(x => x.CustomerName)
            .NotEmpty()
            .WithMessage("Customer name is required.");

        RuleFor(x => x.BranchId)
            .NotEqual(Guid.Empty)
            .WithMessage("Branch id is required.");

        RuleFor(x => x.BranchName)
            .NotEmpty()
            .WithMessage("Branch name is required.");

        RuleFor(x => x.Items)
            .NotEmpty()
            .WithMessage("Sale must have at least one item.");

        RuleForEach(x => x.Items).ChildRules(item =>
        {
            item.RuleFor(i => i.ProductId)
                .NotEqual(Guid.Empty)
                .WithMessage("Product id is required.");

            item.RuleFor(i => i.ProductName)
                .NotEmpty()
                .WithMessage("Product name is required.");

            item.RuleFor(i => i.Quantity)
                .GreaterThan(0)
                .WithMessage("Quantity must be greater than zero.")
                .LessThanOrEqualTo(20)
                .WithMessage("Cannot sell more than 20 identical items.");

            item.RuleFor(i => i.UnitPrice)
                .GreaterThan(0)
                .WithMessage("Unit price must be greater than zero.");
        });
    }
}