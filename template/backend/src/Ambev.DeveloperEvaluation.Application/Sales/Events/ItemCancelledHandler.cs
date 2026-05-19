using Ambev.DeveloperEvaluation.Domain.Events.Sales;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Ambev.DeveloperEvaluation.Application.Sales.Events;

/// <summary>
/// Handles <see cref="ItemCancelledEvent"/> by logging the item cancellation details.
/// </summary>
/// <remarks>
/// This handler serves as a demonstration of event processing.
/// In production, this could trigger:
/// - Partial inventory restoration
/// - Partial refund calculation
/// - Sales analytics updates
/// - Product availability updates
/// </remarks>
public partial class ItemCancelledHandler : INotificationHandler<ItemCancelledEvent>
{
    private readonly ILogger<ItemCancelledHandler> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="ItemCancelledHandler"/> class.
    /// </summary>
    /// <param name="logger">Logger instance for diagnostic output.</param>
    public ItemCancelledHandler(ILogger<ItemCancelledHandler> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Processes the item cancelled event.
    /// </summary>
    /// <param name="notification">The event containing the cancelled item information.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    public Task Handle(ItemCancelledEvent notification, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(notification);

        if (_logger.IsEnabled(LogLevel.Warning))
        {
            Log.ItemCancelled(
                _logger,
                notification.ItemId,
                notification.SaleId,
                notification.ProductName,
                notification.ProductId,
                notification.Quantity,
                notification.TotalAmount);
        }

        // In production, this is where you would:
        // - Restore inventory for the cancelled quantity
        // - Calculate and trigger partial refund
        // - Update product analytics (cancellation rate)
        // - Trigger customer notification for partial cancellation

        return Task.CompletedTask;
    }
}