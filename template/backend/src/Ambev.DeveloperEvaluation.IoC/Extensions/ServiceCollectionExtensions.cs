using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Ambev.DeveloperEvaluation.IoC.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddAutoMapperConfiguration(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var autoMapperLicenseKey = configuration["AutoMapper:LicenseKey"];

        services.AddAutoMapper(
            cfg =>
            {
                cfg.LicenseKey = autoMapperLicenseKey;
            },
            typeof(Application.ApplicationLayer).Assembly
        );

        return services;
    }
}