namespace Ambev.DeveloperEvaluation.Application.Sales.Commands.Update.Dtos;

public class UpdateSaleDto
{
    public Guid SaleId { get; set; }
    public long SaleNumber { get; set; }
    public string BranchName { get; set; } = default!;
    public DateTime SoldAt { get; set; }
    public decimal TotalSaleAmount { get; set; }
    public decimal TotalDiscountAmount { get; set; }
    public string CustomerName { get; set; } = default!;
    public string Status { get; set; } = default!;
    public List<UpdatedSaleItemDto> Items { get; set; } = [];
}
