using Ambev.DeveloperEvaluation.Domain.Specifications.Sales;
using Ambev.DeveloperEvaluation.Integration.Common;
using Ambev.DeveloperEvaluation.ORM.Repositories.Sales;
using FluentAssertions;
using Xunit;

namespace Ambev.DeveloperEvaluation.Integration.Specifications;

/// <summary>
/// Integration tests for <see cref="SaleFilterSpecification"/> with real PostgreSQL.
/// Verifies that specification expressions are correctly translated to SQL by EF Core
/// and produce the expected filtered results.
/// </summary>
/// <remarks>
/// These tests validate that LINQ expressions from <see cref="SaleFilterSpecification.ToExpression()"/>
/// translate to correct PostgreSQL queries (LIKE, timestamp comparison, boolean filtering, numeric ranges).
/// Unlike unit tests, these catch provider-specific translation failures.
/// </remarks>
[Collection(DatabaseCollection.Name)]
public class SaleFilterSpecificationTests(
    PostgreSqlContainerFixture containerFixture
) : IntegrationTestBase(containerFixture)
{
    /// <summary>Creates a <see cref="SaleCreateRepository"/> for seeding test data.</summary>
    private SaleCreateRepository CreateCreateRepository() => new(Context);

    /// <summary>Creates a <see cref="SaleReadRepository"/> for testing filtered queries.</summary>
    private SaleReadRepository CreateReadRepository() => new(Context);

    [Fact(DisplayName = "Given sales with different customers When filter by CustomerName Then returns only matching")]
    public async Task Given_SalesWithDifferentCustomers_When_FilterByCustomerName_Then_ReturnsOnlyMatching()
    {
        // Arrange
        var createRepo = CreateCreateRepository();
        var readRepo = CreateReadRepository();

        await createRepo.CreateAsync(
            SaleTestDataBuilder.Create().WithCustomerName("Alice Johnson").Build(),
            CancellationToken.None);
        await createRepo.CreateAsync(
            SaleTestDataBuilder.Create().WithCustomerName("Bob Smith").Build(),
            CancellationToken.None);
        await createRepo.CreateAsync(
            SaleTestDataBuilder.Create().WithCustomerName("Alice Williams").Build(),
            CancellationToken.None);

        var spec = new SaleFilterSpecification { CustomerName = "Alice" };

        // Act
        var result = await readRepo.GetPagedAsync(spec, page: 1, pageSize: 10, orderBy: null, CancellationToken.None);

        // Assert
        result.TotalCount.Should().Be(2);
        result.Should().AllSatisfy(s => s.CustomerName.Should().Contain("Alice"));
    }

    [Fact(DisplayName = "Given sales with different dates When filter by date range Then returns only in range")]
    public async Task Given_SalesWithDifferentDates_When_FilterByDateRange_Then_ReturnsOnlyInRange()
    {
        // Arrange
        var createRepo = CreateCreateRepository();
        var readRepo = CreateReadRepository();

        var baseDate = new DateTime(2024, 6, 15, 12, 0, 0, DateTimeKind.Utc);

        await createRepo.CreateAsync(
            SaleTestDataBuilder.Create().WithSaleDate(baseDate.AddDays(-30)).Build(),
            CancellationToken.None);
        await createRepo.CreateAsync(
            SaleTestDataBuilder.Create().WithSaleDate(baseDate).Build(),
            CancellationToken.None);
        await createRepo.CreateAsync(
            SaleTestDataBuilder.Create().WithSaleDate(baseDate.AddDays(5)).Build(),
            CancellationToken.None);
        await createRepo.CreateAsync(
            SaleTestDataBuilder.Create().WithSaleDate(baseDate.AddDays(60)).Build(),
            CancellationToken.None);

        var spec = new SaleFilterSpecification
        {
            MinDate = baseDate.AddDays(-1),
            MaxDate = baseDate.AddDays(10)
        };

        // Act
        var result = await readRepo.GetPagedAsync(spec, page: 1, pageSize: 10, orderBy: null, CancellationToken.None);

        // Assert
        result.TotalCount.Should().Be(2);
        result.Should().AllSatisfy(s =>
        {
            s.SaleDate.Should().BeOnOrAfter(spec.MinDate.Value);
            s.SaleDate.Should().BeOnOrBefore(spec.MaxDate.Value);
        });
    }

    [Fact(DisplayName = "Given cancelled and active sales When filter by IsCancelled Then returns only cancelled")]
    public async Task Given_CancelledAndActiveSales_When_FilterByCancelled_Then_ReturnsOnlyCancelled()
    {
        // Arrange
        var createRepo = CreateCreateRepository();
        var readRepo = CreateReadRepository();

        await createRepo.CreateAsync(
            SaleTestDataBuilder.Create().Build(), // Active
            CancellationToken.None);
        await createRepo.CreateAsync(
            SaleTestDataBuilder.Create().AsCancelled().Build(), // Cancelled
            CancellationToken.None);
        await createRepo.CreateAsync(
            SaleTestDataBuilder.Create().Build(), // Active
            CancellationToken.None);
        await createRepo.CreateAsync(
            SaleTestDataBuilder.Create().AsCancelled().Build(), // Cancelled
            CancellationToken.None);

        var spec = new SaleFilterSpecification { IsCancelled = true };

        // Act
        var result = await readRepo.GetPagedAsync(spec, page: 1, pageSize: 10, orderBy: null, CancellationToken.None);

        // Assert
        result.TotalCount.Should().Be(2);
        result.Should().AllSatisfy(s => s.IsCancelled.Should().BeTrue());
    }

    [Fact(DisplayName = "Given sales with different amounts When filter by amount range Then returns only in range")]
    public async Task Given_SalesWithDifferentAmounts_When_FilterByAmountRange_Then_ReturnsOnlyInRange()
    {
        // Arrange
        var createRepo = CreateCreateRepository();
        var readRepo = CreateReadRepository();

        // Sale with low amount: qty=1, price=50 → total = 50
        await createRepo.CreateAsync(
            SaleTestDataBuilder.Create().WithItems(1, quantity: 1, unitPrice: 50m).Build(),
            CancellationToken.None);
        // Sale with medium amount: qty=5, price=100 → total = 450 (10% discount)
        await createRepo.CreateAsync(
            SaleTestDataBuilder.Create().WithItems(1, quantity: 5, unitPrice: 100m).Build(),
            CancellationToken.None);
        // Sale with high amount: qty=10, price=200 → total = 1600 (20% discount)
        await createRepo.CreateAsync(
            SaleTestDataBuilder.Create().WithItems(1, quantity: 10, unitPrice: 200m).Build(),
            CancellationToken.None);

        var spec = new SaleFilterSpecification
        {
            MinAmount = 100m,
            MaxAmount = 1000m
        };

        // Act
        var result = await readRepo.GetPagedAsync(spec, page: 1, pageSize: 10, orderBy: null, CancellationToken.None);

        // Assert
        result.TotalCount.Should().Be(1); // Only the 450 sale
        result.Should().AllSatisfy(s =>
        {
            s.TotalAmount.Should().BeGreaterThanOrEqualTo(100m);
            s.TotalAmount.Should().BeLessThanOrEqualTo(1000m);
        });
    }

    [Fact(DisplayName = "Given sales with different branches When filter by BranchName Then returns only matching")]
    public async Task Given_SalesWithDifferentBranches_When_FilterByBranchName_Then_ReturnsOnlyMatching()
    {
        // Arrange
        var createRepo = CreateCreateRepository();
        var readRepo = CreateReadRepository();

        await createRepo.CreateAsync(
            SaleTestDataBuilder.Create().WithBranchName("São Paulo Downtown").Build(),
            CancellationToken.None);
        await createRepo.CreateAsync(
            SaleTestDataBuilder.Create().WithBranchName("Rio de Janeiro").Build(),
            CancellationToken.None);
        await createRepo.CreateAsync(
            SaleTestDataBuilder.Create().WithBranchName("São Paulo Morumbi").Build(),
            CancellationToken.None);

        var spec = new SaleFilterSpecification { BranchName = "São Paulo" };

        // Act
        var result = await readRepo.GetPagedAsync(spec, page: 1, pageSize: 10, orderBy: null, CancellationToken.None);

        // Assert
        result.TotalCount.Should().Be(2);
        result.Should().AllSatisfy(s => s.BranchName.Should().Contain("São Paulo"));
    }
}