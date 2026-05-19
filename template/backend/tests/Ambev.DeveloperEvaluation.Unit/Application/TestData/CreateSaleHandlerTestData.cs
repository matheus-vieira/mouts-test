using Ambev.DeveloperEvaluation.Application.Sales.CreateSale;
using Ambev.DeveloperEvaluation.Domain.Entities.Sales;
using Bogus;

namespace Ambev.DeveloperEvaluation.Unit.Application.TestData;

/// <summary>
/// Provides Bogus-generated test data for <see cref="CreateSaleHandlerTests"/>.
/// </summary>
public static class CreateSaleHandlerTestData
{
    private static readonly Faker Faker = new();

    public static CreateSaleCommand GenerateValidCommand() => new()
    {
        CustomerId = Faker.Random.Guid(),
        CustomerName = Faker.Person.FullName,
        BranchId = Faker.Random.Guid(),
        BranchName = Faker.Company.CompanyName(),
        Items =
        [
            new CreateSaleItemCommand
            {
                ProductId = Faker.Random.Guid(),
                ProductName = Faker.Commerce.ProductName(),
                Quantity = Faker.Random.Int(1, 3),
                UnitPrice = Faker.Random.Decimal(1, 500)
            },
            new CreateSaleItemCommand
            {
                ProductId = Faker.Random.Guid(),
                ProductName = Faker.Commerce.ProductName(),
                Quantity = Faker.Random.Int(1, 3),
                UnitPrice = Faker.Random.Decimal(1, 500)
            }
        ]
    };

    public static Sale GenerateSaleFromCommand(CreateSaleCommand command)
    {
        var items = command.Items.Select(i =>
            SaleItem.Create(i.ProductId, i.ProductName, i.Quantity, i.UnitPrice));

        return Sale.Create(
            DateTime.UtcNow,
            command.CustomerId, command.CustomerName,
            command.BranchId, command.BranchName,
            items);
    }
}