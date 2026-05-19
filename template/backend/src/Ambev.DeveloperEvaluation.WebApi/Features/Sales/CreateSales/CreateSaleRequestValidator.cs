using FluentValidation;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales.CreateSale;

/// <summary>
/// API-level validator for <see cref="CreateSaleRequest"/>.
/// Ensures basic request integrity before sending to application layer.
/// </summary>
public class CreateSaleRequestValidator : AbstractValidator<CreateSaleRequest>
{
    public CreateSaleRequestValidator()
    {
        RuleFor(r => r.SaleNumber).NotEmpty().MaximumLength(50);
        RuleFor(r => r.CustomerId).NotEqual(Guid.Empty);
        RuleFor(r => r.BranchId).NotEqual(Guid.Empty);
        RuleFor(r => r.Items).NotEmpty();
        
        RuleForEach(r => r.Items).ChildRules(item =>
        {
            item.RuleFor(i => i.ProductId).NotEqual(Guid.Empty);
            item.RuleFor(i => i.Quantity).InclusiveBetween(1, 20);
            item.RuleFor(i => i.UnitPrice).GreaterThan(0);
        });
    }
}