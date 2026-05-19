using Ambev.DeveloperEvaluation.Application.Sales;
using Ambev.DeveloperEvaluation.Application.Sales.CreateSale;
using Ambev.DeveloperEvaluation.Application.Sales.GetSale;
using Ambev.DeveloperEvaluation.Application.Sales.UpdateSale;
using Ambev.DeveloperEvaluation.Domain.Entities.Sales;
using AutoMapper;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Application.Sales;

public class SalesMappingProfileTests
{
    private readonly IMapper _mapper;

    public SalesMappingProfileTests()
    {
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddAutoMapper(cfg =>
        {
            cfg.LicenseKey = string.Empty;
            cfg.AddProfile<SalesMappingProfile>();
        });
        var provider = services.BuildServiceProvider();
        _mapper = provider.GetRequiredService<IMapper>();
    }

    [Fact(DisplayName = "Given CreateSaleCommand When mapping Then maps to Sale entity")]
    public void Map_CreateSaleCommand_To_Sale_ShouldWork()
    {
        // Given
        var command = new CreateSaleCommand
        {
            SaleNumber = "S001",
            SaleDate = DateTime.UtcNow,
            CustomerId = Guid.NewGuid(),
            CustomerName = "Customer",
            BranchId = Guid.NewGuid(),
            BranchName = "Branch",
            Items =
            [
                new CreateSaleItemCommand
                {
                    ProductId = Guid.NewGuid(),
                    ProductName = "Product",
                    Quantity = 2,
                    UnitPrice = 10m
                }
            ]
        };

        // When
        var result = _mapper.Map<Sale>(command);

        // Then
        result.Should().NotBeNull();
        result.SaleNumber.Should().Be(command.SaleNumber);
        result.CustomerName.Should().Be(command.CustomerName);
        result.BranchName.Should().Be(command.BranchName);
        result.Items.Should().HaveCount(1);
    }

    [Fact(DisplayName = "Given Sale When mapping Then maps to CreateSaleResult")]
    public void Map_Sale_To_CreateSaleResult_ShouldWork()
    {
        // Given
        var sale = Sale.Create(
            "S001",
            DateTime.UtcNow,
            Guid.NewGuid(),
            "Customer",
            Guid.NewGuid(),
            "Branch",
            [
                SaleItem.Create(Guid.NewGuid(), "Product", 1, 10m)
            ]);

        // When
        var result = _mapper.Map<CreateSaleResult>(sale);

        // Then
        result.Should().NotBeNull();
        result.Id.Should().Be(sale.Id);
    }

    [Fact(DisplayName = "Given Sale When mapping Then maps to GetSaleResult")]
    public void Map_Sale_To_GetSaleResult_ShouldWork()
    {
        // Given
        var sale = Sale.Create(
            "S001",
            DateTime.UtcNow,
            Guid.NewGuid(),
            "Customer",
            Guid.NewGuid(),
            "Branch",
            [
                SaleItem.Create(Guid.NewGuid(), "Product", 1, 10m)
            ]);

        // When
        var result = _mapper.Map<GetSaleResult>(sale);

        // Then
        result.Should().NotBeNull();
        result.Id.Should().Be(sale.Id);
        result.SaleNumber.Should().Be(sale.SaleNumber);
    }

    [Fact(DisplayName = "Given Sale When mapping Then maps to UpdateSaleResult")]
    public void Map_Sale_To_UpdateSaleResult_ShouldWork()
    {
        // Given
        var sale = Sale.Create(
            "S001",
            DateTime.UtcNow,
            Guid.NewGuid(),
            "Customer",
            Guid.NewGuid(),
            "Branch",
            [
                SaleItem.Create(Guid.NewGuid(), "Product", 1, 10m)
            ]);

        // When
        var result = _mapper.Map<UpdateSaleResult>(sale);

        // Then
        result.Should().NotBeNull();
        result.Id.Should().Be(sale.Id);
    }
}