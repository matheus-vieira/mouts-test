using Xunit;

namespace Ambev.DeveloperEvaluation.Integration.Common;

/// <summary>
/// Dedicated test collection for Sale update tests.
/// Uses an isolated PostgreSQL schema to enable parallel test execution.
/// </summary>
[CollectionDefinition(Name)]
public class SaleUpdateTestsCollection : ICollectionFixture<SaleUpdateTestsFixture>
{
    /// <summary>
    /// Collection name constant for [Collection] attribute decoration.
    /// </summary>
    public const string Name = "Sale Update Tests";

    // This class serves as a collection definition only.
    // It associates the SaleUpdateTestsFixture with all test classes
    // decorated with [Collection(SaleUpdateTestsCollection.Name)].
}