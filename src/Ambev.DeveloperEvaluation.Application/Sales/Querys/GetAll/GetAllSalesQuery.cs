using Ambev.DeveloperEvaluation.Application.Common;
using Ambev.DeveloperEvaluation.Application.Sales.Common.Dtos;
using Ambev.DeveloperEvaluation.Common.Pagination;
using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Sales.Querys.GetAll;

public class GetAllSalesQuery : IRequest<Result<PaginatedList<SaleDto>>>
{
    public int Page { get; set; }
    public int PageSize { get; set; }
    public string? Search { get; set; }

    public GetAllSalesQuery(int page, int pageSize, string? search)
    {
        Page = page;
        PageSize = pageSize;
        Search = search;
    }
}