using Ambev.DeveloperEvaluation.Application.Common;
using Ambev.DeveloperEvaluation.Application.Sales.Commands.Create.Dtos;
using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Sales.Commands.Create;

public class CreateSalesCommand : IRequest<Result<CreateSaleDto>>
{
    public DateTime SoldAt { get; set; }
    public string BranchName { get; set; } = default!;
    public Guid CustomerId { get; set; }
    public Guid CreatedById { get; set; }
    public List<CreateSaleItemDto> Items { get; set; } = new();
}
