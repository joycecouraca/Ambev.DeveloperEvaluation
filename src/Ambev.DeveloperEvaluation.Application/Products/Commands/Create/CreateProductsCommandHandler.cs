using Ambev.DeveloperEvaluation.Application.Common;
using Ambev.DeveloperEvaluation.Application.Products.Common;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Ambev.DeveloperEvaluation.Application.Products.Commands.Create;

public class CreateProductsCommandHandler : IRequestHandler<CreateProductsCommand, Result<ProductDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ILogger<CreateProductsCommandHandler> _logger;


    public CreateProductsCommandHandler(IUnitOfWork unitOfWork, IMapper mapper, ILogger<CreateProductsCommandHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<Result<ProductDto>> Handle(CreateProductsCommand command, CancellationToken cancellationToken)
    {
        var productExists = await _unitOfWork.Products.ExistsAsync(p => p.Name == command.Name, cancellationToken: cancellationToken);

        if (productExists)
        {
            _logger.LogWarning("A product with the name '{ProductName}' already exists.", command.Name);
            return Result<ProductDto>.BusinessFailure($"A product with the name '{command.Name}' already exists.");
        }

        try
        {
            var product = _mapper.Map<Product>(command);

            _unitOfWork.Products.Add(product);
            await _unitOfWork.CommitChangesAsync(cancellationToken);

            var resultDto = _mapper.Map<ProductDto>(product);
            return Result<ProductDto>.Success(resultDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while creating the product. Command: {@Command}", command);            
            return Result<ProductDto>.BusinessFailure("An unexpected error occurred while creating the product.");
        }
    }
}
