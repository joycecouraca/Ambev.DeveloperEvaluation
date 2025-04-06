using Ambev.DeveloperEvaluation.Application.Common;
using Ambev.DeveloperEvaluation.Application.Products.Commands.Create;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Ambev.DeveloperEvaluation.Application.Products.Commands.Delete;

public class DeleteProductCommandHandler : IRequestHandler<DeleteProductsCommand, Result<Guid>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<CreateProductsCommandHandler> _logger;

    public DeleteProductCommandHandler(IUnitOfWork unitOfWork, ILogger<CreateProductsCommandHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result<Guid>> Handle(DeleteProductsCommand command, CancellationToken cancellationToken)
    {
        try
        {
            var product = await _unitOfWork.Products.GetByIdAsync(command.Id, cancellationToken);

            if (product == null)
            {
                _logger.LogWarning("Product with id {ProductId} not found", command.Id);
                return Result<Guid>.Failure($"Product with the id {command.Id} not found.");
            }
            
            //product.Active = false;

            _unitOfWork.Products.Update(product);
            await _unitOfWork.CommitChangesAsync(cancellationToken);

            return Result<Guid>.Success(product.Id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting product with id {ProductId}", command.Id);
            return Result<Guid>.Failure($"Unexpected error: {ex.Message}");
        }
    }
}
