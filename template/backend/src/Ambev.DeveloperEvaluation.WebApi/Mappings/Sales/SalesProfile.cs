using AutoMapper;

namespace Ambev.DeveloperEvaluation.WebApi.Mappings.Sales;

/// <summary>
/// AutoMapper profile for Sales feature. 
/// Organized as partial classes to separate Create, Get, Update and Delete mappings.
/// </summary>
public partial class SalesProfile : Profile
{
    public SalesProfile()
    {
        ConfigureCreateSalesMappings();
        ConfigureGetSalesMappings();
    }
}