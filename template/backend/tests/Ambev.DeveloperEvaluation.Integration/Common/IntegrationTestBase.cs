using Ambev.DeveloperEvaluation.ORM;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Ambev.DeveloperEvaluation.Integration.Common;

/// <summary>
/// Base class for integration tests with shared PostgreSQL container.
/// 
/// Provides:
///   - Pre-configured <see cref="DefaultContext"/> connected to the shared container
///   - Automatic database cleanup between test classes via TRUNCATE CASCADE
///   - Helper method <see cref="CreateNewContext"/> for verifying persistence
/// 
/// Migration from old pattern:
///   BEFORE: class MyTests : IntegrationTestBase          (no constructor args)
///   AFTER:  class MyTests : IntegrationTestBase          (constructor receives fixture)
///           [Collection(DatabaseCollection.Name)]        (add collection attribute)
///           MyTests(PostgreSqlContainerFixture fixture)   (pass fixture to base)
///             : base(fixture) { }
/// </summary>
public abstract class IntegrationTestBase : IAsyncLifetime
{
    private readonly PostgreSqlContainerFixture _containerFixture;
    private IServiceScope _scope = null!;

    /// <summary>
    /// Gets the database context for the current test.
    /// Configured with sensitive data logging and detailed errors for test diagnostics.
    /// </summary>
    protected DefaultContext Context { get; private set; } = null!;

    /// <summary>
    /// Initializes a new instance with the shared container fixture.
    /// </summary>
    /// <param name="containerFixture">Shared PostgreSQL container instance injected by xUnit.</param>
    protected IntegrationTestBase(PostgreSqlContainerFixture containerFixture)
    {
        _containerFixture = containerFixture;
    }

    /// <summary>
    /// Initializes the database context and ensures schema is created.
    /// Called before each test class runs.
    /// 
    /// The first test class to run will create the schema via EnsureCreatedAsync().
    /// Subsequent test classes reuse the existing schema (EnsureCreatedAsync is a no-op
    /// if the database already exists).
    /// </summary>
    public async Task InitializeAsync()
    {
        var services = new ServiceCollection();

        services.AddDbContext<DefaultContext>(options =>
            options.UseNpgsql(_containerFixture.ConnectionString)
                   .EnableSensitiveDataLogging()
                   .EnableDetailedErrors());

        var provider = services.BuildServiceProvider();
        _scope = provider.CreateScope();
        Context = _scope.ServiceProvider.GetRequiredService<DefaultContext>();

        // Ensure database schema exists
        // First test class: creates tables, indexes, constraints
        // Subsequent test classes: no-op (schema already exists)
        await Context.Database.EnsureCreatedAsync();
    }

    /// <summary>
    /// Cleans up the database context after each test class.
    /// The container remains running for the next test class.
    /// Data is cleaned via TRUNCATE CASCADE but schema is preserved.
    /// </summary>
    public async Task DisposeAsync()
    {
        // Clean all data but keep schema for next test class
        await CleanDatabaseAsync();

        await Context.DisposeAsync();
        _scope.Dispose();
    }

    /// <summary>
    /// Creates a new DbContext instance with the same connection string.
    /// Use this to verify data persistence by bypassing EF Core's change tracker cache.
    /// 
    /// Example usage:
    /// <code>
    ///   await repository.CreateAsync(sale, ct);
    ///   using var freshContext = CreateNewContext();
    ///   var persisted = await freshContext.Sales.FindAsync(sale.Id);
    ///   persisted.Should().NotBeNull();
    /// </code>
    /// </summary>
    protected DefaultContext CreateNewContext()
    {
        var options = new DbContextOptionsBuilder<DefaultContext>()
            .UseNpgsql(_containerFixture.ConnectionString)
            .Options;
        return new DefaultContext(options);
    }

    /// <summary>
    /// Cleans all data from the database while preserving the schema.
    /// Executed after each test class to provide a clean slate for the next class.
    /// 
    /// Uses TRUNCATE CASCADE for performance (~100ms vs ~2-3s for drop/recreate).
    /// CASCADE ensures dependent tables (SaleItems) are also truncated.
    /// </summary>
    private async Task CleanDatabaseAsync()
    {
        // TRUNCATE is faster than DELETE — it doesn't scan rows, just deallocates pages
        // CASCADE handles dependent tables (SaleItems FK → Sales)
        await Context.Database.ExecuteSqlRawAsync("TRUNCATE TABLE \"Sales\" CASCADE");

        // If additional tables are added in the future, add them here:
        // await Context.Database.ExecuteSqlRawAsync("TRUNCATE TABLE \"OtherTable\" CASCADE");
    }
}