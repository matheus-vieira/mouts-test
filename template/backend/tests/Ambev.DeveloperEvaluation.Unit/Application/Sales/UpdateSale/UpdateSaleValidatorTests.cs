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

    // ──────────────────────────────────────────────
    // 🔴 NEW TESTS — Field-Level Validations
    // ──────────────────────────────────────────────

    [Fact(DisplayName = "Given empty CustomerId When validating Then fails validation")]
    public void Validate_EmptyCustomerId_ShouldFail()
    {
        // Arrange
        var command = UpdateSaleHandlerTestData.GenerateValidCommand();
        command.CustomerId = Guid.Empty;

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle(e => e.PropertyName == nameof(UpdateSaleCommand.CustomerId));
    }

    [Fact(DisplayName = "Given empty BranchId When validating Then fails validation")]
    public void Validate_EmptyBranchId_ShouldFail()
    {
        // Arrange
        var command = UpdateSaleHandlerTestData.GenerateValidCommand();
        command.BranchId = Guid.Empty;

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle(e => e.PropertyName == nameof(UpdateSaleCommand.BranchId));
    }

    [Fact(DisplayName = "Given item with quantity = 0 When validating Then fails validation")]
    public void Validate_ItemWithZeroQuantity_ShouldFail()
    {
        // Arrange
        var command = UpdateSaleHandlerTestData.GenerateValidCommand();
        command.Items[0].Quantity = 0;

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName.StartsWith("Items[0].Quantity"));
    }

    [Fact(DisplayName = "Given item with quantity < 0 When validating Then fails validation")]
    public void Validate_ItemWithNegativeQuantity_ShouldFail()
    {
        // Arrange
        var command = UpdateSaleHandlerTestData.GenerateValidCommand();
        command.Items[0].Quantity = -1;

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName.StartsWith("Items[0].Quantity"));
    }

    [Fact(DisplayName = "Given item with unitPrice = 0 When validating Then fails validation")]
    public void Validate_ItemWithZeroUnitPrice_ShouldFail()
    {
        // Arrange
        var command = UpdateSaleHandlerTestData.GenerateValidCommand();
        command.Items[0].UnitPrice = 0m;

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName.StartsWith("Items[0].UnitPrice"));
    }

    [Fact(DisplayName = "Given item with unitPrice < 0 When validating Then fails validation")]
    public void Validate_ItemWithNegativeUnitPrice_ShouldFail()
    {
        // Arrange
        var command = UpdateSaleHandlerTestData.GenerateValidCommand();
        command.Items[0].UnitPrice = -10m;

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName.StartsWith("Items[0].UnitPrice"));
    }

    [Fact(DisplayName = "Given empty Items collection When validating Then shows correct message")]
    public void Validate_EmptyItems_ShouldHaveSpecificMessage()
    {
        // Arrange
        var command = UpdateSaleHandlerTestData.GenerateCommandWithInvalidItems();

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e =>
            e.PropertyName == nameof(UpdateSaleCommand.Items) &&
            e.ErrorMessage == "Sale must have at least one item.");
    }
}