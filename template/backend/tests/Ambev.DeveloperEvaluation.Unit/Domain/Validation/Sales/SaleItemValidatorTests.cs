using Ambev.DeveloperEvaluation.Domain.Entities.Sales;
using Ambev.DeveloperEvaluation.Domain.Validation.Sales;
using FluentValidation.TestHelper;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Domain.Sales.Validation;

/// <summary>
/// Unit tests for <see cref="SaleItemValidator"/>.
/// Validates all FluentValidation rules defined in the domain validator.
/// </summary>
/// <remarks>
/// Since <see cref="SaleItem"/> has private setters and factory-enforced invariants,
/// we use reflection to create test instances with specific invalid states
/// that would not be possible through the normal <see cref="SaleItem.Create"/> factory.
/// </remarks>
public class SaleItemValidatorTests
{
    private readonly SaleItemValidator _validator = new();

    /// <summary>
    /// Creates a valid <see cref="SaleItem"/> via the domain factory method.
    /// All properties will satisfy every validation rule.
    /// </summary>
    private static SaleItem CreateValidItem()
        => SaleItem.Create(Guid.NewGuid(), "Valid Product", 5, 10.00m);

    /// <summary>
    /// Sets a private property on a domain entity via reflection.
    /// Required because <see cref="SaleItem"/> properties use private setters.
    /// </summary>
    private static void SetProperty(object target, string propertyName, object? value)
        => target.GetType().GetProperty(propertyName)!.SetValue(target, value);

    [Fact(DisplayName = "Given valid SaleItem When validating Then passes without errors")]
    public void Given_ValidSaleItem_When_Validated_Then_ShouldNotHaveErrors()
    {
        // Arrange
        var item = CreateValidItem();

        // Act
        var result = _validator.TestValidate(item);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact(DisplayName = "Given SaleItem with empty ProductName When validating Then fails")]
    public void Given_EmptyProductName_When_Validated_Then_ShouldHaveError()
    {
        // Arrange
        var item = CreateValidItem();
        SetProperty(item, nameof(SaleItem.ProductName), string.Empty);

        // Act
        var result = _validator.TestValidate(item);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.ProductName)
            .WithErrorMessage("Product name is required.");
    }

    [Fact(DisplayName = "Given SaleItem with quantity zero When validating Then fails")]
    public void Given_QuantityZero_When_Validated_Then_ShouldHaveError()
    {
        // Arrange
        var item = CreateValidItem();
        SetProperty(item, nameof(SaleItem.Quantity), 0);

        // Act
        var result = _validator.TestValidate(item);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Quantity)
            .WithErrorMessage("Quantity must be greater than zero.");
    }

    [Fact(DisplayName = "Given SaleItem with negative quantity When validating Then fails")]
    public void Given_QuantityNegative_When_Validated_Then_ShouldHaveError()
    {
        // Arrange
        var item = CreateValidItem();
        SetProperty(item, nameof(SaleItem.Quantity), -1);

        // Act
        var result = _validator.TestValidate(item);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Quantity)
            .WithErrorMessage("Quantity must be greater than zero.");
    }

    [Fact(DisplayName = "Given SaleItem with quantity above 20 When validating Then fails")]
    public void Given_QuantityAbove20_When_Validated_Then_ShouldHaveError()
    {
        // Arrange
        var item = CreateValidItem();
        SetProperty(item, nameof(SaleItem.Quantity), 21);

        // Act
        var result = _validator.TestValidate(item);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Quantity)
            .WithErrorMessage("Cannot sell more than 20 identical items.");
    }

    [Fact(DisplayName = "Given SaleItem with unit price zero When validating Then fails")]
    public void Given_UnitPriceZero_When_Validated_Then_ShouldHaveError()
    {
        // Arrange
        var item = CreateValidItem();
        SetProperty(item, nameof(SaleItem.UnitPrice), 0m);

        // Act
        var result = _validator.TestValidate(item);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.UnitPrice)
            .WithErrorMessage("Unit price must be greater than zero.");
    }
}