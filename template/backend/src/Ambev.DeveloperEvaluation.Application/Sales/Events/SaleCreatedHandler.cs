using Ambev.DeveloperEvaluation.Domain.Events.Sales;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Ambev.DeveloperEvaluation.Application.Sales.Events;

/// <summary>
/// Handles the <see cref="SaleCreatedEvent"/> to perform side-effects
/// such as updating read models, sending notifications, or publishing to a message bus.
/// </summary>
public partial class SaleCreatedHandler(
    ILogger<SaleCreatedHandler> logger
) : INotificationHandler<SaleCreatedEvent>
{
    /// <summary>
    /// Processes the <see cref="SaleCreatedEvent"/> after successful persistence.
    /// </summary>
    /// <remarks>
    /// TODO: Integrate with external Message Bus (Rebus/RabbitMQ) if required.
    /// Example: await _messageBus.PublishAsync(notification, cancellationToken);
    /// </remarks>
    public Task Handle(SaleCreatedEvent notification, CancellationToken cancellationToken)
    {
        LogSaleCreated(
            notification.Sale.SaleNumber,
            notification.OccurredAt,
            notification.Sale.TotalAmount);

        return Task.CompletedTask;
    }
}