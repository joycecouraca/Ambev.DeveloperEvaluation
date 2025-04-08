using Ambev.DeveloperEvaluation.Application.Common;
using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Sales.Commands.Cancel;

public record CancelSaleCommand(Guid SaleId) : IRequest<Result<Guid>>;

