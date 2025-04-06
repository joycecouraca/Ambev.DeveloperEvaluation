using Ambev.DeveloperEvaluation.Application.Products.Commands.Create;
using Ambev.DeveloperEvaluation.Application.Products.Commands.Create.Dtos;
using Ambev.DeveloperEvaluation.Application.Products.Commands.Update;
using Ambev.DeveloperEvaluation.Application.Products.Commands.Update.Dtos;
using Ambev.DeveloperEvaluation.Domain.Entities;
using AutoMapper;

namespace Ambev.DeveloperEvaluation.Application.Products.Mappings;

public class ProductsProfile : Profile
{
    public ProductsProfile()
    {
        CreateMap<CreateProductsCommand, Product>();
        CreateMap<Product, CreateProductDto>()        
           .ForMember(dest => dest.Category, src => src.MapFrom(s => s.Category.ToString()));

        CreateMap<UpdateProductsCommand, Product>();
        CreateMap<Product, UpdateProductDto>()
           .ForMember(dest => dest.Category, src => src.MapFrom(s => s.Category.ToString()));
    }
}
