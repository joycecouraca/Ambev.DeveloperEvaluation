namespace Ambev.DeveloperEvaluation.Application.Sales.Commands.Cancel.Dtos;

public class CancelledItemDto
{
    public Guid ItemId { get; set; }
    public Guid ProductId { get; set; }
    public string ProductName { get; set; } = default!;
    public int Quantity { get; set; }
    public decimal FinalUnitPrice { get; set; }
    public decimal TotalAmount { get; set; }
    public string Status { get; set; } = default!;
}