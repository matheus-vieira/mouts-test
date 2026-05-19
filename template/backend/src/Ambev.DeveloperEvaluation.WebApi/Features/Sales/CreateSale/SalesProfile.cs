using Ambev.DeveloperEvaluation.Application.Sales.CreateSale;
using Ambev.DeveloperEvaluation.WebApi.Features.Sales.CreateSale;
using AutoMapper;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales;

/// <summary>
/// AutoMapper profile for Web API models in the Sales feature.
/// Handles mapping between API Request/Response DTOs and Application Commands/Results.
/// </summary>
public class SalesProfile : Profile
{
    public SalesProfile()
    {
        // Create Sale Mapping
        CreateMap<CreateSaleRequest, CreateSaleCommand>();
        CreateMap<CreateSaleItemRequest, CreateSaleItemCommand>();
        CreateMap<CreateSaleResult, CreateSaleResponse>();
    }
}