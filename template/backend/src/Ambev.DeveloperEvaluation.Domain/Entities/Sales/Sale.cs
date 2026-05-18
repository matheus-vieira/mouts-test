using Ambev.DeveloperEvaluation.Domain.Common;

namespace Ambev.DeveloperEvaluation.Domain.Entities.Sales;

/// <summary>
/// Aggregate root for the Sales bounded context.
/// </summary>
/// <remarks>
/// All mutations to items and state must go through this aggregate.
/// This is a partial class. Responsibilities are split across:
/// - <c>Sale.Factory.cs</c>    — creation and guard clauses
/// - <c>Sale.Behaviors.cs</c>  — domain behaviors (cancel, recalculate)
/// - <c>Sale.Validation.cs</c> — FluentValidation integration
/// </remarks>
public partial class Sale : BaseEntity
{
    /// <summary>Human-readable unique sale identifier (e.g. "SALE-2024-0001").</summary>
    public string SaleNumber { get; private set; } = string.Empty;

    /// <summary>Date and time when the sale was created.</summary>
    public DateTime SaleDate { get; private set; }

    /// <summary>External identity reference to the customer.</summary>
    public Guid CustomerId { get; private set; }

    /// <summary>Denormalized customer name captured at the time of sale.</summary>
    public string CustomerName { get; private set; } = string.Empty;

    /// <summary>External identity reference to the branch.</summary>
    public Guid BranchId { get; private set; }

    /// <summary>Denormalized branch name captured at the time of sale.</summary>
    public string BranchName { get; private set; } = string.Empty;

    /// <summary>Sum of all item totals after discounts.</summary>
    public decimal TotalAmount { get; private set; }

    /// <summary>Indicates whether this sale has been cancelled.</summary>
    public bool IsCancelled { get; private set; }

    private readonly List<SaleItem> _items = [];

    /// <summary>Read-only collection of items belonging to this sale.</summary>
    public IReadOnlyCollection<SaleItem> Items => _items;

    /// <summary>Required by EF Core. Use <see cref="Create"/> for domain creation.</summary>
    protected Sale() { }
}