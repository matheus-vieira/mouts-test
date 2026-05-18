using MediatR;

namespace Ambev.DeveloperEvaluation.Domain.Events.Sales;

/// <summary>
/// Domain event published when an existing sale is cancelled.
/// </summary>
public class SaleCancelledEvent : INotification
{
    public Guid SaleId { get; }
    public string SaleNumber { get; }
    public DateTime OccurredAt { get; }

    public SaleCancelledEvent(Guid saleId, string saleNumber)
    {
        SaleId = saleId;
        SaleNumber = saleNumber;
        OccurredAt = DateTime.UtcNow;
    }
}