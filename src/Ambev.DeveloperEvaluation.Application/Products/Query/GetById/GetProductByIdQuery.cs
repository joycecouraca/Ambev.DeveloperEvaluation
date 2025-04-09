using Ambev.DeveloperEvaluation.Application.Common;
using Ambev.DeveloperEvaluation.Application.Products.Common;
using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Products.Query.GetById;

public class GetProductByIdQuery : IRequest<Result<ProductDto>>
{
    public Guid Id { get; }
    
    public GetProductByIdQuery(Guid id)
    {
        Id = id;
    }
}