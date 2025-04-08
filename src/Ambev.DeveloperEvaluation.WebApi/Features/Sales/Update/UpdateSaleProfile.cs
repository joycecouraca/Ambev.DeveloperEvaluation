using Ambev.DeveloperEvaluation.Application.Sales.Commands.Create.Dtos;
using Ambev.DeveloperEvaluation.Application.Sales.Commands.Update;
using Ambev.DeveloperEvaluation.Application.Sales.Commands.Update.Dtos;
using Ambev.DeveloperEvaluation.WebApi.Features.Sales.Create;
using AutoMapper;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales.Update;

public class UpdateSaleProfile : Profile
{
    public UpdateSaleProfile()
    {
        CreateMap<UpdateSaleRequest, UpdateSaleCommand>()
             .ForMember(dest => dest.CreatedById, opt => opt.Ignore());

        CreateMap<SaleItemRequest, CreateSaleItemDto>()
            .ForMember(dest => dest.ProductName, opt => opt.Ignore())
            .ForMember(dest => dest.UnitPrice, opt => opt.Ignore())
            .ForMember(dest => dest.TotalAmount, opt => opt.Ignore())
            .ForMember(dest => dest.DiscountTotal, opt => opt.Ignore())
            .ForMember(dest => dest.DiscountPerUnit, opt => opt.Ignore());

        CreateMap<UpdateSaleDto, UpdateSaleResponse>();
        CreateMap<UpdatedSaleItemDto, CreateSaleItemDto>();
    }
}