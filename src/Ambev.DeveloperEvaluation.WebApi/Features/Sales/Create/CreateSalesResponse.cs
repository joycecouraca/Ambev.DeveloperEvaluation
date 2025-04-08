using Ambev.DeveloperEvaluation.Application.Sales.Commands.Create.Dtos;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales.Create;

public class CreateSalesResponse
{
    public Guid Id { get; set; }
    public long SaleNumber { get; set; }
    public DateTime SoldAt { get; set; }
    public decimal TotalSaleAmount { get; set; }
    public decimal TotalDiscountAmount { get; set; }
    public string BranchName { get; set; } = default!;
    public string CustomerName { get; set; } = default!;
    public string Status { get; set; } = default!;
    public List<CreateSaleItemDto> Items { get; set; } = [];
}
