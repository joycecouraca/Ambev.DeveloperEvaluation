using Ambev.DeveloperEvaluation.Application.Common;
using Ambev.DeveloperEvaluation.Application.Users.DeleteUser;
using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Products.Commands.Delete;

public class DeleteProductsCommand : IRequest<Result<Guid>>
{
    /// <summary>
    /// The unique identifier of the product to delete
    /// </summary>
    public Guid Id { get; }

    /// <summary>
    /// Initializes a new instance of DeleteProductCommand
    /// </summary>
    /// <param name="id">The ID of the product to delete</param>
    public DeleteProductsCommand(Guid id)
    {
        Id = id;
    }
}