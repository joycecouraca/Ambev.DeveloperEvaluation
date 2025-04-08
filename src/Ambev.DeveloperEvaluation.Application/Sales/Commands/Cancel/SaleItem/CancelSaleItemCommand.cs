using Ambev.DeveloperEvaluation.Application.Common;
using Ambev.DeveloperEvaluation.Application.Sales.Commands.Cancel.Sale;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ambev.DeveloperEvaluation.Application.Sales.Commands.Cancel.SaleItem;

public record CancelSaleItemsCommand(Guid SaleId, List<Guid> ItemIds) : IRequest<Result<CancelSaleItemResponse>>;


