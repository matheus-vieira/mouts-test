namespace Ambev.DeveloperEvaluation.Domain.Entities.Sales;

public partial class SaleItem
{
    /// <summary>
    /// Applies the quantity-based discount tier and recalculates the total amount.
    /// </summary>
    /// <remarks>
    /// Discount tiers:
    /// - Less than 4 items  → 0%
    /// - 4 to 9 items       → 10%
    /// - 10 to 20 items     → 20%
    /// </remarks>
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
}