using Ambev.DeveloperEvaluation.Application.Products.Commands.Create.Dtos;
using Ambev.DeveloperEvaluation.Application.Products.Commands.Create;
using Ambev.DeveloperEvaluation.Domain.Enums;
using Ambev.DeveloperEvaluation.WebApi.Features.Products.Create;
using AutoMapper;
using Ambev.DeveloperEvaluation.Application.Products.Commands.Update;
using Ambev.DeveloperEvaluation.Application.Products.Commands.Update.Dtos;
using Ambev.DeveloperEvaluation.WebApi.Features.Products.Read;
using Ambev.DeveloperEvaluation.Application.Products.Common;

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

        CreateMap<UpdateProductDto, UpdateProductsResponse>()
            .ForMember(dest => dest.Category, src => src.MapFrom(s => s.Category.ToString()));

        CreateMap<GetProductDto, ProductResponse>().ReverseMap();
    }
}