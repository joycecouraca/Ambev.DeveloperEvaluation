using Ambev.DeveloperEvaluation.Application.Common;
using Ambev.DeveloperEvaluation.Application.Sales.Common.Dtos;
using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Sales.Commands.Create;

public class CreateSalesCommand : IRequest<Result<SaleDto>>
{
    public DateTime SoldAt { get; set; }
    public string BranchName { get; set; } = default!;
    public Guid CustomerId { get; set; }
    public List<SaleItemUpsertDto> Items { get; set; } = [];
}
