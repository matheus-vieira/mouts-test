using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Ambev.DeveloperEvaluation.ORM.Extensions;

public static class DatabaseExtensions
{
    public static IServiceCollection AddDatabaseContext(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddDbContext<DefaultContext>(options =>
            options.UseNpgsql(
                configuration.GetConnectionString("DefaultConnection"),
                b => b.MigrationsAssembly("Ambev.DeveloperEvaluation.ORM")
            )
        );

        return services;
    }

    public static WebApplication MigrateDatabase(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();

        var context = scope.ServiceProvider.GetRequiredService<DefaultContext>();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<DefaultContext>>();

        try
        {
            logger.LogInformation("Applying database migrations...");
            context.Database.Migrate();
            logger.LogInformation("Database migrations applied successfully");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while applying database migrations");
            throw;
        }

        return app;
    }
}