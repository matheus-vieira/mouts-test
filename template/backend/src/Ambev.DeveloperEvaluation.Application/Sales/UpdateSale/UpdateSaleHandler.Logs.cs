using Microsoft.Extensions.Logging;

namespace Ambev.DeveloperEvaluation.Application.Sales.UpdateSale;

public partial class UpdateSaleHandler
{
    [LoggerMessage(
        EventId = 2002,
        Level = LogLevel.Information,
        Message = "Sale {SaleId} was updated. New Total Amount: {TotalAmount}")]
    static partial void LogSaleUpdated(
        ILogger logger,
        Guid saleId,
        decimal totalAmount
    );
}