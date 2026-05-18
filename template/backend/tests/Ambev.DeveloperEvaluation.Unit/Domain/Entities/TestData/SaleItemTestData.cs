using Ambev.DeveloperEvaluation.Domain.Entities.Sales;
using Bogus;

namespace Ambev.DeveloperEvaluation.Unit.Domain.Entities.TestData;

/// <summary>
/// Provides Bogus-generated test data for <see cref="SaleItem"/> unit tests.
/// </summary>
public static class SaleItemTestData
{
    private static readonly Faker Faker = new();

    public static (Guid ProductId, string ProductName, int Quantity, decimal UnitPrice) ValidBelow4()
        => (Faker.Random.Guid(),
            Faker.Commerce.ProductName(),
            Faker.Random.Int(1, 3),
            Faker.Random.Decimal(1, 500));

    public static (Guid ProductId, string ProductName, int Quantity, decimal UnitPrice) ValidWithQuantity(int quantity)
        => (Faker.Random.Guid(),
            Faker.Commerce.ProductName(),
            quantity,
            Faker.Random.Decimal(1, 500));
}