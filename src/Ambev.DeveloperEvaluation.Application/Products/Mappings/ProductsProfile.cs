using Ambev.DeveloperEvaluation.Application.Products.Commands.Create;
using Ambev.DeveloperEvaluation.Application.Products.Commands.Create.Dtos;
using Ambev.DeveloperEvaluation.Application.Products.Commands.Update;
using Ambev.DeveloperEvaluation.Application.Products.Commands.Update.Dtos;
using Ambev.DeveloperEvaluation.Application.Products.Common;
using Ambev.DeveloperEvaluation.Common.Pagination;
using Ambev.DeveloperEvaluation.Domain.Entities;
using AutoMapper;

namespace Ambev.DeveloperEvaluation.Application.Products.Mappings;

public class ProductsProfile : Profile
{
    public ProductsProfile()
    {
        CreateMap<CreateProductsCommand, Product>()
            .ConstructUsing(cmd =>
                new Product(cmd.Name, cmd.Price, cmd.Description, cmd.Quantity, cmd.Category)
            );

        CreateMap<Product, CreateProductDto>()
           .ForMember(dest => dest.Category, src => src.MapFrom(s => s.Category.ToString()));

        CreateMap<UpdateProductsCommand, Product>();
        CreateMap<Product, UpdateProductDto>()
           .ForMember(dest => dest.Category, src => src.MapFrom(s => s.Category.ToString()));

        CreateMap<Product, GetProductDto>();

        CreateMap<PaginatedList<Product>, PaginatedList<GetProductDto>>()
            .ConvertUsing((src, _, context) => new PaginatedList<GetProductDto>
            {
                Page = src.Page,
                PageSize = src.PageSize,
                TotalCount = src.TotalCount,
                Items = [.. src.Items.Select(p => context.Mapper.Map<GetProductDto>(p))]
            });
    }
}
