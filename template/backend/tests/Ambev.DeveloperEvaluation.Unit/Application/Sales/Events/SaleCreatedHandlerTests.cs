// tests/Ambev.DeveloperEvaluation.Unit/Application/Sales/Events/SaleCreatedHandlerTests.cs
using Ambev.DeveloperEvaluation.Application.Sales.Events;
using Ambev.DeveloperEvaluation.Domain.Events.Sales;
using Ambev.DeveloperEvaluation.Unit.Application.TestData;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Application.Sales.Events;

public class SaleCreatedHandlerTests
{
    private readonly ILogger<SaleCreatedHandler> _logger;
    private readonly SaleCreatedHandler _handler;

    public SaleCreatedHandlerTests()
    {
        _logger = Substitute.For<ILogger<SaleCreatedHandler>>();
        _handler = new SaleCreatedHandler(_logger);
    }

    [Fact(DisplayName = "Should process event without errors")]
    public async Task Given_Event_When_Handle_Then_ShouldNotThrow()
    {
        // Arrange
        var sale = SaleEventTestData.GenerateValidSale();
        var notification = new SaleCreatedEvent(sale);

        // Act
        var exception = await Record.ExceptionAsync(() =>
            _handler.Handle(notification, CancellationToken.None));

        // Assert
        Assert.Null(exception);
    }

    [Fact(DisplayName = "Should check logger level when handling event")]
    public async Task Given_Event_When_Handle_Then_ShouldCheckLoggerLevel()
    {
        // Arrange
        var sale = SaleEventTestData.GenerateValidSale();
        var notification = new SaleCreatedEvent(sale);

        // Act
        await _handler.Handle(notification, CancellationToken.None);

        // Assert
        _logger.Received().IsEnabled(LogLevel.Information);
    }
}