using Ambev.DeveloperEvaluation.Domain.Entities.Sales;
using Ambev.DeveloperEvaluation.Domain.Exceptions;
using Ambev.DeveloperEvaluation.Unit.Domain.Entities.TestData;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Domain.Entities;

/// <summary>
/// Unit tests for <see cref="SaleItem"/> discount tier rules and boundary constraints.
/// </summary>
public class SaleItemTests
{
    [Fact(DisplayName = "Given quantity below 4 When creating item Then no discount is applied")]
    public void Given_QuantityBelow4_When_CreatingItem_Then_NoDiscount()
    {
        // Arrange
        var (ProductId, ProductName, Quantity, UnitPrice) = SaleItemTestData.ValidBelow4();

        // Act
        var item = SaleItem.Create(ProductId, ProductName, Quantity, UnitPrice);

        // Assert
        Assert.Equal(0, item.Discount);
        Assert.Equal(Quantity * UnitPrice, item.TotalAmount);
    }

    [Theory(DisplayName = "Given quantity between 4 and 9 When creating item Then 10% discount is applied")]
    [InlineData(4)]
    [InlineData(7)]
    [InlineData(9)]
    public void Given_QuantityBetween4And9_When_CreatingItem_Then_Applies10PercentDiscount(int quantity)
    {
        // Arrange
        var (ProductId, ProductName, Quantity, UnitPrice) = SaleItemTestData.ValidWithQuantity(quantity);

        // Act
        var item = SaleItem.Create(ProductId, ProductName, quantity, UnitPrice);

        // Assert
        var expectedDiscount = quantity * UnitPrice * 0.10m;
        Assert.Equal(expectedDiscount, item.Discount);
        Assert.Equal(quantity * UnitPrice - expectedDiscount, item.TotalAmount);
    }

    [Theory(DisplayName = "Given quantity between 10 and 20 When creating item Then 20% discount is applied")]
    [InlineData(10)]
    [InlineData(15)]
    [InlineData(20)]
    public void Given_QuantityBetween10And20_When_CreatingItem_Then_Applies20PercentDiscount(int quantity)
    {
        // Arrange
        var (ProductId, ProductName, Quantity, UnitPrice) = SaleItemTestData.ValidWithQuantity(quantity);

        // Act
        var item = SaleItem.Create(ProductId, ProductName, quantity, UnitPrice);

        // Assert
        var expectedDiscount = quantity * UnitPrice * 0.20m;
        Assert.Equal(expectedDiscount, item.Discount);
        Assert.Equal((quantity * UnitPrice) - expectedDiscount, item.TotalAmount);
    }

    [Theory(DisplayName = "Given quantity above 20 When creating item Then throws DomainException")]
    [InlineData(21)]
    [InlineData(50)]
    public void Given_QuantityAbove20_When_CreatingItem_Then_ThrowsDomainException(int quantity)
    {
        // Arrange
        var (ProductId, ProductName, Quantity, UnitPrice) = SaleItemTestData.ValidWithQuantity(quantity);

        // Act & Assert
        Assert.Throws<DomainException>(() =>
            SaleItem.Create(ProductId, ProductName, quantity, UnitPrice));
    }

    [Fact(DisplayName = "Given zero quantity When creating item Then throws DomainException")]
    public void Given_ZeroQuantity_When_CreatingItem_Then_ThrowsDomainException()
    {
        // Arrange
        var (ProductId, ProductName, Quantity, UnitPrice) = SaleItemTestData.ValidWithQuantity(0);

        // Act & Assert
        Assert.Throws<DomainException>(() =>
            SaleItem.Create(ProductId, ProductName, 0, UnitPrice));
    }

    [Fact(DisplayName = "Given negative quantity When creating item Then throws DomainException")]
    public void Given_NegativeQuantity_When_CreatingItem_Then_ThrowsDomainException()
    {
        // Arrange
        var (ProductId, ProductName, Quantity, UnitPrice) = SaleItemTestData.ValidWithQuantity(5); // Start with valid data

        // Act & Assert
        var exception = Assert.Throws<DomainException>(() =>
            SaleItem.Create(ProductId, ProductName, -1, UnitPrice));

        Assert.Equal("Quantity must be greater than zero.", exception.Message);
    }

    [Fact(DisplayName = "Given zero unit price When creating item Then throws DomainException")]
    public void Given_ZeroUnitPrice_When_CreatingItem_Then_ThrowsDomainException()
    {
        // Arrange
        var (ProductId, ProductName, Quantity, UnitPrice) = SaleItemTestData.ValidWithQuantity(5);

        // Act & Assert
        var exception = Assert.Throws<DomainException>(() =>
            SaleItem.Create(ProductId, ProductName, 5, 0m));

        Assert.Equal("Unit price must be greater than zero.", exception.Message);
    }

