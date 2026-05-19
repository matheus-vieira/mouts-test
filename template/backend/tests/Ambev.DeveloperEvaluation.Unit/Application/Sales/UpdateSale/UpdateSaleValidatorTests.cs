using Ambev.DeveloperEvaluation.Application.Sales.UpdateSale;
using FluentAssertions;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Application.Sales.UpdateSale;

public class UpdateSaleValidatorTests
{
    private readonly UpdateSaleValidator _validator = new();

    [Fact(DisplayName = "Given valid command When validating Then should be valid")]
    public void Validate_ValidCommand_ShouldBeValid()
    {
        // Given
        var command = CreateValidCommand();

        // When
        var result = _validator.Validate(command);

        // Then
        result.IsValid.Should().BeTrue();
        result.Errors.Should().BeEmpty();
    }

    [Fact(DisplayName = "Given empty id When validating Then should be invalid")]
    public void Validate_EmptyId_ShouldBeInvalid()
    {
        // Given
        var command = CreateValidCommand();
        command.Id = Guid.Empty;

        // When
        var result = _validator.Validate(command);

        // Then
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(UpdateSaleCommand.Id));
    }

    [Fact(DisplayName = "Given empty customer id When validating Then should be invalid")]
    public void Validate_EmptyCustomerId_ShouldBeInvalid()
    {
        // Given
        var command = CreateValidCommand();
        command.CustomerId = Guid.Empty;

        // When
        var result = _validator.Validate(command);

        // Then
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(UpdateSaleCommand.CustomerId));
    }

    [Fact(DisplayName = "Given empty branch id When validating Then should be invalid")]
    public void Validate_EmptyBranchId_ShouldBeInvalid()
    {
        // Given
        var command = CreateValidCommand();
        command.BranchId = Guid.Empty;

        // When
        var result = _validator.Validate(command);

        // Then
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(UpdateSaleCommand.BranchId));
    }

    [Fact(DisplayName = "Given empty items When validating Then should be invalid")]
    public void Validate_EmptyItems_ShouldBeInvalid()
    {
        // Given
        var command = CreateValidCommand();
        command.Items = [];

        // When
        var result = _validator.Validate(command);

        // Then
        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle(e =>
            e.PropertyName == nameof(UpdateSaleCommand.Items) &&
            e.ErrorMessage == "Sale must have at least one item.");
    }

    [Fact(DisplayName = "Given item with quantity zero When validating Then should be invalid")]
    public void Validate_ItemWithQuantityZero_ShouldBeInvalid()
    {
        // Given
        var command = CreateValidCommand();
        command.Items[0].Quantity = 0;

        // When
        var result = _validator.Validate(command);

        // Then
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Items[0].Quantity");
    }

    [Fact(DisplayName = "Given item with negative quantity When validating Then should be invalid")]
    public void Validate_ItemWithNegativeQuantity_ShouldBeInvalid()
    {
        // Given
        var command = CreateValidCommand();
        command.Items[0].Quantity = -1;

        // When
        var result = _validator.Validate(command);

        // Then
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Items[0].Quantity");
    }

    [Fact(DisplayName = "Given item with unit price zero When validating Then should be invalid")]
    public void Validate_ItemWithUnitPriceZero_ShouldBeInvalid()
    {
        // Given
        var command = CreateValidCommand();
        command.Items[0].UnitPrice = 0;

        // When
        var result = _validator.Validate(command);

        // Then
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Items[0].UnitPrice");
    }

    [Fact(DisplayName = "Given item with negative unit price When validating Then should be invalid")]
    public void Validate_ItemWithNegativeUnitPrice_ShouldBeInvalid()
    {
        // Given
        var command = CreateValidCommand();
        command.Items[0].UnitPrice = -1m;

        // When
        var result = _validator.Validate(command);

        // Then
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Items[0].UnitPrice");
    }

    private static UpdateSaleCommand CreateValidCommand() =>
        new()
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
                    Quantity = 2,
                    UnitPrice = 10m
                }
            ]
        };
}