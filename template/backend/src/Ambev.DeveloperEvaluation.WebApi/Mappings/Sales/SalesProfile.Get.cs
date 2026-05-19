using Ambev.DeveloperEvaluation.Application.Sales.GetSale;
using Ambev.DeveloperEvaluation.WebApi.Features.Sales.GetSale;

namespace Ambev.DeveloperEvaluation.WebApi.Mappings.Sales;

public partial class SalesProfile
{
    private void ConfigureGetSalesMappings()
    {
        CreateMap<GetSaleResult, GetSaleResponse>();
        
        // Map Result Amount to Response Price to match contract names
        CreateMap<GetSaleItemResult, GetSaleItemResponse>()
            .ForMember(dest => dest.TotalPrice, opt => opt.MapFrom(src => src.TotalAmount));
    }
}