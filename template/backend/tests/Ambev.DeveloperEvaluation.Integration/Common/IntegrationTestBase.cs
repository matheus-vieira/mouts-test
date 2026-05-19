using Ambev.DeveloperEvaluation.ORM;
using Microsoft.EntityFrameworkCore;
using Testcontainers.PostgreSql;
using Xunit;

namespace Ambev.DeveloperEvaluation.Integration.Common;

/// <summary>
/// Base class for integration tests using a real PostgreSQL database via Testcontainers.
/// Each test class gets its own PostgreSQL container for complete isolation.
/// Implements <see cref="IAsyncLifetime"/> for proper async initialization and disposal.
/// </summary>
/// <remarks>
/// <para>
/// <b>Lifecycle:</b>
/// <list type="bullet">
///   <item><see cref="InitializeAsync"/>: Starts PostgreSQL container, creates schema via EF Core.</item>
///   <item><see cref="DisposeAsync"/>: Disposes DbContext and stops the container.</item>
/// </list>
/// </para>
/// <para>
/// <b>Usage:</b> Inherit from this class in every integration test class.
/// Use <see cref="Context"/> for write operations and <see cref="CreateNewContext"/>
/// for read verifications to avoid EF Core tracking cache.
/// </para>
/// </remarks>
public abstract class IntegrationTestBase : IAsyncLifetime
{
    private readonly PostgreSqlContainer _postgresContainer = new PostgreSqlBuilder()
        .WithImage("postgres:16-alpine")
        .WithDatabase("integration_tests")
        .WithUsername("test_user")
        .WithPassword("test_password")
        .Build();

    /// <summary>
    /// EF Core DbContext connected to the PostgreSQL container.
    /// Available after <see cref="InitializeAsync"/> completes.
    /// Use this context for write operations (insert, update, delete).
    /// </summary>
    protected DefaultContext Context { get; private set; } = null!;

    /// <summary>
    /// Starts the PostgreSQL container and creates the database schema.
    /// Called automatically by xUnit before any test in the class runs.
    /// </summary>
    public async Task InitializeAsync()
    {
        await _postgresContainer.StartAsync();

        var options = new DbContextOptionsBuilder<DefaultContext>()
            .UseNpgsql(_postgresContainer.GetConnectionString())
            .EnableSensitiveDataLogging()
            .Options;

        Context = new DefaultContext(options);
        await Context.Database.EnsureCreatedAsync();
    }

    /// <summary>
    /// Creates a new DbContext instance connected to the same PostgreSQL container.
    /// Useful for verifying data persistence with a separate context (no tracking cache).
    /// </summary>
    /// <returns>A new <see cref="DefaultContext"/> instance without EF Core tracking state.</returns>
    protected DefaultContext CreateNewContext()
    {
        var options = new DbContextOptionsBuilder<DefaultContext>()
            .UseNpgsql(_postgresContainer.GetConnectionString())
            .Options;

        return new DefaultContext(options);
    }

    /// <summary>
    /// Disposes the DbContext and stops the PostgreSQL container.
    /// Called automatically by xUnit after all tests in the class complete.
    /// </summary>
    public async Task DisposeAsync()
    {
        await Context.DisposeAsync();
        await _postgresContainer.DisposeAsync();
    }
}