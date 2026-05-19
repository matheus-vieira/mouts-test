using Xunit;

namespace Ambev.DeveloperEvaluation.Integration.Common;

/// <summary>
/// Defines a test collection that shares a single PostgreSQL container across all test classes.
/// 
/// All test classes decorated with <c>[Collection(DatabaseCollection.Name)]</c> will:
///   1. Share the same <see cref="PostgreSqlContainerFixture"/> instance
///   2. Receive the fixture via constructor injection
///   3. Run sequentially within the collection (xUnit default for collections)
/// 
/// This pattern eliminates the overhead of creating a new container per test class,
/// reducing total test execution time from ~110s to ~25-30s.
/// </summary>
[CollectionDefinition(Name)]
public class DatabaseCollection : ICollectionFixture<PostgreSqlContainerFixture>
{
    /// <summary>
    /// Collection name constant. Use this in [Collection] attributes on test classes.
    /// </summary>
    public const string Name = "Database Collection";

    // This class is never instantiated. Its sole purpose is to:
    //   1. Define the collection name via [CollectionDefinition]
    //   2. Associate the fixture type via ICollectionFixture<T>
}