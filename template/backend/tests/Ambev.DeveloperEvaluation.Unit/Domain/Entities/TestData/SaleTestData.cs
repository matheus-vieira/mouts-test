using Ambev.DeveloperEvaluation.Domain.Entities.Sales;
using Bogus;

namespace Ambev.DeveloperEvaluation.Unit.Domain.Entities.TestData;

/// <summary>
/// Provides Bogus-generated test data for <see cref="Sale"/> unit tests.
/// </summary>
public static class SaleTestData
{
    private static readonly Faker Faker = new();

    public static (
        string SaleNumber, DateTime SaleDate,
        Guid CustomerId, string CustomerName,
        Guid BranchId, string BranchName,
        IEnumerable<SaleItem> Items) ValidWithItems(int itemCount)
    {
        var items = Enumerable.Range(0, itemCount)
            .Select(_ => SaleItem.Create(
                Faker.Random.Guid(),
                Faker.Commerce.ProductName(),
                Faker.Random.Int(1, 3),
                Faker.Random.Decimal(1, 500)));

        return (
            Faker.Random.AlphaNumeric(10),
            Faker.Date.Recent(),
            Faker.Random.Guid(),
            Faker.Person.FullName,
            Faker.Random.Guid(),
            Faker.Company.CompanyName(),
            items);
    }

    public static Sale GenerateValidSale()
    {
        var (SaleNumber, SaleDate, CustomerId, CustomerName, BranchId, BranchName, Items) = ValidWithItems(2);
        return Sale.Create(
            SaleNumber, SaleDate,
            CustomerId, CustomerName,
            BranchId, BranchName,
            Items);
    }
}