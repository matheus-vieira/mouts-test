using Ambev.DeveloperEvaluation.IoC.Extensions;
using Microsoft.AspNetCore.Builder;

namespace Ambev.DeveloperEvaluation.IoC.ModuleInitializers;

public class AutoMapperModuleInitializer : IModuleInitializer
{
    public void Initialize(WebApplicationBuilder builder)
    {
        builder.Services.AddAutoMapperConfiguration(
            builder.Configuration);
    }
}