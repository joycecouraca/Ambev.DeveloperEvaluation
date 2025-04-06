using Ambev.DeveloperEvaluation.Application.Common;
using Ambev.DeveloperEvaluation.Application.Products.Commands.Update.Dtos;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using AutoMapper;
using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Products.Commands.Update;

public class UpdateProductsCommandHandler : IRequestHandler<UpdateProductsCommand, Result<UpdateProductDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public UpdateProductsCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<Result<UpdateProductDto>> Handle(UpdateProductsCommand command, CancellationToken cancellationToken)
    {
        try
        {
            var product = await _unitOfWork.Products.GetAsync(command.Id, cancellationToken);

            if (product == null)
            {
                return Result<UpdateProductDto>.Failure($"Product with the id {command.Id} not found.");
            }

            _mapper.Map(command, product);
            _unitOfWork.Products.Update(product);
            await _unitOfWork.CommitChangesAsync(cancellationToken);

            return Result<UpdateProductDto>.Success(_mapper.Map<UpdateProductDto>(product));
        }
        catch (Exception ex)
        {
            return Result<UpdateProductDto>.Failure("Unexpected error:", ex.Message);
        }
    }
}
