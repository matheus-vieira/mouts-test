using Ambev.DeveloperEvaluation.Common.Validation;
using Ambev.DeveloperEvaluation.Domain.Common;
using Ambev.DeveloperEvaluation.Domain.Exceptions;

namespace Ambev.DeveloperEvaluation.Domain.Entities.Sales;

/// <summary>
/// Aggregate root for the sales context.
/// All changes to sale items must go through this aggregate.
/// </summary>
public class Sale : BaseEntity
{
    public string SaleNumber { get; private set; } = string.Empty;
    public DateTime SaleDate { get; private set; }

    /// <summary>External identity: customer identifier.</summary>
    public Guid CustomerId { get; private set; }

    /// <summary>External identity: denormalized customer name.</summary>
    public string CustomerName { get; private set; } = string.Empty;

    /// <summary>External identity: branch identifier.</summary>
    public Guid BranchId { get; private set; }

    /// <summary>External identity: denormalized branch name.</summary>
    public string BranchName { get; private set; } = string.Empty;

    public decimal TotalAmount { get; private set; }
    public bool IsCancelled { get; private set; }

    private readonly List<SaleItem> _items = [];
    public IReadOnlyCollection<SaleItem> Items => _items;

    protected Sale() { }

    /// <summary>
    /// Creates a new Sale with the provided items.
    /// </summary>
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

    /// <summary>
    /// Cancels the sale. Cancelled sales cannot be modified.
    /// </summary>
    public void Cancel()
    {
        if (IsCancelled)
            throw new DomainException("Sale is already cancelled.");

        IsCancelled = true;
    }

    private void RecalculateTotal()
    {
        TotalAmount = _items.Sum(i => i.TotalAmount);
    }

    public ValidationResultDetail Validate()
    {
        var validator = new SaleValidator();
        var result = validator.Validate(this);
        return new ValidationResultDetail
        {
            IsValid = result.IsValid,
            Errors = result.Errors.Select(o => (ValidationErrorDetail)o)
        };
    }
}