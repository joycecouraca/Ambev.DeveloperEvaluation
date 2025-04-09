using Ambev.DeveloperEvaluation.Application.Sales.Common.Dtos;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.WebApi.Features.Sales.Common.Response;
using AutoMapper;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales.Common.Mappings;

public class SalesProfile : Profile
{
    public SalesProfile()
    {
        CreateMap<SaleDto, SalesResponse>();

        CreateMap<Sale, SalesResponse>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.TotalSaleAmount, opt => opt.MapFrom(src => src.TotalSaleAmount))
            .ForMember(dest => dest.TotalDiscountAmount, opt => opt.MapFrom(src => src.TotalDiscount))
            .ForMember(dest => dest.CustomerName, opt => opt.MapFrom(src => src.BoughtBy.Username))
            .ForMember(dest => dest.Items, opt => opt.MapFrom(src => src.Items))
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()))
            .ForMember(dest => dest.SaleNumber, opt => opt.MapFrom(src => src.SaleNumber))
            .ForMember(dest => dest.SoldAt, opt => opt.MapFrom(src => src.SoldAt))
            .ForMember(dest => dest.BranchName, opt => opt.MapFrom(src => src.BranchName))
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.CreatedAt));

        CreateMap<SaleItem, SaleItemResponse>()
            .ForMember(dest => dest.ProductId, opt => opt.MapFrom(src => src.ProductId))
            .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.Product.Name))
            .ForMember(dest => dest.Quantity, opt => opt.MapFrom(src => src.Quantity))
            .ForMember(dest => dest.UnitPrice, opt => opt.MapFrom(src => src.Product.Price))
            .ForMember(dest => dest.TotalAmount, opt => opt.MapFrom(src => src.TotalAmount))
            .ForMember(dest => dest.DiscountPerUnit, opt => opt.MapFrom(src => src.DiscountPerUnit));
    }
}