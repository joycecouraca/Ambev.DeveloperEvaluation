using Ambev.DeveloperEvaluation.Application.Common;
using Ambev.DeveloperEvaluation.Application.Sales.Common.Dtos;
using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Sales.Commands.Update;

public class UpdateSaleCommand : IRequest<Result<SaleDto>>
{
    public Guid SaleId { get; set; }
    public DateTime SoldAt { get; set; }
    public string BranchName { get; set; } = default!;
    public Guid CustomerId { get; set; }
    public Guid CreatedById { get; set; }
    public List<SaleItemDto> Items { get; set; } = [];
}
