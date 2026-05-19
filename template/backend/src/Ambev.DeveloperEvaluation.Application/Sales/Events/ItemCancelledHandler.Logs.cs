using Ambev.DeveloperEvaluation.Domain.Events.Sales;
using Microsoft.Extensions.Logging;

namespace Ambev.DeveloperEvaluation.Application.Sales.Events;

public partial class ItemCancelledHandler
{
    /// <summary>
    /// Log message generators for item cancellation events.
    /// Uses source-generated logging for high performance.
    /// </summary>
    private static partial class Log
    {
        [LoggerMessage(
            EventId = 1,
            Level = LogLevel.Warning,
            Message = "Item {ItemId} cancelled from Sale {SaleId} | Product: '{ProductName}' (ID: {ProductId}) | Quantity: {Quantity} | Amount: {TotalAmount:C}")]
        public static partial void ItemCancelled(
            ILogger logger,
            Guid itemId,
            Guid saleId,
            string productName,
            Guid productId,
            int quantity,
            decimal totalAmount);
    }
}