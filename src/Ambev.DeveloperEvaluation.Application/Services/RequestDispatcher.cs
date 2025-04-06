using Ambev.DeveloperEvaluation.Application.Common.Abstractions;
using AutoMapper;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace Ambev.DeveloperEvaluation.Application.Services;

public class RequestDispatcher : IRequestDispatcher
{
    private readonly IMediator _mediator;
    private readonly IServiceProvider _serviceProvider;
    private readonly IMapper _mapper;

    public RequestDispatcher(IMediator mediator, IServiceProvider serviceProvider, IMapper mapper)
    {
        _mediator = mediator;
        _serviceProvider = serviceProvider;
        _mapper = mapper;
    }

    public async Task<TResponse> SendValidatedAsync<TRequest, TCommand, TResponse>(TRequest request, CancellationToken cancellationToken)
        where TCommand : class, IRequest<TResponse>
    {
        // 1. Validação do DTO
        var validator = _serviceProvider.GetService<IValidator<TRequest>>();
        if (validator != null)
        {
            var validationResult = await validator.ValidateAsync(request, cancellationToken);
            if (!validationResult.IsValid)
                throw new ValidationException(validationResult.Errors);
        }

        // 2. Mapeamento para o comando (apenas após validação)
        var command = _mapper.Map<TCommand>(request);

        // 3. Envio via MediatR
        return await _mediator.Send(command, cancellationToken);
    }
}
