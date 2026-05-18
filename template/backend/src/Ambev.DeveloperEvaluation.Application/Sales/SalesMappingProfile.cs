using Ambev.DeveloperEvaluation.Application.Sales.CreateSale;
using Ambev.DeveloperEvaluation.Application.Sales.GetSale;
using Ambev.DeveloperEvaluation.Domain.Entities.Sales;
using AutoMapper;

namespace Ambev.DeveloperEvaluation.Application.Sales;

public class SalesMappingProfile : Profile
{
    public SalesMappingProfile()
    {
        // Create Sale
        CreateMap<CreateSaleCommand, Sale>()
            .ConstructUsing(src => Sale.Create(
                src.SaleNumber, src.SaleDate, 
                src.CustomerId, src.CustomerName, 
                src.BranchId, src.BranchName,
                src.Items.Select(i => SaleItem.Create(i.ProductId, i.ProductName, i.Quantity, i.UnitPrice))
            ));
        
        CreateMap<Sale, CreateSaleResult>();

        // Get Sale
        CreateMap<Sale, GetSaleResult>();
        CreateMap<SaleItem, GetSaleItemResult>();
    }
}