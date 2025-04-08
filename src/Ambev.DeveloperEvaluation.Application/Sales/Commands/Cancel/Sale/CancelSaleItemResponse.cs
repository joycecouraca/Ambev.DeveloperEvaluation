namespace Ambev.DeveloperEvaluation.Application.Sales.Commands.Cancel.Sale;

public class CancelSaleItemResponse
{
    public Guid SaleId { get; set; }
    public List<CancelledItemDto> CancelledItems { get; set; } = [];
}

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
