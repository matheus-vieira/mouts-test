using Xunit;

namespace Ambev.DeveloperEvaluation.Integration.Common;

/// <summary>
/// Dedicated test collection for Sale creation tests.
/// Uses an isolated PostgreSQL schema to enable parallel test execution.
/// </summary>
[CollectionDefinition(Name)]
public class SaleCreateTestsCollection : ICollectionFixture<SaleCreateTestsFixture>
{
    /// <summary>
    /// Collection name constant for [Collection] attribute decoration.
    /// </summary>
    public const string Name = "Sale Create Tests";

    // This class serves as a collection definition only.
    // It associates the SaleCreateTestsFixture with all test classes
    // decorated with [Collection(SaleCreateTestsCollection.Name)].
}