using Ambev.DeveloperEvaluation.Domain.Entities.Sales;
using MediatR;

namespace Ambev.DeveloperEvaluation.Domain.Events.Sales;

/// <summary>
/// Domain event published when an existing <see cref="Sale"/> is modified.
/// </summary>
/// <remarks>
/// This event is triggered when sale properties (customer, branch, items) are updated.
/// Carries the modified sale aggregate for side-effects like audit logging,
/// notifications, or downstream system updates.
/// </remarks>
public class SaleModifiedEvent : INotification
{
    /// <summary>The modified sale aggregate containing updated state.</summary>
    public Sale Sale { get; }

    /// <summary>Timestamp when the modification occurred.</summary>
    public DateTime OccurredAt { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="SaleModifiedEvent"/> class.
    /// </summary>
    /// <param name="sale">The modified sale aggregate.</param>
    public SaleModifiedEvent(Sale sale)
    {
        Sale = sale ?? throw new ArgumentNullException(nameof(sale));
        OccurredAt = DateTime.UtcNow;
    }
}