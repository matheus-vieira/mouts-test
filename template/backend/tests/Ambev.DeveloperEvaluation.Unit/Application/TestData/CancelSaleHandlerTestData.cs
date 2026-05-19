using Ambev.DeveloperEvaluation.Application.Sales.CancelSale;
using Bogus;

namespace Ambev.DeveloperEvaluation.Unit.Application.TestData;

public static class CancelSaleHandlerTestData
{
    private static readonly Faker Faker = new();

    public static CancelSaleCommand GenerateValidCommand()
        => new(Faker.Random.Guid());

    public static CancelSaleCommand GenerateInvalidCommand()
        => new(Guid.Empty);
}