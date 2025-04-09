using Ambev.DeveloperEvaluation.Application.Common;
using Ambev.DeveloperEvaluation.Application.Sales.Common.Dtos;
using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Sales.Querys.GetById;

public class GetSaleByIdQuery : IRequest<Result<SaleDto>>
{
    public Guid Id { get; set; }

    public GetSaleByIdQuery(Guid id)
    {
        Id = id;
    }
}
