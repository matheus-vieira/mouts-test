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

    [Fact(DisplayName = "Given active sale When Update() called Then customer/branch properties are updated")]
    public void Given_ActiveSale_When_UpdateCalled_Then_PropertiesAreUpdated()
    {
        // Arrange
        var sale = SaleTestData.GenerateValidSale();
        var newCustomerId = Guid.NewGuid();
        var newCustomerName = "Updated Customer";
        var newBranchId = Guid.NewGuid();
        var newBranchName = "Updated Branch";

        // Act
        sale.Update(newCustomerId, newCustomerName, newBranchId, newBranchName);

        // Assert
        Assert.Equal(newCustomerId, sale.CustomerId);
        Assert.Equal(newCustomerName, sale.CustomerName);
        Assert.Equal(newBranchId, sale.BranchId);
        Assert.Equal(newBranchName, sale.BranchName);
    }

    [Fact(DisplayName = "Given cancelled sale When Update() called Then throws DomainException")]
    public void Given_CancelledSale_When_UpdateCalled_Then_ThrowsDomainException()
    {
        // Arrange
        var sale = SaleTestData.GenerateValidSale();
        sale.Cancel(); // ← Set IsCancelled = true

        // Act & Assert
        var exception = Assert.Throws<DomainException>(() =>
            sale.Update(Guid.NewGuid(), "New Customer", Guid.NewGuid(), "New Branch"));

        Assert.Equal("Cannot update a cancelled sale.", exception.Message);
    }

    [Fact(DisplayName = "Given active sale When UpdateItems() called with new items Then items replaced and total recalculated")]
    public void Given_ActiveSale_When_UpdateItemsCalled_Then_ItemsReplacedAndTotalRecalculated()
    {
        // Arrange
        var sale = SaleTestData.GenerateValidSale();
        var originalItemCount = sale.Items.Count;

        var newItems = new[]
        {
        SaleItem.Create(Guid.NewGuid(), "New Item A", 5, 100m),  // $450 (10% discount)
        SaleItem.Create(Guid.NewGuid(), "New Item B", 12, 50m),  // $480 (20% discount)
        SaleItem.Create(Guid.NewGuid(), "New Item C", 2, 75m)    // $150 (0% discount)
    };

        // Act
        sale.UpdateItems(newItems);

        // Assert
        Assert.Equal(3, sale.Items.Count);
        Assert.NotEqual(originalItemCount, sale.Items.Count); // Items were replaced

        var expectedTotal = newItems.Sum(i => i.TotalAmount); // $450 + $480 + $150 = $1080
        Assert.Equal(expectedTotal, sale.TotalAmount);
    }

    [Fact(DisplayName = "Given active sale When UpdateItems() called with empty list Then throws DomainException")]
    public void Given_ActiveSale_When_UpdateItemsWithEmptyList_Then_ThrowsDomainException()
    {
        // Arrange
        var sale = SaleTestData.GenerateValidSale();
        var emptyItems = new List<SaleItem>();

        // Act & Assert
        var exception = Assert.Throws<DomainException>(() => sale.UpdateItems(emptyItems));
        Assert.Equal("Sale must have at least one item.", exception.Message);
    }

    [Fact(DisplayName = "Given sale When Cancel() returns Then tuple contains correct Id and SaleNumber")]
    public void Given_ActiveSale_When_CancelCalled_Then_ReturnsTupleWithCorrectIdAndNumber()
    {
        // Arrange
        var sale = SaleTestData.GenerateValidSale();
        var expectedId = sale.Id;
        var expectedNumber = sale.SaleNumber;

        // Act
        var (id, number) = sale.Cancel();

        // Assert
        Assert.Equal(expectedId, id);
        Assert.Equal(expectedNumber, number);
    }

    [Fact(DisplayName = "Given new sale When created Then SaleNumber has 'SALE-' prefix")]
    public void Given_NewSale_When_Created_Then_SaleNumberHasSalePrefix()
    {
        // Arrange & Act
        var sale = SaleTestData.GenerateValidSale();

        // Assert
        Assert.StartsWith("SALE-", sale.SaleNumber);
        Assert.Matches(@"^SALE-\d{8}-\d{9}$", sale.SaleNumber); // Format: SALE-YYYYMMDD-HHmmssfff
    }

    [Fact(DisplayName = "Given new sale When created Then SaleDate.Kind is Utc")]
    public void Given_NewSale_When_Created_Then_SaleDateKindIsUtc()
    {
        // Arrange & Act
        var sale = SaleTestData.GenerateValidSale();

        // Assert
        Assert.Equal(DateTimeKind.Utc, sale.SaleDate.Kind);
    }

    [Fact(DisplayName = "Given new sale When created Then Id is a non-empty Guid")]
    public void Given_NewSale_When_Created_Then_IdIsNonEmptyGuid()
    {
        // Arrange & Act
        var sale = SaleTestData.GenerateValidSale();

        // Assert
        Assert.NotEqual(Guid.Empty, sale.Id);
    }

    [Fact(DisplayName = "Given new sale When created Then IsCancelled is false")]
    public void Given_NewSale_When_Created_Then_IsCancelledIsFalse()
    {
        // Arrange & Act
        var sale = SaleTestData.GenerateValidSale();

        // Assert
        Assert.False(sale.IsCancelled);
    }
}