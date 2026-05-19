using Ambev.DeveloperEvaluation.Domain.Events.Sales;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Ambev.DeveloperEvaluation.Application.Sales.Events;

/// <summary>
/// Handles <see cref="SaleModifiedEvent"/> by logging the modification details.
/// </summary>
/// <remarks>
/// This handler serves as a demonstration of event processing.
/// In production, this could trigger:
/// - Audit trail updates
/// - Downstream system notifications
/// - Analytics event publishing
/// - Cache invalidation
/// </remarks>
public partial class SaleModifiedHandler : INotificationHandler<SaleModifiedEvent>
{
    private readonly ILogger<SaleModifiedHandler> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="SaleModifiedHandler"/> class.
    /// </summary>
    /// <param name="logger">Logger instance for diagnostic output.</param>
    public SaleModifiedHandler(ILogger<SaleModifiedHandler> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Processes the sale modified event.
    /// </summary>
    /// <param name="notification">The event containing the modified sale.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    public Task Handle(SaleModifiedEvent notification, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(notification);

        if (_logger.IsEnabled(LogLevel.Information))
        {
            Log.SaleModified(
                _logger,
                notification.Sale.Id,
                notification.Sale.SaleNumber,
                notification.Sale.CustomerName,
                notification.Sale.BranchName,
                notification.Sale.Items.Count,
                notification.Sale.TotalAmount);
        }

        // In production, this is where you would:
        // - Publish to message broker (RabbitMQ, Kafka, Azure Service Bus)
        // - Update audit log tables
        // - Invalidate caches
        // - Trigger downstream workflows

        return Task.CompletedTask;
    }
}