using Ambev.DeveloperEvaluation.Common.Validation;
using Ambev.DeveloperEvaluation.Domain.Validation.Sales;

namespace Ambev.DeveloperEvaluation.Domain.Entities.Sales;

public partial class Sale
{
    /// <summary>
    /// Validates the current state of this aggregate using <see cref="SaleValidator"/>.
    /// </summary>
    /// <returns>A <see cref="ValidationResultDetail"/> with errors if invalid.</returns>
    public ValidationResultDetail Validate()
    {
        var validator = new SaleValidator();
        var result = validator.Validate(this);
        return new ValidationResultDetail
        {
            IsValid = result.IsValid,
            Errors = result.Errors.Select(o => (ValidationErrorDetail)o)
        };
    }
}