using Xunit;

namespace Ambev.DeveloperEvaluation.Integration.Common;

/// <summary>
/// Dedicated test collection for Sale read/query tests.
/// Uses an isolated PostgreSQL schema to enable parallel test execution.
/// </summary>
[CollectionDefinition(Name)]
public class SaleReadTestsCollection : ICollectionFixture<SaleReadTestsFixture>
{
    /// <summary>
    /// Collection name constant for [Collection] attribute decoration.
    /// </summary>
    public const string Name = "Sale Read Tests";

    // This class serves as a collection definition only.
    // It associates the SaleReadTestsFixture with all test classes
    // decorated with [Collection(SaleReadTestsCollection.Name)].
}