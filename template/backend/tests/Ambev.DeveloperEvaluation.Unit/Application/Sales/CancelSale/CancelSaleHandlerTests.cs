using Ambev.DeveloperEvaluation.Application.Sales.CancelSale;
using Ambev.DeveloperEvaluation.Domain.Entities.Sales;
using Ambev.DeveloperEvaluation.Domain.Repositories.Sales;
using Bogus;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Application.Sales.CancelSale;

public class CancelSaleHandlerTests
{
    private readonly ISaleReadRepository _readRepository;
    private readonly ISaleUpdateRepository _updateRepository;
    private readonly ILogger<CancelSaleHandler> _logger;
    private readonly CancelSaleHandler _handler;
    private readonly Faker _faker;

    public CancelSaleHandlerTests()
    {
        _readRepository = Substitute.For<ISaleReadRepository>();
        _updateRepository = Substitute.For<ISaleUpdateRepository>();
        _logger = Substitute.For<ILogger<CancelSaleHandler>>();
        _handler = new CancelSaleHandler(_readRepository, _updateRepository, _logger);
        _faker = new Faker("pt_BR");
    }

    private Sale BuildValidSale() =>
        Sale.Create(
            saleDate: DateTime.UtcNow,
            customerId: Guid.NewGuid(),
            customerName: _faker.Person.FullName,
            branchId: Guid.NewGuid(),
            branchName: _faker.Company.CompanyName(),
            items:
            [
                SaleItem.Create(
                    productId: Guid.NewGuid(),
                    productName: _faker.Commerce.ProductName(),
                    quantity: 1,
                    unitPrice: _faker.Random.Decimal(1, 100))
            ]);

    [Fact]
    public async Task Handle_ShouldCancelSale_WhenSaleExists()
    {
        // Arrange
        var sale = BuildValidSale();
        var command = new CancelSaleCommand(sale.Id);

        _readRepository
            .GetByIdAsync(command.Id, Arg.Any<CancellationToken>())
            .Returns(sale);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().Be(MediatR.Unit.Value);
        sale.IsCancelled.Should().BeTrue();

        await _updateRepository.Received(1)
            .UpdateAsync(sale, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_ShouldThrowKeyNotFoundException_WhenSaleDoesNotExist()
    {
        // Arrange
        var command = new CancelSaleCommand(Guid.NewGuid());

        _readRepository
            .GetByIdAsync(command.Id, Arg.Any<CancellationToken>())
            .Returns((Sale?)null);

        // Act
        var act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should()
            .ThrowAsync<KeyNotFoundException>()
            .WithMessage($"Sale with ID {command.Id} was not found");

        await _updateRepository.DidNotReceive()
            .UpdateAsync(Arg.Any<Sale>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_ShouldPersistCancelledSale_WhenSaleExists()
    {
        // Arrange
        var sale = BuildValidSale();
        var command = new CancelSaleCommand(sale.Id);

        _readRepository
            .GetByIdAsync(command.Id, Arg.Any<CancellationToken>())
            .Returns(sale);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        await _updateRepository.Received(1)
            .UpdateAsync(
                Arg.Is<Sale>(s => s.Id == sale.Id && s.IsCancelled),
                Arg.Any<CancellationToken>());
    }
}