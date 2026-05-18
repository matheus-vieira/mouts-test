using Ambev.DeveloperEvaluation.Domain.Repositories.Sales;
using Ambev.DeveloperEvaluation.ORM.Repositories.Sales;
using Microsoft.Extensions.DependencyInjection;

namespace Ambev.DeveloperEvaluation.IoC.DependencyInjection.Sales;

/// <summary>
/// Extension method to register all Sale repository implementations.
/// Each interface is mapped to its own dedicated implementation,
/// following the Interface Segregation Principle.
/// </summary>
/// <remarks>
/// Usage in <c>InfrastructureModuleInitializer</c>:
/// <code>services.AddSalesRepositories();</code>
/// New aggregates should follow the same pattern:
/// <code>services.AddOrdersRepositories();</code>
/// </remarks>
public static class SaleRepositoryExtensions
{
    public static IServiceCollection AddSalesRepositories(this IServiceCollection services)
    {
        services.AddScoped<ISaleCreateRepository, SaleCreateRepository>();
        services.AddScoped<ISaleReadRepository, SaleReadRepository>();
        services.AddScoped<ISaleUpdateRepository, SaleUpdateRepository>();
        services.AddScoped<ISaleDeleteRepository, SaleDeleteRepository>();

        return services;
    }
}