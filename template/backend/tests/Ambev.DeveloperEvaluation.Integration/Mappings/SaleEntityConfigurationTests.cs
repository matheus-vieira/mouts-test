using Ambev.DeveloperEvaluation.Domain.Entities.Sales;
using Ambev.DeveloperEvaluation.Integration.Common;
using Ambev.DeveloperEvaluation.ORM.Repositories.Sales;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Ambev.DeveloperEvaluation.Integration.Mappings;

/// <summary>
/// Integration tests for EF Core entity configurations (<see cref="SaleConfiguration"/> and
/// <see cref="SaleItemConfiguration"/>). Verifies that database constraints, indexes,
/// precision settings, and cascade behaviors are correctly enforced by real PostgreSQL.
/// </summary>
/// <remarks>
/// These tests validate the mapping layer by:
/// <list type="bullet">
///   <item>Testing unique index enforcement (real PostgreSQL UNIQUE constraint)</item>
///   <item>Inspecting EF Core model metadata for precision/scale configuration</item>
///   <item>Verifying cascade delete behavior with real FK ON DELETE CASCADE</item>
/// </list>
/// </remarks>
public class SaleEntityConfigurationTests : IntegrationTestBase
{
    /// <summary>Creates a <see cref="SaleCreateRepository"/> for seeding test data.</summary>
    private SaleCreateRepository CreateCreateRepository() => new(Context);

    /// <summary>Creates a <see cref="SaleDeleteRepository"/> for testing cascade deletes.</summary>
    private SaleDeleteRepository CreateDeleteRepository() => new(Context);

    [Fact(DisplayName = "Given sale with duplicate SaleNumber When creating Then throws unique constraint exception")]
    public async Task Given_SaleWithDuplicateSaleNumber_When_Creating_Then_ThrowsUniqueConstraintException()
    {
        // Arrange
        var createRepo = CreateCreateRepository();

        var sale1 = SaleTestDataBuilder.Create().Build();
        await createRepo.CreateAsync(sale1, CancellationToken.None);

        // Create a second sale and force the same SaleNumber via direct SQL
        // (since the domain factory auto-generates unique sale numbers,
        // we insert via raw SQL to simulate the constraint violation)
        var sale2Id = Guid.NewGuid();
        var duplicateSaleNumber = sale1.SaleNumber;

        // Act & Assert — PostgreSQL throws real unique violation
        var insertSql = @"
            INSERT INTO ""Sales"" (""Id"", ""SaleNumber"", ""SaleDate"", ""CustomerId"", ""CustomerName"", 
                                   ""BranchId"", ""BranchName"", ""TotalAmount"", ""IsCancelled"")
            VALUES ({0}, {1}, {2}, {3}, {4}, {5}, {6}, {7}, {8})";

        var act = async () => await Context.Database.ExecuteSqlRawAsync(
            insertSql,
            sale2Id,
            duplicateSaleNumber,
            DateTime.UtcNow,
            Guid.NewGuid(),
            "Test Customer",
            Guid.NewGuid(),
            "Test Branch",
            100m,
            false);

        await act.Should().ThrowAsync<Exception>()
            .Where(e => e.InnerException != null &&
                        e.InnerException.Message.Contains("duplicate key") ||
                        e.Message.Contains("duplicate key") ||
                        e.InnerException!.Message.Contains("unique constraint"));
    }

    [Fact(DisplayName = "Given SaleConfiguration When inspecting Then has correct precision for TotalAmount")]
    public async Task Given_SaleConfiguration_When_Inspecting_Then_HasCorrectPrecisionForTotalAmount()
    {
        // Arrange & Act — Inspect EF Core model metadata
        var saleEntity = Context.Model.FindEntityType(typeof(Sale));
        var totalAmountProp = saleEntity!.FindProperty(nameof(Sale.TotalAmount));

        // Assert
        totalAmountProp.Should().NotBeNull();
        totalAmountProp!.GetPrecision().Should().Be(18);
        totalAmountProp.GetScale().Should().Be(2);

        // Also verify SaleItem.UnitPrice and SaleItem.Discount precision
        var saleItemEntity = Context.Model.FindEntityType(typeof(SaleItem));
        var unitPriceProp = saleItemEntity!.FindProperty(nameof(SaleItem.UnitPrice));
        var discountProp = saleItemEntity.FindProperty(nameof(SaleItem.Discount));

        unitPriceProp!.GetPrecision().Should().Be(18);
        unitPriceProp.GetScale().Should().Be(2);
        discountProp!.GetPrecision().Should().Be(18);
        discountProp.GetScale().Should().Be(2);

        await Task.CompletedTask; // Required for async test signature
    }

    [Fact(DisplayName = "Given sale with items When deleting Then cascade delete removes items")]
    public async Task Given_SaleWithItems_When_Deleting_Then_CascadeDeleteConfigured()
    {
        // Arrange
        var createRepo = CreateCreateRepository();
        var deleteRepo = CreateDeleteRepository();

        var sale = SaleTestDataBuilder.Create().WithItems(3).Build();
        await createRepo.CreateAsync(sale, CancellationToken.None);

        // Verify items exist before delete
        await using var preContext = CreateNewContext();
        var itemsBefore = await preContext.SaleItems
            .Where(i => EF.Property<Guid>(i, "SaleId") == sale.Id)
            .CountAsync();
        itemsBefore.Should().Be(3);

        // Act
        await deleteRepo.DeleteAsync(sale.Id, CancellationToken.None);

        // Assert — Real PostgreSQL FK ON DELETE CASCADE
        await using var verifyContext = CreateNewContext();
        var itemsAfter = await verifyContext.SaleItems
            .Where(i => EF.Property<Guid>(i, "SaleId") == sale.Id)
            .CountAsync();
        itemsAfter.Should().Be(0, "FK ON DELETE CASCADE should remove all child SaleItems");

        // Also verify the sale itself is gone
        var saleAfter = await verifyContext.Sales
            .FirstOrDefaultAsync(s => s.Id == sale.Id);
        saleAfter.Should().BeNull();
    }
}