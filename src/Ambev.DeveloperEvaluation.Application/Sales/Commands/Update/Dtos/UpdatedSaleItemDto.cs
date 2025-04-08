namespace Ambev.DeveloperEvaluation.Application.Sales.Commands.Update.Dtos;

public class UpdatedSaleItemDto
{
    public Guid ItemId { get; set; }
    public Guid ProductId { get; set; }
    public string ProductName { get; set; } = default!;
    public int Quantity { get; set; }
    public decimal OriginalUnitPrice { get; set; }
    public decimal DiscountPerUnit { get; set; }
    public decimal FinalUnitPrice { get; set; }
    public decimal TotalAmount { get; set; }
}
