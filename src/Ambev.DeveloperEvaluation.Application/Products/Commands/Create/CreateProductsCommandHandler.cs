using Ambev.DeveloperEvaluation.Application.Common;
using Ambev.DeveloperEvaluation.Application.Products.Commands.Create.Dtos;
using Ambev.DeveloperEvaluation.Application.Users.CreateUser;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using AutoMapper;
using MediatR;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace Ambev.DeveloperEvaluation.Application.Products.Commands.Create;

public class CreateProductsCommandHandler : IRequestHandler<CreateProductsCommand, Result<CreateProductDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public CreateProductsCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<Result<CreateProductDto>> Handle(CreateProductsCommand command, CancellationToken cancellationToken)
    {
        try
        {
            var existingProduct = await _unitOfWork.Products.ExistsAsync(c=> c.Name == command.Name, cancellationToken);
            
            if (existingProduct)
            {
                return Result<CreateProductDto>.Failure($"Product with the name {command.Name} already exists.");
            }

            var product = _mapper.Map<Product>(command);
            _unitOfWork.Products.Add(product);
            await _unitOfWork.CommitChangesAsync(cancellationToken);

            return Result<CreateProductDto>.Success(_mapper.Map<CreateProductDto>(product));
        }
        catch (Exception ex)
        {
            return Result<CreateProductDto>.Failure($"Unexpected error: {ex.Message}");
        }
    }
}
