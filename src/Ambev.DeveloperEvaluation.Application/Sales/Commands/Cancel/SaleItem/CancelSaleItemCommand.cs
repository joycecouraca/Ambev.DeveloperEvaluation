using Ambev.DeveloperEvaluation.Application.Common;
using Ambev.DeveloperEvaluation.Application.Sales.Commands.Cancel.Dtos;
using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Sales.Commands.Cancel.SaleItem;

public record CancelSaleItemsCommand(Guid SaleId, List<Guid> ItemIds) : IRequest<Result<CancelSaleDto>>;


