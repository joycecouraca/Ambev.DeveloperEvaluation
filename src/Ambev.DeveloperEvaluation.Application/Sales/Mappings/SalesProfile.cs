using Ambev.DeveloperEvaluation.Application.Products.Common;
using Ambev.DeveloperEvaluation.Application.Sales.Commands.Cancel.Dtos;
using Ambev.DeveloperEvaluation.Application.Sales.Common.Dtos;
using Ambev.DeveloperEvaluation.Common.Pagination;
using Ambev.DeveloperEvaluation.Domain.Entities;
using AutoMapper;

namespace Ambev.DeveloperEvaluation.Application.Sales.Mappings;

public class SalesProfile : Profile
{
    public SalesProfile()
    {        
        CreateMap<Sale, SaleDto>()
            .ForMember(dest => dest.SaleNumber, opt => opt.MapFrom(src => src.SaleNumber))
            .ForMember(dest => dest.BranchName, opt => opt.MapFrom(src => src.BranchName))
            .ForMember(dest => dest.SoldAt, opt => opt.MapFrom(src => src.SoldAt))
            .ForMember(dest => dest.TotalSaleAmount, opt => opt.MapFrom(src => src.TotalSaleAmount))
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()))
            .ForMember(dest => dest.Items, opt => opt.MapFrom(src => src.Items));

        CreateMap<SaleItem, SaleItemDto>()
            .ForMember(dest => dest.ProductId, opt => opt.MapFrom(src => src.ProductId))
            .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.Product.Name))
            .ForMember(dest => dest.Quantity, opt => opt.MapFrom(src => src.Quantity))
            .ForMember(dest => dest.UnitPrice, opt => opt.MapFrom(src => src.OriginalUnitPrice))            
            .ForMember(dest => dest.TotalAmount, opt => opt.MapFrom(src => src.TotalAmount))
            .ForMember(dest => dest.DiscountPerUnit, opt => opt.MapFrom(src => src.DiscountPerUnit));

        CreateMap<SaleItem, CancelledItemDto>()
          .ForMember(dest => dest.ItemId, opt => opt.MapFrom(src => src.Id))
          .ForMember(dest => dest.ProductId, opt => opt.MapFrom(src => src.ProductId))
          .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.Product.Name))
          .ForMember(dest => dest.Quantity, opt => opt.MapFrom(src => src.Quantity))
          .ForMember(dest => dest.FinalUnitPrice, opt => opt.MapFrom(src => src.FinalUnitPrice))
          .ForMember(dest => dest.TotalAmount, opt => opt.MapFrom(src => src.TotalAmount))
          .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()));

        CreateMap<PaginatedList<Sale>, PaginatedList<SaleDto>>()
            .ConvertUsing((src, _, context) => new PaginatedList<SaleDto>
            {
                Page = src.Page,
                PageSize = src.PageSize,
                TotalCount = src.TotalCount,
                Items = [.. src.Items.Select(p => context.Mapper.Map<SaleDto>(p))]
            });
    }
}
