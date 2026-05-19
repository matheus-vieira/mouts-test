using Ambev.DeveloperEvaluation.Integration.Common;
using Ambev.DeveloperEvaluation.ORM.Repositories.Sales;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Ambev.DeveloperEvaluation.Integration.Repositories;

/// <summary>
/// Integration tests for <see cref="SaleDeleteRepository"/>.
/// Verifies that delete operations correctly remove sales from a real PostgreSQL database
/// and that cascade delete removes associated items via FK ON DELETE CASCADE.
/// </summary>
public class SaleDeleteRepositoryTests : IntegrationTestBase
{
    /// <summary>Creates a <see cref="SaleCreateRepository"/> for seeding test data.</summary>
    private SaleCreateRepository CreateCreateRepository() => new(Context);

    /// <summary>Creates a <see cref="SaleDeleteRepository"/> for testing delete operations.</summary>
    private SaleDeleteRepository CreateDeleteRepository() => new(Context);

    [Fact(DisplayName = "Given existing sale When deleting Then removed from database")]
    public async Task Given_ExistingSale_When_Deleting_Then_RemovedFromDatabase()
    {
        // Arrange
        var createRepo = CreateCreateRepository();
        var deleteRepo = CreateDeleteRepository();

        var sale = SaleTestDataBuilder.Create().Build();
        await createRepo.CreateAsync(sale, CancellationToken.None);

        // Act
        var result = await deleteRepo.DeleteAsync(sale.Id, CancellationToken.None);

        // Assert
        result.Should().BeTrue();

        await using var verifyContext = CreateNewContext();
        var deletedSale = await verifyContext.Sales
            .FirstOrDefaultAsync(s => s.Id == sale.Id);
        deletedSale.Should().BeNull();
    }

    [Fact(DisplayName = "Given sale with items When deleting Then items also cascade deleted")]
    public async Task Given_SaleWithItems_When_Deleting_Then_ItemsAlsoCascadeDeleted()
    {
        // Arrange
        var createRepo = CreateCreateRepository();
        var deleteRepo = CreateDeleteRepository();

        var sale = SaleTestDataBuilder.Create().WithItems(3).Build();
        await createRepo.CreateAsync(sale, CancellationToken.None);

        // Verify items exist before delete
        await using var preCheckContext = CreateNewContext();
        var itemsBefore = await preCheckContext.SaleItems
            .Where(i => EF.Property<Guid>(i, "SaleId") == sale.Id)
            .ToListAsync();
        itemsBefore.Should().HaveCount(3);

        // Act
        var result = await deleteRepo.DeleteAsync(sale.Id, CancellationToken.None);

        // Assert — Real PostgreSQL FK ON DELETE CASCADE enforces cascade deletion
        result.Should().BeTrue();

        await using var verifyContext = CreateNewContext();
        var remainingItems = await verifyContext.SaleItems
            .Where(i => EF.Property<Guid>(i, "SaleId") == sale.Id)
            .ToListAsync();
        remainingItems.Should().BeEmpty("PostgreSQL FK ON DELETE CASCADE should remove all child items");
    }
}