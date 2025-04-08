using Ambev.DeveloperEvaluation.Application.Common;
using Ambev.DeveloperEvaluation.Application.Sales.Commands.Create.Dtos;
using Ambev.DeveloperEvaluation.Application.Sales.Commands.Update.Dtos;
using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Sales.Commands.Update;

public class UpdateSaleCommand : IRequest<Result<UpdateSaleDto>>
{
    public Guid SaleId { get; set; }
    public DateTime SoldAt { get; set; }
    public string BranchName { get; set; } = default!;
    public Guid CustomerId { get; set; }
    public Guid CreatedById { get; set; }
    public List<CreateSaleItemDto> Items { get; set; } = [];
}
