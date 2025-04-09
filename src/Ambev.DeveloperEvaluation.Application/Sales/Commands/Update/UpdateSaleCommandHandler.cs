using Ambev.DeveloperEvaluation.Application.Common;
using Ambev.DeveloperEvaluation.Application.Sales.Common.Dtos;
using Ambev.DeveloperEvaluation.Common.Security;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using AutoMapper;
using MediatR;


namespace Ambev.DeveloperEvaluation.Application.Sales.Commands.Update;

public class UpdateSaleCommandHandler : IRequestHandler<UpdateSaleCommand, Result<SaleDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IContextUserProvider _currentUserAccessor;
    private readonly IMapper _mapper;

    public UpdateSaleCommandHandler(IUnitOfWork unitOfWork, IContextUserProvider currentUserAccessor, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _currentUserAccessor = currentUserAccessor;
        _mapper = mapper;
    }

    public async Task<Result<SaleDto>> Handle(UpdateSaleCommand command, CancellationToken cancellationToken)
    {
        var currentUser =  _currentUserAccessor.GetCurrentUser();
        var sale = await _unitOfWork.Sales.GetByIdAsync(command.SaleId, cancellationToken);

        if (sale is null)
            return Result<SaleDto>.BusinessFailure("Sale not found.");

        if (sale.CanBeCancelled)
            return Result<SaleDto>. BusinessFailure("Sale be Canceled in its current state.");

        if (!sale.CanBeEdited)
            return Result<SaleDto>.BusinessFailure("Sale cannot be edited in its current state.");
        

        var customer = await _unitOfWork.Users.GetByIdAsync(command.CustomerId, cancellationToken);
        if (customer is null)
            return Result<SaleDto>.BusinessFailure("Customer not found.");

        sale.Change(customer, command.SoldAt, command.BranchName);

        if (command.Items is not null && command.Items.Count != 0)
        {
            var productQuantities = command.Items.ToDictionary(i => i.ProductId, i => i.Quantity);
            sale.ChangeQuantities(productQuantities);
        }

        await _unitOfWork.CommitChangesAsync(cancellationToken);

        var response = _mapper.Map<SaleDto>(sale);
        return Result<SaleDto>.Success(response);
    }
}
