﻿using Ambev.DeveloperEvaluation.Application.Common;
using Ambev.DeveloperEvaluation.Application.Sales.Commands.Create.Dtos;
using Ambev.DeveloperEvaluation.Common.Security;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using AutoMapper;
using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Sales.Commands.Create;

public class CreateSalesCommandHandler : IRequestHandler<CreateSalesCommand, Result<CreateSaleDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IContextUserProvider _currentUserAccessor;

    public CreateSalesCommandHandler(IUnitOfWork unitOfWork, IMapper mapper, IContextUserProvider currentUserAccessor)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _currentUserAccessor = currentUserAccessor;
    }

    public async Task<Result<CreateSaleDto>> Handle(CreateSalesCommand command, CancellationToken cancellationToken)
    {
        var (customer, creator, failureResult) = await GetUsersAsync(command.CustomerId, cancellationToken);
        
        if (failureResult is not null)
            return failureResult;

        var saleItemsResult = await BuildSaleItemsAsync(command, cancellationToken);

        if (!saleItemsResult.IsSuccess)
            return Result<CreateSaleDto>.Failure(saleItemsResult.Error!);

        var sale = Domain.Entities.Sales.Create(customer!, creator!, command.SoldAt, command.BranchName);
        sale.AddItems([.. saleItemsResult.Value!]);

        _unitOfWork.Sales.Add(sale);
        await _unitOfWork.CommitChangesAsync(cancellationToken);

        return Result<CreateSaleDto>.Success(_mapper.Map<CreateSaleDto>(sale));
    }

    private static SaleItem CreateSaleItemWithDiscount(Product product, int quantity)
    {
        var saleItem = new SaleItem(product, quantity);

        if (quantity >= 10)
            saleItem.ApplyDiscount();
        else if (quantity >= 4)
            saleItem.ApplyDiscount();        

        return saleItem;
    }

    private async Task<User?> GetCreatorAsync(CancellationToken cancellationToken)
    {
        var creatorId = _currentUserAccessor.GetCurrentUser();
        return await _unitOfWork.Users.GetByIdAsync(creatorId.Id, cancellationToken);
    }

    private async Task<(User? Customer, User? Creator, Result<CreateSaleDto>? FailureResult)> GetUsersAsync(Guid customerId, CancellationToken cancellationToken)
    {
        var customer = await _unitOfWork.Users.GetByIdAsync(customerId, cancellationToken);
        if (customer is null)
            return (null, null, Result<CreateSaleDto>.Failure("Customer not found."));

        var creator = await GetCreatorAsync(cancellationToken);
        if (creator is null)
            return (customer, null, Result<CreateSaleDto>.Failure("User creating the sale was not found."));

        return (customer, creator, null);
    }

    private async Task<Result<List<SaleItem>>> BuildSaleItemsAsync(CreateSalesCommand command, CancellationToken cancellationToken)
    {
        var productIds = command.Items.Select(i => i.ProductId).ToHashSet();

        var products = await _unitOfWork.Products
            .FindAllAsync(p => productIds.Contains(p.Id), cancellationToken);

        var productDict = products.ToDictionary(p => p.Id);
        var saleItems = new List<SaleItem>();

        foreach (var item in command.Items)
        {
            if (!productDict.TryGetValue(item.ProductId, out var product))
                return Result<List<SaleItem>>.Failure($"Product with ID {item.ProductId} not found.");

            if (item.Quantity > 20)
                return Result<List<SaleItem>>.Failure($"Cannot sell more than 20 items for product '{product.Name}'.");

            var saleItem = CreateSaleItemWithDiscount(product, item.Quantity);
            saleItems.Add(saleItem);
        }

        return Result<List<SaleItem>>.Success(saleItems);
    }
}
