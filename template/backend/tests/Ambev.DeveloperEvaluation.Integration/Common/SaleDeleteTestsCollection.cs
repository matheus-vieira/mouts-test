using Xunit;

namespace Ambev.DeveloperEvaluation.Integration.Common;

/// <summary>
/// Dedicated test collection for Sale delete tests.
/// Uses an isolated PostgreSQL schema to enable parallel test execution.
/// </summary>
[CollectionDefinition(Name)]
public class SaleDeleteTestsCollection : ICollectionFixture<SaleDeleteTestsFixture>
{
    /// <summary>
    /// Collection name constant for [Collection] attribute decoration.
    /// </summary>
    public const string Name = "Sale Delete Tests";

    // This class serves as a collection definition only.
    // It associates the SaleDeleteTestsFixture with all test classes
    // decorated with [Collection(SaleDeleteTestsCollection.Name)].
}