    [Fact(DisplayName = "Given negative unit price When creating item Then throws DomainException")]
    public void Given_NegativeUnitPrice_When_CreatingItem_Then_ThrowsDomainException()
    {
        // Arrange
        var (ProductId, ProductName, Quantity, UnitPrice) = SaleItemTestData.ValidWithQuantity(5);

        // Act & Assert
        var exception = Assert.Throws<DomainException>(() =>
            SaleItem.Create(ProductId, ProductName, 5, -10m));

        Assert.Equal("Unit price must be greater than zero.", exception.Message);
    }

    [Fact(DisplayName = "Given exactly quantity 3 When creating item Then 0% discount (upper boundary)")]
    public void Given_ExactlyQuantity3_When_CreatingItem_Then_NoDiscount()
    {
        // Arrange
        var (ProductId, ProductName, Quantity, UnitPrice) = SaleItemTestData.ValidWithQuantity(3);

        // Act
        var item = SaleItem.Create(ProductId, ProductName, 3, UnitPrice);

        // Assert
        Assert.Equal(0, item.Discount);
        Assert.Equal(3 * UnitPrice, item.TotalAmount);
    }

    [Fact(DisplayName = "Given exactly quantity 4 When creating item Then 10% discount (lower boundary)")]
    public void Given_ExactlyQuantity4_When_CreatingItem_Then_10PercentDiscount()
    {
        // Arrange
        var (ProductId, ProductName, Quantity, UnitPrice) = SaleItemTestData.ValidWithQuantity(4);

        // Act
        var item = SaleItem.Create(ProductId, ProductName, 4, UnitPrice);

        // Assert
        var expectedDiscount = 4 * UnitPrice * 0.10m;
        Assert.Equal(expectedDiscount, item.Discount);
        Assert.Equal((4 * UnitPrice) - expectedDiscount, item.TotalAmount);
    }

    [Fact(DisplayName = "Given exactly quantity 10 When creating item Then 20% discount (lower boundary)")]
    public void Given_ExactlyQuantity10_When_CreatingItem_Then_20PercentDiscount()
    {
        // Arrange
        var (ProductId, ProductName, Quantity, UnitPrice) = SaleItemTestData.ValidWithQuantity(10);

        // Act
        var item = SaleItem.Create(ProductId, ProductName, 10, UnitPrice);

        // Assert
        var expectedDiscount = 10 * UnitPrice * 0.20m;
        Assert.Equal(expectedDiscount, item.Discount);
        Assert.Equal((10 * UnitPrice) - expectedDiscount, item.TotalAmount);
    }

    [Fact(DisplayName = "Given exactly quantity 20 When creating item Then 20% discount (max valid qty)")]
    public void Given_ExactlyQuantity20_When_CreatingItem_Then_20PercentDiscount()
    {
        // Arrange
        var (ProductId, ProductName, Quantity, UnitPrice) = SaleItemTestData.ValidWithQuantity(20);

        // Act
        var item = SaleItem.Create(ProductId, ProductName, 20, UnitPrice);

        // Assert
        var expectedDiscount = 20 * UnitPrice * 0.20m;
        Assert.Equal(expectedDiscount, item.Discount);
        Assert.Equal((20 * UnitPrice) - expectedDiscount, item.TotalAmount);
    }

    [Fact(DisplayName = "Given valid item When created Then Id is non-empty Guid")]
    public void Given_ValidItem_When_Created_Then_IdIsNonEmptyGuid()
    {
        // Arrange
        var (ProductId, ProductName, Quantity, UnitPrice) = SaleItemTestData.ValidWithQuantity(5);

        // Act
        var item = SaleItem.Create(ProductId, ProductName, 5, UnitPrice);

        // Assert
        Assert.NotEqual(Guid.Empty, item.Id);
    }

    [Fact(DisplayName = "Given valid item When created Then TotalAmount equals (Qty × Price) - Discount")]
    public void Given_ValidItem_When_Created_Then_TotalAmountEqualsCalculation()
    {
        // Arrange
        const int quantity = 5;
        const decimal unitPrice = 100m;

        // Act
        var item = SaleItem.Create(Guid.NewGuid(), "Test Product", quantity, unitPrice);

        // Assert
        // Quantity 5 → 10% discount tier
        const decimal subtotal = quantity * unitPrice;         // 500
        const decimal discount = subtotal * 0.10m;             // 50
        const decimal expectedTotal = subtotal - discount;     // 450

        Assert.Equal(discount, item.Discount);
        Assert.Equal(expectedTotal, item.TotalAmount);
    }
}