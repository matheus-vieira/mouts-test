using Ambev.DeveloperEvaluation.Domain.Entities.Sales;
using Bogus;

namespace Ambev.DeveloperEvaluation.Integration.Common;

/// <summary>
/// Fluent builder for creating <see cref="Sale"/> test data.
/// Provides customization methods for various test scenarios.
/// </summary>
/// <remarks>
/// <para>
/// Uses the <see cref="Sale.Create"/> factory method to ensure all domain
/// invariants are enforced during test data creation. Default values are
/// provided for all properties — only override what your test needs.
/// </para>
/// <para>
/// <b>Default values:</b>
/// <list type="bullet">
///   <item>SaleDate: <c>DateTime.UtcNow</c></item>
///   <item>Customer: Random GUID + "Test Customer"</item>
///   <item>Branch: Random GUID + "Test Branch"</item>
///   <item>Items: 2 items with quantity 5 and unit price 100m (if none specified)</item>
/// </list>
/// </para>
/// </remarks>
public class SaleTestDataBuilder
{
    private readonly Faker _faker = new();
    private DateTime _saleDate = DateTime.UtcNow;
    private Guid _customerId = Guid.NewGuid();
    private string _customerName = "Test Customer";
    private Guid _branchId = Guid.NewGuid();
    private string _branchName = "Test Branch";
    private bool _isCancelled;
    private List<SaleItem> _items = [];

    /// <summary>
    /// Creates a new instance of the builder with default values.
    /// </summary>
    public static SaleTestDataBuilder Create() => new();

    /// <summary>
    /// Sets the sale date.
    /// </summary>
    public SaleTestDataBuilder WithSaleDate(DateTime saleDate)
    {
        _saleDate = saleDate;
        return this;
    }

    /// <summary>
    /// Sets the customer ID and name.
    /// </summary>
    public SaleTestDataBuilder WithCustomer(Guid customerId, string customerName)
    {
        _customerId = customerId;
        _customerName = customerName;
        return this;
    }

    /// <summary>
    /// Sets the customer name only (auto-generates a customer ID).
    /// </summary>
    public SaleTestDataBuilder WithCustomerName(string customerName)
    {
        _customerName = customerName;
        return this;
    }

    /// <summary>
    /// Sets the branch ID and name.
    /// </summary>
    public SaleTestDataBuilder WithBranch(Guid branchId, string branchName)
    {
        _branchId = branchId;
        _branchName = branchName;
        return this;
    }

    /// <summary>
    /// Sets the branch name only (auto-generates a branch ID).
    /// </summary>
    public SaleTestDataBuilder WithBranchName(string branchName)
    {
        _branchName = branchName;
        return this;
    }

    /// <summary>
    /// Generates the specified number of items with the given quantity and unit price.
    /// Discount is automatically calculated by <see cref="SaleItem.Create"/>.
    /// </summary>
    public SaleTestDataBuilder WithItems(int count, int quantity = 5, decimal unitPrice = 100m)
    {
        _items = Enumerable.Range(0, count)
            .Select(_ => SaleItem.Create(
                Guid.NewGuid(),
                _faker.Commerce.ProductName(),
                quantity,
                unitPrice))
            .ToList();
        return this;
    }

    /// <summary>
    /// Adds a specific item with known values.
    /// </summary>
    public SaleTestDataBuilder WithItem(Guid productId, string productName, int quantity, decimal unitPrice)
    {
        _items.Add(SaleItem.Create(productId, productName, quantity, unitPrice));
        return this;
    }

    /// <summary>
    /// Marks the sale as cancelled after creation.
    /// </summary>
    public SaleTestDataBuilder AsCancelled()
    {
        _isCancelled = true;
        return this;
    }

    /// <summary>
    /// Builds the <see cref="Sale"/> aggregate using the domain factory method.
    /// If no items were specified, defaults to 2 items with quantity 5 and price 100m.
    /// Optionally cancels the sale if <see cref="AsCancelled"/> was called.
    /// </summary>
    public Sale Build()
    {
        if (_items.Count == 0)
            WithItems(2); // Default: 2 items

        var sale = Sale.Create(_saleDate, _customerId, _customerName, _branchId, _branchName, _items);

        if (_isCancelled)
            sale.Cancel();

        return sale;
    }
}