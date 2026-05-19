using Ambev.DeveloperEvaluation.Application.Sales.CreateSale;
using FluentAssertions;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Application.Sales;

public class CreateSaleCommandValidatorTests
{
    private readonly CreateSaleCommandValidator _validator = new();

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

    [Fact(DisplayName = "Given empty sale number When validating Then should be invalid")]
    public void Validate_EmptySaleNumber_ShouldBeInvalid()
    {
        // Given
        var command = CreateValidCommand();
        command.SaleNumber = string.Empty;

        // When
        var result = _validator.Validate(command);

        // Then
        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle(x =>
            x.PropertyName == nameof(CreateSaleCommand.SaleNumber) &&
            x.ErrorMessage == "Sale number is required.");
    }

    [Fact(DisplayName = "Given default sale date When validating Then should be invalid")]
    public void Validate_DefaultSaleDate_ShouldBeInvalid()
    {
        // Given
        var command = CreateValidCommand();
        command.SaleDate = default;

        // When
        var result = _validator.Validate(command);

        // Then
        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle(x =>
            x.PropertyName == nameof(CreateSaleCommand.SaleDate) &&
            x.ErrorMessage == "Sale date is required.");
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
        result.Errors.Should().ContainSingle(x =>
            x.PropertyName == nameof(CreateSaleCommand.CustomerId) &&
            x.ErrorMessage == "Customer id is required.");
    }

    [Fact(DisplayName = "Given empty customer name When validating Then should be invalid")]
    public void Validate_EmptyCustomerName_ShouldBeInvalid()
    {
        // Given
        var command = CreateValidCommand();
        command.CustomerName = string.Empty;

        // When
        var result = _validator.Validate(command);

        // Then
        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle(x =>
            x.PropertyName == nameof(CreateSaleCommand.CustomerName) &&
            x.ErrorMessage == "Customer name is required.");
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
        result.Errors.Should().ContainSingle(x =>
            x.PropertyName == nameof(CreateSaleCommand.BranchId) &&
            x.ErrorMessage == "Branch id is required.");
    }

    [Fact(DisplayName = "Given empty branch name When validating Then should be invalid")]
    public void Validate_EmptyBranchName_ShouldBeInvalid()
    {
        // Given
        var command = CreateValidCommand();
        command.BranchName = string.Empty;

        // When
        var result = _validator.Validate(command);

        // Then
        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle(x =>
            x.PropertyName == nameof(CreateSaleCommand.BranchName) &&
            x.ErrorMessage == "Branch name is required.");
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
        result.Errors.Should().ContainSingle(x =>
            x.PropertyName == nameof(CreateSaleCommand.Items) &&
            x.ErrorMessage == "Sale must have at least one item.");
    }

    [Fact(DisplayName = "Given item with empty product id When validating Then should be invalid")]
    public void Validate_ItemWithEmptyProductId_ShouldBeInvalid()
    {
        // Given
        var command = CreateValidCommand();
        command.Items[0].ProductId = Guid.Empty;

        // When
        var result = _validator.Validate(command);

        // Then
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(x =>
            x.PropertyName == "Items[0].ProductId" &&
            x.ErrorMessage == "Product id is required.");
    }

    [Fact(DisplayName = "Given item with empty product name When validating Then should be invalid")]
    public void Validate_ItemWithEmptyProductName_ShouldBeInvalid()
    {
        // Given
        var command = CreateValidCommand();
        command.Items[0].ProductName = string.Empty;

        // When
        var result = _validator.Validate(command);

        // Then
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(x =>
            x.PropertyName == "Items[0].ProductName" &&
            x.ErrorMessage == "Product name is required.");
    }

    [Fact(DisplayName = "Given item with quantity less than one When validating Then should be invalid")]
    public void Validate_ItemWithQuantityLessThanOne_ShouldBeInvalid()
    {
        // Given
        var command = CreateValidCommand();
        command.Items[0].Quantity = 0;

        // When
        var result = _validator.Validate(command);

        // Then
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(x =>
            x.PropertyName == "Items[0].Quantity" &&
            x.ErrorMessage == "Quantity must be greater than zero.");
    }

    [Fact(DisplayName = "Given item with quantity greater than twenty When validating Then should be invalid")]
    public void Validate_ItemWithQuantityGreaterThanTwenty_ShouldBeInvalid()
    {
        // Given
        var command = CreateValidCommand();
        command.Items[0].Quantity = 21;

        // When
        var result = _validator.Validate(command);

        // Then
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(x =>
            x.PropertyName == "Items[0].Quantity" &&
            x.ErrorMessage == "Cannot sell more than 20 identical items.");
    }

    [Fact(DisplayName = "Given item with invalid unit price When validating Then should be invalid")]
    public void Validate_ItemWithInvalidUnitPrice_ShouldBeInvalid()
    {
        // Given
        var command = CreateValidCommand();
        command.Items[0].UnitPrice = 0;

        // When
        var result = _validator.Validate(command);

        // Then
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(x =>
            x.PropertyName == "Items[0].UnitPrice" &&
            x.ErrorMessage == "Unit price must be greater than zero.");
    }

    private static CreateSaleCommand CreateValidCommand()
    {
        return new CreateSaleCommand
        {
            SaleNumber = "SALE-001",
            SaleDate = DateTime.UtcNow,
            CustomerId = Guid.NewGuid(),
            CustomerName = "Customer",
            BranchId = Guid.NewGuid(),
            BranchName = "Branch",
            Items =
            [
                new CreateSaleItemCommand
                {
                    ProductId = Guid.NewGuid(),
                    ProductName = "Product",
                    Quantity = 2,
                    UnitPrice = 10m
                }
            ]
        };
    }
}