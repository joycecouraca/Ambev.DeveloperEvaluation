using Ambev.DeveloperEvaluation.Application.Sales.Commands.Create;
using Ambev.DeveloperEvaluation.Application.Sales.Commands.Create.Dtos;
using AutoMapper;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales.Create;

public class CreateSalesProfile : Profile
{
    public CreateSalesProfile()
    {
        CreateMap<CreateSalesRequest, CreateSalesCommand>()
             .ForMember(dest => dest.CreatedById, opt => opt.Ignore());

        CreateMap<SaleItemRequest, CreateSaleItemDto>()
            .ForMember(dest => dest.ProductName, opt => opt.Ignore())
            .ForMember(dest => dest.UnitPrice, opt => opt.Ignore())
            .ForMember(dest => dest.TotalAmount, opt => opt.Ignore())
            .ForMember(dest => dest.DiscountTotal, opt => opt.Ignore())
            .ForMember(dest => dest.DiscountPerUnit, opt => opt.Ignore());

        CreateMap<CreateSaleDto, CreateSalesResponse>();
    }
}
