using Ambev.DeveloperEvaluation.Domain.Entities.Sales;
using Ambev.DeveloperEvaluation.Domain.Specifications.Sales;
using FluentAssertions;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Domain.Specifications.Sales;

public class SaleFilterSpecificationTests
{
    private readonly List<Sale> _sales;

    public SaleFilterSpecificationTests()
    {
        var customerId = Guid.NewGuid();
        var branchId = Guid.NewGuid();
        var defaultItem = new[] { SaleItem.Create(Guid.NewGuid(), "Product A", 1, 10m) };

        _sales =
        [
            Sale.Create(DateTime.UtcNow.AddDays(-10), customerId, "Matheus Vieira", branchId, "Joinville Store", defaultItem),
            Sale.Create(DateTime.UtcNow.AddDays(-5), Guid.NewGuid(), "Other Customer", branchId, "Joinville Store", defaultItem),
            Sale.Create(DateTime.UtcNow, customerId, "Matheus Vieira", Guid.NewGuid(), "São Paulo Store", defaultItem)
        ];
    }

    [Fact(DisplayName = "Given customer name filter When applying Specification Then returns only matching sales")]
    public void SatisfiedBy_CustomerName_ReturnsMatchingSales()
    {
        // Given
        var spec = new SaleFilterSpecification { CustomerName = "Matheus" };

        // When
        var result = _sales.Where(spec.ToExpression().Compile()).ToList();

        // Then
        result.Should().HaveCount(2);
        result.All(s => s.CustomerName.Contains("Matheus")).Should().BeTrue();
    }

    [Fact(DisplayName = "Given branch name filter When applying Specification Then returns only matching sales")]
    public void SatisfiedBy_BranchName_ReturnsMatchingSales()
    {
        // Given
        var spec = new SaleFilterSpecification { BranchName = "Joinville" };

        // When
        var result = _sales.Where(spec.ToExpression().Compile()).ToList();

        // Then
        result.Should().HaveCount(2);
        result.All(s => s.BranchName.Contains("Joinville")).Should().BeTrue();
    }

    [Fact(DisplayName = "Given date range filter When applying Specification Then returns sales within range")]
    public void SatisfiedBy_DateRange_ReturnsSalesWithinRange()
    {
        // Given
        var spec = new SaleFilterSpecification
        {
            MinDate = DateTime.UtcNow.AddDays(-7),
            MaxDate = DateTime.UtcNow.AddDays(-1)
        };

        // When
        var result = _sales.Where(spec.ToExpression().Compile()).ToList();

        // Then
        result.Should().HaveCount(1);
        result.First().CustomerName.Should().Be("Other Customer");
    }

    [Fact(DisplayName = "Given no filters When applying Specification Then returns all sales")]
    public void SatisfiedBy_NoFilters_ReturnsAllSales()
    {
        // Given
        var spec = new SaleFilterSpecification();

        // When
        var result = _sales.Where(spec.ToExpression().Compile()).ToList();

        // Then
        result.Should().HaveCount(3);
    }

    [Fact(DisplayName = "Given isCancelled filter When applying Specification Then returns only cancelled sales")]
    public void SatisfiedBy_IsCancelled_ReturnsOnlyCancelledSales()
    {
        // Given
        _sales[0].Cancel();
        var spec = new SaleFilterSpecification { IsCancelled = true };

        // When
        var result = _sales.Where(spec.ToExpression().Compile()).ToList();

        // Then
        result.Should().HaveCount(1);
        result.First().IsCancelled.Should().BeTrue();
    }

    [Fact(DisplayName = "Given amount range filter When applying Specification Then returns sales within range")]
    public void SatisfiedBy_AmountRange_ReturnsSalesWithinRange()
    {
        // Given
        // defaultItem: qty=1, price=10 => TotalAmount=10 (no discount, qty < 4)
        var spec = new SaleFilterSpecification
        {
            MinAmount = 5m,
            MaxAmount = 15m
        };

        // When
        var result = _sales.Where(spec.ToExpression().Compile()).ToList();

        // Then
        result.Should().HaveCount(3);
        result.All(s => s.TotalAmount >= 5m && s.TotalAmount <= 15m).Should().BeTrue();
    }
}