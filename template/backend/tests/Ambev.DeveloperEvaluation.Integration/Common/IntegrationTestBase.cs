using Ambev.DeveloperEvaluation.ORM;
using Microsoft.EntityFrameworkCore;

namespace Ambev.DeveloperEvaluation.Integration.Common;

/// <summary>
/// Base class for all integration tests that need database access.
/// Provides shared DbContext configuration and helper methods.
/// </summary>
/// <remarks>
/// Inherit from this class in test classes that are decorated with [Collection(DatabaseCollection.Name)].
/// The fixture is injected via the constructor and provides the connection string for DbContext creation.
/// 
/// For schema-isolated tests, derived fixtures can override <see cref="GetSchemaName"/> to return
/// a dedicated schema name, enabling parallel test execution.
/// </remarks>
public abstract class IntegrationTestBase : IDisposable
{
    private readonly PostgreSqlContainerFixture _fixture;
    private DefaultContext? _context;

    /// <summary>
    /// Gets the DbContext for the current test.
    /// Lazily initialized on first access.
    /// </summary>
    protected DefaultContext Context => _context ??= CreateNewContext();

    /// <summary>
    /// Initializes a new instance of <see cref="IntegrationTestBase"/>.
    /// </summary>
    /// <param name="fixture">The PostgreSQL container fixture providing the connection string.</param>
    protected IntegrationTestBase(PostgreSqlContainerFixture fixture)
    {
        _fixture = fixture ?? throw new ArgumentNullException(nameof(fixture));
    }

    /// <summary>
    /// Creates a new DbContext instance configured for the test database.
    /// Each call creates a fresh context to avoid tracking conflicts.
    /// </summary>
    /// <returns>A new <see cref="DefaultContext"/> connected to the test database.</returns>
    protected DefaultContext CreateNewContext()
    {
        var optionsBuilder = new DbContextOptionsBuilder<DefaultContext>();
        optionsBuilder.UseNpgsql(_fixture.ConnectionString);

        var context = new DefaultContext(optionsBuilder.Options);

        // Apply schema if specified by derived fixture
        var schema = GetSchemaName();
        if (!string.IsNullOrEmpty(schema))
        {
            context.Database.ExecuteSqlRaw($"SET search_path TO {schema};");
        }

        // Ensure database and tables are created for the current schema
        context.Database.EnsureCreated();

        return context;
    }

    /// <summary>
    /// Gets the schema name for test isolation.
    /// Override in derived classes or provide via fixture to use dedicated schemas.
    /// </summary>
    /// <returns>The schema name, or null/empty for the default public schema.</returns>
    protected virtual string? GetSchemaName()
    {
        return _fixture.Schema;
    }

    /// <summary>
    /// Disposes the DbContext if it was created.
    /// Called automatically after each test completes.
    /// </summary>
    public void Dispose()
    {
        _context?.Dispose();
        GC.SuppressFinalize(this);
    }
}