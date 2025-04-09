using Ambev.DeveloperEvaluation.Application.Common;
using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Sales.Commands.Cancel.Sale;

public record CancelSaleCommand(Guid SaleId) : IRequest<Result<Guid>>;

