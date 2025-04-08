using Ambev.DeveloperEvaluation.Application.Common;
using Ambev.DeveloperEvaluation.Application.Sales.Commands.Update.Dtos;
using Ambev.DeveloperEvaluation.Common.Security;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using AutoMapper;
using MediatR;


namespace Ambev.DeveloperEvaluation.Application.Sales.Commands.Update;

public class UpdateSaleCommandHandler : IRequestHandler<UpdateSaleCommand, Result<UpdateSaleDto>>
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

    public async Task<Result<UpdateSaleDto>> Handle(UpdateSaleCommand command, CancellationToken cancellationToken)
    {
        var currentUser =  _currentUserAccessor.GetCurrentUser();
        var sale = await _unitOfWork.Sales.GetByIdAsync(command.SaleId, cancellationToken);

        if (sale is null)
            return Result<UpdateSaleDto>.Failure("Sale not found.");

        if (!sale.CanBeEdited)
            return Result<UpdateSaleDto>.Failure("Sale cannot be edited in its current state.");

        if (sale.CanBeCancelled)
            return Result<UpdateSaleDto>.Failure("Sale cannot be edited in its current state.");

        var customer = await _unitOfWork.Users.GetByIdAsync(command.CustomerId, cancellationToken);
        if (customer is null)
            return Result<UpdateSaleDto>.Failure("Customer not found.");

        sale.Change(customer, command.SoldAt, command.BranchName);

        if (command.Items is not null && command.Items.Count != 0)
        {
            var productQuantities = command.Items.ToDictionary(i => i.ProductId, i => i.Quantity);
            sale.ChangeQuantities(productQuantities);
        }

        await _unitOfWork.CommitChangesAsync(cancellationToken);

        var response = _mapper.Map<UpdateSaleDto>(sale);
        return Result<UpdateSaleDto>.Success(response);
    }
}
