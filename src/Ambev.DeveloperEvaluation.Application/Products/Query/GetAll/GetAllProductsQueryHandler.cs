using Ambev.DeveloperEvaluation.Application.Common;
using Ambev.DeveloperEvaluation.Application.Products.Commands.Create;
using Ambev.DeveloperEvaluation.Application.Products.Common;
using Ambev.DeveloperEvaluation.Common.Pagination;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using System.Linq.Expressions;

namespace Ambev.DeveloperEvaluation.Application.Products.Query.GetAll;

public class GetAllProductsQueryHandler : IRequestHandler<GetAllProductsQuery, Result<PaginatedList<GetProductDto>>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ILogger<CreateProductsCommandHandler> _logger;

    public GetAllProductsQueryHandler(IUnitOfWork unitOfWork, IMapper mapper, ILogger<CreateProductsCommandHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<Result<PaginatedList<GetProductDto>>> Handle(GetAllProductsQuery query, CancellationToken cancellationToken)
    {
        try
        {
            Expression<Func<Product, bool>>? filter = null;

            if (!string.IsNullOrWhiteSpace(query.Search))
            {
                filter = p => p.Name.Contains(query.Search) || p.Description.Contains(query.Search) && p.Active;
            }

            Func<IQueryable<Product>, IOrderedQueryable<Product>> orderBy = q => q.OrderBy(p => p.Name);

            var result = await _unitOfWork.Products.GetPaginatedAsync(
                query.Page,
                query.PageSize,
                filter,
                orderBy,
                cancellationToken);       

            return Result<PaginatedList<GetProductDto>>.Success(_mapper.Map<PaginatedList<GetProductDto>>(result));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while retrieving products");
            return Result<PaginatedList<GetProductDto>>.Failure($"Failed to load products: {ex.Message}");
        }
    }
}
