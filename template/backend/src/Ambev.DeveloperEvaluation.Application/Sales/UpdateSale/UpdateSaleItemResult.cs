namespace Ambev.DeveloperEvaluation.Application.Sales.UpdateSale;

public class UpdateSaleItemResult
{
    public Guid ProductId { get; set; }
    public int Quantity { get; set; }
    public decimal TotalAmount { get; set; }
}