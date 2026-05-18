using Ambev.DeveloperEvaluation.Common.Validation;
using Ambev.DeveloperEvaluation.Domain.Common;
using Ambev.DeveloperEvaluation.Domain.Exceptions;

namespace Ambev.DeveloperEvaluation.Domain.Entities.Sales;

/// <summary>
/// Represents an item within a sale.
/// Discount is automatically calculated based on quantity business rules.
/// </summary>
public class SaleItem : BaseEntity
{
    /// <summary>External identity: product identifier.</summary>
    public Guid ProductId { get; private set; }

    /// <summary>External identity: denormalized product name.</summary>
    public string ProductName { get; private set; } = string.Empty;

    public int Quantity { get; private set; }
    public decimal UnitPrice { get; private set; }
    public decimal Discount { get; private set; }
    public decimal TotalAmount { get; private set; }

    protected SaleItem() { }

    /// <summary>
    /// Creates a new SaleItem applying quantity-based discount rules.
    /// </summary>
    public static SaleItem Create(Guid productId, string productName, int quantity, decimal unitPrice)
    {
        if (quantity <= 0)
            throw new DomainException("Quantity must be greater than zero.");

        if (quantity > 20)
            throw new DomainException("Cannot sell more than 20 identical items.");

        if (unitPrice <= 0)
            throw new DomainException("Unit price must be greater than zero.");

        var item = new SaleItem
        {
            Id = Guid.NewGuid(),
            ProductId = productId,
            ProductName = productName,
            Quantity = quantity,
            UnitPrice = unitPrice
        };

        item.ApplyDiscount();
        return item;
    }

    /// <summary>
    /// Applies discount based on quantity tiers and recalculates total.
    /// </summary>
    private void ApplyDiscount()
    {
        var discountRate = Quantity switch
        {
            >= 10 => 0.20m,
            >= 4  => 0.10m,
            _     => 0.00m
        };

        Discount = UnitPrice * Quantity * discountRate;
        TotalAmount = (UnitPrice * Quantity) - Discount;
    }

    public ValidationResultDetail Validate()
    {
        var validator = new SaleItemValidator();
        var result = validator.Validate(this);
        return new ValidationResultDetail
        {
            IsValid = result.IsValid,
            Errors = result.Errors.Select(o => (ValidationErrorDetail)o)
        };
    }
}