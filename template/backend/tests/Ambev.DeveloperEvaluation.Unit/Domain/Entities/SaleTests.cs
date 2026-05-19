using Ambev.DeveloperEvaluation.Domain.Entities.Sales;
using Ambev.DeveloperEvaluation.Domain.Exceptions;
using Ambev.DeveloperEvaluation.Unit.Domain.Entities.TestData;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Domain.Entities;

/// <summary>
/// Unit tests for the <see cref="Sale"/> aggregate root.
/// </summary>
public class SaleTests
{
    [Fact(DisplayName = "Given valid items When creating sale Then total is sum of item totals")]
    public void Given_ValidItems_When_CreatingSale_Then_TotalAmountIsCorrect()
    {
        // Arrange
        var (SaleDate, CustomerId, CustomerName, BranchId, BranchName, Items) = SaleTestData.ValidWithItems(3);

        // Act
        var sale = Sale.Create(
            SaleDate,
            CustomerId, CustomerName,
            BranchId, BranchName,
            Items);

        // Assert
        Assert.Equal(Items.Sum(i => i.TotalAmount), sale.TotalAmount);
    }

    [Fact(DisplayName = "Given empty items When creating sale Then throws DomainException")]
    public void Given_EmptyItems_When_CreatingSale_Then_ThrowsDomainException()
    {
        // Arrange
        var data = SaleTestData.ValidWithItems(0);

        // Act & Assert
        Assert.Throws<DomainException>(() => Sale.Create(
            data.SaleDate,
            data.CustomerId, data.CustomerName,
            data.BranchId, data.BranchName,
            data.Items));
    }

    [Fact(DisplayName = "Given active sale When cancelling Then IsCancelled is true")]
    public void Given_ActiveSale_When_Cancelling_Then_IsCancelledIsTrue()
    {
        // Arrange
        var sale = SaleTestData.GenerateValidSale();

        // Act
        sale.Cancel();

        // Assert
        Assert.True(sale.IsCancelled);
    }

    [Fact(DisplayName = "Given already cancelled sale When cancelling again Then throws DomainException")]
    public void Given_CancelledSale_When_CancellingAgain_Then_ThrowsDomainException()
    {
        // Arrange
        var sale = SaleTestData.GenerateValidSale();
        sale.Cancel();

        // Act & Assert
        Assert.Throws<DomainException>(() => sale.Cancel());
    }
}