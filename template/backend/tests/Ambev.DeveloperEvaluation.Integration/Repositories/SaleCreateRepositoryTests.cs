using Ambev.DeveloperEvaluation.Integration.Common;
using Ambev.DeveloperEvaluation.ORM.Repositories.Sales;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Ambev.DeveloperEvaluation.Integration.Repositories;

/// <summary>
/// Integration tests for <see cref="SaleCreateRepository"/>.
/// Verifies that sales and their items are correctly persisted to a real PostgreSQL database.
/// </summary>
/// <remarks>
/// Uses Testcontainers PostgreSQL for production-equivalent testing.
/// Each test creates data via <see cref="SaleCreateRepository.CreateAsync"/> and verifies
/// persistence using a separate DbContext to avoid EF Core tracking cache.
/// </remarks>
public class SaleCreateRepositoryTests : IntegrationTestBase
{
    /// <summary>
    /// Creates a new <see cref="SaleCreateRepository"/> using the test DbContext.
    /// </summary>
    private SaleCreateRepository CreateRepository() => new(Context);

    [Fact(DisplayName = "Given valid sale When creating Then persisted in database")]
    public async Task Given_ValidSale_When_Creating_Then_PersistedInDatabase()
    {
        // Arrange
        var repository = CreateRepository();
        var sale = SaleTestDataBuilder.Create()
            .WithCustomer(Guid.NewGuid(), "John Doe")
            .WithItems(2, quantity: 5, unitPrice: 100m)
            .Build();

        // Act
        var result = await repository.CreateAsync(sale, CancellationToken.None);

        // Assert — Use a new context to verify persistence (no tracking cache)
        await using var verifyContext = CreateNewContext();
        var savedSale = await verifyContext.Sales
            .Include(s => s.Items)
            .FirstOrDefaultAsync(s => s.Id == result.Id);

        savedSale.Should().NotBeNull();
        savedSale!.CustomerName.Should().Be("John Doe");
        savedSale.Items.Should().HaveCount(2);
        savedSale.SaleNumber.Should().NotBeNullOrEmpty();
        savedSale.IsCancelled.Should().BeFalse();
    }

    [Fact(DisplayName = "Given new sale When creating Then ID is auto-generated")]
    public async Task Given_NewSale_When_Creating_Then_IdGenerated()
    {
        // Arrange
        var repository = CreateRepository();
        var sale = SaleTestDataBuilder.Create().Build();

        // Act
        var result = await repository.CreateAsync(sale, CancellationToken.None);

        // Assert
        result.Id.Should().NotBe(Guid.Empty);
    }

    [Fact(DisplayName = "Given sale with multiple items When creating Then all items persisted")]
    public async Task Given_SaleWithMultipleItems_When_Creating_Then_AllItemsPersisted()
    {
        // Arrange
        var repository = CreateRepository();
        var sale = SaleTestDataBuilder.Create()
            .WithItems(3, quantity: 10, unitPrice: 50m)
            .Build();

        // Act
        var result = await repository.CreateAsync(sale, CancellationToken.None);

        // Assert — Use a new context to verify persistence
        await using var verifyContext = CreateNewContext();
        var savedSale = await verifyContext.Sales
            .Include(s => s.Items)
            .FirstOrDefaultAsync(s => s.Id == result.Id);

        savedSale.Should().NotBeNull();
        savedSale!.Items.Should().HaveCount(3);
        savedSale.Items.Should().AllSatisfy(item =>
        {
            item.Quantity.Should().Be(10);
            item.UnitPrice.Should().Be(50m);
        });
    }

    [Fact(DisplayName = "Given valid sale When creating Then TotalAmount is calculated correctly")]
    public async Task Given_ValidSale_When_Creating_Then_TotalAmountCalculatedCorrectly()
    {
        // Arrange
        var repository = CreateRepository();
        // 1 item: quantity=5 (10% discount tier), unitPrice=100m
        // Gross = 5 * 100 = 500, Discount = 500 * 0.10 = 50, Net = 450
        var sale = SaleTestDataBuilder.Create()
            .WithItems(1, quantity: 5, unitPrice: 100m)
            .Build();

        // Act
        var result = await repository.CreateAsync(sale, CancellationToken.None);

        // Assert
        await using var verifyContext = CreateNewContext();
        var savedSale = await verifyContext.Sales
            .FirstOrDefaultAsync(s => s.Id == result.Id);

        savedSale.Should().NotBeNull();
        savedSale!.TotalAmount.Should().Be(450m); // (5 * 100) - 10% discount = 450
    }
}