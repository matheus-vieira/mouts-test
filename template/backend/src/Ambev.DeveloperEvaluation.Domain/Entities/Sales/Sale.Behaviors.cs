using Ambev.DeveloperEvaluation.Domain.Exceptions;

namespace Ambev.DeveloperEvaluation.Domain.Entities.Sales;

public partial class Sale
{
    /// <summary>
    /// Cancels this sale. Once cancelled, no further modifications are allowed.
    /// </summary>
    /// <returns>Metadata required to publish a cancellation event.</returns>
    public (Guid Id, string Number) Cancel()
    {
        if (IsCancelled)
            throw new DomainException("Sale is already cancelled.");

        IsCancelled = true;
        return (Id, SaleNumber);
    }

    /// <summary>
    /// Updates the customer and branch associations of the sale.
    /// Both IDs and their denormalized names must be provided together
    /// to keep the aggregate consistent.
    /// </summary>
    /// <param name="customerId">The new customer identifier.</param>
    /// <param name="customerName">The denormalized customer name at update time.</param>
    /// <param name="branchId">The new branch identifier.</param>
    /// <param name="branchName">The denormalized branch name at update time.</param>
    /// <exception cref="DomainException">Thrown when the sale is already cancelled.</exception>
    public void Update(Guid customerId, string customerName, Guid branchId, string branchName)
    {
        if (IsCancelled)
            throw new DomainException("Cannot update a cancelled sale.");

        CustomerId = customerId;
        CustomerName = customerName;
        BranchId = branchId;
        BranchName = branchName;
    }

    /// <summary>
    /// Replaces all current items with the provided collection.
    /// Discount rules are enforced by <see cref="SaleItem.Create"/> at item creation time.
    /// Total amount is recalculated after replacement.
    /// </summary>
    /// <param name="newItems">The new set of items to replace the current ones.</param>
    /// <exception cref="DomainException">Thrown when the new items collection is empty.</exception>
    public void UpdateItems(IEnumerable<SaleItem> newItems)
    {
        var itemList = newItems?.ToList() ?? [];

        if (itemList.Count == 0)
            throw new DomainException("Sale must have at least one item.");

        _items.Clear();
        _items.AddRange(itemList);
        RecalculateTotal();
    }

    /// <summary>
    /// Recalculates <see cref="TotalAmount"/> from the current items collection.
    /// Must be called after any mutation to the items list.
    /// </summary>
    private void RecalculateTotal()
    {
        TotalAmount = _items.Sum(i => i.TotalAmount);
    }
}