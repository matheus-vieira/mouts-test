using Ambev.DeveloperEvaluation.Application.Sales.UpdateSale;
using Ambev.DeveloperEvaluation.Domain.Entities.Sales;
using Ambev.DeveloperEvaluation.Domain.Exceptions;
using Ambev.DeveloperEvaluation.Domain.Repositories.Sales;
using Ambev.DeveloperEvaluation.Unit.Application.TestData;
using AutoMapper;
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

    public UpdateSaleHandlerTests()
    {
        _readRepository = Substitute.For<ISaleReadRepository>();
        _updateRepository = Substitute.For<ISaleUpdateRepository>();
        _mapper = Substitute.For<IMapper>();
        _logger = Substitute.For<ILogger<UpdateSaleHandler>>();
        _handler = new UpdateSaleHandler(_readRepository, _updateRepository, _mapper, _logger);
    }

    [Fact(DisplayName = "Given existing sale When handling Then updates and returns result")]
    public async Task Handle_ExistingSale_UpdatesSuccessfully()
    {
        // Arrange
        var sale = SaleEventTestData.GenerateValidSale();
        var command = UpdateSaleHandlerTestData.GenerateValidCommand(sale.Id);
        var expectedResult = new UpdateSaleResult { Id = sale.Id };

        _readRepository.GetByIdAsync(command.Id, Arg.Any<CancellationToken>())
            .Returns(sale);
        _mapper.Map<UpdateSaleResult>(sale).Returns(expectedResult);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        await _updateRepository.Received(1).UpdateAsync(sale, Arg.Any<CancellationToken>());
        await _readRepository.Received(1).GetByIdAsync(command.Id, Arg.Any<CancellationToken>());
    }

    [Fact(DisplayName = "Given non-existent sale When handling Then throws KeyNotFoundException")]
    public async Task Handle_NonExistentSale_ThrowsKeyNotFoundException()
    {
        // Arrange
        var command = UpdateSaleHandlerTestData.GenerateValidCommand();
        _readRepository.GetByIdAsync(command.Id, Arg.Any<CancellationToken>())
            .Returns((Sale?)null);

        // Act
        var act = () => _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<KeyNotFoundException>()
            .WithMessage($"Sale with ID {command.Id} was not found");
    }

    [Fact(DisplayName = "Given cancelled sale When handling Then domain rules should throw")]
    public async Task Handle_CancelledSale_ThrowsInvalidOperationException()
    {
        // Arrange
        var sale = SaleEventTestData.GenerateValidSale();
        sale.Cancel();
        var command = UpdateSaleHandlerTestData.GenerateValidCommand(sale.Id);

        _readRepository.GetByIdAsync(command.Id, Arg.Any<CancellationToken>())
            .Returns(sale);

        // Act
        var act = () => _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<DomainException>()
            .WithMessage("Cannot update a cancelled sale.");
    }

    [Fact(DisplayName = "Given command with item quantity > 20 When handling Then throws DomainException")]
    public async Task Handle_ItemWithInvalidQuantity_ThrowsDomainException()
    {
        // Arrange
        var sale = SaleEventTestData.GenerateValidSale();
        var command = UpdateSaleHandlerTestData.GenerateCommandWithInvalidQuantity();
        command.Id = sale.Id; // Use existing sale ID

        _readRepository.GetByIdAsync(command.Id, Arg.Any<CancellationToken>())
            .Returns(sale);

        // Act
        var act = () => _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<DomainException>()
            .WithMessage("Cannot sell more than 20 identical items.");
        await _updateRepository.DidNotReceive()
            .UpdateAsync(Arg.Any<Sale>(), Arg.Any<CancellationToken>());
    }

    [Fact(DisplayName = "Given command with item unit price = 0 When handling Then throws DomainException")]
    public async Task Handle_ItemWithZeroPrice_ThrowsDomainException()
    {
        // Arrange
        var sale = SaleEventTestData.GenerateValidSale();
        var command = UpdateSaleHandlerTestData.GenerateCommandWithZeroPrice();
        command.Id = sale.Id;

        _readRepository.GetByIdAsync(command.Id, Arg.Any<CancellationToken>())
            .Returns(sale);

        // Act
        var act = () => _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<DomainException>()
            .WithMessage("Unit price must be greater than zero.");
        await _updateRepository.DidNotReceive()
            .UpdateAsync(Arg.Any<Sale>(), Arg.Any<CancellationToken>());
    }

    [Fact(DisplayName = "Given valid update with multiple items When handling Then recalculates total amount with correct discounts")]
    public async Task Handle_ValidUpdate_RecalculatesTotalWithDiscounts()
    {
        // Arrange
        var sale = SaleEventTestData.GenerateValidSale();
        var command = UpdateSaleHandlerTestData.GenerateCommandWithMultipleDiscountTiers();
        command.Id = sale.Id;

        _readRepository.GetByIdAsync(command.Id, Arg.Any<CancellationToken>())
            .Returns(sale);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        // Item 1: Qty=5, Price=$100 → Subtotal=$500, 10% discount=$50, Total=$450
        // Item 2: Qty=12, Price=$50 → Subtotal=$600, 20% discount=$120, Total=$480
        // Expected Sale.TotalAmount = $450 + $480 = $930
        var expectedItem1Total = (100m * 5) - (100m * 5 * 0.10m); // $450
        var expectedItem2Total = (50m * 12) - (50m * 12 * 0.20m); // $480
        var expectedSaleTotal = expectedItem1Total + expectedItem2Total; // $930

        sale.TotalAmount.Should().Be(expectedSaleTotal);
        sale.Items.Should().HaveCount(2);
        await _updateRepository.Received(1)
            .UpdateAsync(Arg.Is<Sale>(s => s.TotalAmount == expectedSaleTotal), Arg.Any<CancellationToken>());
    }

    [Fact(DisplayName = "Given valid update When handling Then mapper receives the updated sale entity")]
    public async Task Handle_ValidUpdate_CallsMapperWithUpdatedSale()
    {
        // Arrange
        var sale = SaleEventTestData.GenerateValidSale();
        var command = UpdateSaleHandlerTestData.GenerateValidCommand(sale.Id);
        var expectedResult = new UpdateSaleResult { Id = sale.Id };

        _readRepository.GetByIdAsync(command.Id, Arg.Any<CancellationToken>())
            .Returns(sale);
        _mapper.Map<UpdateSaleResult>(Arg.Any<Sale>()).Returns(expectedResult);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(sale.Id);
        _mapper.Received(1).Map<UpdateSaleResult>(
            Arg.Is<Sale>(s =>
                s.Id == command.Id &&
                s.CustomerId == command.CustomerId &&
                s.CustomerName == command.CustomerName &&
                s.Items.Count == command.Items.Count));
    }
}