using Microsoft.Extensions.Logging;

namespace Ambev.DeveloperEvaluation.Application.Sales.Events;

public partial class SaleCreatedHandler
{
    [LoggerMessage(
        EventId = 1001,
        Level = LogLevel.Information,
        Message = "Domain Event: Sale {SaleNumber} was created at {OccurredAt}. Total Amount: {TotalAmount}")]
    static partial void LogSaleCreated(string saleNumber, DateTime occurredAt, decimal totalAmount);
}