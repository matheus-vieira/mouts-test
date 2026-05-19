using Ambev.DeveloperEvaluation.Domain.Entities.Sales;
using Bogus;

namespace Ambev.DeveloperEvaluation.Unit.Application.TestData;

public static class SaleEventTestData
{
    private static readonly Faker Faker = new();

    public static Sale GenerateValidSale()
    {
        var items = new List<SaleItem>
        {
            SaleItem.Create(
                Faker.Random.Guid(),
                Faker.Commerce.ProductName(),
                Faker.Random.Int(1, 3),
                Faker.Random.Decimal(1, 500)
            ),
            SaleItem.Create(
                Faker.Random.Guid(),
                Faker.Commerce.ProductName(),
                Faker.Random.Int(1, 3),
                Faker.Random.Decimal(1, 500)
            )
        };

        return Sale.Create(
            DateTime.UtcNow,
            Faker.Random.Guid(),
            Faker.Person.FullName,
            Faker.Random.Guid(),
            Faker.Company.CompanyName(),
            items
        );
    }
}