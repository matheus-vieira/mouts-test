using Microsoft.Extensions.Logging;

namespace Ambev.DeveloperEvaluation.Application.Sales.CancelSale;

public partial class CancelSaleHandler
{
    [LoggerMessage(
        EventId = 2001,
        Level = LogLevel.Information,
        Message = "Sale {SaleId} (Number: {SaleNumber}) has been successfully cancelled.")]
    static partial void LogSaleCancelled(
        ILogger logger,
        Guid saleId,
        string saleNumber
    );
}