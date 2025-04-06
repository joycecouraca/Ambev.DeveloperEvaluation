using Ambev.DeveloperEvaluation.Application.Common;
using Ambev.DeveloperEvaluation.Application.Products.Commands.Create;
using Ambev.DeveloperEvaluation.Application.Products.Common;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Ambev.DeveloperEvaluation.Application.Products.Query.GetById;

public class GetProductByIdQueryHandler : IRequestHandler<GetProductByIdQuery, Result<GetProductDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ILogger<CreateProductsCommandHandler> _logger;

    public GetProductByIdQueryHandler(IUnitOfWork unitOfWork, IMapper mapper, ILogger<CreateProductsCommandHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<Result<GetProductDto>> Handle(GetProductByIdQuery query, CancellationToken cancellationToken)
    {
        try
        {
            var product = await _unitOfWork.Products.FindAsync(c=> c.Id == query.Id && c.Active, cancellationToken);

            if (product == null)
            {
                _logger.LogWarning("Product with id {Id} not found", query.Id);
                return Result<GetProductDto>.Failure($"Product with the id {query.Id} not found.");
            }
            
            return Result<GetProductDto>.Success(_mapper.Map<GetProductDto>(product));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while retrieving product with id {Id}", query.Id);
            return Result<GetProductDto>.Failure($"Failed to load products: {ex.Message}");
        }
    }
}
