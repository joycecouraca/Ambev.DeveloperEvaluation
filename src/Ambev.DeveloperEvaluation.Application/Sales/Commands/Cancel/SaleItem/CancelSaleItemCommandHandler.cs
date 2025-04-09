using Ambev.DeveloperEvaluation.Application.Common;
using Ambev.DeveloperEvaluation.Application.Sales.Commands.Cancel.Dtos;
using Ambev.DeveloperEvaluation.Common.Security;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Enums;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Sales.Commands.Cancel.SaleItem;

public class CancelSaleItemCommandHandler : IRequestHandler<CancelSaleItemsCommand, Result<CancelSaleDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IContextUserProvider _userProvider;

    public CancelSaleItemCommandHandler(IUnitOfWork unitOfWork, IContextUserProvider userProvider)
    {
        _unitOfWork = unitOfWork;
        _userProvider = userProvider;
    }

    public async Task<Result<CancelSaleDto>> Handle(CancelSaleItemsCommand request, CancellationToken cancellationToken)
    {
        var currentUser = _userProvider.GetCurrentUser();

        if (currentUser is null)
            return Result<CancelSaleDto>.BusinessFailure("CurrentUser not found.");

        var userId = currentUser.Id; // vindo do CurrentUserClaims
        var user = await _unitOfWork.Users.GetByIdAsync(userId, cancellationToken);
        if (user is null)
            return Result<CancelSaleDto>.BusinessFailure("User not found.");

        var sale = await _unitOfWork.Sales.GetByIdWithItemsAsync(request.SaleId, cancellationToken);
        if (sale is null)
            return Result<CancelSaleDto>.BusinessFailure("Sale not found.");

        var itemsToCancel = sale.Items.Where(i => request.ItemIds.Contains(i.Id)).ToList();
        if (itemsToCancel.Count == 0)
            return Result<CancelSaleDto>.BusinessFailure("No valid items found for cancellation.");

        if(itemsToCancel.Exists(c=> c.Status == SaleItemStatus.Cancelled))
            return Result<CancelSaleDto>.BusinessFailure("Some items are already cancelled.");

        sale.CancelItems(user, [.. itemsToCancel]);

        _unitOfWork.Sales.Update(sale);
        await _unitOfWork.CommitChangesAsync(cancellationToken);

        var response = new CancelSaleDto
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

        return Result<CancelSaleDto>.Success(response);
    }
}
