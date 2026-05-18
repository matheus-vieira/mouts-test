using Ambev.DeveloperEvaluation.Domain.Entities.Sales;
using FluentValidation;

namespace Ambev.DeveloperEvaluation.Domain.Validation.Sales;

public class SaleItemValidator : AbstractValidator<SaleItem>
{
    public SaleItemValidator()
    {
        RuleFor(x => x.ProductName)
            .NotEmpty()
            .WithMessage("Product name is required.");

        RuleFor(x => x.Quantity)
            .GreaterThan(0)
            .WithMessage("Quantity must be greater than zero.")
            .LessThanOrEqualTo(20)
            .WithMessage("Cannot sell more than 20 identical items.");

        RuleFor(x => x.UnitPrice)
            .GreaterThan(0)
            .WithMessage("Unit price must be greater than zero.");
    }
}