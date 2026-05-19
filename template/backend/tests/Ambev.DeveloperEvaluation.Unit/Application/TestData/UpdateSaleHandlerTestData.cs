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

    /// <summary>
    /// Generates a command with item quantity exceeding the max allowed (>20).
    /// Used to test domain exception handling in the handler.
    /// </summary>
    public static UpdateSaleCommand GenerateCommandWithInvalidQuantity() => new()
    {
        Id = Faker.Random.Guid(),
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
            Quantity = 21, // ⚠️ Exceeds max allowed
            UnitPrice = Faker.Random.Decimal(10, 100)
        }
        ]
    };

    /// <summary>
    /// Generates a command with an item having zero unit price.
    /// Used to test domain exception handling in the handler.
    /// </summary>
    public static UpdateSaleCommand GenerateCommandWithZeroPrice() => new()
    {
        Id = Faker.Random.Guid(),
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
            Quantity = 5,
            UnitPrice = 0m // ⚠️ Invalid price
        }
        ]
    };

    /// <summary>
    /// Generates a command with multiple items at different discount tiers.
    /// Used to test total amount recalculation with correct discount application.
    /// </summary>
    public static UpdateSaleCommand GenerateCommandWithMultipleDiscountTiers() => new()
    {
        Id = Faker.Random.Guid(),
        CustomerId = Faker.Random.Guid(),
        CustomerName = Faker.Person.FullName,
        BranchId = Faker.Random.Guid(),
        BranchName = Faker.Company.CompanyName(),
        Items =
        [
            new UpdateSaleItemCommand
        {
            ProductId = Faker.Random.Guid(),
            ProductName = "Item A",
            Quantity = 5,    // 10% discount tier
            UnitPrice = 100m
        },
        new UpdateSaleItemCommand
        {
            ProductId = Faker.Random.Guid(),
            ProductName = "Item B",
            Quantity = 12,   // 20% discount tier
            UnitPrice = 50m
        }
        ]
    };
}