using Ambev.DeveloperEvaluation.Application.Sales.GetSale;
using FluentAssertions;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Application.Sales.GetSale;

public class GetSaleValidatorTests
{
    private readonly GetSaleValidator _validator;

    public GetSaleValidatorTests()
    {
        _validator = new GetSaleValidator();
    }

    [Fact(DisplayName = "Given valid ID When validating Then passes validation")]
    public void Validate_ValidId_ShouldPass()
    {
        // Given
        var command = new GetSaleCommand(Guid.NewGuid());

        // When
        var result = _validator.Validate(command);

        // Then
        result.IsValid.Should().BeTrue();
    }

    [Fact(DisplayName = "Given empty ID When validating Then fails validation")]
    public void Validate_EmptyId_ShouldFail()
    {
        // Given
        var command = new GetSaleCommand(Guid.Empty);

        // When
        var result = _validator.Validate(command);

        // Then
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Id");
    }
}