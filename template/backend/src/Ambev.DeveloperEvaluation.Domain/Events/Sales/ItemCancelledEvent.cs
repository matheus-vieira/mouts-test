using MediatR;

namespace Ambev.DeveloperEvaluation.Domain.Events.Sales;

/// <summary>
/// Domain event published when a sale item is cancelled.
/// </summary>
/// <remarks>
/// This event tracks individual item cancellations within a sale,
/// enabling granular audit trails and business logic triggers
/// (e.g., inventory restoration, partial refunds).
/// </remarks>
public class ItemCancelledEvent : INotification
{
    /// <summary>The ID of the parent sale containing the cancelled item.</summary>
    public Guid SaleId { get; }

    /// <summary>The ID of the cancelled item.</summary>
    public Guid ItemId { get; }

    /// <summary>The product ID of the cancelled item.</summary>
    public Guid ProductId { get; }

    /// <summary>The product name of the cancelled item.</summary>
    public string ProductName { get; }

    /// <summary>The quantity of the cancelled item.</summary>
    public int Quantity { get; }

    /// <summary>The total amount of the cancelled item.</summary>
    public decimal TotalAmount { get; }

    /// <summary>Timestamp when the item was cancelled.</summary>
    public DateTime OccurredAt { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="ItemCancelledEvent"/> class.
    /// </summary>
    /// <param name="saleId">The ID of the parent sale.</param>
    /// <param name="itemId">The ID of the cancelled item.</param>
    /// <param name="productId">The product ID.</param>
    /// <param name="productName">The product name.</param>
    /// <param name="quantity">The quantity cancelled.</param>
    /// <param name="totalAmount">The total amount of the cancelled item.</param>
    public ItemCancelledEvent(
        Guid saleId,
        Guid itemId,
        Guid productId,
        string productName,
        int quantity,
        decimal totalAmount)
    {
        SaleId = saleId;
        ItemId = itemId;
        ProductId = productId;
        ProductName = productName ?? throw new ArgumentNullException(nameof(productName));
        Quantity = quantity;
        TotalAmount = totalAmount;
        OccurredAt = DateTime.UtcNow;
    }
}