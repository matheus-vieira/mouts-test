using Ambev.DeveloperEvaluation.Domain.Specifications.Sales;
using Ambev.DeveloperEvaluation.Integration.Common;
using Ambev.DeveloperEvaluation.ORM.Repositories.Sales;
using FluentAssertions;
using Xunit;

namespace Ambev.DeveloperEvaluation.Integration.Repositories;

/// <summary>
/// Integration tests for <see cref="SaleReadRepository"/>.
/// Verifies that read operations (GetById, GetBySaleNumber, GetPaged)
/// correctly query a real PostgreSQL database with eager loading of items.
/// </summary>
/// <remarks>
/// Test data is seeded via <see cref="SaleCreateRepository.CreateAsync"/> before each test.
/// Read operations are tested against the same PostgreSQL container.
/// </remarks>
[Collection(DatabaseCollection.Name)]
public class SaleReadRepositoryTests(
    PostgreSqlContainerFixture containerFixture
) : IntegrationTestBase(containerFixture)
{
    /// <summary>Creates a <see cref="SaleCreateRepository"/> for seeding test data.</summary>
    private SaleCreateRepository CreateCreateRepository() => new(Context);

    /// <summary>Creates a <see cref="SaleReadRepository"/> for testing read operations.</summary>
    private SaleReadRepository CreateReadRepository() => new(Context);

    [Fact(DisplayName = "Given existing sale When GetById Then returns sale with items")]
    public async Task Given_ExistingSale_When_GetById_Then_ReturnsSaleWithItems()
    {
        // Arrange
        var createRepo = CreateCreateRepository();
        var readRepo = CreateReadRepository();

        var sale = SaleTestDataBuilder.Create().WithItems(2).Build();
        await createRepo.CreateAsync(sale, CancellationToken.None);

        // Act
        var result = await readRepo.GetByIdAsync(sale.Id, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(sale.Id);
        result.CustomerName.Should().Be(sale.CustomerName);
        result.Items.Should().HaveCount(2);
    }

    [Fact(DisplayName = "Given non-existent sale When GetById Then returns null")]
    public async Task Given_NonExistentSale_When_GetById_Then_ReturnsNull()
    {
        // Arrange
        var readRepo = CreateReadRepository();

        // Act
        var result = await readRepo.GetByIdAsync(Guid.NewGuid(), CancellationToken.None);

        // Assert
        result.Should().BeNull();
    }

    [Fact(DisplayName = "Given existing sale When GetBySaleNumber Then returns sale")]
    public async Task Given_ExistingSale_When_GetBySaleNumber_Then_ReturnsSale()
    {
        // Arrange
        var createRepo = CreateCreateRepository();
        var readRepo = CreateReadRepository();

        var sale = SaleTestDataBuilder.Create().Build();
        await createRepo.CreateAsync(sale, CancellationToken.None);

        // Act
        var result = await readRepo.GetBySaleNumberAsync(sale.SaleNumber, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result!.SaleNumber.Should().Be(sale.SaleNumber);
        result.Id.Should().Be(sale.Id);
    }

    [Fact(DisplayName = "Given multiple sales When GetPaged with customer filter Then returns only matching")]
    public async Task Given_MultipleSales_When_GetPagedWithCustomerFilter_Then_ReturnsFilteredResults()
    {
        // Arrange
        var createRepo = CreateCreateRepository();
        var readRepo = CreateReadRepository();

        var sale1 = SaleTestDataBuilder.Create().WithCustomerName("Alice Johnson").Build();
        var sale2 = SaleTestDataBuilder.Create().WithCustomerName("Bob Smith").Build();
        var sale3 = SaleTestDataBuilder.Create().WithCustomerName("Alice Williams").Build();

        await createRepo.CreateAsync(sale1, CancellationToken.None);
        await createRepo.CreateAsync(sale2, CancellationToken.None);
        await createRepo.CreateAsync(sale3, CancellationToken.None);

        var spec = new SaleFilterSpecification { CustomerName = "Alice" };

        // Act
        var result = await readRepo.GetPagedAsync(spec, page: 1, pageSize: 10, orderBy: null, CancellationToken.None);

        // Assert
        result.Should().HaveCount(2);
        result.TotalCount.Should().Be(2);
        result.Should().AllSatisfy(s => s.CustomerName.Should().Contain("Alice"));
    }

    [Fact(DisplayName = "Given multiple sales When GetPaged with pagination Then returns correct page")]
    public async Task Given_MultipleSales_When_GetPagedWithPagination_Then_ReturnsCorrectPage()
    {
        // Arrange
        var createRepo = CreateCreateRepository();
        var readRepo = CreateReadRepository();

        for (int i = 0; i < 15; i++)
        {
            var sale = SaleTestDataBuilder.Create().Build();
            await createRepo.CreateAsync(sale, CancellationToken.None);
        }

        var spec = new SaleFilterSpecification();

        // Act
        var result = await readRepo.GetPagedAsync(spec, page: 2, pageSize: 5, orderBy: null, CancellationToken.None);

        // Assert
        result.CurrentPage.Should().Be(2);
        result.PageSize.Should().Be(5);
        result.Should().HaveCount(5);
        result.TotalCount.Should().Be(15);
    }

    [Fact(DisplayName = "Given multiple sales When GetPaged with orderBy Then returns sorted results")]
    public async Task Given_MultipleSales_When_GetPagedWithOrderBy_Then_ReturnsSortedResults()
    {
        // Arrange
        var createRepo = CreateCreateRepository();
        var readRepo = CreateReadRepository();

        // Create sales with different amounts:
        // quantity=2 (no discount), unitPrice=50  → total = 100
        // quantity=5 (10% discount), unitPrice=100 → total = 450
        // quantity=10 (20% discount), unitPrice=80 → total = 640
        var sale1 = SaleTestDataBuilder.Create().WithItems(1, quantity: 2, unitPrice: 50m).Build();
        var sale2 = SaleTestDataBuilder.Create().WithItems(1, quantity: 5, unitPrice: 100m).Build();
        var sale3 = SaleTestDataBuilder.Create().WithItems(1, quantity: 10, unitPrice: 80m).Build();

        await createRepo.CreateAsync(sale1, CancellationToken.None);
        await createRepo.CreateAsync(sale2, CancellationToken.None);
        await createRepo.CreateAsync(sale3, CancellationToken.None);

        var spec = new SaleFilterSpecification();

        // Act
        var result = await readRepo.GetPagedAsync(spec, page: 1, pageSize: 10, orderBy: "TotalAmount", CancellationToken.None);

        // Assert
        result.Should().HaveCount(3);
        result[0].TotalAmount.Should().BeLessThanOrEqualTo(result[1].TotalAmount);
        result[1].TotalAmount.Should().BeLessThanOrEqualTo(result[2].TotalAmount);
    }
}