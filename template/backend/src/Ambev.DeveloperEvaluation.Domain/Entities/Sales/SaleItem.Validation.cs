using Ambev.DeveloperEvaluation.Common.Validation;
using Ambev.DeveloperEvaluation.Domain.Validation.Sales;

namespace Ambev.DeveloperEvaluation.Domain.Entities.Sales;

public partial class SaleItem
{
    /// <summary>
    /// Validates the current state of this item using <see cref="SaleItemValidator"/>.
    /// </summary>
    /// <returns>A <see cref="ValidationResultDetail"/> with errors if invalid.</returns>
    public ValidationResultDetail Validate()
    {
        var validator = new SaleItemValidator();
        var result = validator.Validate(this);
        return new ValidationResultDetail
        {
            IsValid = result.IsValid,
            Errors = result.Errors.Select(o => (ValidationErrorDetail)o)
        };
    }
}