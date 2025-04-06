using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Common.Abstractions;

public interface IRequestDispatcher
{
    Task<TResponse> SendValidatedAsync<TRequest, TCommand, TResponse>(TRequest request, CancellationToken cancellationToken)
        where TCommand : class, IRequest<TResponse>;
}
