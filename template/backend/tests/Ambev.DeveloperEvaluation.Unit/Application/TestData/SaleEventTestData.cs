using Ambev.DeveloperEvaluation.Domain.Entities.Sales;

namespace Ambev.DeveloperEvaluation.Unit.Application.TestData;

/// <summary>
/// Provides test data builders for sale event testing.
/// </summary>
public static class SaleEventTestData
{
    /// <summary>
    /// Generates a valid sale with default test data.
    /// </summary>
    public static Sale GenerateValidSale()
    {
        var items = new[]
        {
            SaleItem.Create(Guid.NewGuid(), "Product A", 5, 100m),
            SaleItem.Create(Guid.NewGuid(), "Product B", 10, 50m)
        };

        return Sale.Create(
            saleDate: DateTime.UtcNow,
            customerId: Guid.NewGuid(),
            customerName: "John Doe",
            branchId: Guid.NewGuid(),
            branchName: "Downtown Store",
            items: items);
    }

    /// <summary>
    /// Generates a valid sale with a specified number of items.
    /// </summary>
    /// <param name="itemCount">Number of items to include in the sale.</param>
    public static Sale GenerateValidSaleWithItems(int itemCount)
    {
        var items = new List<SaleItem>();

        for (int i = 0; i < itemCount; i++)
        {
            items.Add(SaleItem.Create(
                Guid.NewGuid(),
                $"Product {i + 1}",
                quantity: 5 + i,
                unitPrice: 100m + (i * 10)));
        }

        return Sale.Create(
            saleDate: DateTime.UtcNow,
            customerId: Guid.NewGuid(),
            customerName: "Jane Smith",
            branchId: Guid.NewGuid(),
            branchName: "Uptown Branch",
            items: items.ToArray());
    }

    /// <summary>
    /// Generates a valid sale with custom customer and branch information.
    /// </summary>
    public static Sale GenerateValidSaleWithCustomerAndBranch(
        string customerName,
        string branchName)
    {
        var items = new[]
        {
            SaleItem.Create(Guid.NewGuid(), "Default Product", 5, 100m)
        };

        return Sale.Create(
            saleDate: DateTime.UtcNow,
            customerId: Guid.NewGuid(),
            customerName: customerName,
            branchId: Guid.NewGuid(),
            branchName: branchName,
            items: items);
    }
}