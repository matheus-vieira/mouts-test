using Ambev.DeveloperEvaluation.Domain.Exceptions;

namespace Ambev.DeveloperEvaluation.Domain.Entities.Sales;

public partial class Sale
{
    /// <summary>
    /// Cancels this sale. Once cancelled, no further modifications are allowed.
    /// </summary>
    /// <exception cref="DomainException">Thrown when the sale is already cancelled.</exception>
    public void Cancel()
    {
        if (IsCancelled)
            throw new DomainException("Sale is already cancelled.");

        IsCancelled = true;
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