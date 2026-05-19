using Ambev.DeveloperEvaluation.Application.Sales.UpdateSale;
using FluentAssertions;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Application.Sales.UpdateSale;

public class UpdateSaleValidatorTests
{
    private readonly UpdateSaleValidator _validator;

    public UpdateSaleValidatorTests()
    {
        _validator = new UpdateSaleValidator();
    }

    [Fact(DisplayName = "Given valid command When validating Then passes validation")]
    public void Validate_ValidCommand_ShouldPass()
    {
        // Given
        var command = new UpdateSaleCommand
        {
            Id = Guid.NewGuid(),
            CustomerId = Guid.NewGuid(),
            CustomerName = "Customer",
            BranchId = Guid.NewGuid(),
            BranchName = "Branch",
            Items =
            [
                new UpdateSaleItemCommand
                {
                    ProductId = Guid.NewGuid(),
                    ProductName = "Product",
                    Quantity = 1,
                    UnitPrice = 10m
                }
            ]
        };

        // When
        var result = _validator.Validate(command);

        // Then
        result.IsValid.Should().BeTrue();
    }

    [Fact(DisplayName = "Given empty ID When validating Then fails validation")]
    public void Validate_EmptyId_ShouldFail()
    {
        // Given
        var command = new UpdateSaleCommand { Id = Guid.Empty, Items = [] };

        // When
        var result = _validator.Validate(command);

        // Then
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Id");
    }

    [Fact(DisplayName = "Given empty items When validating Then fails validation")]
    public void Validate_EmptyItems_ShouldFail()
    {
        // Given
        var command = new UpdateSaleCommand
        {
            Id = Guid.NewGuid(),
            Items = []
        };

        // When
        var result = _validator.Validate(command);

        // Then
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Items");
    }
}