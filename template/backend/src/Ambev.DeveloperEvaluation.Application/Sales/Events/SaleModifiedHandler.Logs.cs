using Ambev.DeveloperEvaluation.Domain.Events.Sales;
using Microsoft.Extensions.Logging;

namespace Ambev.DeveloperEvaluation.Application.Sales.Events;

public partial class SaleModifiedHandler
{
    /// <summary>
    /// Log message generators for sale modification events.
    /// Uses source-generated logging for high performance.
    /// </summary>
    private static partial class Log
    {
        [LoggerMessage(
            EventId = 1,
            Level = LogLevel.Information,
            Message = "Sale {SaleId} '{SaleNumber}' was modified | Customer: {CustomerName} | Branch: {BranchName} | Items: {ItemCount} | Total: {TotalAmount:C}")]
        public static partial void SaleModified(
            ILogger logger,
            Guid saleId,
            string saleNumber,
            string customerName,
            string branchName,
            int itemCount,
            decimal totalAmount);
    }
}