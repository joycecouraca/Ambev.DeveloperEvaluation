using Ambev.DeveloperEvaluation.Application.Common;
using Ambev.DeveloperEvaluation.Application.Products.Common;
using Ambev.DeveloperEvaluation.Common.Pagination;
using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Products.Query.GetAll;

public class GetAllProductsQuery : IRequest<Result<PaginatedList<ProductDto>>>
{
    public int Page { get; set; }
    public int PageSize { get; set; }
    public string? Search { get; set; }
}
