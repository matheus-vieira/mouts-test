using Ambev.DeveloperEvaluation.Common.Validation;
using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Sales.CreateSale;

/// <summary>
/// Command for creating a new sale in the system.
/// Implements <see cref="IRequest{TResponse}"/> to integrate with the MediatR pipeline.
/// </summary>
/// <remarks>
/// This command is the entry point for the create sale use case.
/// It carries all data required to construct a valid
/// <see cref="Ambev.DeveloperEvaluation.Domain.Entities.Sales.Sale"/> aggregate.
///
/// Validation is performed in two layers:
/// - <see cref="CreateSaleCommandValidator"/> — input consistency (required fields, ranges)
/// - Domain entity — business invariants (discount rules, cancellation guards)
/// </remarks>
public class CreateSaleCommand : IRequest<CreateSaleResult>
{
    /// <summary>
    /// Human-readable unique sale identifier (e.g. "SALE-2024-0001").
    /// Must be unique across the system.
    /// </summary>
    public string SaleNumber { get; set; } = string.Empty;

    /// <summary>Date and time when the sale is being registered.</summary>
    public DateTime SaleDate { get; set; }

    /// <summary>External identity reference to the customer.</summary>
    public Guid CustomerId { get; set; }

    /// <summary>
    /// Denormalized customer name captured at request time.
    /// Stored as-is to preserve historical accuracy even if the customer record changes.
    /// </summary>
    public string CustomerName { get; set; } = string.Empty;

    /// <summary>External identity reference to the branch.</summary>
    public Guid BranchId { get; set; }

    /// <summary>
    /// Denormalized branch name captured at request time.
    /// Stored as-is to preserve historical accuracy even if the branch record changes.
    /// </summary>
    public string BranchName { get; set; } = string.Empty;

    /// <summary>
    /// List of product lines included in this sale.
    /// At least one item is required — enforced by both validator and domain aggregate.
    /// </summary>
    public List<CreateSaleItemCommand> Items { get; set; } = [];

    /// <summary>
    /// Validates this command using <see cref="CreateSaleCommandValidator"/>.
    /// </summary>
    /// <returns>A <see cref="ValidationResultDetail"/> with errors if invalid.</returns>
    public ValidationResultDetail Validate()
    {
        var validator = new CreateSaleCommandValidator();
        var result = validator.Validate(this);
        return new ValidationResultDetail
        {
            IsValid = result.IsValid,
            Errors = result.Errors.Select(o => (ValidationErrorDetail)o)
        };
    }
}