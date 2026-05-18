using Ambev.DeveloperEvaluation.Domain.Exceptions;

namespace Ambev.DeveloperEvaluation.Domain.Entities.Sales;

public partial class SaleItem
{
    /// <summary>
    /// Creates a new <see cref="SaleItem"/> enforcing all domain invariants.
    /// Discount is automatically calculated based on quantity tier rules.
    /// </summary>
    /// <param name="productId">External identity of the product.</param>
    /// <param name="productName">Denormalized product name.</param>
    /// <param name="quantity">Units sold. Must be between 1 and 20.</param>
    /// <param name="unitPrice">Price per unit. Must be greater than zero.</param>
    /// <exception cref="DomainException">Thrown when any invariant is violated.</exception>
    public static SaleItem Create(
        Guid productId,
        string productName,
        int quantity,
        decimal unitPrice)
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
}