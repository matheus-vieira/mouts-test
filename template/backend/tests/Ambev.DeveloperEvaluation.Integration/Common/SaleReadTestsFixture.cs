using System.Diagnostics;

namespace Ambev.DeveloperEvaluation.Integration.Common;

/// <summary>
/// Fixture for Sale read tests that provides an isolated PostgreSQL schema.
/// Inherits container management from <see cref="PostgreSqlContainerFixture"/>
/// and adds schema-specific initialization.
/// 
/// Diagnostics:
///   - Schema creation time
///   - Schema cleanup time
///   - Total schema lifecycle
/// </summary>
public class SaleReadTestsFixture : PostgreSqlContainerFixture
{
    private const string SchemaName = "sale_read_tests";
    private readonly Stopwatch _schemaLifetimeTimer = new();

    /// <summary>
    /// Gets the schema name used for test isolation.
    /// </summary>
    public string Schema => SchemaName;

    /// <summary>
    /// Initializes the container and creates the dedicated schema.
    /// </summary>
    public override async Task InitializeAsync()
    {
        // Initialize the shared container (with detailed timing from base)
        await base.InitializeAsync();

        _schemaLifetimeTimer.Start();

        // Create dedicated schema for this collection
        var schemaTimer = Stopwatch.StartNew();

        await using var connection = CreateConnection();
        await connection.OpenAsync();
        await using var command = connection.CreateCommand();
        command.CommandText = $"CREATE SCHEMA IF NOT EXISTS {SchemaName};";
        await command.ExecuteNonQueryAsync();

        schemaTimer.Stop();

        Console.WriteLine($"📦 Schema '{SchemaName}' created in {schemaTimer.ElapsedMilliseconds}ms");
        Console.WriteLine();
    }

    /// <summary>
    /// Cleans up the schema before disposing the container.
    /// </summary>
    public override async Task DisposeAsync()
    {
        _schemaLifetimeTimer.Stop();

        Console.WriteLine();
        Console.WriteLine($"📦 Schema '{SchemaName}' Cleanup");
        Console.WriteLine("───────────────────────────────────────────────");

        var cleanupTimer = Stopwatch.StartNew();

        try
        {
            await using var connection = CreateConnection();
            await connection.OpenAsync();
            await using var command = connection.CreateCommand();
            command.CommandText = $"DROP SCHEMA IF EXISTS {SchemaName} CASCADE;";
            await command.ExecuteNonQueryAsync();

            cleanupTimer.Stop();

            Console.WriteLine($"   ✓ Schema dropped successfully");
            Console.WriteLine($"   Cleanup time:     {cleanupTimer.ElapsedMilliseconds}ms");
            Console.WriteLine($"   Schema lifetime:  {_schemaLifetimeTimer.Elapsed.TotalSeconds:F2}s");
        }
        catch (Exception ex)
        {
            cleanupTimer.Stop();

            Console.WriteLine($"   ✗ Schema cleanup failed");
            Console.WriteLine($"   Error:            {ex.Message}");
            Console.WriteLine($"   Cleanup attempt:  {cleanupTimer.ElapsedMilliseconds}ms");
        }

        Console.WriteLine("───────────────────────────────────────────────");

        await base.DisposeAsync();
    }
}