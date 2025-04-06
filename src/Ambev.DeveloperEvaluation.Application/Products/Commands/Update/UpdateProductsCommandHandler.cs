using Ambev.DeveloperEvaluation.Application.Common;
using Ambev.DeveloperEvaluation.Application.Products.Commands.Create;
using Ambev.DeveloperEvaluation.Application.Products.Commands.Update.Dtos;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Ambev.DeveloperEvaluation.Application.Products.Commands.Update;

public class UpdateProductsCommandHandler : IRequestHandler<UpdateProductsCommand, Result<UpdateProductDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ILogger<CreateProductsCommandHandler> _logger;

    public UpdateProductsCommandHandler(IUnitOfWork unitOfWork, IMapper mapper, ILogger<CreateProductsCommandHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<Result<UpdateProductDto>> Handle(UpdateProductsCommand command, CancellationToken cancellationToken)
    {
        try
        {
            var product = await _unitOfWork.Products.GetByIdAsync(command.Id, cancellationToken);

            if (product == null)
            {
                _logger.LogWarning("Product with id {ProductId} not found", command.Id);
                return Result<UpdateProductDto>.Failure($"Product with the id {command.Id} not found.");
            }

            _mapper.Map(command, product);
            _unitOfWork.Products.Update(product);
            await _unitOfWork.CommitChangesAsync(cancellationToken);

            return Result<UpdateProductDto>.Success(_mapper.Map<UpdateProductDto>(product));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating product with id {ProductId}", command.Id);
            return Result<UpdateProductDto>.Failure("Unexpected error:", ex.Message);
        }
    }
}
