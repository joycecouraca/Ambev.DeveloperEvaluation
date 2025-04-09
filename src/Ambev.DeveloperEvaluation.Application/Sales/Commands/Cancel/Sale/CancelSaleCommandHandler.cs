using Ambev.DeveloperEvaluation.Application.Common;
using Ambev.DeveloperEvaluation.Common.Security;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Sales.Commands.Cancel.Sale;

public class CancelSaleCommandHandler : IRequestHandler<CancelSaleCommand, Result<Guid>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IContextUserProvider _contextUserProvider;

    public CancelSaleCommandHandler(IUnitOfWork unitOfWork, IContextUserProvider contextUserProvider)
    {
        _unitOfWork = unitOfWork;
        _contextUserProvider = contextUserProvider;
    }

    public async Task<Result<Guid>> Handle(CancelSaleCommand request, CancellationToken cancellationToken)
    {
        var sale = await _unitOfWork.Sales.GetByIdAsync(request.SaleId, cancellationToken);

        if (sale is null)
            return Result<Guid>.BusinessFailure("Sale not found");

        if (sale.CanBeCancelled)
            return Result<Guid>.BusinessFailure("This sale cannot be cancelled");

        var currentUser = _contextUserProvider.GetCurrentUser();
        var userId = currentUser.Id; // vindo do CurrentUserClaims

        var user = await _unitOfWork.Users.GetByIdAsync(userId, cancellationToken);
        sale.Cancel(user!);

        _unitOfWork.Sales.Update(sale);
        await _unitOfWork.CommitChangesAsync(cancellationToken);

        return Result<Guid>.Success(sale.Id);
    }
}
