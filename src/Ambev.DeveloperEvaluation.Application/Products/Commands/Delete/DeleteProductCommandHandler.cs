using Ambev.DeveloperEvaluation.Application.Common;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Products.Commands.Delete;

public class DeleteProductCommandHandler : IRequestHandler<DeleteProductsCommand, Result<Guid>>
{
    private readonly IUnitOfWork _unitOfWork;
    
    public DeleteProductCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;    
    }

    public async Task<Result<Guid>> Handle(DeleteProductsCommand command, CancellationToken cancellationToken)
    {
        try
        {
            var product = await _unitOfWork.Products.GetAsync(command.Id, cancellationToken);

            if (product == null)
            {
                return Result<Guid>.Failure($"Product with the id {command.Id} not found.");
            }
            
            product.Active = false;

            _unitOfWork.Products.Update(product);
            await _unitOfWork.CommitChangesAsync(cancellationToken);

            return Result<Guid>.Success(product.Id);
        }
        catch (Exception ex)
        {
            return Result<Guid>.Failure($"Unexpected error: {ex.Message}");
        }
    }
}
