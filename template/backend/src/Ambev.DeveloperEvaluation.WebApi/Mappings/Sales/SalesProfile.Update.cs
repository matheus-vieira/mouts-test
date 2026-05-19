using Ambev.DeveloperEvaluation.Application.Sales.UpdateSale;
using Ambev.DeveloperEvaluation.WebApi.Features.Sales.UpdateSale;

namespace Ambev.DeveloperEvaluation.WebApi.Mappings.Sales;

public partial class SalesProfile
{
    /// <summary>
    /// Configures mappings for the UpdateSale feature.
    /// Maps from WebAPI Request to Application Command and from Application Result to WebAPI Response.
    /// </summary>
    private void ConfigureUpdateSalesMappings()
    {
        // WebAPI Request → Application Command
        CreateMap<UpdateSaleRequest, UpdateSaleCommand>();
        CreateMap<UpdateSaleItemRequest, UpdateSaleItemCommand>();

        // Application Result → WebAPI Response
        CreateMap<UpdateSaleResult, UpdateSaleResponse>();
        CreateMap<UpdateSaleItemResult, UpdateSaleItemResponse>();
    }
}