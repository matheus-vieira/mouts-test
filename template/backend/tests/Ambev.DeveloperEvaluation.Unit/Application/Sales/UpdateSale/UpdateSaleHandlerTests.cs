using Ambev.DeveloperEvaluation.Application.Sales.UpdateSale;
using Ambev.DeveloperEvaluation.Domain.Entities.Sales;
using Ambev.DeveloperEvaluation.Domain.Exceptions;
using Ambev.DeveloperEvaluation.Domain.Repositories.Sales;
using AutoMapper;
using Bogus;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Application.Sales.UpdateSale;

public class UpdateSaleHandlerTests
{
    private readonly ISaleReadRepository _readRepository;
    private readonly ISaleUpdateRepository _updateRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<UpdateSaleHandler> _logger;
    private readonly UpdateSaleHandler _handler;
    private readonly Faker _faker;

    public UpdateSaleHandlerTests()
    {
        _readRepository = Substitute.For<ISaleReadRepository>();
        _updateRepository = Substitute.For<ISaleUpdateRepository>();
        _mapper = Substitute.For<IMapper>();
        _logger = Substitute.For<ILogger<UpdateSaleHandler>>();
        _handler = new UpdateSaleHandler(_readRepository, _updateRepository, _mapper, _logger);
        _faker = new Faker("pt_BR");
    }

    private static Sale BuildSale() =>
        Sale.Create(
            DateTime.UtcNow,
            Guid.NewGuid(),
            "Customer",
            Guid.NewGuid(),
            "Branch",
            [SaleItem.Create(Guid.NewGuid(), "Old Item", 1, 10m)]);

    private UpdateSaleCommand BuildValidCommand(Guid? saleId = null) =>
        new()
        {
            Id = saleId ?? Guid.NewGuid(),
            CustomerId = Guid.NewGuid(),
            CustomerName = _faker.Person.FullName,
            BranchId = Guid.NewGuid(),
            BranchName = _faker.Company.CompanyName(),
            Items =
            [
                new UpdateSaleItemCommand
                {
                    ProductId = Guid.NewGuid(),
                    ProductName = _faker.Commerce.ProductName(),
                    Quantity = _faker.Random.Int(1, 5),
                    UnitPrice = _faker.Random.Decimal(1, 100)
                }
            ]
        };

    [Fact(DisplayName = "Given existing sale When updating Then returns success response")]
    public async Task Handle_ExistingSale_ReturnsSuccessResponse()
    {
        // Given
        var existingSale = BuildSale();
        var command = BuildValidCommand(existingSale.Id);
        var expectedResult = new UpdateSaleResult { Id = existingSale.Id };

        _readRepository.GetByIdAsync(existingSale.Id, Arg.Any<CancellationToken>()).Returns(existingSale);
        _mapper.Map<UpdateSaleResult>(existingSale).Returns(expectedResult);

        // When
        var result = await _handler.Handle(command, CancellationToken.None);

        // Then
        result.Should().NotBeNull();
        result.Id.Should().Be(existingSale.Id);
        await _updateRepository.Received(1).UpdateAsync(existingSale, Arg.Any<CancellationToken>());
    }

    [Fact(DisplayName = "Given existing sale When updating Then recalculates items and persists")]
    public async Task Handle_ExistingSale_UpdatesItemsAndPersists()
    {
        // Given
        var existingSale = BuildSale();
        var command = BuildValidCommand(existingSale.Id);
        command.Items =
        [
            new UpdateSaleItemCommand
            {
                ProductId = Guid.NewGuid(),
                ProductName = "New Product",
                Quantity = 10,
                UnitPrice = 20m
            }
        ];

        _readRepository.GetByIdAsync(existingSale.Id, Arg.Any<CancellationToken>()).Returns(existingSale);
        _mapper.Map<UpdateSaleResult>(Arg.Any<Sale>()).Returns(new UpdateSaleResult { Id = existingSale.Id });

        // When
        await _handler.Handle(command, CancellationToken.None);

        // Then
        existingSale.Items.Should().HaveCount(1);
        existingSale.Items.First().ProductName.Should().Be("New Product");
        existingSale.Items.First().Quantity.Should().Be(10);
    }

    [Fact(DisplayName = "Given non-existing sale When updating Then throws KeyNotFoundException")]
    public async Task Handle_NonExistingSale_ThrowsKeyNotFoundException()
    {
        // Given
        var command = BuildValidCommand();
        _readRepository.GetByIdAsync(command.Id, Arg.Any<CancellationToken>()).Returns((Sale?)null);

        // When
        var act = () => _handler.Handle(command, CancellationToken.None);

        // Then
        await act.Should().ThrowAsync<KeyNotFoundException>()
            .WithMessage($"Sale with ID {command.Id} was not found");
    }

    [Fact(DisplayName = "Given cancelled sale When updating Then throws DomainException")]
    public async Task Handle_CancelledSale_ThrowsDomainException()
    {
        // Given
        var existingSale = BuildSale();
        existingSale.Cancel();
        var command = BuildValidCommand(existingSale.Id);

        _readRepository.GetByIdAsync(existingSale.Id, Arg.Any<CancellationToken>()).Returns(existingSale);

        // When
        var act = () => _handler.Handle(command, CancellationToken.None);

        // Then
        await act.Should().ThrowAsync<DomainException>()
            .WithMessage("Cannot update a cancelled sale.");
    }

    [Fact(DisplayName = "Given existing sale When updating Then maps sale to result")]
    public async Task Handle_ExistingSale_MapsSaleToResult()
    {
        // Given
        var existingSale = BuildSale();
        var command = BuildValidCommand(existingSale.Id);

        _readRepository.GetByIdAsync(existingSale.Id, Arg.Any<CancellationToken>()).Returns(existingSale);
        _mapper.Map<UpdateSaleResult>(Arg.Any<Sale>()).Returns(new UpdateSaleResult { Id = existingSale.Id });

        // When
        await _handler.Handle(command, CancellationToken.None);

        // Then
        _mapper.Received(1).Map<UpdateSaleResult>(Arg.Is<Sale>(s => s.Id == existingSale.Id));
    }
}