using System.Diagnostics;
using DotNet.Testcontainers.Builders;
using Testcontainers.PostgreSql;
using Xunit;

namespace Ambev.DeveloperEvaluation.Integration.Common;

/// <summary>
/// Shared PostgreSQL container fixture for all integration tests.
/// Container is started once at the beginning of test execution and reused across all test classes.
/// This significantly reduces test execution time by eliminating container startup overhead per class.
/// 
/// Usage:
///   - Registered via <see cref="DatabaseCollection"/> as an ICollectionFixture
///   - Injected into test classes via constructor parameter
///   - Provides ConnectionString for DbContext configuration
/// 
/// Lifecycle:
///   1. InitializeAsync() → Container starts (once, before any test)
///   2. Tests run → All test classes share the same container
///   3. DisposeAsync() → Container stops (once, after all tests complete)
/// </summary>
public class PostgreSqlContainerFixture : IAsyncLifetime
{
    private PostgreSqlContainer? _container;

    /// <summary>
    /// Gets the connection string to the running PostgreSQL container.
    /// </summary>
    /// <exception cref="InvalidOperationException">Thrown if container has not been initialized.</exception>
    public string ConnectionString => _container?.GetConnectionString()
        ?? throw new InvalidOperationException("Container not initialized. Ensure InitializeAsync() has been called.");

    /// <summary>
    /// Gets the running container instance.
    /// </summary>
    /// <exception cref="InvalidOperationException">Thrown if container has not been initialized.</exception>
    public PostgreSqlContainer Container => _container
        ?? throw new InvalidOperationException("Container not initialized. Ensure InitializeAsync() has been called.");

    /// <summary>
    /// Initializes and starts the PostgreSQL container.
    /// Called once before any test in the collection runs.
    /// Uses postgres:16-alpine for fast startup and small image size.
    /// </summary>
    public async Task InitializeAsync()
    {
        var sw = Stopwatch.StartNew();

        _container = new PostgreSqlBuilder()
            .WithImage("postgres:16-alpine")
            .WithDatabase("ambev_integration_test")
            .WithUsername("test_user")
            .WithPassword("test_pass")
            .WithCleanUp(true)
            .WithWaitStrategy(Wait.ForUnixContainer().UntilPortIsAvailable(5432))
            .Build();

        await _container.StartAsync();

        sw.Stop();
        Console.WriteLine($"✅ PostgreSQL container started in {sw.ElapsedMilliseconds}ms");
        Console.WriteLine($"   Connection: {ConnectionString}");
    }

    /// <summary>
    /// Stops and disposes the PostgreSQL container.
    /// Called once after all tests in the collection complete.
    /// </summary>
    public async Task DisposeAsync()
    {
        if (_container != null)
        {
            await _container.DisposeAsync();
            Console.WriteLine("🛑 PostgreSQL container disposed");
        }
    }
}