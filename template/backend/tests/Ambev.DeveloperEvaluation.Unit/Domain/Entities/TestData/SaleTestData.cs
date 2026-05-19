using Ambev.DeveloperEvaluation.Domain.Entities.Sales;
using Bogus;

namespace Ambev.DeveloperEvaluation.Unit.Domain.Entities.TestData;

public static class SaleTestData
{
    private static readonly Faker Faker = new();

    public static (
        DateTime SaleDate,
        Guid CustomerId, string CustomerName,
        Guid BranchId, string BranchName,
        List<SaleItem> Items) ValidWithItems(int itemCount)
    {
        var items = Enumerable.Range(0, itemCount)
            .Select(_ => SaleItem.Create(
                Faker.Random.Guid(),
                Faker.Commerce.ProductName(),
                Faker.Random.Int(1, 3),
                Faker.Random.Decimal(1, 500)))
            .ToList();

        return (
            Faker.Date.Recent(),
            Faker.Random.Guid(),
            Faker.Person.FullName,
            Faker.Random.Guid(),
            Faker.Company.CompanyName(),
            items
        );
    }

    public static Sale GenerateValidSale()
    {
        var (SaleDate, CustomerId, CustomerName, BranchId, BranchName, Items) = ValidWithItems(2);

        return Sale.Create(
            SaleDate,
            CustomerId, CustomerName,
            BranchId, BranchName,
            Items);
    }
}