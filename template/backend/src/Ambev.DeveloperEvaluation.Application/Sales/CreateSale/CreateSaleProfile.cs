using Ambev.DeveloperEvaluation.Domain.Entities.Sales;
using AutoMapper;

namespace Ambev.DeveloperEvaluation.Application.Sales.CreateSale;

/// <summary>
/// AutoMapper profile for the create sale use case.
/// </summary>
/// <remarks>
/// Maps between the application layer DTOs and the domain entities.
///
/// Important: <see cref="SaleItem"/> and <see cref="Sale"/> are never
/// constructed directly by AutoMapper — domain factory methods
/// (<c>SaleItem.Create</c> and <c>Sale.Create</c>) are always used instead.
///
/// This profile is used only for the reverse mapping:
/// domain entity → result DTO, after the aggregate has been persisted.
/// </remarks>
public class CreateSaleProfile : Profile
{
    public CreateSaleProfile()
    {
        /// Maps the persisted <see cref="Sale"/> aggregate to the response DTO.
        /// Only scalar properties are mapped — items are excluded from this result.
        CreateMap<Sale, CreateSaleResult>();
    }
}