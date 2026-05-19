using Ambev.DeveloperEvaluation.Application.Sales.UpdateSale;
using Bogus;

namespace Ambev.DeveloperEvaluation.Unit.Application.TestData;

public static class UpdateSaleHandlerTestData
{
    private static readonly Faker Faker = new();

    public static UpdateSaleCommand GenerateValidCommand(Guid? saleId = null) => new()
    {
        Id = saleId ?? Faker.Random.Guid(),
        CustomerId = Faker.Random.Guid(),
        CustomerName = Faker.Person.FullName,
        BranchId = Faker.Random.Guid(),
        BranchName = Faker.Company.CompanyName(),
        Items =
        [
            new UpdateSaleItemCommand
            {
                ProductId = Faker.Random.Guid(),
                ProductName = Faker.Commerce.ProductName(),
                Quantity = Faker.Random.Int(1, 5),
                UnitPrice = Faker.Random.Decimal(10, 100)
            }
        ]
    };

    public static UpdateSaleCommand GenerateCommandWithInvalidItems() => new()
    {
        Id = Faker.Random.Guid(),
        CustomerId = Faker.Random.Guid(),
        CustomerName = Faker.Person.FullName,
        BranchId = Faker.Random.Guid(),
        BranchName = Faker.Company.CompanyName(),
        Items = [] // Empty items usually fail validation
    };
}