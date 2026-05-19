using Ambev.DeveloperEvaluation.Domain.Entities.Sales;
using Ambev.DeveloperEvaluation.Integration.Common;
using Ambev.DeveloperEvaluation.ORM.Repositories.Sales;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Ambev.DeveloperEvaluation.Integration.Repositories;

/// <summary>
/// Integration tests for <see cref="SaleUpdateRepository"/>.
/// Verifies that update operations correctly persist changes to a real PostgreSQL database.
/// </summary>
/// <remarks>
/// The <see cref="SaleUpdateRepository.UpdateAsync"/> method handles:
/// <list type="bullet">
///   <item>Scalar property updates (customer, branch, etc.)</item>
///   <item>Item upsert strategy: updates existing items, inserts new, removes orphans.</item>
/// </list>
/// Tests use a separate DbContext (<see cref="IntegrationTestBase.CreateNewContext"/>)
/// to verify persistence without EF Core tracking cache interference.
/// </remarks>
public class SaleUpdateRepositoryTests : IntegrationTestBase
{
    /// <summary>Creates a <see cref="SaleCreateRepository"/> for seeding test data.</summary>
    private SaleCreateRepository CreateCreateRepository() => new(Context);

    /// <summary>Creates a <see cref="SaleUpdateRepository"/> for testing update operations.</summary>
    private SaleUpdateRepository CreateUpdateRepository() => new(Context);

    [Fact(DisplayName = "Given existing sale When updating customer/branch Then changes persisted")]
    public async Task Given_ExistingSale_When_Updating_Then_ChangesPersisted()
    {
        // Arrange
        var createRepo = CreateCreateRepository();
        var sale = SaleTestDataBuilder.Create()
            .WithCustomer(Guid.NewGuid(), "Original Customer")
            .WithBranch(Guid.NewGuid(), "Original Branch")
            .Build();
        await createRepo.CreateAsync(sale, CancellationToken.None);

        // Act — Modify via domain method, then persist via UpdateAsync
        var newCustomerId = Guid.NewGuid();
        var newBranchId = Guid.NewGuid();
        sale.Update(newCustomerId, "Updated Customer", newBranchId, "Updated Branch");

        var updateRepo = CreateUpdateRepository();
        await updateRepo.UpdateAsync(sale, CancellationToken.None);

        // Assert — Use a new context to verify persistence (no tracking cache)
        await using var verifyContext = CreateNewContext();
        var savedSale = await verifyContext.Sales
            .FirstOrDefaultAsync(s => s.Id == sale.Id);

        savedSale.Should().NotBeNull();
        savedSale!.CustomerName.Should().Be("Updated Customer");
        savedSale.BranchName.Should().Be("Updated Branch");
        savedSale.CustomerId.Should().Be(newCustomerId);
        savedSale.BranchId.Should().Be(newBranchId);
    }

    [Fact(DisplayName = "Given existing sale When UpdateItems Then old items replaced with new")]
    public async Task Given_ExistingSale_When_UpdateItems_Then_OldItemsReplacedWithNew()
    {
        // Arrange
        var createRepo = CreateCreateRepository();
        var sale = SaleTestDataBuilder.Create()
            .WithItems(2, quantity: 5, unitPrice: 100m)
            .Build();
        await createRepo.CreateAsync(sale, CancellationToken.None);

        // Act — Replace items via domain method, then persist
        var newItems = new List<SaleItem>
        {
            SaleItem.Create(Guid.NewGuid(), "New Product A", 3, 200m),
            SaleItem.Create(Guid.NewGuid(), "New Product B", 7, 150m),
            SaleItem.Create(Guid.NewGuid(), "New Product C", 1, 500m)
        };
        sale.UpdateItems(newItems);

        var updateRepo = CreateUpdateRepository();
        await updateRepo.UpdateAsync(sale, CancellationToken.None);

        // Assert — Use a new context to verify persistence
        await using var verifyContext = CreateNewContext();
        var savedSale = await verifyContext.Sales
            .Include(s => s.Items)
            .FirstOrDefaultAsync(s => s.Id == sale.Id);

        savedSale.Should().NotBeNull();
        savedSale!.Items.Should().HaveCount(3);
        savedSale.Items.Select(i => i.ProductName).Should()
            .Contain("New Product A")
            .And.Contain("New Product B")
            .And.Contain("New Product C");
    }
}