using Ambev.DeveloperEvaluation.Application.Common;
using Ambev.DeveloperEvaluation.Application.Products.Common;
using Ambev.DeveloperEvaluation.Domain.Enums;
using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Products.Commands.Create;

public class CreateProductsCommand : IRequest<Result<ProductDto>>
{
    public string Name { get; set; } = default!;
    public decimal Price { get; set; }
    public string Description { get; set; } = default!;
    public int Quantity { get; set; }
    public ProductCategory Category { get; set; } = default!;
}
