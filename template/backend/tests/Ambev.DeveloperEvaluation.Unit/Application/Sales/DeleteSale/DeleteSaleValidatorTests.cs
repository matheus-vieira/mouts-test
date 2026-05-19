using Ambev.DeveloperEvaluation.Application.Sales.DeleteSale;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Application.Sales.DeleteSale;

public class DeleteSaleValidatorTests
{
    private readonly DeleteSaleValidator _validator = new();

    [Fact(DisplayName = "Valid command should pass validation")]
    public void Given_ValidCommand_When_Validated_Then_ShouldPass()
    {
        // Arrange
        var command = new DeleteSaleCommand { Id = Guid.NewGuid() };

        // Act
        var result = _validator.Validate(command);

        // Assert
        Assert.True(result.IsValid);
    }

    [Fact(DisplayName = "Empty Id should fail validation")]
    public void Given_EmptyId_When_Validated_Then_ShouldFail()
    {
        // Arrange
        var command = new DeleteSaleCommand { Id = Guid.Empty };

        // Act
        var result = _validator.Validate(command);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "Id");
    }

    [Fact(DisplayName = "Default Id should fail validation")]
    public void Given_DefaultId_When_Validated_Then_ShouldFail()
    {
        // Arrange
        var command = new DeleteSaleCommand();

        // Act
        var result = _validator.Validate(command);

        // Assert
        Assert.False(result.IsValid);
        Assert.Single(result.Errors);
        Assert.Equal("Sale ID must be provided.", result.Errors[0].ErrorMessage);
    }
}