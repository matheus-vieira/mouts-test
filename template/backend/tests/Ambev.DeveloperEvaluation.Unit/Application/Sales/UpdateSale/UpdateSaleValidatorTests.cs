using Ambev.DeveloperEvaluation.Application.Sales.UpdateSale;
using Ambev.DeveloperEvaluation.Unit.Application.TestData;
using FluentAssertions;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Application.Sales.UpdateSale;

public class UpdateSaleValidatorTests
{
    private readonly UpdateSaleValidator _validator = new();

    [Fact(DisplayName = "Given valid command When validating Then passes validation")]
    public void Validate_ValidCommand_ShouldPass()
    {
        var command = UpdateSaleHandlerTestData.GenerateValidCommand();

        var result = _validator.Validate(command);

        result.IsValid.Should().BeTrue();
    }

    [Fact(DisplayName = "Given empty Id When validating Then fails validation")]
    public void Validate_EmptyId_ShouldFail()
    {
        var command = UpdateSaleHandlerTestData.GenerateValidCommand();
        command.Id = Guid.Empty;

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle(e => e.PropertyName == "Id");
    }

    [Fact(DisplayName = "Given command without items When validating Then fails validation")]
    public void Validate_NoItems_ShouldFail()
    {
        var command = UpdateSaleHandlerTestData.GenerateCommandWithInvalidItems();

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Items");
    }
}