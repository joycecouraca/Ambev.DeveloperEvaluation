using Ambev.DeveloperEvaluation.Application.Products.Commands.Create;
using Ambev.DeveloperEvaluation.Application.Products.Commands.Update;
using Ambev.DeveloperEvaluation.Application.Products.Common;
using Ambev.DeveloperEvaluation.Common.Pagination;
using Ambev.DeveloperEvaluation.Domain.Entities;
using AutoMapper;

namespace Ambev.DeveloperEvaluation.Application.Products.Mappings;

public class ProductsProfile : Profile
{
    public ProductsProfile()
    {
        AddGlobalIgnore("Events");

        CreateMap<CreateProductsCommand, Product>()
            .ConstructUsing(cmd =>
                new Product(cmd.Name, cmd.Price, cmd.Description, cmd.Quantity, cmd.Category)
            ).ForMember(dest => dest.Active, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.Id, opt => opt.Ignore()); ;

        CreateMap<Product, ProductDto>()
           .ForMember(dest => dest.Category, src => src.MapFrom(s => s.Category.ToString()));

        CreateMap<UpdateProductsCommand, Product>()
            .ForMember(dest => dest.Active, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore());

        CreateMap<PaginatedList<Product>, PaginatedList<ProductDto>>()
            .ConvertUsing((src, _, context) => new PaginatedList<ProductDto>
            {
                Page = src.Page,
                PageSize = src.PageSize,
                TotalCount = src.TotalCount,
                Items = [.. src.Items.Select(p => context.Mapper.Map<ProductDto>(p))]
            });
    }
}
