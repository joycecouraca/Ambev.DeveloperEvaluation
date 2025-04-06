using Ambev.DeveloperEvaluation.Application.Products.Commands.Create;
using Ambev.DeveloperEvaluation.Application.Products.Commands.Create.Dtos;
using Ambev.DeveloperEvaluation.Domain.Enums;
using AutoMapper;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Products.Create;

/// <summary>
/// Defines AutoMapper configuration for mapping product-related objects.
/// </summary>
public class CreateProductsProfile : Profile
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CreateProductsProfile"/> class.
    /// This class is used to configure object mappings related to products with AutoMapper.
    /// </summary>
    public CreateProductsProfile()
    {
        CreateMap<CreateProductsRequest, CreateProductsCommand>()
            .ForMember(dest => dest.Category, src => src.MapFrom(s => Enum.Parse<ProductCategory>(s.Category)));
      
        CreateMap<CreateProductDto, CreateProductsResponse>()
            .ForMember(dest => dest.Category, src=> src.MapFrom(s => s.Category.ToString()));
    }
}
