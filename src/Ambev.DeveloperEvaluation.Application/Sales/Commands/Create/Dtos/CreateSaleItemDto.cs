namespace Ambev.DeveloperEvaluation.Application.Sales.Commands.Create.Dtos;

public class CreateSaleItemDto
{
    public Guid ProductId { get; init; }
    public int Quantity { get; init; }
}
