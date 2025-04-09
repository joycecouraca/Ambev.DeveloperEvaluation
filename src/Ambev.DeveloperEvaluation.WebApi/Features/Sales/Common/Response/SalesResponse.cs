namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales.Common.Response;

public class SalesResponse
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
    public List<SaleItemResponse> Items { get; set; } = [];
}
public class SaleItemResponse
{
    public Guid ProductId { get; init; }
    public int Quantity { get; init; }
    public string ProductName { get; init; } = default!;
    public decimal UnitPrice { get; init; }
    public decimal TotalAmount { get; init; }
    public decimal DiscountPerUnit { get; init; }
    public decimal DiscountTotal => DiscountPerUnit * Quantity;
}
