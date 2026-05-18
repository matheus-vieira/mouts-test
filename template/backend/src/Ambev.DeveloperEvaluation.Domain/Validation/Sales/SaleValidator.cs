using Ambev.DeveloperEvaluation.Domain.Entities.Sales;
using FluentValidation;

namespace Ambev.DeveloperEvaluation.Domain.Validation.Sales;

public class SaleValidator : AbstractValidator<Sale>
{
    public SaleValidator()
    {
        RuleFor(x => x.SaleNumber)
            .NotEmpty()
            .WithMessage("Sale number is required.");

        RuleFor(x => x.CustomerName)
            .NotEmpty()
            .WithMessage("Customer name is required.");

        RuleFor(x => x.BranchName)
            .NotEmpty()
            .WithMessage("Branch name is required.");

        RuleFor(x => x.Items)
            .NotEmpty()
            .WithMessage("A sale must contain at least one item.");

        RuleForEach(x => x.Items)
            .SetValidator(new SaleItemValidator());
    }
}