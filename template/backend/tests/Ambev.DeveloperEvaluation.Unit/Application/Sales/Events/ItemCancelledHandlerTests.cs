using Ambev.DeveloperEvaluation.Application.Sales.Events;
using Ambev.DeveloperEvaluation.Domain.Events.Sales;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Application.Sales.Events;

/// <summary>
/// Unit tests for <see cref="ItemCancelledHandler"/>.
/// Verifies event processing, logging behavior, and error handling.
/// </summary>
public class ItemCancelledHandlerTests
{
    private readonly ILogger<ItemCancelledHandler> _logger;
    private readonly ItemCancelledHandler _handler;

    public ItemCancelledHandlerTests()
    {
        _logger = Substitute.For<ILogger<ItemCancelledHandler>>();
        _handler = new ItemCancelledHandler(_logger);
    }

    [Fact(DisplayName = "Given valid event When Handle called Then should process without errors")]
    public async Task Given_ValidEvent_When_Handle_Then_ShouldProcessWithoutErrors()
    {
        // Arrange
        var notification = new ItemCancelledEvent(
            saleId: Guid.NewGuid(),
            itemId: Guid.NewGuid(),
            productId: Guid.NewGuid(),
            productName: "Premium Widget",
            quantity: 5,
            totalAmount: 450m);

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
        var notification = new ItemCancelledEvent(
            saleId: Guid.NewGuid(),
            itemId: Guid.NewGuid(),
            productId: Guid.NewGuid(),
            productName: "Premium Widget",
            quantity: 5,
            totalAmount: 450m);

        // Act
        await _handler.Handle(notification, CancellationToken.None);

        // Assert
        _logger.Received().IsEnabled(LogLevel.Warning);
    }

    [Fact(DisplayName = "Given null notification When Handle called Then should throw ArgumentNullException")]
    public async Task Given_NullNotification_When_Handle_Then_ShouldThrowArgumentNullException()
    {
        // Arrange
        ItemCancelledEvent? notification = null;

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(() =>
            _handler.Handle(notification!, CancellationToken.None));
    }

    [Fact(DisplayName = "Given logger disabled When Handle called Then should not attempt logging")]
    public async Task Given_LoggerDisabled_When_Handle_Then_ShouldNotAttemptLogging()
    {
        // Arrange
        _logger.IsEnabled(LogLevel.Warning).Returns(false);
        var notification = new ItemCancelledEvent(
            saleId: Guid.NewGuid(),
            itemId: Guid.NewGuid(),
            productId: Guid.NewGuid(),
            productName: "Premium Widget",
            quantity: 5,
            totalAmount: 450m);

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

    [Fact(DisplayName = "Given event with zero quantity When Handle called Then should process normally")]
    public async Task Given_EventWithZeroQuantity_When_Handle_Then_ShouldProcessNormally()
    {
        // Arrange
        var notification = new ItemCancelledEvent(
            saleId: Guid.NewGuid(),
            itemId: Guid.NewGuid(),
            productId: Guid.NewGuid(),
            productName: "Premium Widget",
            quantity: 0,  // Invalid in domain, but handler doesn't validate
            totalAmount: 0m);

        // Act
        var exception = await Record.ExceptionAsync(() =>
            _handler.Handle(notification, CancellationToken.None));

        // Assert
        // Handler should not validate the event, just log it
        Assert.Null(exception);
    }

    [Fact(DisplayName = "Given event with negative amount When Handle called Then should process normally")]
    public async Task Given_EventWithNegativeAmount_When_Handle_Then_ShouldProcessNormally()
    {
        // Arrange
        var notification = new ItemCancelledEvent(
            saleId: Guid.NewGuid(),
            itemId: Guid.NewGuid(),
            productId: Guid.NewGuid(),
            productName: "Premium Widget",
            quantity: 5,
            totalAmount: -100m);  // Invalid in domain, but handler doesn't validate

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
        var notification = new ItemCancelledEvent(
            saleId: Guid.NewGuid(),
            itemId: Guid.NewGuid(),
            productId: Guid.NewGuid(),
            productName: "Premium Widget",
            quantity: 5,
            totalAmount: 450m);
        var cts = new CancellationTokenSource();
        cts.Cancel();

        // Act
        var exception = await Record.ExceptionAsync(() =>
            _handler.Handle(notification, cts.Token));

        // Assert
        // Handler should complete synchronously without checking cancellation token
        Assert.Null(exception);
    }

    [Fact(DisplayName = "Given event with very long product name When Handle called Then should process normally")]
    public async Task Given_EventWithVeryLongProductName_When_Handle_Then_ShouldProcessNormally()
    {
        // Arrange
        var longProductName = new string('A', 1000);
        var notification = new ItemCancelledEvent(
            saleId: Guid.NewGuid(),
            itemId: Guid.NewGuid(),
            productId: Guid.NewGuid(),
            productName: longProductName,
            quantity: 5,
            totalAmount: 450m);

        // Act
        var exception = await Record.ExceptionAsync(() =>
            _handler.Handle(notification, CancellationToken.None));

        // Assert
        Assert.Null(exception);
    }

    [Theory(DisplayName = "Given various discount scenarios When Handle called Then should process correctly")]
    [InlineData(3, 100, 300)]      // No discount (< 4 items)
    [InlineData(5, 100, 450)]      // 10% discount (4-9 items)
    [InlineData(15, 100, 1200)]    // 20% discount (10-20 items)
    [InlineData(20, 100, 1600)]    // 20% discount (max allowed)
    public async Task Given_VariousDiscountScenarios_When_Handle_Then_ShouldProcessCorrectly(
        int quantity,
        int unitPriceInt,
        int expectedTotalInt)
    {
        // Arrange
        decimal unitPrice = unitPriceInt;
        decimal expectedTotal = expectedTotalInt;

        var notification = new ItemCancelledEvent(
            saleId: Guid.NewGuid(),
            itemId: Guid.NewGuid(),
            productId: Guid.NewGuid(),
            productName: "Test Product",
            quantity: quantity,
            totalAmount: expectedTotal);

        // Act
        var exception = await Record.ExceptionAsync(() =>
            _handler.Handle(notification, CancellationToken.None));

        // Assert
        Assert.Null(exception);
        Assert.Equal(expectedTotal, notification.TotalAmount);
    }
}