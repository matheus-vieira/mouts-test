using FluentValidation;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales.UpdateSale;

/// <summary>
/// Validator for the UpdateSaleRequest contract.
/// Ensures the request body follows the required business rules before processing.
/// </summary>
public class UpdateSaleRequestValidator : AbstractValidator<UpdateSaleRequest>
{
    /// <summary>
    /// Initializes validation rules for the update sale request.
    /// </summary>
    public UpdateSaleRequestValidator()
    {
        // Sale identity must be valid and not empty
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage("Sale ID is required.");

        // Customer and Branch associations are mandatory
        RuleFor(x => x.CustomerId)
            .NotEmpty()
            .WithMessage("Customer ID is required.");

        RuleFor(x => x.BranchId)
            .NotEmpty()
            .WithMessage("Branch ID is required.");

        // Business Rule: A sale cannot exist without items
        RuleFor(x => x.Items)
            .NotEmpty()
            .WithMessage("The sale must contain at least one item.");

        // Validation for each individual item within the list
        RuleForEach(x => x.Items).ChildRules(items =>
        {
            items.RuleFor(i => i.ProductId)
                .NotEmpty()
                .WithMessage("Product ID is required for each item.");

            items.RuleFor(i => i.Quantity)
                .GreaterThan(0)
                .WithMessage("Quantity must be greater than zero.");

            items.RuleFor(i => i.UnitPrice)
                .GreaterThan(0)
                .WithMessage("Unit price must be greater than zero.");
        });
    }
}