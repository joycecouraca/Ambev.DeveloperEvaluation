using Ambev.DeveloperEvaluation.Application.Products.Commands.Update;
using Ambev.DeveloperEvaluation.Domain.Enums;
using AutoMapper;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Products.Update;

/// <summary>
/// Defines AutoMapper configuration for mapping product-related objects.
/// </summary>
public class UpdateProductsProfile : Profile
{
    /// <summary>
    /// Initializes a new instance of the <see cref="UpdateProductsProfile"/> class.
    /// This class is used to configure object mappings related to products with AutoMapper.
    /// </summary>
    public UpdateProductsProfile()
    {
        CreateMap<UpdateProductsRequest, UpdateProductsCommand>()
            .ForMember(dest => dest.Category, src => src.MapFrom(s => Enum.Parse<ProductCategory>(s.Category)));
    }
}