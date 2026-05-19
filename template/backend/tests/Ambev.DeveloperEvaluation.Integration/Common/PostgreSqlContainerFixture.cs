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
///   - Can be inherited to create schema-specific fixtures for parallel execution
/// 
/// Lifecycle:
///   1. InitializeAsync() → Container starts (once, before any test)
///   2. Tests run → All test classes share the same container
///   3. DisposeAsync() → Container stops (once, after all tests complete)
/// 
/// Diagnostics:
///   - Container startup time
///   - Total fixture lifetime
///   - Phase-specific timing (build, start, wait)
/// </summary>
public class PostgreSqlContainerFixture : IAsyncLifetime
{
    private PostgreSqlContainer? _container;
    private readonly Stopwatch _totalTimer = new();
    private readonly Stopwatch _initTimer = new();

    /// <summary>
    /// Gets the connection string to the running PostgreSQL container.
    /// </summary>
    /// <exception cref="InvalidOperationException">Thrown if container has not been initialized.</exception>
    public string ConnectionString => _container?.GetConnectionString()
        ?? throw new InvalidOperationException("Container not initialized. Ensure InitializeAsync() has been called.");

    /// <summary>
    /// Gets the schema name for test isolation.
    /// Returns null for the default public schema.
    /// Derived fixtures can override this to provide dedicated schemas.
    /// </summary>
    public virtual string? Schema => null;

    /// <summary>
    /// Gets the running container instance.
    /// </summary>
    /// <exception cref="InvalidOperationException">Thrown if container has not been initialized.</exception>
    public PostgreSqlContainer Container => _container
        ?? throw new InvalidOperationException("Container not initialized. Ensure InitializeAsync() has been called.");

    /// <summary>
    ///  Creates a new NpgsqlConnection using the fixture's connection string.
    ///  Tests can use this to execute raw SQL commands against the container.
    /// </summary>
    /// <returns>A new <see cref="NpgsqlConnection"/> instance.</returns>
    public Npgsql.NpgsqlConnection CreateConnection() => new(ConnectionString);

    /// <summary>
    /// Initializes and starts the PostgreSQL container.
    /// Called once before any test in the collection runs.
    /// Uses postgres:16-alpine for fast startup and small image size.
    /// </summary>
    /// <remarks>
    /// Virtual to allow derived fixtures to add schema initialization or other setup logic.
    /// Always call base.InitializeAsync() first when overriding.
    /// </remarks>
    public virtual async Task InitializeAsync()
    {
        _totalTimer.Start();
        _initTimer.Start();

        Console.WriteLine("⏱️  PostgreSQL Container Initialization Started");
        Console.WriteLine("═══════════════════════════════════════════════");

        // Phase 1: Container Build
        var buildTimer = Stopwatch.StartNew();
        _container = new PostgreSqlBuilder()
            .WithImage("postgres:16-alpine")
            .WithDatabase("ambev_integration_test")
            .WithUsername("test_user")
            .WithPassword("test_pass")
            .WithCleanUp(true)
            .WithWaitStrategy(Wait.ForUnixContainer().UntilPortIsAvailable(5432))
            .Build();
        buildTimer.Stop();

        // Phase 2: Container Start
        var startTimer = Stopwatch.StartNew();
        await _container.StartAsync();
        startTimer.Stop();

        _initTimer.Stop();

        // Log detailed diagnostics
        Console.WriteLine($"✅ PostgreSQL container started successfully");
        Console.WriteLine($"   Image:            postgres:16-alpine");
        Console.WriteLine($"   Database:         ambev_integration_test");
        Console.WriteLine($"   Username:         test_user");
        Console.WriteLine($"   Connection:       {ConnectionString}");
        Console.WriteLine();
        Console.WriteLine($"📊 Timing Breakdown:");
        Console.WriteLine($"   Build phase:      {buildTimer.ElapsedMilliseconds}ms");
        Console.WriteLine($"   Start phase:      {startTimer.ElapsedMilliseconds}ms");
        Console.WriteLine($"   Total init time:  {_initTimer.ElapsedMilliseconds}ms");
        Console.WriteLine("═══════════════════════════════════════════════");
        Console.WriteLine();
    }

    /// <summary>
    /// Stops and disposes the PostgreSQL container.
    /// Called once after all tests in the collection complete.
    /// </summary>
    /// <remarks>
    /// Virtual to allow derived fixtures to add cleanup logic before disposal.
    /// Always call base.DisposeAsync() last when overriding.
    /// </remarks>
    public virtual async Task DisposeAsync()
    {
        _totalTimer.Stop();

        Console.WriteLine();
        Console.WriteLine("═══════════════════════════════════════════════");
        Console.WriteLine("⏱️  PostgreSQL Container Shutdown");
        Console.WriteLine("═══════════════════════════════════════════════");

        if (_container != null)
        {
            var disposeTimer = Stopwatch.StartNew();
            await _container.DisposeAsync();
            disposeTimer.Stop();

            Console.WriteLine($"🛑 PostgreSQL container disposed");
            Console.WriteLine();
            Console.WriteLine($"📊 Lifecycle Summary:");
            Console.WriteLine($"   Initialization:   {_initTimer.ElapsedMilliseconds}ms");
            Console.WriteLine($"   Total lifetime:   {_totalTimer.Elapsed.TotalSeconds:F2}s ({_totalTimer.ElapsedMilliseconds}ms)");
            Console.WriteLine($"   Disposal time:    {disposeTimer.ElapsedMilliseconds}ms");
            Console.WriteLine("═══════════════════════════════════════════════");
        }
    }
}