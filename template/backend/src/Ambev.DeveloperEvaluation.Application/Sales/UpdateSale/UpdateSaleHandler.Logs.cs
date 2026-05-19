using Microsoft.Extensions.Logging;

namespace Ambev.DeveloperEvaluation.Application.Sales.UpdateSale;

public partial class UpdateSaleHandler
{
    [LoggerMessage(
        EventId = 2002,
        Level = LogLevel.Information,
        Message = "Update operation initiated for Sale {SaleId}.")]
    static partial void LogUpdateInitiated(ILogger logger, Guid saleId);

    [LoggerMessage(
        EventId = 2003,
        Level = LogLevel.Information,
        Message = "Sale {SaleId} successfully updated. Total items: {ItemCount}.")]
    static partial void LogUpdateSuccess(ILogger logger, Guid saleId, int itemCount);
}