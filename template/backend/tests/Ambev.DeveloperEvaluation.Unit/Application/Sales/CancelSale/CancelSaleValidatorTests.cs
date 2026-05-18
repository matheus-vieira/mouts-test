using Ambev.DeveloperEvaluation.Application.Sales.CancelSale;
using FluentAssertions;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Application.Sales.CancelSale;

public class CancelSaleValidatorTests
{
    private readonly CancelSaleValidator _validator;

    public CancelSaleValidatorTests()
    {
        _validator = new CancelSaleValidator();
    }

    [Fact(DisplayName = "Given valid Sale ID When validating Then passes validation")]
    public void Validate_ValidId_ShouldPass()
    {
        // Given
        var command = new CancelSaleCommand(Guid.NewGuid());

        // When
        var result = _validator.Validate(command);

        // Then
        result.IsValid.Should().BeTrue();
    }

    [Fact(DisplayName = "Given empty Sale ID When validating Then fails validation")]
    public void Validate_EmptyId_ShouldFail()
    {
        // Given
        var command = new CancelSaleCommand(Guid.Empty);

        // When
        var result = _validator.Validate(command);

        // Then
        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle(e =>
            e.PropertyName == "Id" &&
            e.ErrorMessage == "Sale ID is required");
    }
}