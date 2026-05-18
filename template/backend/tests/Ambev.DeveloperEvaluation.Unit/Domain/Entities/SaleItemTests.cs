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
        var data = SaleItemTestData.ValidBelow4();

        // Act
        var item = SaleItem.Create(data.ProductId, data.ProductName, data.Quantity, data.UnitPrice);

        // Assert
        Assert.Equal(0, item.Discount);
        Assert.Equal(data.Quantity * data.UnitPrice, item.TotalAmount);
    }

    [Theory(DisplayName = "Given quantity between 4 and 9 When creating item Then 10% discount is applied")]
    [InlineData(4)]
    [InlineData(7)]
    [InlineData(9)]
    public void Given_QuantityBetween4And9_When_CreatingItem_Then_Applies10PercentDiscount(int quantity)
    {
        // Arrange
        var data = SaleItemTestData.ValidWithQuantity(quantity);

        // Act
        var item = SaleItem.Create(data.ProductId, data.ProductName, quantity, data.UnitPrice);

        // Assert
        var expectedDiscount = quantity * data.UnitPrice * 0.10m;
        Assert.Equal(expectedDiscount, item.Discount);
        Assert.Equal(quantity * data.UnitPrice - expectedDiscount, item.TotalAmount);
    }

    [Theory(DisplayName = "Given quantity between 10 and 20 When creating item Then 20% discount is applied")]
    [InlineData(10)]
    [InlineData(15)]
    [InlineData(20)]
    public void Given_QuantityBetween10And20_When_CreatingItem_Then_Applies20PercentDiscount(int quantity)
    {
        // Arrange
        var data = SaleItemTestData.ValidWithQuantity(quantity);

        // Act
        var item = SaleItem.Create(data.ProductId, data.ProductName, quantity, data.UnitPrice);

        // Assert
        var expectedDiscount = quantity * data.UnitPrice * 0.20m;
        Assert.Equal(expectedDiscount, item.Discount);
        Assert.Equal(quantity * data.UnitPrice - expectedDiscount, item.TotalAmount);
    }

    [Theory(DisplayName = "Given quantity above 20 When creating item Then throws DomainException")]
    [InlineData(21)]
    [InlineData(50)]
    public void Given_QuantityAbove20_When_CreatingItem_Then_ThrowsDomainException(int quantity)
    {
        // Arrange
        var data = SaleItemTestData.ValidWithQuantity(quantity);

        // Act & Assert
        Assert.Throws<DomainException>(() =>
            SaleItem.Create(data.ProductId, data.ProductName, quantity, data.UnitPrice));
    }

    [Fact(DisplayName = "Given zero quantity When creating item Then throws DomainException")]
    public void Given_ZeroQuantity_When_CreatingItem_Then_ThrowsDomainException()
    {
        // Arrange
        var data = SaleItemTestData.ValidWithQuantity(0);

        // Act & Assert
        Assert.Throws<DomainException>(() =>
            SaleItem.Create(data.ProductId, data.ProductName, 0, data.UnitPrice));
    }
}