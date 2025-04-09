namespace Ambev.DeveloperEvaluation.Application.Sales.Common.Dtos;

public class SaleItemDto
{
    public Guid ProductId { get; init; }
    public int Quantity { get; init; }
    public string ProductName { get; init; } = default!;
    public decimal UnitPrice { get; init; }
    public decimal TotalAmount { get; init; }
    public decimal DiscountPerUnit { get; init; }
    public decimal DiscountTotal => DiscountPerUnit * Quantity;
}
