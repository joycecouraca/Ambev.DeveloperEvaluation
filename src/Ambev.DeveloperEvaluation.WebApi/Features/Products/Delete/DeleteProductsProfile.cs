using Ambev.DeveloperEvaluation.Application.Products.Commands.Delete;
using AutoMapper;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Products.Delete;

public class DeleteProductsProfile : Profile
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DeleteProductsProfile"/> class.
    /// This class is used to configure object mappings related to products with AutoMapper.
    /// </summary>
    public DeleteProductsProfile()
    {
        CreateMap<DeleteProductRequest, DeleteProductsCommand>();
    }
}
