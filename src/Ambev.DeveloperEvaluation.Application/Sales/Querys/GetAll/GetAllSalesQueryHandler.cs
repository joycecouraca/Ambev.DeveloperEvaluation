using Ambev.DeveloperEvaluation.Application.Common;
using Ambev.DeveloperEvaluation.Application.Sales.Common.Dtos;
using Ambev.DeveloperEvaluation.Common.Pagination;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Enums;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using System.Linq.Expressions;

namespace Ambev.DeveloperEvaluation.Application.Sales.Querys.GetAll;

public class GetAllSalesQueryHandler : IRequestHandler<GetAllSalesQuery, Result<PaginatedList<SaleDto>>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ILogger<GetAllSalesQueryHandler> _logger;

    public GetAllSalesQueryHandler(IUnitOfWork unitOfWork, IMapper mapper, ILogger<GetAllSalesQueryHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<Result<PaginatedList<SaleDto>>> Handle(GetAllSalesQuery query, CancellationToken cancellationToken)
    {
        try
        {
            Expression<Func<Sale, bool>>? filter = null;

            if (!string.IsNullOrWhiteSpace(query.Search))
            {
                filter = s => s.CreatedBy.Username.Contains(query.Search) && s.Status != SaleStatus.Cancelled;
            }


            var result = await _unitOfWork.Sales.GetPaginatedAsync(
                query.Page,
                query.PageSize,
                filter,
                OrderBy,
                cancellationToken);

            var mapped = _mapper.Map<PaginatedList<SaleDto>>(result);

            return Result<PaginatedList<SaleDto>>.Success(mapped);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while retrieving sales");
            return Result<PaginatedList<SaleDto>>.BusinessFailure($"Failed to load sales: {ex.Message}");
        }
    }
    static IOrderedQueryable<Sale> OrderBy(IQueryable<Sale> q) => q.OrderBy(s => s.CreatedAt);
}
