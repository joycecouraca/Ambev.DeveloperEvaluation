using Ambev.DeveloperEvaluation.Application.Sales.Querys.GetById;
using AutoMapper;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales.Read.GetById;

public class GetSaleByIdProfile : Profile
{
    public GetSaleByIdProfile()
    {
        CreateMap<GetSaleByIdRequest, GetSaleByIdQuery>();
    }
}

