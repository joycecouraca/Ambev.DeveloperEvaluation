using Ambev.DeveloperEvaluation.Application.Common;
using Ambev.DeveloperEvaluation.Application.Users.DeleteUser;
using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Products.Commands.Delete;

public class DeleteProductsCommand : IRequest<Result<Guid>>
{
    public Guid Id { get; }

    public DeleteProductsCommand(Guid id)
    {
        Id = id;
    }
}
