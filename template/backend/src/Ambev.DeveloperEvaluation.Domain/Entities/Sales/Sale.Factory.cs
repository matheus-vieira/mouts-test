using Ambev.DeveloperEvaluation.Domain.Exceptions;

namespace Ambev.DeveloperEvaluation.Domain.Entities.Sales;

public partial class Sale
{
    /// <summary>
    /// Creates a new <see cref="Sale"/> enforcing all aggregate invariants.
    /// Total amount is automatically calculated from the provided items.
    /// </summary>
    /// <param name="saleNumber">Unique human-readable sale identifier.</param>
    /// <param name="saleDate">Date and time of the sale.</param>
    /// <param name="customerId">External identity of the customer.</param>
    /// <param name="customerName">Denormalized customer name.</param>
    /// <param name="branchId">External identity of the branch.</param>
    /// <param name="branchName">Denormalized branch name.</param>
    /// <param name="items">At least one sale item is required.</param>
    /// <exception cref="DomainException">Thrown when items collection is empty.</exception>
    public static Sale Create(
        string saleNumber,
        DateTime saleDate,
        Guid customerId,
        string customerName,
        Guid branchId,
        string branchName,
        IEnumerable<SaleItem> items)
    {
        var itemList = items?.ToList() ?? [];

        if (itemList.Count == 0)
            throw new DomainException("A sale must contain at least one item.");

        var sale = new Sale
        {
            Id = Guid.NewGuid(),
            SaleNumber = saleNumber,
            SaleDate = saleDate,
            CustomerId = customerId,
            CustomerName = customerName,
            BranchId = branchId,
            BranchName = branchName,
            IsCancelled = false
        };

        sale._items.AddRange(itemList);
        sale.RecalculateTotal();
        return sale;
    }
}