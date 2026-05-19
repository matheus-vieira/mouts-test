using Ambev.DeveloperEvaluation.Application.Sales.GetSale;
using Ambev.DeveloperEvaluation.Unit.Application.TestData;
using FluentAssertions;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Application.Sales.GetSale;

public class GetSaleValidatorTests
{
    private readonly GetSaleValidator _validator = new();

    [Fact(DisplayName = "Given valid Sale ID When validating Then passes validation")]
    public void Validate_ValidId_ShouldPass()
    {
        var command = GetSaleHandlerTestData.GenerateValidCommand();

        var result = _validator.Validate(command);

        result.IsValid.Should().BeTrue();
    }

    [Fact(DisplayName = "Given empty Sale ID When validating Then fails validation")]
    public void Validate_EmptyId_ShouldFail()
    {
        var command = new GetSaleCommand(Guid.Empty);

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle(e => e.PropertyName == "Id");
    }
}