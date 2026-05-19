using Ambev.DeveloperEvaluation.Application.Sales.CancelSale;
using Ambev.DeveloperEvaluation.Domain.Repositories.Sales;
using Ambev.DeveloperEvaluation.Domain.Entities.Sales;
using Ambev.DeveloperEvaluation.Unit.Application.TestData;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Xunit;
using Ambev.DeveloperEvaluation.Domain.Exceptions;
using MediatR;

namespace Ambev.DeveloperEvaluation.Unit.Application.Sales.CancelSale;

public class CancelSaleHandlerTests
{
    private readonly ISaleReadRepository _readRepository;
    private readonly ISaleUpdateRepository _updateRepository;
    private readonly ILogger<CancelSaleHandler> _logger;
    private readonly IMediator _mediator;
    private readonly CancelSaleHandler _handler;

    public CancelSaleHandlerTests()
    {
        _readRepository = Substitute.For<ISaleReadRepository>();
        _updateRepository = Substitute.For<ISaleUpdateRepository>();
        _logger = Substitute.For<ILogger<CancelSaleHandler>>();
        _mediator = Substitute.For<IMediator>();
        _handler = new CancelSaleHandler(_readRepository, _updateRepository, _mediator, _logger);
    }

    [Fact]
    public async Task Handle_ShouldCancelSale_WhenSaleExists()
    {
        var sale = SaleEventTestData.GenerateValidSale();
        var command = new CancelSaleCommand(sale.Id);

        _readRepository
            .GetByIdAsync(command.Id, Arg.Any<CancellationToken>())
            .Returns(sale);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.Should().Be(MediatR.Unit.Value);
        sale.IsCancelled.Should().BeTrue();
        await _updateRepository.Received(1)
            .UpdateAsync(sale, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_ShouldThrowKeyNotFoundException_WhenSaleDoesNotExist()
    {
        var command = CancelSaleHandlerTestData.GenerateValidCommand();

        _readRepository
            .GetByIdAsync(command.Id, Arg.Any<CancellationToken>())
            .Returns((Sale?)null);

        var act = async () => await _handler.Handle(command, CancellationToken.None);

        await act.Should()
            .ThrowAsync<KeyNotFoundException>()
            .WithMessage($"Sale with ID {command.Id} was not found");
        await _updateRepository.DidNotReceive()
            .UpdateAsync(Arg.Any<Sale>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_ShouldPersistCancelledSale_WhenSaleExists()
    {
        var sale = SaleEventTestData.GenerateValidSale();
        var command = new CancelSaleCommand(sale.Id);

        _readRepository
            .GetByIdAsync(command.Id, Arg.Any<CancellationToken>())
            .Returns(sale);

        await _handler.Handle(command, CancellationToken.None);

        await _updateRepository.Received(1)
            .UpdateAsync(
                Arg.Is<Sale>(s => s.Id == sale.Id && s.IsCancelled),
                Arg.Any<CancellationToken>());
    }

    [Fact(DisplayName = "Given already-cancelled sale When cancelling Then throws DomainException")]
    public async Task Handle_AlreadyCancelledSale_ThrowsDomainException()
    {
        // Arrange
        var sale = SaleEventTestData.GenerateValidSale();
        sale.Cancel(); // ← Pre-cancel the sale (IsCancelled = true)
        var command = new CancelSaleCommand(sale.Id);

        _readRepository
            .GetByIdAsync(command.Id, Arg.Any<CancellationToken>())
            .Returns(sale);

        // Act
        var act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should()
            .ThrowAsync<DomainException>()
            .WithMessage("Sale is already cancelled.");

        // Verify repository was NOT called (exception thrown before persistence)
        await _updateRepository.DidNotReceive()
            .UpdateAsync(Arg.Any<Sale>(), Arg.Any<CancellationToken>());
    }
}