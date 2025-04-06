using Ambev.DeveloperEvaluation.Application.Common;
using Ambev.DeveloperEvaluation.Application.Products.Common;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using AutoMapper;
using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Products.Query.GetById;

public class GetProductByIdQueryHandler : IRequestHandler<GetProductByIdQuery, Result<GetProductDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetProductByIdQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<Result<GetProductDto>> Handle(GetProductByIdQuery query, CancellationToken cancellationToken)
    {
        try
        {
            var product = await _unitOfWork.Products.FindAsync(c=> c.Id == query.Id && c.Active, cancellationToken);

            if (product == null)
            {
                return Result<GetProductDto>.Failure($"Product with the id {query.Id} not found.");
            }
            
            return Result<GetProductDto>.Success(_mapper.Map<GetProductDto>(product));
        }
        catch (Exception ex)
        {
            return Result<GetProductDto>.Failure($"Failed to load products: {ex.Message}");
        }
    }
}
