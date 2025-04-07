namespace Ambev.DeveloperEvaluation.Application.Sales.Commands.Create.Dtos;

public class CreateSaleItemDto
{
    public Guid ProductId { get; init; }
    public int Quantity { get; init; }
    public string ProductName { get; init; } = default!;
    public decimal UnitPrice { get; init; }
    public decimal TotalAmount { get; init; }
    public decimal DiscountPerUnit { get; init; }
    public decimal DiscountTotal => DiscountPerUnit * Quantity;
}
