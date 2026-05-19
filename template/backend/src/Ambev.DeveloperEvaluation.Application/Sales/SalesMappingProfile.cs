using Ambev.DeveloperEvaluation.Application.Sales.CreateSale;
using Ambev.DeveloperEvaluation.Application.Sales.GetSale;
using Ambev.DeveloperEvaluation.Application.Sales.UpdateSale;
using Ambev.DeveloperEvaluation.Domain.Entities.Sales;
using AutoMapper;

namespace Ambev.DeveloperEvaluation.Application.Sales;

/// <summary>
/// Mapping profile for Sales-related conversions.
/// Handles commands to entities and entities to results.
/// </summary>
public class SalesMappingProfile : Profile
{
    public SalesMappingProfile()
    {
        // CreateSale
        CreateMap<CreateSaleCommand, Sale>()
            .ConstructUsing(src => Sale.Create(
                src.SaleNumber, 
                src.SaleDate, 
                src.CustomerId, 
                src.CustomerName, 
                src.BranchId, 
                src.BranchName,
                src.Items.Select(i => SaleItem.Create(i.ProductId, i.ProductName, i.Quantity, i.UnitPrice))
            ));

        CreateMap<CreateSaleItemCommand, SaleItem>();
        CreateMap<Sale, CreateSaleResult>();

        // GetSale
        CreateMap<Sale, GetSaleResult>();
        CreateMap<SaleItem, GetSaleItemResult>();

        // UpdateSale
        CreateMap<Sale, UpdateSaleResult>();
        CreateMap<SaleItem, UpdateSaleItemResult>();
    }
}