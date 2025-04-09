namespace Ambev.DeveloperEvaluation.Application.Sales.Commands.Cancel.Dtos;

public class CancelSaleDto
{
    public Guid SaleId { get; set; }
    public List<CancelledItemDto> CancelledItems { get; set; } = [];
}

