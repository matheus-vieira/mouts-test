using Ambev.DeveloperEvaluation.Domain.Entities.Sales;
using MediatR;

namespace Ambev.DeveloperEvaluation.Domain.Events.Sales;

/// <summary>
/// Domain event published when a new <see cref="Sale"/> is successfully created.
/// </summary>
/// <remarks>
/// This event carries the minimal state required for side-effects like
/// sending notifications, updating inventory, or logging audit trails.
/// </remarks>
public class SaleCreatedEvent : INotification
{
    /// <summary>The newly created sale aggregate.</summary>
    public Sale Sale { get; }

    /// <summary>Timestamp when the event occurred.</summary>
    public DateTime OccurredAt { get; }

    public SaleCreatedEvent(Sale sale)
    {
        Sale = sale;
        OccurredAt = DateTime.UtcNow;
    }
}