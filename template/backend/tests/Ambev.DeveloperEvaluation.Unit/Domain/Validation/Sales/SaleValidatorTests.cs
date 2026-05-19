using Ambev.DeveloperEvaluation.Domain.Entities.Sales;
using Ambev.DeveloperEvaluation.Domain.Validation.Sales;
using FluentValidation.TestHelper;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Domain.Sales.Validation;

/// <summary>
/// Unit tests for <see cref="SaleValidator"/>.
/// Validates all FluentValidation rules defined in the domain validator,
/// including child validator delegation to <see cref="SaleItemValidator"/>.
/// </summary>
/// <remarks>
/// Since <see cref="Sale"/> has private setters and factory-enforced invariants,
/// we use reflection to create test instances with specific invalid states
/// that would not be possible through the normal <see cref="Sale.Create"/> factory.
/// </remarks>
public class SaleValidatorTests
{
    private readonly SaleValidator _validator = new();

    /// <summary>
    /// Creates a valid <see cref="Sale"/> via the domain factory method.
    /// All properties will satisfy every validation rule.
    /// </summary>
    private static Sale CreateValidSale()
        => Sale.Create(
            DateTime.UtcNow,
            Guid.NewGuid(), "Valid Customer",
            Guid.NewGuid(), "Valid Branch",
            [SaleItem.Create(Guid.NewGuid(), "Product A", 2, 50.00m)]);

    /// <summary>
    /// Sets a private property on a domain entity via reflection.
    /// Required because <see cref="Sale"/> properties use private setters.
    /// </summary>
    private static void SetProperty(object target, string propertyName, object? value)
        => target.GetType().GetProperty(propertyName)!.SetValue(target, value);

    [Fact(DisplayName = "Given valid Sale When validating Then passes without errors")]
    public void Given_ValidSale_When_Validated_Then_ShouldNotHaveErrors()
    {
        // Arrange
        var sale = CreateValidSale();

        // Act
        var result = _validator.TestValidate(sale);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact(DisplayName = "Given Sale with empty CustomerName When validating Then fails")]
    public void Given_EmptyCustomerName_When_Validated_Then_ShouldHaveError()
    {
        // Arrange
        var sale = CreateValidSale();
        SetProperty(sale, nameof(Sale.CustomerName), string.Empty);

        // Act
        var result = _validator.TestValidate(sale);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.CustomerName)
            .WithErrorMessage("Customer name is required.");
    }

    [Fact(DisplayName = "Given Sale with empty BranchName When validating Then fails")]
    public void Given_EmptyBranchName_When_Validated_Then_ShouldHaveError()
    {
        // Arrange
        var sale = CreateValidSale();
        SetProperty(sale, nameof(Sale.BranchName), string.Empty);

        // Act
        var result = _validator.TestValidate(sale);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.BranchName)
            .WithErrorMessage("Branch name is required.");
    }

    [Fact(DisplayName = "Given Sale with no items When validating Then fails")]
    public void Given_EmptyItems_When_Validated_Then_ShouldHaveError()
    {
        // Arrange — create valid sale then clear items via reflection on the backing field
        var sale = CreateValidSale();
        var itemsField = typeof(Sale)
            .GetField("_items", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)!;
        var itemsList = (List<SaleItem>)itemsField.GetValue(sale)!;
        itemsList.Clear();

        // Act
        var result = _validator.TestValidate(sale);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Items)
            .WithErrorMessage("A sale must contain at least one item.");
    }

    [Fact(DisplayName = "Given Sale with invalid item When validating Then child validator reports error")]
    public void Given_ItemWithInvalidData_When_Validated_Then_ShouldHaveChildError()
    {
        // Arrange — create valid sale then corrupt one item's ProductName
        var sale = CreateValidSale();
        var itemsField = typeof(Sale)
            .GetField("_items", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)!;
        var itemsList = (List<SaleItem>)itemsField.GetValue(sale)!;
        var firstItem = itemsList[0];

        typeof(SaleItem).GetProperty(nameof(SaleItem.ProductName))!
            .SetValue(firstItem, string.Empty);

        // Act
        var result = _validator.TestValidate(sale);

        // Assert
        result.ShouldHaveValidationErrorFor("Items[0].ProductName")
            .WithErrorMessage("Product name is required.");
    }
}