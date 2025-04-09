using Ambev.DeveloperEvaluation.Application.Common;
using Ambev.DeveloperEvaluation.Application.Sales.Common.Dtos;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Ambev.DeveloperEvaluation.Application.Sales.Querys.GetById;

public class GetSaleByIdQueryHandler : IRequestHandler<GetSaleByIdQuery, Result<SaleDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ILogger<GetSaleByIdQueryHandler> _logger;

    public GetSaleByIdQueryHandler(IUnitOfWork unitOfWork, IMapper mapper, ILogger<GetSaleByIdQueryHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<Result<SaleDto>> Handle(GetSaleByIdQuery query, CancellationToken cancellationToken)
    {
        try
        {
            var sale = await _unitOfWork.Sales.GetByIdWithItemsAsync(query.Id, cancellationToken);

            if (sale == null)
            {
                _logger.LogWarning("Sale with id {Id} not found", query.Id);
                return Result<SaleDto>.BusinessFailure($"Sale with the id {query.Id} not found.");
            }

            var dto = _mapper.Map<SaleDto>(sale);
            return Result<SaleDto>.Success(dto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while retrieving sale with id {Id}", query.Id);
            return Result<SaleDto>.BusinessFailure($"Failed to load sale: {ex.Message}");
        }
    }
}
