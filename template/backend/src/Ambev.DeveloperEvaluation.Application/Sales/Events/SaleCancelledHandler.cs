using Ambev.DeveloperEvaluation.Domain.Events.Sales;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Ambev.DeveloperEvaluation.Application.Sales.Events;

/// <summary>
/// Handles <see cref="SaleCancelledEvent"/> by logging the cancellation details.
/// </summary>
/// <remarks>
/// This handler serves as a demonstration of event processing.
/// In production, this could trigger:
/// - Inventory restoration
/// - Refund processing
/// - Customer notifications
/// - Analytics event publishing
/// - Fraud detection workflows
/// </remarks>
public partial class SaleCancelledHandler : INotificationHandler<SaleCancelledEvent>
{
    private readonly ILogger<SaleCancelledHandler> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="SaleCancelledHandler"/> class.
    /// </summary>
    /// <param name="logger">Logger instance for diagnostic output.</param>
    public SaleCancelledHandler(ILogger<SaleCancelledHandler> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Processes the sale cancelled event.
    /// </summary>
    /// <param name="notification">The event containing the cancelled sale information.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    public Task Handle(SaleCancelledEvent notification, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(notification);

        if (_logger.IsEnabled(LogLevel.Warning))
        {
            Log.SaleCancelled(
                _logger,
                notification.SaleId,
                notification.SaleNumber,
                notification.OccurredAt);
        }

        // In production, this is where you would:
        // - Publish to message broker for refund processing
        // - Trigger inventory restoration workflows
        // - Send customer cancellation notifications
        // - Update analytics dashboards
        // - Log for fraud detection analysis

        return Task.CompletedTask;
    }
}