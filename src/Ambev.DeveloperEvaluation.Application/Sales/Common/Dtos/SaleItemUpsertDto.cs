namespace Ambev.DeveloperEvaluation.Application.Sales.Common.Dtos;

public class SaleItemUpsertDto
{
    public Guid ProductId { get; set; }
    public int Quantity { get; set; }
}