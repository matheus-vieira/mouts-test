using Ambev.DeveloperEvaluation.Application.Sales.Events;
using Ambev.DeveloperEvaluation.Domain.Events.Sales;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Application.Sales.Events;

/// <summary>
/// Unit tests for <see cref="SaleCancelledHandler"/>.
/// Verifies event processing, logging behavior, and error handling.
/// </summary>
public class SaleCancelledHandlerTests
{
    private readonly ILogger<SaleCancelledHandler> _logger;
    private readonly SaleCancelledHandler _handler;

    public SaleCancelledHandlerTests()
    {
        _logger = Substitute.For<ILogger<SaleCancelledHandler>>();
        _handler = new SaleCancelledHandler(_logger);
    }

    [Fact(DisplayName = "Given valid event When Handle called Then should process without errors")]
    public async Task Given_ValidEvent_When_Handle_Then_ShouldProcessWithoutErrors()
    {
        // Arrange
        var saleId = Guid.NewGuid();
        var saleNumber = "SALE-20240119-143025123";
        var notification = new SaleCancelledEvent(saleId, saleNumber);

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
        var saleId = Guid.NewGuid();
        var saleNumber = "SALE-20240119-143025123";
        var notification = new SaleCancelledEvent(saleId, saleNumber);

        // Act
        await _handler.Handle(notification, CancellationToken.None);

        // Assert
        _logger.Received().IsEnabled(LogLevel.Warning);
    }

    [Fact(DisplayName = "Given null notification When Handle called Then should throw ArgumentNullException")]
    public async Task Given_NullNotification_When_Handle_Then_ShouldThrowArgumentNullException()
    {
        // Arrange
        SaleCancelledEvent? notification = null;

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(() =>
            _handler.Handle(notification!, CancellationToken.None));
    }

    [Fact(DisplayName = "Given logger disabled When Handle called Then should not attempt logging")]
    public async Task Given_LoggerDisabled_When_Handle_Then_ShouldNotAttemptLogging()
    {
        // Arrange
        _logger.IsEnabled(LogLevel.Warning).Returns(false);
        var saleId = Guid.NewGuid();
        var saleNumber = "SALE-20240119-143025123";
        var notification = new SaleCancelledEvent(saleId, saleNumber);

        // Act
        await _handler.Handle(notification, CancellationToken.None);

        // Assert
        _logger.Received(1).IsEnabled(LogLevel.Warning);
        // Verify no actual log calls were made (only the IsEnabled check)
        _logger.DidNotReceive().Log(
            Arg.Any<LogLevel>(),
            Arg.Any<EventId>(),
            Arg.Any<object>(),
            Arg.Any<Exception>(),
            Arg.Any<Func<object, Exception?, string>>());
    }

    [Fact(DisplayName = "Given event with empty Guid When Handle called Then should process normally")]
    public async Task Given_EventWithEmptyGuid_When_Handle_Then_ShouldProcessNormally()
    {
        // Arrange
        var saleId = Guid.Empty;
        var saleNumber = "SALE-20240119-143025123";
        var notification = new SaleCancelledEvent(saleId, saleNumber);

        // Act
        var exception = await Record.ExceptionAsync(() =>
            _handler.Handle(notification, CancellationToken.None));

        // Assert
        // Handler should not validate the event, just log it
        Assert.Null(exception);
    }

    [Fact(DisplayName = "Given cancellation token When Handle called with cancelled token Then should complete")]
    public async Task Given_CancellationToken_When_HandleWithCancelledToken_Then_ShouldComplete()
    {
        // Arrange
        var saleId = Guid.NewGuid();
        var saleNumber = "SALE-20240119-143025123";
        var notification = new SaleCancelledEvent(saleId, saleNumber);
        var cts = new CancellationTokenSource();
        cts.Cancel();

        // Act
        var exception = await Record.ExceptionAsync(() =>
            _handler.Handle(notification, cts.Token));

        // Assert
        // Handler should complete synchronously without checking cancellation token
        Assert.Null(exception);
    }

    [Fact(DisplayName = "Given event with occurred timestamp When Handle called Then should preserve timestamp")]
    public async Task Given_EventWithOccurredTimestamp_When_Handle_Then_ShouldPreserveTimestamp()
    {
        // Arrange
        var saleId = Guid.NewGuid();
        var saleNumber = "SALE-20240119-143025123";
        var notification = new SaleCancelledEvent(saleId, saleNumber);
        var beforeCall = DateTime.UtcNow;

        // Act
        await _handler.Handle(notification, CancellationToken.None);
        var afterCall = DateTime.UtcNow;

        // Assert
        Assert.InRange(notification.OccurredAt, beforeCall.AddSeconds(-1), afterCall);
    }
}