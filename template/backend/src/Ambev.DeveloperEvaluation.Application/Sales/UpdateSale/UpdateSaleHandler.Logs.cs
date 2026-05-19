using Microsoft.Extensions.Logging;

namespace Ambev.DeveloperEvaluation.Application.Sales.UpdateSale;

public partial class UpdateSaleHandler
{
    [LoggerMessage(
        Level = LogLevel.Information,
        Message = "Initiating update for Sale ID: {SaleId}")]
    static partial void LogUpdateInitiated(ILogger logger, Guid saleId);

    [LoggerMessage(
        Level = LogLevel.Information,
        Message = "Sale {SaleId} successfully updated with {ItemCount} items")]
    static partial void LogUpdateSuccess(ILogger logger, Guid saleId, int itemCount);
}