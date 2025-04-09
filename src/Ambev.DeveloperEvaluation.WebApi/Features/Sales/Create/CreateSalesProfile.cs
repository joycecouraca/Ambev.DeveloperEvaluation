using Ambev.DeveloperEvaluation.Application.Sales.Commands.Create;
using Ambev.DeveloperEvaluation.Application.Sales.Common.Dtos;
using Ambev.DeveloperEvaluation.WebApi.Features.Sales.Common.Response;
using AutoMapper;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales.Create;

public class CreateSalesProfile : Profile
{
    public CreateSalesProfile()
    {
        CreateMap<CreateSalesRequest, CreateSalesCommand>();

        CreateMap<SaleDto, CreateSalesResponse>()
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.CreatedAt))
            .ForMember(dest => dest.TotalDiscountAmount, opt => opt.MapFrom(src => src.TotalDiscountAmount));

        CreateMap<SaleItemRequest, SaleItemUpsertDto>();

        CreateMap<SaleItemDto, SaleItemResponse>();
    }
}
