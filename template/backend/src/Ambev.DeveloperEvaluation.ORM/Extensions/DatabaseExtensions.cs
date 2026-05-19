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

    public static WebApplication MigrateDatabase(this WebApplication app, int maxRetries = 5, int delaySeconds = 3)
    {
        using var scope = app.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<DefaultContext>();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<DefaultContext>>();

        for (int attempt = 1; attempt <= maxRetries; attempt++)
        {
            try
            {
                logger.LogInformation("Applying database migrations (attempt {Attempt}/{MaxRetries})...", attempt, maxRetries);
                context.Database.Migrate();
                logger.LogInformation("Database migrations applied successfully");
                return app;
            }
            catch (Exception ex) when (attempt < maxRetries)
            {
                logger.LogWarning(ex, "Migration attempt {Attempt} failed. Retrying in {Delay}s...", attempt, delaySeconds);
                Thread.Sleep(TimeSpan.FromSeconds(delaySeconds));
            }
        }

        // última tentativa — deixa lançar se falhar
        context.Database.Migrate();
        return app;
    }
}