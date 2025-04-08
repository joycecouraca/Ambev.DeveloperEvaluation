using Ambev.DeveloperEvaluation.Application.Common;
using Ambev.DeveloperEvaluation.Application.Sales.Commands.Cancel.Sale;
using Ambev.DeveloperEvaluation.Common.Security;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Sales.Commands.Cancel.SaleItem;

public class CancelSaleItemCommandHandler : IRequestHandler<CancelSaleItemsCommand, Result<CancelSaleItemResponse>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IContextUserProvider _userProvider;

    public CancelSaleItemCommandHandler(IUnitOfWork unitOfWork, IContextUserProvider userProvider)
    {
        _unitOfWork = unitOfWork;
        _userProvider = userProvider;
    }

    public async Task<Result<CancelSaleItemResponse>> Handle(CancelSaleItemsCommand request, CancellationToken cancellationToken)
    {
        var user = _userProvider.GetCurrentUser();
        if (user is null)
            return Result<CancelSaleItemResponse>.Failure("User not found.");

        var sale = await _unitOfWork.Sales.GetByIdWithItemsAsync(request.SaleId, cancellationToken);
        if (sale is null)
            return Result<CancelSaleItemResponse>.Failure("Sale not found.");

        var itemsToCancel = sale.Items.Where(i => request.ItemIds.Contains(i.Id)).ToList();
        if (itemsToCancel.Count == 0)
            return Result<CancelSaleItemResponse>.Failure("No valid items found for cancellation.");

        sale.CancelItems((User)user, [.. itemsToCancel]);

        _unitOfWork.Sales.Update(sale);
        await _unitOfWork.CommitChangesAsync(cancellationToken);

        var response = new CancelSaleItemResponse
        {
            SaleId = sale.Id,
            CancelledItems = [.. itemsToCancel.Select(item => new CancelledItemDto
            {
                ItemId = item.Id,
                ProductId = item.ProductId,
                ProductName = item.Product.Name,
                Quantity = item.Quantity,
                FinalUnitPrice = item.FinalUnitPrice,
                TotalAmount = item.TotalAmount,
                Status = item.Status.ToString()
            })]
        };

        return Result<CancelSaleItemResponse>.Success(response);
    }
}
