using Ambev.DeveloperEvaluation.Application.Sales.CreateSale;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Ambev.DeveloperEvaluation.IoC.ModuleInitializers;

public class AutoMapperModuleInitializer : IModuleInitializer
{
    public void Initialize(WebApplicationBuilder builder)
    {
        var autoMapperLicenseKey = builder.Configuration["AutoMapper:LicenseKey"];
        builder.Services.AddAutoMapper(cfg => cfg.LicenseKey = autoMapperLicenseKey, 
            typeof(CreateSaleCommand).Assembly);
    }
}