namespace Ambev.DeveloperEvaluation.Application.Sales.Common.Dtos;

public class SaleDto
{
    public Guid Id { get; set; }
    public long SaleNumber { get; set; }
    public DateTime SoldAt { get; set; }
    public DateTime CreatedAt { get; set; }
    public decimal TotalSaleAmount { get; set; }
    public decimal TotalDiscountAmount { get; set; }
    public string BranchName { get; set; } = default!;
    public string CustomerName { get; set; } = default!;
    public string Status { get; set; } = default!;
    public List<SaleItemDto> Items { get; set; } = [];
}   
