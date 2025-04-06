using Ambev.DeveloperEvaluation.Application.Common;
using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Products.Query.GetAll;

internal class GetAllProductsQuery : IRequest<Result<ProductDto>>
{
}
