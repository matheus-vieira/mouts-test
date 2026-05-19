using Ambev.DeveloperEvaluation.Application.Sales.GetSale;
using Bogus;

namespace Ambev.DeveloperEvaluation.Unit.Application.TestData;

public static class GetSaleHandlerTestData
{
    private static readonly Faker Faker = new();

    public static GetSaleCommand GenerateValidCommand()
        => new(Faker.Random.Guid());
}