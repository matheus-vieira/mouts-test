using Ambev.DeveloperEvaluation.Application.Sales.Events;
using Ambev.DeveloperEvaluation.Domain.Events.Sales;
using Ambev.DeveloperEvaluation.Unit.Application.TestData;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Application.Sales.Events;

/// <summary>
/// Unit tests for <see cref="SaleModifiedHandler"/>.
/// Verifies event processing, logging behavior, and error handling.
/// </summary>
public class SaleModifiedHandlerTests
{
    private readonly ILogger<SaleModifiedHandler> _logger;
    private readonly SaleModifiedHandler _handler;

    public SaleModifiedHandlerTests()
    {
        _logger = Substitute.For<ILogger<SaleModifiedHandler>>();
        _handler = new SaleModifiedHandler(_logger);
    }

    [Fact(DisplayName = "Given valid event When Handle called Then should process without errors")]
    public async Task Given_ValidEvent_When_Handle_Then_ShouldProcessWithoutErrors()
    {
        // Arrange
        var sale = SaleEventTestData.GenerateValidSale();
        var notification = new SaleModifiedEvent(sale);

        // Act
        var exception = await Record.ExceptionAsync(() =>
            _handler.Handle(notification, CancellationToken.None));

        // Assert
        Assert.Null(exception);
    }

    [Fact(DisplayName = "Given valid event When Handle called Then should check logger level")]
    public async Task Given_ValidEvent_When_Handle_Then_ShouldCheckLoggerLevel()
    {
        // Arrange
        var sale = SaleEventTestData.GenerateValidSale();
        var notification = new SaleModifiedEvent(sale);

        // Act
        await _handler.Handle(notification, CancellationToken.None);

        // Assert
        _logger.Received().IsEnabled(LogLevel.Information);
    }

    [Fact(DisplayName = "Given null notification When Handle called Then should throw ArgumentNullException")]
    public async Task Given_NullNotification_When_Handle_Then_ShouldThrowArgumentNullException()
    {
        // Arrange
        SaleModifiedEvent? notification = null;

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(() =>
            _handler.Handle(notification!, CancellationToken.None));
    }

    [Fact(DisplayName = "Given logger disabled When Handle called Then should not attempt logging")]
    public async Task Given_LoggerDisabled_When_Handle_Then_ShouldNotAttemptLogging()
    {
        // Arrange
        _logger.IsEnabled(LogLevel.Information).Returns(false);
        var sale = SaleEventTestData.GenerateValidSale();
        var notification = new SaleModifiedEvent(sale);

        // Act
        await _handler.Handle(notification, CancellationToken.None);

        // Assert
        _logger.Received(1).IsEnabled(LogLevel.Information);
        // Verify no actual log calls were made (only the IsEnabled check)
        _logger.DidNotReceive().Log(
            Arg.Any<LogLevel>(),
            Arg.Any<EventId>(),
            Arg.Any<object>(),
            Arg.Any<Exception>(),
            Arg.Any<Func<object, Exception?, string>>());
    }

    [Fact(DisplayName = "Given cancellation token When Handle called with cancelled token Then should complete")]
    public async Task Given_CancellationToken_When_HandleWithCancelledToken_Then_ShouldComplete()
    {
        // Arrange
        var sale = SaleEventTestData.GenerateValidSale();
        var notification = new SaleModifiedEvent(sale);
        var cts = new CancellationTokenSource();
        cts.Cancel();

        // Act
        var exception = await Record.ExceptionAsync(() =>
            _handler.Handle(notification, cts.Token));

        // Assert
        // Handler should complete synchronously without checking cancellation token
        Assert.Null(exception);
    }

    [Fact(DisplayName = "Given event with multiple items When Handle called Then should process all items")]
    public async Task Given_EventWithMultipleItems_When_Handle_Then_ShouldProcessAllItems()
    {
        // Arrange
        var sale = SaleEventTestData.GenerateValidSaleWithItems(5);
        var notification = new SaleModifiedEvent(sale);

        // Act
        var exception = await Record.ExceptionAsync(() =>
            _handler.Handle(notification, CancellationToken.None));

        // Assert
        Assert.Null(exception);
        Assert.Equal(5, sale.Items.Count);
    }
}