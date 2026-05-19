using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Ambev.DeveloperEvaluation.IoC.Extensions;

/// <summary>
/// Extension methods for configuring services in the IoC container.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Registers AutoMapper scanning the Application assembly plus any additional
    /// assemblies provided by the caller (e.g., WebAPI profiles).
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="configuration">The application configuration.</param>
    /// <param name="additionalAssemblies">
    /// Extra assemblies to scan for AutoMapper profiles.
    /// Pass <c>typeof(Program).Assembly</c> from the WebAPI entry point.
    /// </param>
    public static IServiceCollection AddAutoMapperConfiguration(
        this IServiceCollection services,
        IConfiguration configuration,
        params Assembly[] additionalAssemblies
)
    {
        var autoMapperLicenseKey = configuration["AutoMapper:LicenseKey"];

        var assemblies = new[] { typeof(Application.ApplicationLayer).Assembly }
            .Concat(additionalAssemblies)
            .ToArray();

        services.AddAutoMapper(
            cfg => { cfg.LicenseKey = autoMapperLicenseKey; },
            assemblies
        );

        return services;
    }
}