using Ambev.DeveloperEvaluation.Domain.Events.Sales;
using Microsoft.Extensions.Logging;

namespace Ambev.DeveloperEvaluation.Application.Sales.Events;

public partial class SaleCancelledHandler
{
    /// <summary>
    /// Log message generators for sale cancellation events.
    /// Uses source-generated logging for high performance.
    /// </summary>
    private static partial class Log
    {
        [LoggerMessage(
            EventId = 1,
            Level = LogLevel.Warning,
            Message = "Sale {SaleId} '{SaleNumber}' was cancelled | Occurred at: {OccurredAt:u}")]
        public static partial void SaleCancelled(
            ILogger logger,
            Guid saleId,
            string saleNumber,
            DateTime occurredAt);
    }
}